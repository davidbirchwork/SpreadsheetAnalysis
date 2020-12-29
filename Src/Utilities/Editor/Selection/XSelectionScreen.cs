using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Xml.Linq;
using Infralution.Controls.VirtualTree;
using Utilities.Editor.Editors;
using Utilities.Tree;
using Utilities.Windowing;

namespace Utilities.Editor.Selection {

    [Export(typeof(IWindow))]
    [Window(WindowTypeName = "Selection Window",
        WindowTypeDescription = "A Window to Select Items of a Tree",
        InstanceRequired = false,
        AllowMultiple = true)]
    public partial class XSelectionScreen : Form, IWindow  {
        public XSelectionScreen() {
            InitializeComponent();
        }        

        #region Implementation of IWindow

        public string GetInstanceName() {
            return "Selection Window";
        }

        public string GetInstanceDescription() {
            return "A Window to Select Items of a Tree";
        }

        public XElement SaveAsXML() {
            throw new NotImplementedException();
        }

        public XElement LoadFromXML() {
            throw new NotImplementedException();
        }

        public void BindTo(Dictionary<string, object> objectList) {
            if (!objectList.EnsureNameType("Tree", typeof(XTreeView)) || 
                !objectList.EnsureNameType("CallBack", typeof(Action<List<string>>)) ||
                !objectList.EnsureNameType("NameSelector", typeof(Func<XElement,string>))) {
                return;
            }

            this.selectionTree.XTree = (XTreeView)objectList["Tree"];
            this.Text = this.selectionTree.XTree.Name;
            this.CallBack = (Action<List<string>>)objectList["CallBack"];
            this.NameSelector = (Func<XElement, string>)objectList["NameSelector"];

            if (objectList.ContainsKey("InitialSelection")) {
                this._initialSelection = (List<string>) objectList["InitialSelection"];
                this._selectedNames = new List<string>(this._initialSelection.ToArray());// hacky clone                
            }

            // now lets add a Boolean  editor to do the selection
            BooleanEditor selector = new BooleanEditor("Selection",
                                                       "Select items here", false);
            selector.GetValue = element =>   this._selectedNames.Contains(this.NameSelector.Invoke(element));
            selector.OnSetTrue = element =>  this._selectedNames.Add(     this.NameSelector.Invoke(element));
            selector.OnSetFalse = element => this._selectedNames.Remove(  this.NameSelector.Invoke(element));

            this.selectionTree.XTree.AddColumns((new List<AEditor> { selector }));
        }

        private Action<List<string>> CallBack { get; set; }
        private List<string> _initialSelection = new List<string>();
        private List<string> _selectedNames = new List<string>();
        private Func<XElement, string> NameSelector { get; set; }

        public void RefreshView() {
            this._selectedNames = new List<string>(this._initialSelection.ToArray());
            this.selectionTree.XTree.UpdateView();
        }

        #endregion

        #region Window Interactions

        private void BtnAcceptClick(object sender, EventArgs e) {
            this.CallBack.Invoke(this._selectedNames);
            this.Close();            
        }        

        private void BtnCancelClick(object sender, EventArgs e) {
            this.Close();            
        }

        private void BtnSelectNoneClick(object sender, EventArgs e) {
            this._selectedNames = new List<string>();
            this.selectionTree.UpdateRowData();
        }

        private void BtnSelectAllClick(object sender, EventArgs e) {
            this.SetAllItems(this.selectionTree.RootRow,true);
            this.selectionTree.UpdateRowData();
        }

        private void SetAllItems(Row rootRow, bool select) {
            if (rootRow == null) return;

            //set the item
            XElement elem = (XElement) rootRow.Item;
            string name = this.NameSelector(elem);

            if (name != null) {
                if (select) {
                    if (!this._selectedNames.Contains(name)) {
                        this._selectedNames.Add(name);
                    }
                } else {
                    if (this._selectedNames.Contains(name)) {
                        this._selectedNames.Remove(name);
                    }
                }
            }

            // now recurse
            rootRow.Expand();
            for (int r = 0; r < rootRow.NumChildren; r++) {
                SetAllItems(rootRow.ChildRowByIndex(r), select);
            }
        }

        private void BtnSelectChidlrenClick(object sender, EventArgs e) {
            SetAllItems(this.selectionTree.SelectedRow,true);
            this.selectionTree.UpdateRowData();
        }

        private void BtnDeslectChildrenClick(object sender, EventArgs e) {
            SetAllItems(this.selectionTree.SelectedRow, false);
            this.selectionTree.UpdateRowData();
        }

        #endregion
    }
}
