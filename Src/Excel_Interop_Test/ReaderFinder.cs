using System;
using ExcelInterop;
using Excel_Interop_ClosedXML;
using Excel_Interop_COM;

namespace Excel_Interop_Test {
    public static class ReaderFinder {
        public static IExcelReader GetReader(string readerType, string fname) {
            if(readerType =="COM") {
                return ExcelCOMFile.Factory(fname);
            } else if (readerType == "ClosedXML") {
                return ExcelReaderClosedXml.Factory(fname);
            }
            throw new ArgumentNullException(nameof(readerType));
        }
    }
}