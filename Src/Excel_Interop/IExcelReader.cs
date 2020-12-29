using System;
using System.Collections.Generic;
using ExcelInterop.Domain;

namespace ExcelInterop {
    public interface IExcelReader : IDisposable {
        
        /// <summary>
        /// Read a set of values from the excel sheet
        /// </summary>
        /// <param name="cells">list of cells</param>
        /// <returns>list of string values</returns>
        Dictionary<ExcelAddress, string> ReadValues(IEnumerable<ExcelAddress> cells);

        Dictionary<string, string> ReadValues(Dictionary<string, ExcelAddress> excelReadValues, string notFoundValue = null);
        string ReadFormula(ExcelAddress address);

        /// <summary>
        /// Finds the Excel address a name refers to
        /// </summary>
        /// <param name="globalName">Name of the global.</param>
        /// <returns></returns>
        ExcelAddress FindName(string globalName);

        string ReadValue(ExcelAddress address);
        string ReadGivenName(ExcelAddress address);
    }
}