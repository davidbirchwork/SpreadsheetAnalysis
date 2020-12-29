using System.Collections.Generic;
using ExcelInterop.Domain;

namespace ExcelInterop {
    /// <summary>
    /// A reader which can read the whole of a spreadsheet
    /// for example returning the used ranges for each sheet
    /// </summary>
    public interface IExcelWholeReader : IExcelReader {

        /// <summary>
        /// Gets the sheet names.
        /// </summary>
        /// <returns>a list of sheet names</returns>
        List<string> GetSheetNames();

        /// <summary>
        /// Returns the range of used cells within the worksheet
        /// </summary>
        /// <param name="sheet">The sheet name.</param>
        /// <returns>range of used cells </returns>
        ExcelAddress GetUsedCells(string sheet);
    }
}