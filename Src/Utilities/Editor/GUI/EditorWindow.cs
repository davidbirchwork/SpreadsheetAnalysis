using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Utilities.Loggers;
using Utilities.Tree;
using Utilities.Windowing;

namespace Utilities.Editor.GUI {

    [Export(typeof(IWindow))]
    [WindowAttribute(WindowTypeName = "Editor Window",
        WindowTypeDescription = "Editor Window displays the contents of a node and allows editing of it",
        InstanceRequired = false,
        AllowMultiple = false)]
    public partial class EditorWindow : Form, IWindow {
        private EditController EditController;

        public EditorWindow() {
            InitializeComponent();            
        }

        #region Logging

        private delegate void LogMessageCallback(LogLevel loglevel, string logmessage);

        private void LogUpdateDelegate(LogLevel level, string message) {
            if (!InvokeRequired) {
                this.logBox.AppendText("\r\n" + FormatMessage(level, message));
            } else {
// ReSharper disable RedundantDelegateCreation
                LogMessageCallback d = new LogMessageCallback(LogUpdateDelegate);
// ReSharper restore RedundantDelegateCreation
                this.Invoke(d, new object[] { level, message });
            }
        }

        private static string FormatMessage(LogLevel level, string message) {
            return string.Format("{0}:{1}: [{2}]: {3}", DateTime.Now, DateTime.Now.Millisecond, level, message);
        }

        #region register log delegates

        private void EditorWindowLoad(object sender, EventArgs e) {
            Logger.RegisterDelegate(this.LogUpdateDelegate);
        }

        private void EditorWindowFormClosed(object sender, FormClosedEventArgs e) {
            Logger.RemoveDelegate(this.LogUpdateDelegate);
        }

        #endregion

        #endregion
        
        #region Implementation of IWindow

        /// <summary>
        /// Gets the name of the instance - for example "Edit Window - C:\File.txt"
        /// </summary>
        /// <returns>name of the instance</returns>
        public string GetInstanceName() {
            return "Editor Window";
        }

        /// <summary>
        /// Gets the instance description - for example "Window for editing C:\File.txt Project File"
        /// </summary>
        /// <returns>instance description</returns>
        public string GetInstanceDescription() {
            return "Allows the editing of a class via its xml representation";
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
            this.EditController = (EditController) objectList["EditController"];
        }

        public void RefreshView() {
            // nout to do here
        }

        #endregion        

        private void BtnApplyClick(object sender, EventArgs e) {
            if (this.EditController.Validate(this.XTreeView.DataSource as XElement)) {
                EditCommand editCommand = new EditCommand(this.EditController.Name,
                                                          this.EditController.OldXElement,
                                                          this.XTreeView.DataSource.ToString(),
                                                          this.EditController.SetObject,
                                                          this.EditController.ReturnXml);
                Logger.RemoveDelegate(this.LogUpdateDelegate);
                if (this.EditController.CommandHistory == null) {
                    if (!this.EditController.ReadOnly) {                        
                        Task.Factory.StartNew(() =>
                                              editCommand.Execute());
                        Logger.DEBUG("just launched an edit command without a history - did you mean to provide a history?");
                    } else {                        
                        Logger.FAILURE("This object is read only");
                    }
                } else {
                    Task.Factory.StartNew(() =>
                    this.EditController.CommandHistory.Execute(editCommand));
                }
                this.Close();
            }
        }
    }    
}
