using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Extractor_Test.TestConfig;
using GraphAnalysis.InputPartitioner;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParseTreeExtractor;
using ParseTreeExtractor.Graph;

// ReSharper disable InconsistentNaming

namespace Extractor_Test {
    [TestClass]
    public class DatasetExportTests : LogTestBase {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [TestMethod]
        public void VectorStats() {
            /*
            Cells=3012
            InputVectorCells=1
            Percentage=0.00
            FoundVector=0
            FoundVectorCells=0
            Percentage=0.00
             */

            var directory = @"B:\enron\spreadsheets\";
            var testFiles = Directory.GetFiles(directory, "*_stats.txt");
            int numTests = testFiles.Length;
            Console.WriteLine("about to explore results of " + numTests + " tests.");
            var res = testFiles.Select(f => {
                var stats = File.ReadAllLines(f).Select(l => l.Split('='))
                    .ToDictionary(k => k[0].Trim(), k => double.Parse(k[1]));
                bool allRan = stats.Count == 7 && stats.ContainsKey("NonBlankCells");
                return new {
                    File = f,
                    Stats = stats,
                    allRan = allRan,
                    InputPercent =    !allRan ? double.NaN :( stats["InputVectorCells"] / stats["NonBlankCells"]),
                    VectorPercent =   !allRan ? double.NaN :( stats["FoundVectorCells"] / stats["NonBlankCells"]),
                    TotalPercentage = !allRan ? double.NaN :( (stats["InputVectorCells"] + stats["FoundVectorCells"]) / stats["NonBlankCells"])
                };
            }).Where(f => f.allRan && f.Stats["NonBlankCells"]>0 ).ToList();
            
            Console.WriteLine("Successful Tests = "+res.Count);
            Console.WriteLine("Average Cells = " + res.Average(i => i.Stats["Cells"]).ToString("F2"));
            Console.WriteLine("Average Cells Non blank = "+res.Average( i=> i.Stats["NonBlankCells"]).ToString("F2"));
            Console.WriteLine("Average InputVectorCells = " + res.Average(i => i.Stats["InputVectorCells"]).ToString("F2"));
            Console.WriteLine("Average FoundVector = " + res.Average(i => i.Stats["FoundVector"]).ToString("F2"));
            Console.WriteLine("Average FoundVectorCells = " + res.Average(i => i.Stats["FoundVectorCells"]).ToString("F2"));
            Console.WriteLine("Average InputPercent = " + res.Average(i => i.InputPercent).ToString("F2"));
            Console.WriteLine("Average VectorPercent = " + res.Average(i => i.VectorPercent).ToString("F2"));
            Console.WriteLine("Average TotalPercentage = " + res.Average(i => i.TotalPercentage).ToString("F2"));

            Console.WriteLine("runs ordered by  highest total percentage where some vectors were found:");
            Console.WriteLine("File,Cells,NonBlank,InputCells,VectorCells,TotalPercent");
            foreach (var f in res.Where(a => a.allRan && a.Stats["FoundVectorCells"]>0).OrderByDescending(f => f.TotalPercentage)) {
                Console.WriteLine(
                    $"{f.File},{f.Stats["Cells"]},{f.Stats["NonBlankCells"]},{f.Stats["InputVectorCells"]},{f.Stats["FoundVectorCells"]},{f.TotalPercentage:F2}");
            }
        }

        [Timeout(120*1000)]
        [TestMethod, TestFileData(@"B:\Enron\spreadsheets\", 40000, "", "", "__")]
        public void RunEnronTests(string filename) {
            bool skipPreviouslyRun = false;
            bool skipPreviousSuccess = false;
            bool onlyRunPreviousSuccess = true;
            bool runIsomorph = true;
            bool extractInputs = true;
            bool skipPreviousVectors = true;



            Log.Info("Starting test for " + filename);
            var graphmlFile = Path.ChangeExtension(filename, "graphml");
            
            string errorFile = Path.ChangeExtension(filename, "._errors.log").Replace("._", ".");
            if (skipPreviouslyRun && File.Exists(errorFile)) {
                Log.Info("Skipping previously run test");
                return;
            }

            if (skipPreviousSuccess && File.Exists(errorFile) && string.IsNullOrEmpty(File.ReadAllText(errorFile)) && File.Exists(graphmlFile)) {
                Log.Info("Skipping previous success");
                return;
            }
            if(onlyRunPreviousSuccess && (!File.Exists(errorFile) || !string.IsNullOrEmpty(File.ReadAllText(errorFile)) || !File.Exists(graphmlFile))) {
                Log.Info("Skipping previous failure");
                return;
            }

            if ((extractInputs || runIsomorph) && skipPreviousVectors &&
                File.Exists(Path.ChangeExtension(filename, "._stats.txt").Replace("._", "_"))) {
                Log.Info("skipping previously analyzed");
            }

            ConfigureTestLoggers.RegisterFileLogger(filename, extractInputs || runIsomorph);

            try {

                var extractor = new ExcelExtractor();
                var extraction = extractor.Extract(filename);
              //  File.WriteAllText(Path.ChangeExtension(filename, "json"), JsonConvert.SerializeObject(extraction));
                var graph = SpreadGraphCreator.Create(extraction);
                
                SpreadGraphExporter.Export(graph, graphmlFile);

                if (extractInputs) {
                    var inputs = InputPartitioner.PartitionInputs(graph, extraction);
                    InputPartitioner.PrintInputs(inputs, filename);
                    int totalCells = inputs.Sum(i => i.Cells.Count);
                    File.WriteAllText(Path.ChangeExtension(filename,"._stats.txt").Replace("._","_"), 

                        "NonBlankCells = "+extraction.CellFormulas.Count(f => !string.IsNullOrWhiteSpace(f.Value))+Environment.NewLine+
                        "Cells="+extraction.Cells.Count+Environment.NewLine+
                        "InputVectorCells=" + totalCells+ Environment.NewLine+
                        "InputPercentage="+(totalCells / (double) extraction.Cells.Count).ToString("F2") );
                }

                if (runIsomorph) {
                    var foundVectors = IsoMorph.Partition(graph, extraction);

                    var sum = foundVectors.FoundVectors.Sum(v => v.Item2.Count);
                    File.AppendAllText(Path.ChangeExtension(filename, "._stats.txt").Replace("._", "_"),
                        Environment.NewLine+"FoundVector=" + foundVectors.FoundVectors.Count + Environment.NewLine +
                        "FoundVectorCells=" + sum + Environment.NewLine +
                        "VectorPercentage=" + (sum / (double)extraction.Cells.Count).ToString("F2"));
                }

            }
            catch (Exception e) {
                Log.Error("Exception ", e);
            }

            ConfigureTestLoggers.CloseLoggers();
        }

        [TestMethod]
        public void RunSpecific() {
            RunEnronTests(@"B:\enron\spreadsheets\barry_tycholiz__859__New Counterparties.xlsx");

            //todo these are known to fail in interesting ways:
         //   RunEnronTests(@"B:\enron\spreadsheets\darrell_schoolcraft__7304__imball0201.xlsx");
            // RunEnronTests(@"B:\enron\spreadsheets\benjamin_rogers__1002__NYISO Price Information version 2.xlsx"); < out of memory but now does not crash! 
            //RunEnronTests(@"B:\enron\spreadsheets\benjamin_rogers__1044__Peaker Valuation 060500.xlsx"); // fails as has array formulas
            //"B:\enron\spreadsheets\kam_keiser__17787__phys procedures-new.errors.log"
            //"B:\enron\spreadsheets\kenneth_lay__19464__ENE111000.errors.log"
            //"B:\enron\spreadsheets\kenneth_lay__19469__ENE112400.errors.log"
            //"B:\enron\spreadsheets\elizabeth_sager__9503__SagerIIMen.xlsx" < out of memory but not as big as i'd expect... < find out why its finding a million cells


            // interesting runs:
            //tori_kuykendall__39983__Quote VVMed 04 - 27 - 01
          //  RunEnronTests(@"B:\enron\spreadsheets\susan_bailey__39391__mercedconfirms.xlsx");
            //B:\enron\spreadsheets\susan_bailey__39391__mercedconfirms_stats.txt cells = 649 Total Percentage = 183.82
            //B:\enron\spreadsheets\susan_bailey__39393__pscoconfirms_stats.txt cells = 1375 Total Percentage = 170.40
            //B:\enron\spreadsheets\thomas_martin__39786__Sandi050801_stats.txt cells = 5106 Total Percentage = 129.61
            //B:\enron\spreadsheets\tom_donohoe__39951__enron1lng_stats.txt cells = 378 Total Percentage = 90.48
            //B:\enron\spreadsheets\susan_bailey__39392__eugeneconfirms_stats.txt cells = 209 Total Percentage = 89.47
        }
        
        [TestMethod]
        public void CalculateStats() {
            var directory = @"B:\enron\spreadsheets\";
            var testFiles = Directory.GetFiles(directory, "*.errors.log");
            int numTests = testFiles.Length;
            Console.WriteLine("about to explore results of " + numTests + " tests.");
            ConcurrentDictionary<string, Outcome> outcomes = new ConcurrentDictionary<string, Outcome>();
            Parallel.ForEach(testFiles, testFile => {
                var text = File.ReadAllText(testFile);
                if (string.IsNullOrWhiteSpace(text)) {
                    outcomes[testFile] = Outcome.PASSED;
                    return;
                }

                if (text.Contains("ClosedXML.Excel.XLPivot")) {
                    outcomes[testFile] = Outcome.EXTERN_ClosedXML_Pivot;
                    return;
                }

                if (text.Contains("at ClosedXML.Excel")) {
                    outcomes[testFile] = Outcome.EXTERN_ClosedXML_Parse_Failed;
                    return;
                }

                if (text.Contains("Vlookup", StringComparison.OrdinalIgnoreCase)) {
                    outcomes[testFile] = Outcome.EXTERN_ClosedXML_VlookupFailed;
                    return;
                }

                if (text.Contains("Array Formulas are Not Supported")) {
                    outcomes[testFile] = Outcome.EXTERN_XlParser_ArrayFormula;
                    return;
                }

                if (text.Contains("at XLParser.ExcelFormulaParser")) {
                    outcomes[testFile] = Outcome.EXTERN_XlParser_Failed;
                    return;
                }

                if (text.Contains("ERROR - found a strange reference function call")) {
                    outcomes[testFile] = Outcome.BUG_StrangeReference;
                    return;
                }

                if (text.Contains("ERROR - found a simple reference that is more complex")) {
                    outcomes[testFile] = Outcome.BUG_ComplexReference;
                    return;
                }

                if (text.Contains("System.NullReferenceException") &&
                    text.Contains(@"B:\ExcelAnalytics\ParseTreeExtractor\ExcelExtractor.cs:line 322")) {
                    outcomes[testFile] = Outcome.BUG_PrefixReferenceNull;
                    return;
                }

                if (text.Contains("ERROR - found a weird named range")) {
                    outcomes[testFile] = Outcome.BUG_WeirdNamedRange;
                    return;
                }

                if (text.Contains("at Newtonsoft.Json")) {
                    outcomes[testFile] = Outcome.BUG_JsonSerialization;
                    return;
                }

                if (text.Contains("Exception of type 'System.OutOfMemoryException' was thrown") || 
                    text.Contains("more than a million cells")) {
                    outcomes[testFile] = Outcome.EXTERN_OutOfMemory;
                    return;
                }
                
                if (text.Contains("System.ArgumentOutOfRangeException: no known source node")) {
                    if (text.Contains("WARNING Reading values from Pivot Tables is not supported")) {
                        outcomes[testFile] = Outcome.EXTERN_ClosedXML_Pivot;
                        return;
                    }
                    outcomes[testFile] = Outcome.BUG_NoKnownNode;
                    return;
                }

                if (text.Contains("at ParseTreeExtractor.Domain.SpreadGraph.AddEdge")) {
                    if (text.Contains("WARNING Reading values from Pivot Tables is not supported")) {
                        outcomes[testFile] = Outcome.EXTERN_ClosedXML_Pivot;
                        return;
                    }
                    outcomes[testFile] = Outcome.BUG_NoKnownEdge;
                    return;
                }

                outcomes[testFile] = Outcome.Other;
            });

            var grouped = outcomes.GroupBy(o => o.Value).OrderBy(a => a.Key);
            Console.WriteLine(grouped.Aggregate("Total Results:",
                (acc, next) =>
                    $"{acc}{Environment.NewLine}{next.Key}:{next.Count()} ({(100.0 * (next.Count() / (double) numTests)):N2}%)"));
            var passed = outcomes.Count(o => o.Value == Outcome.PASSED);
            var bugged = outcomes.Count(o => o.Value.ToString().Contains("BUG"));

            Console.WriteLine();
            var result = 100.0 * (passed / (double) (passed + bugged));
            Console.WriteLine($"Final score = {result:N2}%");
            Console.WriteLine();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("file, outcome");
            foreach (var o in outcomes.OrderBy(o => o.Value)) {
                sb.AppendLine(o.Key + "," + o.Value);
            }

            File.WriteAllText(directory + "summary.csv", sb.ToString());

            foreach (var other in outcomes.Where(o => o.Value == Outcome.Other).Take(10)) {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(other.Key);
                Console.WriteLine();
                Console.WriteLine(File.ReadAllText(other.Key));

            }

            Assert.IsTrue(result > 70,"Not enough tests passed");
        }

        public enum Outcome {
            PASSED,
            EXTERN_ClosedXML_Pivot,
            EXTERN_ClosedXML_Parse_Failed,
            EXTERN_ClosedXML_VlookupFailed,
            EXTERN_XlParser_Failed,
            EXTERN_XlParser_ArrayFormula,
            BUG_NoKnownNode,
            BUG_StrangeReference,
            BUG_ComplexReference,
            BUG_PrefixReferenceNull,
            BUG_WeirdNamedRange,
            EXTERN_OutOfMemory,
            BUG_JsonSerialization,
            BUG_NoKnownEdge,
            Other
        }
    }
}
