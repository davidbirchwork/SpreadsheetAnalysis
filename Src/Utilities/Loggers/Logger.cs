using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using LinqExtensions.Extensions;
// ReSharper disable InconsistentNaming

namespace Utilities.Loggers {

    // ReSharper disable RedundantNameQualifier
    // ReSharper disable UnusedMember.Global
    /// <summary>
    /// Log level - in order of increasing severity
    /// </summary>
    public enum LogLevel {
        ///<summary>
        /// Debug - info a user would not normally see
        ///</summary>
        DEBUG, 
        ///<summary>
        ///</summary>
        INFO, SUCCESS, FAILURE, ERROR
    }

    //todo the log system is causing far too much locking between threads when it tries to access GUI stuff. 
    // we should use an array of concurrent queues and make log delegates clear the queue when they are called - only calling them once - so only one thread sticks on invoke
    // alternative is to make GUI thread poll the events but this is likely to be inefficient.
   
    /// <summary>
    /// Logger class used for external and internal logging / user notifications
    /// </summary>    
    public class Logger {

        #region singleton and ctor        

        // Explicit static constructor to tell C# compiler
        // not to mark type as before field init
// ReSharper disable EmptyConstructor
        static Logger() {
// ReSharper restore EmptyConstructor
        }

        private readonly FileLogger _filelogger;

        private Logger() {
            this._throwExceptionOnError = UserConfig.UserConfig.Configure<LoggingConfig>().ThrowExceptionOnError;                
          
            // register file logger ! 
            if (UserConfig.UserConfig.Configure<LoggingConfig>().UseLogFile) {

                if (UserConfig.UserConfig.Configure<LoggingConfig>().UseParallelFileLogger) {
                    ParallelFileLogger parallelFileLogger = new ParallelFileLogger(UserConfig.UserConfig.Configure<LoggingConfig>().LogFile);
                    //parallelFileLogger.RegisterParallelLogger();   this doesnt work since it calls back to the logger which is still being constructed
                    parallelFileLogger.LogId = this.RegisterAThreadedLogger(parallelFileLogger.LogUpdateDelegate);
                } else {
                    _filelogger = new FileLogger(UserConfig.UserConfig.Configure<LoggingConfig>().LogFile);
                    this.OnLogMessage += _filelogger.WritetoLog;
                }

            }            
        }        

// ReSharper disable InconsistentNaming
        private static readonly Logger instance = new Logger();       
// ReSharper restore InconsistentNaming

        private static Logger Instance {
            get {
                return instance;
            }
        }       
                
        private readonly bool _throwExceptionOnError;

        #endregion

        #region Single Threaded Logging        

        /// <summary>
        /// Create delegates of this type to handle internal log messages
        /// use registerDelegate to add delegates
        /// </summary>
        /// <param name="level">severity level of the log</param>
        /// <param name="msg">message</param>
        public delegate void LogHandler(LogLevel level, string msg);

        /// <summary>
        /// attach to this event to handle messages passed inside the app
        /// use registerDelegate to add delegates
        /// </summary>
        public event LogHandler OnLogMessage;

        #region Register/DeRegister Loggers        

        /// <summary>
        /// Call this to register message handling delegates
        /// </summary>
        /// <param name="del">delegate to add</param>
        public static void RegisterDelegate(LogHandler del) {
            Instance.OnLogMessage += del;            
        }

        /// <summary>
        /// Call this to remove a delegate
        /// </summary>
        /// <param name="del">delegate to remove</param>
        public static void RemoveDelegate(LogHandler del) {
            Instance.OnLogMessage -= del;
        }

        #endregion                
        
        #endregion

        private void HandleMessage(LogLevel debug, string str) {
            if (this.OnLogMessage != null) {
                this.OnLogMessage(debug, str); // single threaded code
            }
            // create a log message
            LogMessage logMessage = new LogMessage(debug,str);

            // queue message to all loggers
            foreach (var queue in _concurrentQueues) {
                queue.Value.Item2.Enqueue(logMessage);                
            }
            // then tell them its here
            foreach (var queue in _concurrentQueues) {
                // if the handler hasn't currently been called then 
                if (!queue.Value.Item2.IsEmpty) {
                    if (queue.Value.Item1.WaitOne(0)) {
                        queue.Value.Item3(queue.Key);
                        Interlocked.Increment(ref _called);
                    } else {
                        Interlocked.Increment(ref _notcalled);
                    }
                }
            }
        }

        #region multi-threaded Logging

        private readonly object _idlock = new object();
        private int _maxID;

        private readonly ConcurrentDictionary<int, Tuple<Semaphore, ConcurrentQueue<LogMessage>, CallBackLogHandler>> _concurrentQueues =
                new ConcurrentDictionary<int, Tuple<Semaphore, ConcurrentQueue<LogMessage>, CallBackLogHandler>>();
        private int _called;
        private int _notcalled;
        private int _calledFromGetMessages;

        public delegate void CallBackLogHandler(int id);

        public static IEnumerable<LogMessage> AccessMessages(int id) {
            return Instance.GetMessages(id);
        }

        #region register/deregister

        public static int RegisterThreadedLogger(CallBackLogHandler callback) {
            return Instance.RegisterAThreadedLogger(callback);
            
        }

        private int RegisterAThreadedLogger(CallBackLogHandler callback) {
            int id;
            lock (this._idlock) {
                Interlocked.Increment(ref this._maxID);
                id = this._maxID;
            }
            this._concurrentQueues.AddOrThrow(id,
                                              new Tuple
                                                  <Semaphore, ConcurrentQueue<LogMessage>,
                                                  CallBackLogHandler>(new Semaphore(1, 1),
                                                                      new ConcurrentQueue<LogMessage>(),
                                                                      callback));
            return id;
        }

        public static bool DeRegisterThreadedLogger(int id) {
            Tuple<Semaphore, ConcurrentQueue<LogMessage>, CallBackLogHandler> queue;
            return Instance._concurrentQueues.TryRemove(id, out queue);
        }

        #endregion

        public static void LogLoggerStatistics() {
            Instance.LogLoggerStats();
        }

        private void LogLoggerStats() {
            DEBUG("Times we called callbacks " + this._called);
            DEBUG("Times we called didn't callback " + this._notcalled);
            DEBUG("Times called from GetMessages " + this._calledFromGetMessages);
        }

        public static IEnumerable<LogMessage> GetMessagesForLogger(int id, bool releaseLock = true) {
            return Instance.GetMessages(id, releaseLock);
        }

        private IEnumerable<LogMessage> GetMessages(int id, bool releaseLock = true) {
            List<LogMessage> messages = new List<LogMessage>();

            //get Messages

            var queue = this._concurrentQueues[id].Item2;
            while (!queue.IsEmpty) {
                LogMessage message;
                if (queue.TryDequeue(out message)) {
                    messages.Add(message);
                }
            }

            if (releaseLock) {
                FinishProcessingMessagesForLogger(id);
            }

            return messages;
        }

        public static void FinishProcessingMessages(int id) {
            Instance.FinishProcessingMessagesForLogger(id);
        }

        private void FinishProcessingMessagesForLogger(int id) {
            // we've finished finding messages 
            this._concurrentQueues[id].Item1.Release();

            // clear up any messages that might have been added between last check and release of semaphore

            if (!this._concurrentQueues[id].Item2.IsEmpty) {
                if (this._concurrentQueues[id].Item1.WaitOne(0)) {
                    this._concurrentQueues[id].Item3(id);
                    Interlocked.Increment(ref _calledFromGetMessages);
                }
            }
        }

        #endregion

        /// <summary>
        /// format message for logging
        /// </summary>
        /// <param name="level">Log Level</param>
        /// <param name="message">message </param>
        /// <returns>formatted message</returns>
        public static string Format(LogLevel level, string message) {
            return string.Format("{0}:{1:0000}:{2}: [{3}]: {4}", DateTime.Now.ToShortTimeString(), DateTime.Now.Second, String.Format("{0:000}", DateTime.Now.Millisecond), level, message);
        }

        #region Log methods        

        /// <summary>
        /// low level logging only persisted to .log files 
        /// </summary>
        /// <param name="str"></param>
        public static bool DEBUG(string str) {
            Instance.HandleMessage(LogLevel.DEBUG, str);
            return false;
        }        

        /// <summary>
        /// low level logging maybe persisted to user
        /// </summary>
        /// <param name="str"></param>
        public static void INFO(string str) {
            Instance.HandleMessage(LogLevel.INFO, str);
        }

        /// <summary>
        /// log level persisted to user
        /// </summary>
        /// <param name="str"></param>
        public static bool SUCCESS(string str) {
            Instance.HandleMessage(LogLevel.SUCCESS, str);
            return true;
        }

        /// <summary>
        /// log level persisted to user
        /// </summary>
        /// <param name="str"></param>
        public static bool FAILURE(string str) {
            Instance.HandleMessage(LogLevel.FAILURE, str);
            return false;
        }

        /// <summary>
        /// Log an ERROR message
        /// Throw an exception to halt execution 
        /// </summary>
        /// <param name="str"></param>
        public static bool ERROR(string str) {
            Instance.HandleMessage(LogLevel.ERROR, str);
            if (Instance._throwExceptionOnError) {
                throw new Exception(str);
            }
            return false;
        }

        #endregion
    }

    // ReSharper restore UnusedMember.Global
    // ReSharper restore RedundantNameQualifier
}