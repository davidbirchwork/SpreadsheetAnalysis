using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Xml.Linq;
using Graph;
using Utilities.Loggers;
using Utilities.Windowing;

namespace GraphGUI.GUI.WinForms {
    /// <summary>
    /// A window for viewing graphml graphs [& others ;)]
    /// </summary>
    [Export(typeof(IWindow))]
    [Window(WindowTypeName = "Graph Viewer",
        WindowTypeDescription = "A Window to view a graph",
        InstanceRequired = false,
        AllowMultiple = true)]
    public partial class GraphViewer : Form , IWindow {

        private IDAG<ExcelVertex> idag;
        private object graph;
        private bool _doNotReCreateIdag;        

        public GraphViewer() {
            InitializeComponent();
        }

        #region Implementation of IWindow

        public string GetInstanceName() {
            return "Graph viewer";
        }

        public string GetInstanceDescription() {
            return "A Window to view a graph";
        }

        public XElement SaveAsXML() {
            throw new NotImplementedException();
        }

        public XElement LoadFromXML() {
            throw new NotImplementedException();
        }

        public void BindTo(Dictionary<string, object> objectList) {
            if (objectList.ContainsKey("URI")) {
                string filename = (string)objectList["URI"];
                this.viewer.LoadAGraphFromFile(filename);
            } else {
                if (objectList.ContainsKey("DoNotReCreateIDAG")) {
                    this._doNotReCreateIdag = true;
                }                                

                if (objectList.ContainsKey("DAG")) {
                    this.idag = (IDAG<ExcelVertex>)objectList["DAG"];                    
                    
                    HandleIdag(this.idag);
                    RefreshView();
                } else {
                    Logger.FAILURE("Could not find a DAG");
                }
            }
        }

        // invoke required delegate
        private delegate void HandleIdagCallback(IDAG<ExcelVertex> dag);

        private void HandleIdag(IDAG<ExcelVertex> dag) {
            if (InvokeRequired) {
                HandleIdagCallback callback = HandleIdag;
                this.Invoke(callback,new object[] { dag});
            } else {
                this.graph = idag.CreateGraph();
                this.viewer.ViewGraph(this.graph);
            }
        }

        public void RefreshView() {
            if (InvokeRequired) {
                Action callback = RefreshView;
                this.Invoke(callback);
            } else {
                if (!_doNotReCreateIdag && this.idag != null) {
                    HandleIdag(this.idag);
                    this.viewer.ViewGraph(this.graph);
                }
               // this.viewer.RefreshGraphLayout();
              //  this.viewer.UpdateLayout();
            }
        }

        #endregion
    }
}
