using System;
using System.IO;

namespace Utilities.Loggers {
    public class FileLogger {
        private readonly string _fileName;
        private readonly object _threadlock = new object(); 

        /// <summary>
        /// Registers a File Logger to a particular filename
        /// </summary>
        /// <param name="fileName">log file to use</param>
        public FileLogger(string fileName) {
            this._fileName = fileName;            
            using (StreamWriter swriter = (new FileInfo(this._fileName)).CreateText()) {
                swriter.WriteLine(DateTime.Now + ": New Log file created: " + this._fileName);
            }
//            Logger.RegisterDelegate(this.WritetoLog); this is done in the Loggor ctor as otherwise we call the singleton constructor recursively
        }        

        public void WritetoLog(LogLevel level, string message) {
           lock (_threadlock) {
                using (StreamWriter swriter = (new FileInfo(this._fileName)).AppendText()) {
                    swriter.WriteLine(Logger.Format(level, message));
                }
           }
        }        

    }
}


