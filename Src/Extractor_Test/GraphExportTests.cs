using System.IO;
using System.Reflection;
using Extractor_Test.TestConfig;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParseTreeExtractor;
using ParseTreeExtractor.Graph;

namespace Extractor_Test {
    [TestClass]
    public class GraphExportTests : LogTestBase {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        [TestMethod, DeploymentItem("TestFiles"), TestFileData("TestFiles", 15,"Simple")]
        public void RunGraphCreation(string filename) {
            Log.Info("Starting test for " + filename);
            var extractor = new ExcelExtractor();
            var extraction = extractor.Extract(filename);
            var graph = SpreadGraphCreator.Create(extraction);
            SpreadGraphExporter.Export(graph, Path.ChangeExtension(filename, "graphml"));
        }

        [TestMethod]
        public void TestSpecific() {
        //  RunGraphCreation(@"B:\ExcelAnalytics\TestFiles\Simple_16_PseudoFormulas.xlsx");
        }

    }
}
