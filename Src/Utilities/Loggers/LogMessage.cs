using System;

namespace Utilities.Loggers {
    public class LogMessage {                
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        public LogMessage(LogLevel level, string message) {
            this.Level = level;
            this.Message = message;
            this.DateTime = DateTime.Now;            
        }

        public override string ToString() {
            return string.Format("{0}:{1:000}: [{2}]: {3}", this.DateTime, this.DateTime.Millisecond , this.Level, this.Message);
        }
    }
}