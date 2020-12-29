using System.Reflection;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Extractor_Test.TestConfig {
    [TestClass]
    public class TestLog4NetWarningChecker {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [TestCleanup] //after each test
        public void CheckTestLoggers() {
            Assert.IsTrue(ConfigureTestLoggers.WereErrorsLogged(false));
        }

        [TestMethod]
        public void CheckWarn() {
            Log.Warn("bad things have happened");
        }

    }
}