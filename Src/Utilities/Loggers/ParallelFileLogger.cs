using System;
using System.Collections.Generic;
using System.IO;

namespace Utilities.Loggers {
    public class ParallelFileLogger {
        private readonly string _fileName;
        internal int LogId;

        /// <summary>
        /// Registers a Parallel File Logger to a particular filename
        /// </summary>
        /// <param name="fileName">log file to use</param>
        public ParallelFileLogger(string fileName) {
            this._fileName = fileName;
            using (StreamWriter swriter = (new FileInfo(this._fileName)).CreateText()) {
                swriter.WriteLine(DateTime.Now + ": New Log file created: " + this._fileName);
            }        
        }

        public void RegisterParallelLogger() {
            this.LogId = Logger.RegisterThreadedLogger(this.LogUpdateDelegate);
        }

        public void DeRegisterParallelLogger() {
            Logger.DeRegisterThreadedLogger(this.LogId);
        }

        public void LogUpdateDelegate(int id) {
            IEnumerable<LogMessage> messages = Logger.GetMessagesForLogger(id,false);
            using (StreamWriter swriter = (new FileInfo(this._fileName)).AppendText()) {
                foreach (var logMessage in messages) {


                    swriter.WriteLine(string.Format("{0}:{1:0000}: [{2}]: {3}", logMessage.DateTime,
                                                     logMessage.DateTime.Millisecond, logMessage.Level,
                                                     logMessage.Message));
                }                
            }

            Logger.FinishProcessingMessages(id);
        }
    }
}


