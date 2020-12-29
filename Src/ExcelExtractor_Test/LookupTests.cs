using System;
using System.IO;
using ExcelInterop;
using Excel_Interop_ClosedXML;
using Excel_Interop_COM;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExcelExtractor_Test {
    [TestClass]
    public class LookupTests {
        [TestMethod]
        [DataRow("COM")]
        [DataRow("ClosedXML")]
        public void RunLookupTests(string readerType) {
            var testFile = Directory.GetCurrentDirectory() + @"\TestFiles\LookupTests.csv";
            
                Assert.IsTrue(File.Exists(testFile), "Deployment failed: {0} did not get deployed", testFile);
                UnitTester.RunUnitTests(testFile, fname => GetReader(readerType, fname));
            
        }

        private IExcelReader GetReader(string readerType, string fname)
        {
            if (readerType == "COM")
            {
                return ExcelCOMFile.Factory(fname);
            } else if (readerType == "ClosedXML")
            {
                return ExcelReaderClosedXml.Factory(fname);
            }
            throw new ArgumentNullException(nameof(readerType));
        }
    }
}
