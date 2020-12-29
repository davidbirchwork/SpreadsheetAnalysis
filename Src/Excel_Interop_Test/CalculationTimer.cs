using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExcelInterop.Domain;
using Excel_Interop_COM;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excel_Interop_Test {
    [TestClass]
    public class CalculationTimer {
        [TestMethod]
        public void RunTest() {
            List<string> fnames = new List<string> {
                @"S:\testModel.xlsm"
            };

            var runcount = 1000;

            Dictionary<string, Action<ExcelCOMFile>> actions = new Dictionary<string, Action<ExcelCOMFile>>() {
                {
                    "noop", (reader) => {
                        var excelAddress = new ExcelAddress("Water!A8");
                        var res = reader.ReadValue(excelAddress);
                    }
                },
                {"calculate", reader => { reader.Calculate(); }}
            };

            foreach (var fname in fnames) {

                foreach (var action in actions) {



                    using (ExcelCOMFile reader = new ExcelCOMFile(fname)) {

                        reader.SetManualCalculation();

                        var timings = Enumerable.Range(1, runcount).Select(r => {
                            var watch = Stopwatch.StartNew();

                            action.Value(reader);


                            watch.Stop();
                            return watch.ElapsedMilliseconds;
                        }).ToList();

                        Console.WriteLine("Action " + action.Key + "Average timing over  " + runcount + " runs " +
                                          fname + " was " +
                                          timings.Average() + "ms");
                    }
                }
            }
        }
    }
}
