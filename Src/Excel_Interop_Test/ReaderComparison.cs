using System;
using System.IO;
using ExcelInterop.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excel_Interop_Test
{
    [TestClass]
    public class ReaderComparison
    {
        [TestMethod]
   //     [DataRow("COM", "ClosedXML", @"\TestFiles\LookupTests.xlsx", @"\TestFiles\LookupTests_Copy.xlsx", "VLOOKUP!A1:C10")]
        [DataRow("COM", "ClosedXML", @"\TestFiles\LookupTests.xlsx", @"\TestFiles\LookupTests_Copy.xlsx","VLOOKUP!B12:B18")]
        [DataRow("COM", "ClosedXML", @"\TestFiles\LookupTests.xlsx", @"\TestFiles\LookupTests_Copy.xlsx","VLOOKUP!A12:A18")]
        [DataRow("COM", "ClosedXML", @"\TestFiles\LookupTests.xlsx", @"\TestFiles\LookupTests_Copy.xlsx","MATCH!A23:B23")]
        public void ReaderValueComparer(string left, string right, string lfname, string rfname, string range){
            using (var leftr = ReaderFinder.GetReader(left, Directory.GetCurrentDirectory() + lfname)) {
                using (var rightr = ReaderFinder.GetReader(right, Directory.GetCurrentDirectory() + rfname)) {
                    
                    ExcelAddress r = new ExcelAddress(range);
                    foreach (var cell in ExcelAddress.ExpandRangeToExcelAddresses(r)) {
                        var leftv = leftr.ReadValue(cell);
                        var rightv = rightr.ReadValue(cell);
                        Assert.IsTrue(ExcelValueComparer.CompareExcelValues(leftv, rightv), " Cell" + cell);
                    }
                }
            }            
        }

        [TestMethod]
        [DataRow("COM", "ClosedXML", @"\TestFiles\LookupTests.xlsx", @"\TestFiles\LookupTests_Copy.xlsx", "VLOOKUP!A12:A18")]
        public void ReaderFormulaComparer(string left, string right, string lfname, string rfname, string range) {
            using (var leftr = ReaderFinder.GetReader(left, Directory.GetCurrentDirectory() + lfname)) {
                using (var rightr = ReaderFinder.GetReader(right, Directory.GetCurrentDirectory() + rfname)) {

                    ExcelAddress r = new ExcelAddress(range);
                    foreach (var cell in ExcelAddress.ExpandRangeToExcelAddresses(r)) {
                        var leftv = leftr.ReadFormula(cell);
                        var rightv = rightr.ReadFormula(cell);
                        Assert.IsTrue(ExcelValueComparer.CompareExcelValues(leftv, rightv), " Cell" + cell);
                    }
                }
            }
        }
    }
}
