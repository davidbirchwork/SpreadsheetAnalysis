using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ExcelExtractor;
using ExcelExtractor.Domain;
using ExcelInterop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExcelExtractor_Test {

    public static class UnitTester {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void RunUnitTests(string fileName, Func<string, IExcelReader> readerFactory) {
            ConcurrentBag<string> passedTests = new ConcurrentBag<string>();
            ConcurrentBag<string> failedTests = new ConcurrentBag<string>();
            const int concurrentTests = 1;
            Semaphore s = new Semaphore(concurrentTests, concurrentTests);
            int tests = 0;
            foreach (var teststring in File.ReadAllLines(fileName)) {
                tests++;
                string teststring2 = teststring;
                Task.Factory.StartNew(() => {
                    s.WaitOne();
                    Log.Info("Running Test: " + teststring2);
                    var test = teststring2.Split(new[] {","},
                        StringSplitOptions.RemoveEmptyEntries);

                    // set up the view:
                    // XElement rootElement = SetUpTree();
                    // new code
                    var factory = ExtractionController.CreateExpressionFactory();

                    List<Tuple<string, string>> shims = new List<Tuple<string, string>>();
                    var extractor = new FunctionExtractor(factory, Path.GetDirectoryName(fileName) + "\\" + test[0],
                        shims, 1, readerFactory);
                    Log.Info("Launching Extraction");
                    string teststring1 = teststring2;
                    extractor.BeginExtractionFrom(test[1], () => {
                        extractor.EvaluateAll();
                        ExtractedCell testcell = extractor.ProcessedCells[test[1]];
                        var left = testcell.EvaluatedValue.ToString();
                        var right = testcell.Value;
                        if (testcell.IsEvaluated &&
                            CompareExcelValues(left, right)) {
                            Log.Info( "Passed Test " +teststring1);
                            passedTests.Add(teststring1);
                        }
                        else {
                            Log.Error( "Failed Test" + teststring1);
                            string errormsg = "Expected '" + right +"' received '" +left +"'";
                            Log.Error( errormsg);failedTests.Add(teststring1 + " >>> " + errormsg);}

                        s.Release();
                    });
                });
            }


            while ((passedTests.Count + failedTests.Count) != tests) {
                s.WaitOne();
                s.Release();
            }

            Log.Info("All tests Run!");
            Log.Info("Passed " + passedTests.Count + " Tests");
            Log.Info("Failed " + failedTests.Count + " Tests");
            foreach (string failedTest in failedTests) {
                Log.Error("Failed Test" + failedTest);
            }

            Assert.AreEqual(0,failedTests.Count);

            s.Dispose();

        }

        private static bool CompareExcelValues(string left, string right) {
            if (left == right) return true;
            if (Math.Abs(Convert.ToDouble(left) - Convert.ToDouble(right)) < 0.001) {
                return true;
            }

            return false;
        }
    }
}
