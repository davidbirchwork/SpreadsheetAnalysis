using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Xml.Linq;
using Utilities.Loggers;
using Utilities.Windowing;

namespace GraphGUI.Compound.ColourGraph.GUI.WinForms {
    [Export(typeof(IWindow))]
    [Window(WindowTypeName = "Compound Graph Viewer",
        WindowTypeDescription = "A Window to view a compound graph",
        InstanceRequired = false,
        AllowMultiple = true)]
    public partial class CompoundGraphViewer : Form, IWindow{
        public CompoundGraphViewer() {
            InitializeComponent();
        }

        private ICompoundGraphProvider _provider;

        #region Implementation of IWindow

        public string GetInstanceName() {
            return "Compound Graph Viewer";
        }

        public string GetInstanceDescription() {
            return "A Window to view a compound graph";
        }

        public XElement SaveAsXML() {
            throw new NotImplementedException();
        }

        public XElement LoadFromXML() {
            throw new NotImplementedException();
        }

        public void BindTo(Dictionary<string, object> objectList) {
            if (objectList.ContainsKey("ICompoundGraphProvider")) {
                this._provider = (ICompoundGraphProvider)objectList["ICompoundGraphProvider"];
                this.RefreshView();
            } else {
                Logger.FAILURE("Could not find a ICompoundGraphProvider");
            }
        }

        public void RefreshView() {
            this.GraphViewer.SetGraphProvider(this._provider);
        }

        #endregion
    }
}
