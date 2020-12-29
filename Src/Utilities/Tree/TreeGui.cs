using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Xml.Linq;
using Utilities.Windowing;

namespace Utilities.Tree {

    [Export(typeof(IWindow))]
    [Window(WindowTypeName = "XTree Window",
        WindowTypeDescription = "Displays a XTree",
        InstanceRequired = false,
        AllowMultiple = true)]
    public partial class TreeGui : Form,IWindow {
        public TreeGui() {
            InitializeComponent();
        }

        #region Implementation of IWindow

        /// <summary>
        /// Gets the name of the instance - for example "Edit Window - C:\File.txt"
        /// </summary>
        /// <returns>name of the instance</returns>
        public string GetInstanceName() {
            return this.XTreeView.XTree != null ? this.XTreeView.XTree.Name : "An XTree View";
        }

        /// <summary>
        /// Gets the instance description - for example "Window for editing C:\File.txt HProject File"
        /// </summary>
        /// <returns>instance description</returns>
        public string GetInstanceDescription() {
            return this.XTreeView.XTree != null ? this.XTreeView.XTree.Description : "An XTree View";
        }

        public XElement SaveAsXML() {
            throw new NotImplementedException();
        }

        public XElement LoadFromXML() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Binds a GUI window to a data container by passing it a reference.
        /// </summary>
        /// <param name="objectList">The list of objects to bind to - indexed by name</param>
        public void BindTo(Dictionary<string, object> objectList) {

            if (!objectList.EnsureNameType("Tree", typeof(XTreeView))) {
                return;
            }

            this.XTreeView.XTree = (XTreeView)objectList["Tree"];
            this.Text = this.XTreeView.XTree.Name;

            // now sort out form menu... helpToolStripMenuItem
            this.windowToolStripMenuItem.Text = this.XTreeView.XTree.Name;            
            if (this.XTreeView.XTree.MenuCommands != null) {
                foreach (KeyValuePair<string, Action<XElement>> menuCommand in this.XTreeView.XTree.MenuCommands) {
                    ToolStripItem item = new ToolStripMenuItem(menuCommand.Key) {AutoSize = true};
                    KeyValuePair<string, Action<XElement>> command = menuCommand;
                    item.Click += delegate { command.Value.Invoke(this.XTreeView.FocusItem as XElement); };
                    this.windowToolStripMenuItem.DropDownItems.Add(item);
                }
            }            
        }

        public void RefreshView() {
            this.XTreeView.XTree.UpdateView(); // a nasty backward call
        }

        #endregion

        private void CloseToolStripMenuItemClick(object sender, EventArgs e) {
            this.Close();
        }

        private void HelpToolStripMenuItemClick(object sender, EventArgs e) {
            if (this.XTreeView.XTree != null && !string.IsNullOrWhiteSpace(this.XTreeView.XTree.Description)) {
                MessageBox.Show(this.XTreeView.XTree.Name + " - " + this.XTreeView.XTree.Description);
            } else {
                MessageBox.Show("This tree view did not provide any help message :(");
            }
        }
  
    }
}
