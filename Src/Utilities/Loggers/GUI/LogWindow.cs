using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Utilities.Windowing;

//TODOLIST
// > TODO add a menu to filter messages
// > TODO add a menu item to clear log
// > TODO add a menu item to filter on text
// > TODO add a colour scheme?

namespace Utilities.Loggers.GUI {

    [Export(typeof(IWindow))]
    [WindowAttribute(WindowTypeName = "Log Window",
        WindowTypeDescription="Log Window displays Log messages about the running of the application",    
        InstanceRequired=true,
        AllowMultiple=false)]
    public partial class LogWindow : Form, IWindow {
        private int LogId;

        public LogWindow() {
            InitializeComponent();
        }

        #region register delegates        

        private void LogWindowLoad(object sender, EventArgs e) {
            // set up logger Delegates
          //SINGLE  Logger.RegisterDelegate(this.LogUpdateDelegate);
            this.LogId = Logger.RegisterThreadedLogger(this.LogUpdateDelegate);
        }

        private void LogWindowFormClosed(object sender, FormClosedEventArgs e) {
            //SINGLE  Logger.RemoveDelegate(this.LogUpdateDelegate);
            Logger.DeRegisterThreadedLogger(this.LogId);
        }

        #endregion

        #region Logging

        #region //SINGLE
        /*
          private delegate void LogMessageCallback(LogLevel loglevel, string logmessage);

        private void LogUpdateDelegate(LogLevel level, string message) {
            if (!InvokeRequired) {
                this.LogBox.AppendText("\r\n" + FormatMessage(level, message));
            } else {
                LogMessageCallback d = LogUpdateDelegate;
                this.Invoke(d, new object[] { level, message });
            }
        }
         * 
         * private static string FormatMessage(LogLevel level, string message) {
            return string.Format("{0}:{1:####}: [{2}]: {3}", DateTime.Now, DateTime.Now.Millisecond, level, message); 
        }

         */
        #endregion

        private delegate void LogMessageCallback(int id);

        private void LogUpdateDelegate(int id) {
            if (!InvokeRequired) {
                IEnumerable<LogMessage> messages = Logger.GetMessagesForLogger(id,false);
                int i = messages.Count();
                foreach (var logMessage in messages) {
                    this.LogBox.AppendText("\r\n" +
                                           string.Format("{0}:{1:000}: [{2}]: {3}", logMessage.DateTime,
                                                         logMessage.DateTime.Millisecond, logMessage.Level,
                                                         logMessage.Message));
                }                
                Logger.FinishProcessingMessages(id);
            } else {
                LogMessageCallback d = LogUpdateDelegate;
                this.Invoke(d, new object[] { id });
            }
        }

        

        #endregion

        #region Implementation of IWindow

        /// <summary>
        /// Gets the name of the instance - for example "Edit Window - C:\File.txt"
        /// </summary>
        /// <returns>name of the instance</returns>
        public string GetInstanceName() {
            return "Log Window";
        }

        /// <summary>
        /// Gets the instance description - for example "Window for editing C:\File.txt Project File"
        /// </summary>
        /// <returns>instance description</returns>
        public string GetInstanceDescription() {
            return "Log Window displays Log messages about the running of the application";
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
          // nout to do here
        }

        public void RefreshView() {
            // nout to do here
        }

        #endregion
    }
}
