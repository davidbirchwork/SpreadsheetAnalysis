using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Xml.Linq;
using Utilities.Windowing;

namespace Utilities.Command {
    /// <summary>
    /// An IWindow for viewing and interacting with a command history
    /// </summary>
    [Export(typeof(IWindow))]
    [WindowAttribute(WindowTypeName = "Command History Window",
        WindowTypeDescription = "Command History Window shows the list of commands executed so far.",
        InstanceRequired = true,
        AllowMultiple = false)]
    public partial class HistoryViewer : Form, IWindow { 
        private CommandHistory _history;
        public HistoryViewer() {
            this.InitializeComponent();            
        }

        private void UpdateViews() {
            if (this._history != null) {
                this.CommandHistoryBox.Text = this._history.PrintHistory(this.checkFullHistory.Checked);
                this.UndoStackBox.Text = this._history.PrintUndoStack();
                //scroll to end
                this.UndoStackBox.SelectionStart = this.UndoStackBox.TextLength;
                this.UndoStackBox.ScrollToCaret();
            }
        }

        private void BtnCleanClick(object sender, EventArgs e) {
            if (this._history != null) {
                if (MessageBox.Show(
                    "Cleaning a history means that no commands which have been undone will be stored in the history. \n" +
                    " The Undo Stack will also be cleared. \n" +
                    "Do you want to Continue",
                    "Clean History?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    this._history.Clean();
                    UpdateViews();
                }
            }
        }

        private void BtnDeleteHistoryClick(object sender, EventArgs e) {
            if (this._history != null) {
                if (MessageBox.Show(
                    "Removing history means that all records of the do / undo / redo commands executed so far will be lost. \n" +
                    "You will not be able to undo any commands you have executed to this point. \n" +
                    " Do you want to Continue",
                    "Delete History?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    this._history.DeleteHistory();
                    UpdateViews();
                }
            }
        }

        private void BtnUndoClick(object sender, EventArgs e) {
            if (this._history != null && this._history.Undo()) {
                UpdateViews();
            }
        }

        private void BtnRedoClick(object sender, EventArgs e) {
            if (this._history != null && this._history.Redo()) {
                UpdateViews();
            }
        }

        private void CheckFullHistoryCheckedChanged(object sender, EventArgs e) {
            this.UpdateViews();
        }

        #region Implementation of IWindow

        /// <summary>
        /// Gets the name of the instance - for example "Edit Window - C:\File.txt"
        /// </summary>
        /// <returns>name of the instance</returns>
        public string GetInstanceName() {
            return "Command History";
        }

        /// <summary>
        /// Gets the instance description - for example "Window for editing C:\File.txt Project File"
        /// </summary>
        /// <returns>instance description</returns>
        public string GetInstanceDescription() {
            return "Command History window - view commands undo and redo them";
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
            if (!objectList.EnsureNameType("CommandHistory", typeof(CommandHistory))) {
                return;
            }

            this._history = (CommandHistory)objectList["CommandHistory"];

            UpdateViews();

        }

        public void RefreshView() {
            UpdateViews();
        }

        #endregion        
    }
}