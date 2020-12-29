using System.IO;
using ExcelInterop;
using ExcelInterop.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excel_Interop_Test {
    [TestClass]
    public class ReaderTests {

        private static readonly string Fname = Directory.GetCurrentDirectory() + @"\TestFiles\ReadTests.xlsx";
        [TestMethod]
        [DataRow("COM")]
        [DataRow("ClosedXML")]
        public void CanReadValues(string readerType) {
            using (IExcelReader reader = ReaderFinder.GetReader(readerType, Fname)) {
                Assert.AreEqual("Text", reader.ReadValue(new ExcelAddress("Sheet1!A1")));
                Assert.AreEqual("123", reader.ReadValue(new ExcelAddress("Sheet1!A2")));
                Assert.IsTrue(ExcelValueComparer.CompareExcelValues("12.12", reader.ReadValue(new ExcelAddress("Sheet1!A3"))));
                Assert.IsTrue(ExcelValueComparer.CompareExcelValues("135.12", reader.ReadValue(new ExcelAddress("Sheet1!A4"))));
                Assert.AreEqual("named", reader.ReadValue(new ExcelAddress("Sheet1!A5")));
                Assert.IsTrue(ExcelValueComparer.CompareExcelValues("270.24", reader.ReadValue(new ExcelAddress("Sheet1!A6"))));
            }
        }

        [TestMethod]
        [DataRow("COM")]
        [DataRow("ClosedXML")]
        public void CanReadErrors(string readerType) {
            using (IExcelReader reader = ReaderFinder.GetReader(readerType, Fname)) {
                Assert.AreEqual(ExcelErrors.NA, reader.ReadValue(new ExcelAddress("Sheet1!A7")));
                Assert.AreEqual(ExcelErrors.VALUE, reader.ReadValue(new ExcelAddress("Sheet1!A8")));
                Assert.AreEqual(ExcelErrors.DIV0, reader.ReadValue(new ExcelAddress("Sheet1!A9")));
            }
        }

        [TestMethod]
        [DataRow("COM")]
        [DataRow("ClosedXML")]
        public void CanReadFormulass(string readerType) {
            using (IExcelReader reader = ReaderFinder.GetReader(readerType, Fname)) {
                Assert.AreEqual("=A2+A3",reader.ReadFormula(new ExcelAddress("Sheet1!A4")));
                Assert.AreEqual("=SUM(A2:A4)", reader.ReadFormula(new ExcelAddress("Sheet1!A6")));
                
            }
        }
    }
}
