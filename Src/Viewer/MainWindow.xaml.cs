using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GraphAnalysis.InputPartitioner;
using log4net;
using log4net.Config;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.WpfGraphControl;
using Microsoft.Win32;
using ParseTreeExtractor;
using ParseTreeExtractor.Graph;
using Label = Microsoft.Msagl.Core.Layout.Label;
using Node = Microsoft.Msagl.Drawing.Node;
using Point = Microsoft.Msagl.Core.Geometry.Point;

namespace HyperGraphViewer {
    /// <summary>
    /// A Graph viewer based on Microsoft Automatic Graph Layout
    /// https://github.com/Microsoft/automatic-graph-layout
    /// note the dll reference is due to this issue :-/ 
    /// https://github.com/Microsoft/automatic-graph-layout/issues/171
    /// This code is based upon their samples which are under MIT license 
    /// </summary>
    public partial class MainWindow  {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private GraphViewer _graphViewer;
        private GraphViewer _hyperGraphViewer;

        public MainWindow() {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            _graphViewer = new GraphViewer();
            _graphViewer.BindToPanel(Panel);
            SetGraph(CreateDummyGraph(), _graphViewer);
            _hyperGraphViewer = new GraphViewer();
            _hyperGraphViewer.BindToPanel(HyperPanel);
          //  SetGraph(CreateDummyGraph(), _hyperGraphViewer);
            SetUpLogging();
        }

        private static void SetUpLogging() {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            Log.Info("Welcome to Graph Viewer");
        }

        private static void SetGraph(Graph graph, GraphViewer graphViewer) {
            graph.CreateGeometryGraph();
            foreach (Node node in graph.Nodes)
                node.GeometryNode.BoundaryCurve = CreateLabelAndBoundary(node);

            foreach (var edge in graph.Edges) {
                if (edge.Label != null) {
                    var geomEdge = edge.GeometryEdge;
                    StringMeasure.MeasureWithFont(edge.LabelText,
                        new Font(edge.Label.FontName, (float) edge.Label.FontSize), out var width, out var height);
                    edge.Label.GeometryLabel = geomEdge.Label = new Label(width, height, geomEdge);
                }

            }

            var geomGraph = graph.GeometryGraph;

            List<GeometryGraph> geomGraphComponents = GraphConnectedComponents.CreateComponents(geomGraph.Nodes, geomGraph.Edges).ToList();
            var settings = new SugiyamaLayoutSettings();
            
            foreach (GeometryGraph subGraph in geomGraphComponents) {
              
                var layout = new LayeredLayout(subGraph, settings);
                subGraph.Margins = settings.NodeSeparation / 2;
                layout.Run();
            }

            Microsoft.Msagl.Layout.MDS.MdsGraphLayout.PackGraphs(geomGraphComponents, settings);

            geomGraph.UpdateBoundingBox();
            
            graphViewer.NeedToCalculateLayout = false;
            graphViewer.Graph = graph;
        }

        private static Graph CreateDummyGraph() {
            var graph = new Graph();

            graph.AddEdge("a", "b");
            graph.AddEdge("e", "b");
            graph.AddEdge("d", "b");
            graph.AddEdge("b", "c");

            graph.AddEdge("a22", "b22");
            graph.AddEdge("e22", "b22");
            graph.AddEdge("d22", "b22");
            graph.AddEdge("b22", "c22");

            graph.AddEdge("a0", "b0");
            graph.AddEdge("b0", "c0");

            graph.AddEdge("a33", "b33");
            graph.AddEdge("e33", "b33");
            graph.AddEdge("d33", "b33");
            graph.AddEdge("b33", "c33");

            graph.AddEdge("a11", "b11");
            graph.AddEdge("b11", "c11").LabelText = "Test labels!";
            return graph;
        }

        static ICurve CreateLabelAndBoundary(Node node) {
            node.Attr.Shape = Shape.DrawFromGeometry;
            node.Attr.LabelMargin *= 2;
            double width;
            double height;
            StringMeasure.MeasureWithFont(node.Label.Text,
                new Font(node.Label.FontName, (float) node.Label.FontSize), out width, out height);
            node.Label.Width = width;
            node.Label.Height = height;
            int r = node.Attr.LabelMargin;
            return CurveFactory.CreateRectangleWithRoundedCorners(width + r * 2, height + r * 2, r, r, new Point());
        }

        private void OpenClick(object sender, RoutedEventArgs e) {
            OpenFileDialog opener = new OpenFileDialog {
                Title = @"Load File",
                DefaultExt = ".graphml",
                Filter = @"gml|*.graphml",
                CheckPathExists = true,
                CheckFileExists = true
            };

            if (opener.ShowDialog() != true) return;
            var fileName = opener.FileName;

            SetGraph(GraphmlReader.ReadGraphml(fileName), _graphViewer);
        }

        private async void ParseClick(object sender, RoutedEventArgs e) {
            OpenFileDialog opener = new OpenFileDialog {
                Title = @"Load Spreadsheet",
                DefaultExt = ".xlsx",
                Filter = @"xlsx|*.xlsx",
                CheckPathExists = true,
                CheckFileExists = true
            };

            if (opener.ShowDialog() != true) return;
            var filename = opener.FileName;
            

            await ChangeTabTo(LogTab);


            ExtractSpreadsheet(filename);

            SetGraph(GraphmlReader.ReadGraphml(Path.ChangeExtension(filename, "graphml")), _graphViewer);
            await ChangeTabTo(GraphTab);

            SetHyperGraph(GraphmlReader.ReadGraphml(Path.ChangeExtension(filename, "graphml"),hyperGraph:true), _hyperGraphViewer);


         }

        private void SetHyperGraph(Graph graph, GraphViewer hyperGraphViewer) {
            graph.Attr.LayerDirection = LayerDirection.TB;
            hyperGraphViewer.NeedToCalculateLayout = true;
            hyperGraphViewer.Graph = graph;
        }

        private void LoadScript(string filename, string extension, TextBox box) {
            var actualFile = Path.ChangeExtension(filename, extension);
            if (actualFile!= null && File.Exists(actualFile)) {
                box.Text = File.ReadAllText(actualFile);
            }
        }

        private async Task ChangeTabTo(TabItem wantedTab) {
            int index = 0;
            for (var i = 0; i < TabControl.Items.Count; i++) {
                var tab = TabControl.Items[i];
                var t = tab as TabItem;
                if (t == null) continue;
                if (t == wantedTab) {
                    index = i;
                    break;
                }
            }

            await Dispatcher.BeginInvoke((Action)(() => TabControl.SelectedIndex = index));
        }

        private void ExtractSpreadsheet(string filename) {
            var extractor = new ExcelExtractor();
            var extraction = extractor.Extract(filename);
            var graph = SpreadGraphCreator.Create(extraction);
            var graphmlFilename = Path.ChangeExtension(filename, "graphml");
            SpreadGraphExporter.Export(graph, graphmlFilename);
            InputPartitioner.PartitionInputs(graph,extraction);
        }
    }
}
