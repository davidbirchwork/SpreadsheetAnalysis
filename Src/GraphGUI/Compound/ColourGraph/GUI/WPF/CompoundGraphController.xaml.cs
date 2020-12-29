using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Graph.Compound;

namespace GraphGUI.Compound.ColourGraph.GUI.WPF {
    /// <summary>
    /// Interaction logic for CompoundGraphController.xaml
    /// </summary>
    public partial class CompoundGraphController : UserControl, INotifyPropertyChanged {
        private CompoundGraph _graph;
        private bool _initFinished;

        public CompoundGraphController() {            
            InitializeComponent();
            this.GraphViewer.OnVertexClicked += vertex => this.GraphProvider.InnerFocusChanged(this._graph, vertex);
        }

        public void SetGraphProvider(ICompoundGraphProvider graphProvider) {
            this.GraphProvider = graphProvider;
            this.Mappings = this.GraphProvider.Mappings;
            this.GraphOptions = this.GraphProvider.GraphOptions;
            _initFinished = true;
            UpdateGraph(this.Mappings.FirstOrDefault());            
         //   ShowInnerGraphs(this.Mappings.FirstOrDefault(), this.GraphOptions.Take(this.GraphProvider.MaxGraphOptions));            
        }

        private void UpdateGraph(string mapping) {
            this._graph = this.GraphProvider.CreateGraphforMapping(mapping);
            this.GraphViewer.ViewGraph(this._graph);                        
        }

        private void ShowInnerGraphs(string mapping, IEnumerable<string> graphOptions) {            
            this.GraphViewer.AddInnerGraphs( vertex => this.GraphProvider.GetInnerGraph(vertex, mapping, graphOptions, this.showOutputs.IsChecked == true));
        }

        private IEnumerable<string> _mappings;
        public IEnumerable<string> Mappings {
            get { return _mappings; }
            set {
                _mappings = value;
                NotifyPropertyChanged("Mappings");
                this.MappingsBox.ItemsSource = this._mappings;
                this.MappingsBox.SelectedIndex = 0;
            }
        }

        private IEnumerable<string> _graphOptions;        

        public IEnumerable<string> GraphOptions {
            get { return _graphOptions; }
            set {
                _graphOptions = value;
                if (value != null) {
                    NotifyPropertyChanged("GraphOptions");
                }
                    //        var elems = this._graphOptions.Take(this.GraphProvider.MaxGraphOptions).ToList();
                    this.GraphOptionsBox.ItemsSource = this._graphOptions;
                    //      this.GraphOptionsBox.SelectAll();// a nice hack ;)
                    //    elems.AddRange(this._graphOptions.Skip(this.GraphProvider.MaxGraphOptions));  
                    //  this.GraphOptionsBox.UpdateLayout();                
            }
        }

        private ICompoundGraphProvider GraphProvider { get; set; }

        private void MappingsBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!_initFinished) return;            
            this.UpdateGraph(this.MappingsBox.SelectedItem as string);
//            this.ShowInnerGraphs(this.MappingsBox.SelectedItem as string, (this.GraphOptionsBox.SelectedItems.Cast<string>()).Take(this.GraphProvider.MaxGraphOptions));
        }

        private void GraphOptionsBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!_initFinished) return;
  //          this.ShowInnerGraphs(this.MappingsBox.SelectedItem as string, (this.GraphOptionsBox.SelectedItems.Cast<string>()).Take(this.GraphProvider.MaxGraphOptions));
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string info) {
            if (PropertyChanged != null) {                
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }


        #endregion

        private void UpdateInnerGraphsClick(object sender, System.Windows.RoutedEventArgs e) {
            this.ShowInnerGraphs(this.MappingsBox.SelectedItem as string,
                (this.GraphOptionsBox.SelectedItems.Cast<string>()).Take(this.GraphProvider.MaxGraphOptions));
            this.GraphProvider.RefreshInnerGraphColours(this._graph, this.MappingsBox.SelectedItem as string,
                                                        (this.GraphOptionsBox.SelectedItems.Cast<string>()).Take(
                                                            this.GraphProvider.MaxGraphOptions));            
        }

        #region flag printing        

        private void VisualiseFlagsClick(object sender, System.Windows.RoutedEventArgs e) {
            PrintandViewFlags(true);
        }

        private void PrintFlagsClick(object sender, System.Windows.RoutedEventArgs e) {
            PrintandViewFlags(false);
        }

        private void PrintandViewFlags(bool visualise) {
            var printresults = this.GraphViewer.PrintFlags(this.GraphProvider.VertexToName);            
            this.GraphProvider.PrintVertexes(printresults.Item1,printresults.Item2);
            if (visualise) {
                object visualisationContext = this.GraphProvider.VisualiseFlags(printresults.Item1, this.MappingsBox.SelectedItem as string);
                this.GraphViewer.OnVertexClicked += vertex => this.GraphProvider.UpdateVisualisationForInnerVertex(this._graph, vertex, visualisationContext, this.MappingsBox.SelectedItem as string);
            }
        }

        #endregion

        private void BtnFormatGraphClick(object sender, System.Windows.RoutedEventArgs e) {
            // Vertex Font Size
            int vertexFontSize;
            if (!int.TryParse(this.tbFontSize.Text, out vertexFontSize) || vertexFontSize<6 || vertexFontSize>200) {
                vertexFontSize = 12;
                this.tbFontSize.Text = vertexFontSize.ToString();
            }
            this.GraphViewer.SetNodeFontSize(vertexFontSize);

            // Inner Graph Width
            int innerGraphWidth;
            if (!int.TryParse(this.tbInnerGraphWidth.Text, out innerGraphWidth) || innerGraphWidth < 100 || innerGraphWidth > 100000) {
                innerGraphWidth = 520;
                this.tbInnerGraphWidth.Text = innerGraphWidth.ToString();
            }
            this.GraphViewer.InnerGraphWidth = innerGraphWidth;
            this.GraphViewer.InnerGraphWidthInternal = innerGraphWidth - 20;

            // Inner Graph Height
            int innerGraphHeight;
            if (!int.TryParse(this.tbInnerGraphHeight.Text, out innerGraphHeight) || innerGraphHeight < 100 || innerGraphHeight > 100000) {
                innerGraphHeight = 320;
                this.tbInnerGraphHeight.Text = innerGraphHeight.ToString();
            }
            this.GraphViewer.InnerGraphHeight = innerGraphHeight;
            this.GraphViewer.InnerGraphHeightInternal = innerGraphHeight - 20;

            // InnerVertex Font Size
            int innerVertexFontSize;
            if (!int.TryParse(this.tbInnerVertexFontSize.Text, out innerVertexFontSize) || innerVertexFontSize < 6 || innerVertexFontSize > 200) {
                innerVertexFontSize = 8;
                this.tbInnerVertexFontSize.Text = innerVertexFontSize.ToString();
            }
            this.GraphViewer.InnerNodeFontSize = innerVertexFontSize;

            // InnerVertexValue Font Size
            int innerVertexValueFontSize;
            if (!int.TryParse(this.tbInnerVertexValueFontSize.Text, out innerVertexValueFontSize) || innerVertexValueFontSize < 6 || innerVertexValueFontSize > 200) {
                innerVertexValueFontSize = 8;
                this.tbInnerVertexValueFontSize.Text = innerVertexValueFontSize.ToString();
            }
            this.GraphViewer.InnerNodeValueFontSize = innerVertexValueFontSize;

            this.GraphViewer.RefreshGraphLayout();
        }

        private void LaunchUnityClick(object sender, System.Windows.RoutedEventArgs e) {
            this.GraphProvider.VisualiseFlags(null, this.MappingsBox.SelectedItem as string);
        }        
    }
}
