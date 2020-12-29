using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Extractor_Test.TestConfig {
    [TestClass]
    public static class ConfigureTestLoggers {
        private static MemoryAppender failAppender;
        [AssemblyInitialize]
        public static void Configure(TestContext tc) {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            
            failAppender = new MemoryAppender {Threshold = Level.Warn};
            BasicConfigurator.Configure(logRepository, failAppender);
            
            var logger = LogManager.GetLogger(typeof(ConfigureTestLoggers));
            logger.Info("Unit test starting");            
        }

        private static FileAppender _currentErrorLog;
        private static FileAppender _currentAllLog;

        public static void RegisterFileLogger(string filename, bool extension = false) {
            CloseLoggers();
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

            LevelMatchFilter filter = new LevelMatchFilter {LevelToMatch = Level.All};
            filter.ActivateOptions();

            var errorFile = Path.ChangeExtension(filename, extension ? "._extra_errors.log"  :"._errors.log").Replace("._", ".");
            if(File.Exists(errorFile)) { File.Delete(errorFile);}
            _currentErrorLog = new FileAppender {
                File = errorFile,
                AppendToFile = true,
                Threshold = Level.Warn,
                Layout = new PatternLayout("%date %-5level - %message%newline") 
                
        };
            _currentErrorLog.ActivateOptions();

            BasicConfigurator.Configure(logRepository, _currentErrorLog);

            var allFile = Path.ChangeExtension(filename, extension ? "._extra_all.log" : "._all.log").Replace("._", ".");
            if (File.Exists(allFile)) { File.Delete(allFile); }
            _currentAllLog = new FileAppender
            {
                File = allFile,
                AppendToFile = true,
                Threshold = Level.All,
                Layout = new PatternLayout("%date %-5level - %message%newline")
            };
            _currentAllLog.ActivateOptions();

            BasicConfigurator.Configure(logRepository, _currentAllLog);

            var logger = LogManager.GetLogger(typeof(ConfigureTestLoggers));
            logger.Info("New Loggers Registered");
        }

        public static void CloseLoggers() {
            _currentErrorLog?.Close();
            _currentAllLog?.Close();
        }

        public static bool WereErrorsLogged(bool assertFailure = true) {
            if (!failAppender.GetEvents().Any()) return false;

            if (assertFailure) {
                Assert.Fail("There were errors logged, so failing the test");
            }
            return true;
        }

        public static void ClearLog() {
            failAppender.Clear();
        }
    }
}