using System.ComponentModel.Composition;
using Utilities.Editor.Editors;
using Utilities.UserConfig;

namespace Utilities.Loggers {
    
    [Config("Logging Config","Configures Logging","logconfig.xml","LogConfig")]
    [Export(typeof(IConfigFile))]
    public class LoggingConfig : IConfigFile{

        [BooleanEditor("Use a log file", "Log messages to a file?", true)]
        public bool UseLogFile { get; set; }

        [StringEditor("Log File","The Log file to log to",true)]
        public string LogFile { get; set; }

        [StringEditor("EventTimer File", "The timer file to log to", true)]
        public string TimerFile { get; set; }

        [BooleanEditor("Throw Exception On Error","If an error is encountered should the program throw an exception to notify the user and halt the execution?",true)]
        public bool ThrowExceptionOnError { get; set; }

        [BooleanEditor("Use a Parallel file Logger", "Parallel logger may miss messages in the event of a crash but is faster. If false a serial logger will be used.", true)]
        public bool UseParallelFileLogger { get; set; }

        #region ctor

        public LoggingConfig() {
            this.LogFile = "log.log";
            this.TimerFile = "timings.log";
            this.ThrowExceptionOnError = true;
            this.UseLogFile = true;
            this.UseParallelFileLogger = true;
        }

        #endregion

        #region Implementation of IConfigFile

        public IConfigFile ReturnDefault() {
            return this;
        }

        public void NotifyUpdated() {
            // nout to do
        }

        #endregion

    }
}
