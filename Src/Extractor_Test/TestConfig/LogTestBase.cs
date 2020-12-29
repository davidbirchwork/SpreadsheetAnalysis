using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Extractor_Test.TestConfig {
    /// <summary>
    /// Checks if any Warnings or Errors were logged to Log4Net
    /// </summary>
    public class LogTestBase {
        [TestCleanup]
        public void CheckTestLoggers() {
            ConfigureTestLoggers.WereErrorsLogged();
        }

        [TestInitialize]
        public void TestInit() {
            ConfigureTestLoggers.ClearLog();
        }
    }
}