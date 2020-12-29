using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using Graph;
using Graph.Compound;
using GraphSharp.Controls;
using Microsoft.Win32;
using QuickGraph.Serialization;
using Image = System.Drawing.Image;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using GraphSharp.Algorithms.Layout;

namespace GraphGUI.GUI.WPF {

    /// <summary>
    /// Interaction logic for Viewer.xaml
    /// </summary>
    public partial class Viewer : INotifyPropertyChanged {

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }


        #endregion

        #region internal graph sizing

        #region Width/Height                

        private int _innerGraphWidth = 3500;
        public int InnerGraphWidth {
            get {
                return _innerGraphWidth;
            }
            set {
                _innerGraphWidth = value;
                NotifyPropertyChanged("InnerGraphWidth");
            }
        }

        private int _innerGraphHeight = 320;
        public int InnerGraphHeight {
            get { return _innerGraphHeight; }
            set {
                _innerGraphHeight = value;
                NotifyPropertyChanged("InnerGraphHeight");
            }
        }

        private int _innerGraphWidthInternal = 3480;
        public int InnerGraphWidthInternal {
            get { return _innerGraphWidthInternal; }
            set {
                _innerGraphWidthInternal = value;
                NotifyPropertyChanged("InnerGraphWidthInternal");
            }
        }

        private int _innerGraphHeightInternal = 300;
        public int InnerGraphHeightInternal {
            get { return _innerGraphHeightInternal; }
            set {
                _innerGraphHeightInternal = value;
                NotifyPropertyChanged("InnerGraphHeightInternal");
            }
        }

        #endregion

        #region font size:

        private int _innerNodeFontSize = 8;
        public int InnerNodeFontSize {
            get { return _innerNodeFontSize; }
            set {
                _innerNodeFontSize = value;
                NotifyPropertyChanged("InnerNodeFontSize");
            }
        }

        private int _innerNodeValueFontSize = 8;
        public int InnerNodeValueFontSize {
            get { return _innerNodeValueFontSize; }
            set {
                _innerNodeValueFontSize = value;
                NotifyPropertyChanged("InnerNodeValueFontSize");
            }
        }

        #endregion

        #endregion

        private bool _usingCompound;
        private AGraph AGraphToVisualize { get; set; }
        private AGraphLayout AGraphLayoutDisplay { get; set; }
        private CompoundGraph CompoundGraphToVisualize { get; set; }
        private CompoundGraphLayout CompoundGraphLayoutDisplay { get; set; }

        private object GetLayout { get { return this._usingCompound ? (object)this.CompoundGraphLayoutDisplay : this.AGraphLayoutDisplay; } }

        public IEnumerable<ExcelVertex> Vertices {
            get {
                if (this._usingCompound) {
                    return (IEnumerable<ExcelVertex>)this.CompoundGraphToVisualize.Vertices;
                } else {
                    return this.AGraphToVisualize.Vertices;
                }
            }
        }

        public IEnumerable<AEdge> Edges {
            get {
                if (this._usingCompound) {
                    return (IEnumerable<AEdge>)this.CompoundGraphToVisualize.Edges;
                } else {
                    return this.AGraphToVisualize.Edges;
                }
            }
        }

        private bool HasGraph {
            get {
                return (this._usingCompound && this.CompoundGraphToVisualize != null) ||
                       (!this._usingCompound && this.AGraphToVisualize != null);

            }
        }

        public List<Tuple<string, string>> ColourMapping { get; set; }
        public List<string> RefersToList { get; set; }
        public List<string> ReferedToByList { get; set; }

        public Viewer() {
            this.ColourMapping = new List<Tuple<string, string>>();

            this._innerGraphHeight = 320;
            this._innerGraphHeightInternal = 300;
            this._innerGraphWidth = 520;
            this._innerGraphWidthInternal = 500;

            InitializeComponent();
            this.LayoutSelector.SelectedItem = "Tree";
            this.RefersToList = new List<string>();
            this.ReferedToByList = new List<string>();            
        }

        public void LoadAGraphFromFile(string graphfile, bool loadMeta = true) {
            //graph where the vertices and edges should be put in
            var pocGraph = new AGraph();

            //open the file of the graph
            using (XmlReader reader = XmlReader.Create(graphfile)) {

                //create the serializer
                var serializer = new GraphMLDeserializer<ExcelVertex, AEdge, AGraph>();
                
                //deserialize the graph
                serializer.Deserialize(reader, pocGraph,
                                       id => new ExcelVertex(id),
                                       (source, target, id) => new AEdge(id, source, target));                

            }
          /*  if (loadMeta) { this is old code which avoids the need to create a c# class reflecting the meta data of the nodes, however its not possible to tell quickgraph not to try and read it
                // load any meta in the graphml file stored on the node elements under a "meta" attribute
                XElement graphroot = XElement.Load(graphfile);

                XElement graphelemt = graphroot.Elements().First();
                if (graphelemt != null) {
                    IEnumerable<XElement> nodes = graphelemt.Elements().Where(node => node.Name.LocalName.ToString()=="node");
                    //id="C!Fill_Time" meta="500/H19"
// ReSharper disable PossibleNullReferenceException
                    var metaDict = nodes.Where(elem => elem.Attribute("meta") != null).ToDictionary(elem => elem.Attribute("id").Value,
                                                                                     elem =>
                                                                                     elem.Attribute("meta").Value);
                    // ReSharper restore PossibleNullReferenceException

                    foreach (var vertex in pocGraph.Vertices) {
                        string metavalue;
                        if (metaDict.TryGetValue(vertex.ID, out metavalue)) {
                            vertex.MetaData = metavalue;
                        }
                    }
                }
            }*/

            ViewGraph(pocGraph);
        }

        public void LoadCompoundGraphFromFile(string fileName) {
            //create graph
            //open the file of the graph
            XmlReader reader = XmlReader.Create(fileName);

            //create the serializer
            var serializer = new GraphMLDeserializer<CompoundVertex, CompoundEdge, CompoundGraph>();

            //graph where the vertices and edges should be put in
            var pocGraph = new CompoundGraph();

            //deserialize the graph
            serializer.Deserialize(reader, pocGraph,
                                    id => new CompoundVertex(id, "unknown"),
                                    (source, target, id) => new CompoundEdge(id, source, target));


            ViewGraph(pocGraph);
        }

        public void ViewGraph<TGraph>(TGraph graphToVisualize) where TGraph : class {
            const string agraph = "<Graph:AGraphLayout xmlns:Graph=\"clr-namespace:Graph;assembly=Graph\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"  xmlns:Data=\"clr-namespace:System.Windows.Data;assembly=PresentationFramework\" x:Name=\"graphLayout\" Graph=\"{Data:Binding }\" LayoutAlgorithmType=\"Round\" OverlapRemovalAlgorithmType=\"FSA\" HighlightAlgorithmType=\"Simple\" />";
            const string compoundgraph = "<Graph:CompoundGraphLayout xmlns:Graph=\"clr-namespace:Graph.Compound;assembly=Graph\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"  xmlns:Data=\"clr-namespace:System.Windows.Data;assembly=PresentationFramework\" x:Name=\"graphLayout\"  IsAnimationEnabled=\"False\" Graph=\"{Data:Binding }\" LayoutAlgorithmType=\"Round\" OverlapRemovalAlgorithmType=\"FSA\" HighlightAlgorithmType=\"Simple\" />";

            StringReader stringReader;

            if (graphToVisualize.GetType().Equals(typeof(AGraph))) {
                stringReader = new StringReader(agraph);
                this._usingCompound = false;
            } else if (graphToVisualize.GetType().Equals(typeof(CompoundGraph))) {
                stringReader = new StringReader(compoundgraph);
                this._usingCompound = true;
            } else {
                throw new ArgumentException("Did not recognise type of graph");
            }


            XmlReader xmlReader = XmlReader.Create(stringReader);
            UIElement graphviewer = (UIElement)XamlReader.Load(xmlReader);

            this.AGraphToVisualize = graphToVisualize as AGraph;
            this.CompoundGraphToVisualize = graphToVisualize as CompoundGraph;

            this.AGraphLayoutDisplay = graphviewer as AGraphLayout;
            this.CompoundGraphLayoutDisplay = graphviewer as CompoundGraphLayout;

            this.zoomer.Content = graphviewer;

            DataContext = graphToVisualize;

            foreach (ExcelVertex aVertex in this.Vertices) {
                object c = aVertex.Colour;
            }

            this.ColourMapping = new List<Tuple<string, string>>();
            foreach (KeyValuePair<string, string> colouring in ExcelVertex.ColourMap) {
                this.ColourMapping.Add(new Tuple<string, string>(colouring.Key, colouring.Value));
            }
            this.ColouringList.ItemsSource = this.ColourMapping;

            LayoutSelectorSelectionChanged(null, null);
        }

        private void PrintbtnClick(object sender, RoutedEventArgs e) {

            SaveFileDialog saveFialog = new SaveFileDialog { DefaultExt = ".PNG" };
            if (saveFialog.ShowDialog() == true) {
                WpfElementToImage.ExportToPng((FrameworkElement)this.GetLayout, new Uri(saveFialog.FileName));
            }
        }

        private void BtnXpsClick(object sender, RoutedEventArgs e) {
            //Print the whole document onto a single huge XPS page.
            PrintDialog printDialog = new PrintDialog();

            //Remember to choose the "Microsoft XPS Document Writer".
            if (printDialog.ShowDialog() == true) {
                //Change graphLayout to your graphLayout.
                //"My Canvas" is the name of the print item in the queue.
                printDialog.PrintVisual((Visual)GetLayout, "Graph");
            }
        }

        #region change layout algorithms

        private readonly List<string> _layoutAlgorithms = new List<string> { "Circular", "Tree", "FR", "BoundedFR", "KK", "ISOM", "LinLog", "EfficientSugiyama", "CompoundFDP" };

        public List<string> LayoutAlgorithms {
            get {
                return this._layoutAlgorithms;
            }
        }

        private void DirectionChanged(object sender, RoutedEventArgs e) {
            LayoutSelectorSelectionChanged(sender, null);
        }

        private void LayoutSelectorSelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (this.HasGraph) {
                if (this._usingCompound) {
                    this.CompoundGraphLayoutDisplay.LayoutAlgorithmType = this.LayoutSelector.SelectedItem.ToString();
                    if (this.LayoutSelector.SelectedItem.ToString() == "Tree") {
                        this.CompoundGraphLayoutDisplay.LayoutParameters = new SimpleTreeLayoutParameters {
                                                                            Direction =
                                                                                ckbxDirection.IsChecked == true
                                                                                    ? LayoutDirection.TopToBottom
                                                                                    : LayoutDirection.RightToLeft,
                                                                            SpanningTreeGeneration =  SpanningTreeGeneration.DFS
                                                                        };                         
                    }                    
                } else {
                    this.AGraphLayoutDisplay.LayoutAlgorithmType = this.LayoutSelector.SelectedItem.ToString();
                    if (this.LayoutSelector.SelectedItem.ToString() == "Tree") {
                        this.AGraphLayoutDisplay.LayoutParameters = new SimpleTreeLayoutParameters {
                            Direction =
                                ckbxDirection.IsChecked == true
                                    ? LayoutDirection.TopToBottom
                                    : LayoutDirection.RightToLeft,
                            SpanningTreeGeneration = SpanningTreeGeneration.DFS
                        };
                    }
                }

                RefreshGraphLayout();
            }
        }

        /// <summary>
        /// Updates the graph layout
        /// </summary>
        public void RefreshGraphLayout() {
            if (this.HasGraph) {
                if (this._usingCompound) {
                    this.CompoundGraphLayoutDisplay.Relayout();
                } else {
                    this.AGraphLayoutDisplay.Relayout();
                }
                this.zoomer.ZoomToFill();
            }
        }

        #endregion

        private void BtnZoomClick(object sender, RoutedEventArgs e) {
            // undo the last highlighting
            foreach (ExcelVertex vertex in this.Vertices.Where(v => v.HighlightFontSize == "12")) {
                vertex.HighlightFontSize = "12";
            }
            // then do the new one :)
            foreach (ExcelVertex vertex in this.Vertices.Where(v => v.ID.Contains(this.ZoomContent.Text))) {
                vertex.HighlightFontSize = "72";
            }
            //this.graphLayout.RefreshHighlight();
            //this.graphLayout.InvalidateVisual();
            //this.graphLayout.UpdateLayout();
        }

        private void TextBlockMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var vertexClicked = sender as TextBlock;
            List<List<string>> paths = new List<List<string>>();
            if (vertexClicked != null) {
                ExcelVertex ev = vertexClicked.DataContext as ExcelVertex;
                if (ev != null) {
                    this.LastClicked = ev;
                    string vertexid = ev.ID;
                    this.RefersToList = new List<string>();
                    this.ReferedToByList = new List<string>();
                    foreach (AEdge edge in this.Edges) {
                        if (vertexid.Equals(edge.Source.ID)) {
                            if (edge.Target.ID != vertexid) {
                                this.ReferedToByList.Add(edge.Target.ID);
                            }
                        } else if (vertexid.Equals(edge.Target.ID)) {
                            if (edge.Source.ID != vertexid) {
                                this.RefersToList.Add(edge.Source.ID);
                            }
                        }
                    }

                    // now trace path to end ...     
                    paths.Add(new List<string> { ev.ID });
                //    paths = TracePaths(ev, paths); TODO i have removed this because its specific to excel
                paths = new List<List<string>>();
                }
            }
            this.RefersTo.ItemsSource = this.RefersToList;
            this.ReferedToBy.ItemsSource = this.ReferedToByList;
            this.PathsBox.ItemsSource = new List<string>();
            this.PathsBox.ItemsSource = PathsToListBox(paths);
        }

        private List<List<string>> TracePaths(ExcelVertex aVertex, List<List<string>> paths) {
            List<List<string>> foundPaths = new List<List<string>>();
            foreach (ExcelVertex target in this.Edges.Where(edge => edge.Source.ID == aVertex.ID).Select(edge => edge.Target)) {
                List<List<string>> newPaths = new List<List<string>>();
                foreach (List<string> path in paths) {
                    if (!path.Contains(target.ID)) {
                        List<string> clonedPath = path.ToArray().ToList();
                        clonedPath.Add(target.ID);
                        newPaths.Add(clonedPath);
                    }
                }
                if (newPaths.Count > 0) {
                    foundPaths.AddRange(TracePaths(target, newPaths));
                }
            }
            if (foundPaths.Count == 0) { // deal with the ends
                foundPaths = paths;
            }
            return foundPaths;
        }
        private IEnumerable<string> PathsToListBox(List<List<string>> paths) {
            List<string> text = new List<string>();
            int i = 0;
            text.Add("Found " + paths.Count + " Paths");
            foreach (List<string> path in paths) {
                i++;
                text.Add("Path #" + i);
                foreach (string node in path) {
                    text.Add(node);
                }
            }

            return text;
        }

        public void AddInnerGraphs(Func<CompoundVertex, AGraph> innerGraphExtractor) {
            foreach (var aVertex in this.Vertices) {

                CompoundVertex vert = ((CompoundVertex)aVertex);
                if (vert != null) {
                    vert.InnerGraph = innerGraphExtractor.Invoke(vert);
                }
            }
        }

        /// <summary>
        /// Prints each vertex to a file specified be the name converter
        /// </summary>
        /// <param name="vertexToFileNameConvert">A function to convert between a vertex and its file name convert.</param>
        /// <returns>xml roof file name [not saved here] and a list of vertex, file name mappings</returns>
        public Tuple<string, List<Tuple<object, string,string>>> PrintFlags(Func<object, string> vertexToFileNameConvert) {
            
            SaveFileDialog saveFialog = new SaveFileDialog {DefaultExt = ".xml"};
            if (saveFialog.ShowDialog() == true) {
                int i = 1;
                string dirname = Path.GetFileNameWithoutExtension(saveFialog.FileName);
                string fname = saveFialog.FileName.Replace(".xml", "") + "\\";
                Directory.CreateDirectory(fname);
                fname = fname + dirname;
                List<Tuple<object, string>> dict = new List<Tuple<object, string>>();

                UIElementCollection uiObjects = this._usingCompound
                                                ? this.CompoundGraphLayoutDisplay.Children
                                                : this.AGraphLayoutDisplay.Children;

                foreach (object child in uiObjects) {
                    // edges or vertexes

                    VertexControl vertexControl = child as VertexControl;
                    if (vertexControl != null) {


                        // because of what looks like a framework memory leak we dont regenerate and hope it will create the flags in two tries!
                        string filename = fname + "_" + i + "_" +
                                          vertexToFileNameConvert.Invoke(vertexControl.Vertex) + ".png";                        
                        Tuple<object, string> res = File.Exists(filename)? 
                            new Tuple<object, string>(vertexControl.Vertex,filename)
                            : PrintVertex(vertexControl, child,filename);
                        i++;
                        dict.Add(res);
                        if (i%15 == 0) {
                            GC.Collect();
                        }
                    }
                }
                // now create low res versions of all the flags.

                return new Tuple<string, List<Tuple<object, string, string>>>
                    (saveFialog.FileName,dict.Select(image => new Tuple<object, string, string>(image.Item1, image.Item2, ResizeImageToPNG(image.Item2))).ToList());                
            }

            return null;
        }

        private Tuple<object, string> PrintVertex(VertexControl vertexControl, object child, string filename) {
            int children = VisualTreeHelper.GetChildrenCount((DependencyObject) child);
            //todo this only prints the first child
            if (children > 0) {

                // vertexes dont seem to print so we find their visual child - normally a border or grid or something
                var child1 = VisualTreeHelper.GetChild((DependencyObject) child, 0);

                var target = (Visual) child1;
                var frameworkElem = (FrameworkElement) child1;

                int actualWidth = (int) frameworkElem.ActualWidth;
                int actualHeight = (int) frameworkElem.ActualHeight;

                //render it visually to a bitmap                            
                RenderTargetBitmap rtb = new RenderTargetBitmap(actualWidth,
                    actualHeight, 96, 96,
                    PixelFormats.Default);
                rtb.Render(target);

                // & save it as a png :)
                PngBitmapEncoder png = new PngBitmapEncoder();

                png.Frames.Add(BitmapFrame.Create(rtb));

                using (Stream stm = File.Create(filename)) {
                    png.Save(stm);
                }

                return new Tuple<object, string>(vertexControl.Vertex, filename);

            }

            return null;
        }

        private static string ResizeImageToPNG(string image) {
            string ext = Path.GetExtension(image);
            string newFname = image.Replace(ext, "_lowres" + ext);
            
            using (Image source = Image.FromFile(image)) {
                const double maxWidth = 750;
                const double maxHeight = 500;
                const double boxRatio = maxWidth/maxHeight;

                double imageHeight = source.Height;
                double imageWidth = source.Width;

                double aspectRatio = imageWidth/imageHeight;

                double scaleFactor = 0;
                if (boxRatio > aspectRatio)
                    //Use height, since that is the most restrictive dimension of box.
                    scaleFactor = maxHeight/imageHeight;
                else
                    scaleFactor = maxWidth/imageWidth;

                int destWidth = (int) (imageWidth*scaleFactor);
                int destHeight = (int) (imageHeight*scaleFactor);


                using (Bitmap b = new Bitmap(destWidth, destHeight)) {
                    using (Graphics g = Graphics.FromImage(b)) {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        g.DrawImage(source, 0, 0, destWidth, destHeight);
                    }

                    b.Save(newFname, System.Drawing.Imaging.ImageFormat.Png);

                }                
            }

            GC.Collect();

            return newFname;
        }

        public void SetNodeFontSize(int vertexFontSize) {
            string fontsize = vertexFontSize.ToString();
            if (this.HasGraph) {
                foreach (ExcelVertex aVertex in this.Vertices) {
                    aVertex.HighlightFontSize = fontsize;
                }
            }
        }

        #region inner Vertex Clicks

        public delegate void VertexClicked(ExcelVertex vertex);

        /// <summary>
        /// attach to this event to handle messages passed inside the app
        /// use registerDelegate to add delegates
        /// </summary>
        public event VertexClicked OnVertexClicked;

        public ExcelVertex LastClicked;

        private void InnerVertexMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            TextBlockMouseDown(sender, e);
            var vertexClicked = sender as TextBlock;           
            if (vertexClicked != null) {
                ExcelVertex clickedVertex = vertexClicked.DataContext as ExcelVertex;
                if (OnVertexClicked != null) {
                    this.LastClicked = clickedVertex;
                    OnVertexClicked(clickedVertex);
                }
            }
        }

        #endregion
    }
}
