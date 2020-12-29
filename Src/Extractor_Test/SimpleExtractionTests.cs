using System.IO;
using System.Reflection;
using Extractor_Test.TestConfig;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParseTreeExtractor;
using ParseTreeExtractor.Graph;

namespace Extractor_Test {
    [TestClass]
    public class SimpleExtractionTests : LogTestBase {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [TestMethod, DeploymentItem("TestFiles"), TestFileData("TestFiles",5,"Simple")]
        public void RunExportTests(string filename) {
            Log.Info("Starting test for " + filename);
            var extractor = new ExcelExtractor();
            var extraction = extractor.Extract(filename);
            var graph = SpreadGraphCreator.Create(extraction);
            Assert.IsTrue( graph.Nodes.Count > 0);
        }
    }
}