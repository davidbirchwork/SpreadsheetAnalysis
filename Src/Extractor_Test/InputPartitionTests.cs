using System.Linq;
using System.Reflection;
using Extractor_Test.TestConfig;
using GraphAnalysis.InputPartitioner;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParseTreeExtractor;
using ParseTreeExtractor.Graph;

namespace Extractor_Test {
    [TestClass]
    public class InputPartitionTests : LogTestBase {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [TestMethod, DeploymentItem("TestFiles"), TestFileData("TestFiles", 1, "InputVector")]
        public void RunTests(string filename) {
            Log.Info("Starting test for " + filename);
            var extractor = new ExcelExtractor();
            var extraction = extractor.Extract(filename);
            var graph = SpreadGraphCreator.Create(extraction);
            var found = InputPartitioner.PartitionInputs(graph,extraction);
            Assert.IsTrue(found.Any());
        }
    }
}