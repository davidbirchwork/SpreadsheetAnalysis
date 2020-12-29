using System.Collections.Generic;
using System.Linq;
using ExcelInterop.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excel_Interop_Test {
    [TestClass]
    public class CellNameTests {
        private readonly Dictionary<string, int> tests = new Dictionary<string, int>() {
            {"A1", 1},
            {"E1", 5},
            {"z1", 26},
            {"AA5", 27},
            {"DM27", 117},
            {"ABV2", 750}
        };

        [TestMethod]
        public void ColumnNumber() {
            foreach (var test in tests) {
                Assert.AreEqual(test.Key.Where(char.IsLetter).Aggregate("", (acc, next) => acc+next).ToUpperInvariant(),
                                  CellName.IntToColumn(test.Value));
            }
        }

        [TestMethod]
        public void ColumnLetter() {   
            foreach (var test in tests) {
                CellName cellName = new CellName(test.Key);
                Assert.AreEqual(test.Value, cellName.ColumnNumber());
            }
        }
    }
}