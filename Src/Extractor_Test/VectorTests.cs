using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Extractor_Test.TestConfig;
using GraphAnalysis.InputPartitioner;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParseTreeExtractor;
using ParseTreeExtractor.Graph;

namespace Extractor_Test {
    [TestClass]
    public class VectorTests : LogTestBase {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [TestMethod, DeploymentItem("TestFiles")]
        [TestFileData("TestFiles", 4, "VectorTest")]
        public void RunVectorTests(string filename) {
            RunTest(filename);
        }

        private static void RunTest(string filename) {
            Log.Info("Starting test for " + filename);
            var extractor = new ExcelExtractor();
            var extraction = extractor.Extract(filename);
            var graph = SpreadGraphCreator.Create(extraction);

            var inputs = InputPartitioner.PartitionInputs(graph, extraction);
            InputPartitioner.PrintInputs(inputs, filename);
            var foundVectors = IsoMorph.Partition(graph, extraction);
        }


        [TestMethod]
        [DataRow(@"B:\enron\spreadsheets\kevin_presto__19959__MarketPowerNE020211.xlsx")]
        [DataRow(@"B:\enron\spreadsheets\darrell_schoolcraft__7403__0202 NNG Lamar Nomination.xlsx")]
        [DataRow(@"B:\enron\spreadsheets\louise_kitchen__22829__Headct Reconcile_020102.xlsx")] 
        [DataRow(@"B:\enron\spreadsheets\sally_beck__35539__MSA_Sales_Volumes_01.xlsx")]
        public void RunSimpleEnrons(string file ) {
            RunTest(file);
        }

        [TestMethod]
        public void RunSpecific() {
            RunTest(@"B:\enron\spreadsheets\sally_beck__35539__MSA_Sales_Volumes_01.xlsx");
        }

        [TestMethod]
        [DeploymentItem("TestFiles")]
        [DataRow("TestFiles\\VectorTests_01.xlsx","Sheet1!D7","82.5")]
        [DataRow("TestFiles\\VectorTests_02_BadRef_Single.xlsx", "Sheet1!B7", "20")]
        public void EndToEndTest(string filename,string address,string value){
            
            Log.Info("Starting test for " + filename);
            var extractor = new ExcelExtractor();
            var extraction = extractor.Extract(filename);
            var graph = SpreadGraphCreator.Create(extraction);

            var inputs = InputPartitioner.PartitionInputs(graph, extraction);
            InputPartitioner.PrintInputs(inputs, filename);
            var foundVectors = IsoMorph.Partition(graph, extraction);
            Assert.IsTrue(foundVectors.FoundVectors.Any());

        }
    }
}