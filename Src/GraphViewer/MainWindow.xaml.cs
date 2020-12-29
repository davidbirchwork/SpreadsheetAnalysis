using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml.Linq;
using Graph;
using Utilities.Config;
using Utilities.Loggers;
using GraphMetrics;

namespace GraphViewer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        
        public MainWindow() {
            InitializeComponent();
            Logger.RegisterDelegate(LogBoxUpdateDelegate);
        }

        #region progressbar Logger Methods

        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        private delegate void LogMessageCallback(LogLevel loglevel, string logmessage);

        private void LogBoxUpdateDelegate(LogLevel level, string message) {
            if (this.logBox.Dispatcher.CheckAccess()) {
                if (level > LogLevel.DEBUG) {
                    this.logBox.AppendText("\n"+message);
                }
            } else {
                this.logBox.Dispatcher.Invoke(DispatcherPriority.Normal,
                                              new LogMessageCallback(LogBoxUpdateDelegate),                                              
                                              level, message);
            }                        
        }
       
        #endregion

        private void OpenClick(object sender, RoutedEventArgs e) {
            OpenFileDialog opener = new OpenFileDialog {
                Title = @"Load File",
                DefaultExt =
                  ".graphml",
                Filter = @"graphml|*.graphml",
                AutoUpgradeEnabled = true,
                CheckPathExists = true,
                CheckFileExists = true
            };

            if (opener.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                this.usingHiddenGraph = false;
                this.viewer1.LoadAGraphFromFile(opener.FileName);
            }
        }
        
        private void CloseClick(object sender, RoutedEventArgs e) {
            this.Close();
        }

        #region metrics

        private Dictionary<string, IMetric> _computedMetrics;
        private List<string> _computedMetricNamesNames = new List<string> {"Computed Metrics Will Appear Here"};

        public List<string> ComputedMetricsNames {
            get {
                return _computedMetricNamesNames;
            }
            set {
                _computedMetricNamesNames = value;
                NotifyPropertyChanged("ComputedMetricsNames");
            }
        }

        private List<string> _colours = new List<string>();
        // dict < vertex , list uses, list used in>
        private readonly Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> _vertexdict = new Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>>();        

        private void CalculateMetricsClick(object sender, RoutedEventArgs e) {

            CalculateVertexDict();            

            CalculateMetrics();
            
        }

        private void CalculateVertexDict() {
            foreach (AEdge aEdge in this.GetEdges()) {
                if (!this._vertexdict.ContainsKey(aEdge.Source)) {
                    this._vertexdict.Add(aEdge.Source, new Tuple<List<ExcelVertex>, List<ExcelVertex>>(new List<ExcelVertex>(), new List<ExcelVertex>()));
                }
                this._vertexdict[aEdge.Source].Item2.Add(aEdge.Target);// source - source used in target

                if (!this._vertexdict.ContainsKey(aEdge.Target)) {
                    this._vertexdict.Add(aEdge.Target, new Tuple<List<ExcelVertex>, List<ExcelVertex>>(new List<ExcelVertex>(), new List<ExcelVertex>()));
                }
                this._vertexdict[aEdge.Target].Item1.Add(aEdge.Source); // target - target uses source
            }
        }

        private IEnumerable<AEdge> GetEdges() {
            return this.usingHiddenGraph ? this.Edges : this.viewer1.Edges;
        }

        private IEnumerable<ExcelVertex> GetVertices() {
            return this.usingHiddenGraph ? this.Vertices : this.viewer1.Vertices;
        }

        private void CalculateMetrics() {
            this._colours = this.GetVertices().Select(vert => vert.GetColourID()).Distinct().ToList();
            this._colours.Sort();

            this._computedMetrics = new Dictionary<string, IMetric>();


            CompositionContainer compositionContainer = AppConfig.GetCompositionContainer();
            Dictionary<string, Lazy<IMetric, IMetricAttribute>> knownMetrics =
                compositionContainer.GetExports<IMetric, IMetricAttribute>().ToDictionary(
                    metric => metric.Metadata.MetricName,
                    metric => metric);

            Task.Factory.StartNew(() => {

                                      ConcurrentDictionary<string, IMetric> evaluatedMetrics =
                                          Algorithms.WorkList.ParallelRecursiveEval
                                              <string, Lazy<IMetric, IMetricAttribute>, IMetric>
                                              (knownMetrics,
                                               (metricToCompute, computedMetrics) => {
                                                   var metric = metricToCompute.Value;
                                                   // cant run if prereq's haven't been eval'd
                                                   if (
                                                       !metric.Value.PreRequisiteMetrics().All(
                                                           computedMetrics.ContainsKey)) {
                                                       return false;
                                                   }
                                                   // can't run if any prereq's failed
                                                   if (
                                                       metric.Value.PreRequisiteMetrics().Any(
                                                           prereq => computedMetrics[prereq] == null)) {
                                                       computedMetrics.TryAdd(
                                                           metric.Metadata.MetricName, null);
                                                       return true;
                                                   }

                                                   // now try and compute the metric
                                                   Logger.INFO("Starting to Compute "+metric.Metadata.MetricName);
                                                   string error =
                                                       metric.Value.Compute(computedMetrics,
                                                                            this.GetVertices(),
                                                                            this.GetEdges(),
                                                                            this._colours,
                                                                            this._vertexdict);
                                                   if (error != null) {
                                                       Logger.FAILURE("Failed to compute metric: " +
                                                                      metric.Metadata.MetricName +
                                                                      " Error " + error);
                                                       computedMetrics.TryAdd(
                                                           metric.Metadata.MetricName, null);
                                                   } else {
                                                       this._computedMetrics.Add(metric.Metadata.MetricName,metric.Value);
                                                       computedMetrics.TryAdd(
                                                           metric.Metadata.MetricName, metric.Value);
                                                       Logger.SUCCESS("Computed " +
                                                                      metric.Metadata.MetricName);
                                                   }

                                                   return true;

                                               });
                                      // save results for printing ignoring any which failed
                                      this._computedMetrics =
                                          evaluatedMetrics.Where(metric => metric.Value != null).ToDictionary(metric => metric.Key, metric => metric.Value);

                                      PrintMetrics();
                                  });
        }        

        private delegate void PrintMetricsCallBack();

        private void PrintMetrics() {
            if (this.MetricsList.Dispatcher.CheckAccess()) {

                this.ComputedMetricsNames = this._computedMetrics.Keys.ToList();

            } else {
                this.MetricsList.Dispatcher.Invoke(DispatcherPriority.Normal, new PrintMetricsCallBack(PrintMetrics));
            }                                                  
        }

        #endregion

        #region abstraction

        private void CalculateAbstractions(object sender, RoutedEventArgs e) {
            if (this.viewer1.LastClicked != null) {
                CalculateVertexDict();
               var click = Abstractor.Calculate(this.viewer1.LastClicked, this._vertexdict,this.GetVertices());
              PrintClick(click);
              if (click != null) {
                  // highlighting
                  foreach (ExcelVertex aVertex in this.GetVertices()) {
                      aVertex.HighlightFontSize = (click.Contains(aVertex)) ? "40" : "12";
                  }
              }
            }
        }

        private void PrintClick(ICollection<ExcelVertex> click) {
            this.metricsBox.Clear();
            if (click == null) {
                this.metricsBox.AppendText("Failed :(");
                return;
            }
            this.metricsBox.AppendText("Click Found :) \n");
            this.metricsBox.AppendText("Count: "+click.Count+"\n");
            foreach (ExcelVertex aVertex in click) {
                this.metricsBox.AppendText(aVertex.ID+"\n");
            }
        }

        private ConcurrentDictionary<ExcelVertex, List<ExcelVertex>> _clicks;        

        private void CalculateAllAbstractions(object sender, RoutedEventArgs e) {
            CalculateVertexDict();
            this.metricsBox.Clear();
            int vertCount = this.GetVertices().Count();
            this._clicks = new ConcurrentDictionary<ExcelVertex, List<ExcelVertex>>();
            Parallel.ForEach(this.GetVertices(), aVertex => {
                                                        if (this._vertexdict[aVertex].Item1.Count == 0) {
                                                            // dont look at end values
                                                            aVertex.HighlightFontSize = "20";
                                                            this._clicks.AddOrUpdate(aVertex,
                                                                                    vertex =>
                                                                                    new List<ExcelVertex> {aVertex},
                                                                                    (vertex,kpv) =>
                                                                                    new List<ExcelVertex> {aVertex});
                                                        } else {
                                                            var click = Abstractor.Calculate(aVertex, this._vertexdict,
                                                                                             this.GetVertices());

                                                            aVertex.HighlightFontSize = (click == null ||
                                                                                         click.Count == vertCount)
                                                                                            ? "12"
                                                                                            : "40";                                                           

                                                            this._clicks.AddOrUpdate(aVertex,
                                                                                    vertex => click,
                                                                                    (vertex, kpv) => click);
                                                        }
                                                    });
            
            List<Tuple<ExcelVertex,List<ExcelVertex>>> sortedclicks = this._clicks.Select(click => new Tuple<ExcelVertex, List<ExcelVertex>>(click.Key, click.Value)).ToList();
           
            sortedclicks.Sort((a,b) =>b.Item2.Count.CompareTo(a.Item2.Count));

            foreach (Tuple<ExcelVertex, List<ExcelVertex>> sortedclick in sortedclicks) {
                if (sortedclick.Item2.Count > 1 && sortedclick.Item2.Count < vertCount) {
                    this.metricsBox.AppendText("Can abstract " + sortedclick.Item2.Count + " vertexes into " +
                                               sortedclick.Item1.ID + " \n");
                }
            }
        }

        private void MenuItemClick(object sender, RoutedEventArgs e) {
            if (this.viewer1.LastClicked != null) {
                if (this._clicks != null) {
                    List<ExcelVertex> click;
                    if (this._clicks.TryGetValue(this.viewer1.LastClicked, out click)) {
                        PrintClick(click);
                    }
                }
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        private void MetricsListSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {

            this.metricsBox.Text = "";

            foreach (var computedMetric in this.MetricsList.SelectedItems) {
                IMetric metric = this._computedMetrics[computedMetric.ToString()];
                this.metricsBox.AppendText(metric.Print() + "\n");
            }      
        }

        #region nonvisual loading        

        private bool usingHiddenGraph = false;
        private List<ExcelVertex> Vertices;
        private List<AEdge> Edges;

        private void OpenHiddenClick(object sender, RoutedEventArgs e) {
            //todo this need to be rewritten within the Quickgraph graphml reader - goal is that we just dont create / link to the GUI to avoid its limitations
            OpenFileDialog opener = new OpenFileDialog {
                Title = @"Load File",
                DefaultExt =
                  ".graphml",
                Filter = @"gml|*.graphml",
                AutoUpgradeEnabled = true,
                CheckPathExists = true,
                CheckFileExists = true
            };

            if (opener.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

                XDocument graph = XDocument.Load(opener.FileName);

                this.Vertices = new List<ExcelVertex>();
                this.Edges = new List<AEdge>();
                Dictionary<string,ExcelVertex> vertdict = new Dictionary<string, ExcelVertex>();
                XNamespace ns = XNamespace.Get(@"http://graphml.graphdrawing.org/xmlns");
                foreach (XElement nodeelem in graph.Root.Element(ns + "graph").Elements(ns + "node")) {
                    ExcelVertex vertex = new ExcelVertex(nodeelem.Attribute("id").Value) { Formula = nodeelem.Attribute("meta")?.Value};
                    vertdict.Add(vertex.ID, vertex);
                    this.Vertices.Add(vertex);
                }

                foreach (XElement edge in graph.Root.Element(ns + "graph").Elements(ns + "edge")) {
                    this.Edges.Add(new AEdge(edge.Attribute("id").Value, vertdict[edge.Attribute("source").Value], vertdict[edge.Attribute("target").Value]));
                }
                this.usingHiddenGraph = true;
                Logger.SUCCESS(string.Format("Loaded Graph with {0} vertices and {1} edges",this.Vertices.Count,this.Edges.Count));
            }
        }

        private string FilterMeta(string value) {
            int aint;
            float afloat;
            double adouble;
            decimal adecimal;
            bool abool;
            if (int.TryParse(value, out aint) || 
                bool.TryParse(value,out abool) ||
                float.TryParse(value,out afloat) || 
                double.TryParse(value,out adouble) || 
                decimal.TryParse(value,out adecimal)) {
                return value;
            }
            if (value.Contains("(") || value.Contains(")") || value.Contains("*") || value.Contains("+")) {
                return value;
            } else {
                return "'" + value + "'";
            }
        }

        #endregion
    }
}
