using System.Collections.Generic;
using ExcelInterop.Domain;

namespace ExcelInterop {
    public interface IExcelReaderWriter : IExcelReader {
        /// <summary>
        /// Write a set of values to a excel sheet
        /// </summary>
        /// <param name="cellstoUpdate">SheetNames,cell names , values</param>
        void SetValues(Dictionary<ExcelAddress, string> cellstoUpdate);
    }
}