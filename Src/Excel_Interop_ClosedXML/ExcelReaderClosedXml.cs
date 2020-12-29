using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ClosedXML.Excel;
using ExcelInterop;
using ExcelInterop.Domain;
using log4net;

namespace Excel_Interop_ClosedXML {
    public class ExcelReaderClosedXml : IExcelWholeReader {

        private readonly string _filename;
        private readonly bool _deleteFileOnDispose;

        #region IExcelReader

        public static IExcelReader Factory(string filename) {
            return new ExcelReaderClosedXml(filename, true);
        }

        public static IExcelWholeReader FactoryWhole(string filename) {
            return new ExcelReaderClosedXml(filename, true);
        }

        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly XLWorkbook _wb;

        public ExcelReaderClosedXml(string filename, bool copyFirst) {
            if (!File.Exists(filename)) throw new ArgumentNullException(nameof(filename), "Cannot access file");
            if (copyFirst) {
                var tempFilename = Path.GetTempFileName();
                tempFilename = Path.ChangeExtension(tempFilename, ".xlsx");
                File.Copy(filename, tempFilename);
                Thread.Sleep(1000);
                this._filename = tempFilename;
                this._deleteFileOnDispose = true;
            }

            this._wb = new XLWorkbook(this._filename);

        }

        public void Dispose() {
            this._wb?.Dispose();
            if (this._deleteFileOnDispose) {
                File.Delete(this._filename);
            }

        }

        public Dictionary<ExcelAddress, string> ReadValues(IEnumerable<ExcelAddress> cells) {
            return cells.ToDictionary(c => c, ReadValue);
        }

        public Dictionary<string, string> ReadValues(Dictionary<string, ExcelAddress> excelReadValues,
            string notFoundValue = null) {
            return excelReadValues.ToDictionary(c => c.Key, c => ReadValue(c.Value) ?? notFoundValue);
        }

        public string ReadFormula(ExcelAddress address) {
            IXLCell cell = FindCell(address);
            
            if (cell.HasFormula) {
                string value = cell.CachedValue?.ToString();
                if (value == "#REF!") { return "=NA()"; }
                if (value == "#NAME?") { return "=NA()"; }
                return "=" + cell.FormulaA1; // :-p
            }
            else {
                //return null;
                return ReadValue(address); // todo this is weird but it is what the Excel COM property does
            }
        }

        public ExcelAddress FindName(string globalName) {
            if (!this._wb.NamedRanges.TryGetValue(globalName, out var range)) {
                Log.Info("Could not find named range " + globalName);
                return null;
            }

            if (range.Ranges.Count > 1)
                throw new NotImplementedException("have not supported named ranges referencing multiple ranges");
            var rangeAddress = range.Ranges.First().RangeAddress;
            return new ExcelAddress(rangeAddress.Worksheet.Name, rangeAddress.ToStringFixed(XLReferenceStyle.A1));
        }

        public Dictionary<string, List<ExcelAddress>> FindAllNamedRanges() {
            return this._wb.NamedRanges.ToDictionary(nr => nr.Name,
                nr => nr.Ranges.Cells().Select(c =>
                    new ExcelAddress(c.Address.Worksheet.Name, c.Address.ColumnLetter + c.Address.RowNumber)).ToList());
        }

        public string ReadValue(ExcelAddress address) {
            IXLCell cell = FindCell(address);
            if (cell == null) return null;
            try {
                string value = cell?.CachedValue?.ToString() ??
                               cell?.Value
                                   ?.ToString(); // if you use VALUE then you try and run the built in parser
                if (cell.DataType == XLDataType.Number) {
                    if (double.TryParse(value,  out var val)) {
                        value = val.ToString();
                        return value;
                    }
                }
                // try and deal with textual comments... 
                if (cell.DataType == XLDataType.Text  && !string.IsNullOrWhiteSpace(value) &&
                    (!double.TryParse(value, out _) || value=="NaN" || value.Contains(","))) {  // some unexpected things parse as doubles...
                    value = "'" + value;
                }
                //if (value == "#REF!") return ExcelErrors.REF;todo
                if (value == "#N/A") return ExcelErrors.NA;
                if (value == "#VALUE!") return ExcelErrors.VALUE;
                if (value == "#DIV/0!") return ExcelErrors.DIV0;
                //if (cell.DataType == XLDataType.Text) {
                //    var tvalue = cell.Value;
                //    return tvalue?.ToString();
                //}
                return value;
            }
            catch (Exception e) {
                var n = e.GetType().Name;
                if (n == "NoValueAvailableException") {
                    return ExcelErrors.NA;
                }

                Log.Error("failed to Read Value " + address, e);
                return null;
            }

        }

        private IXLCell FindCell(ExcelAddress address) {
            var ws = this._wb.Worksheet(address.WorkSheet);
            return ws?.Cell(address.CellReference);
        }

        public string ReadGivenName(ExcelAddress address) {
            var names = this._wb.NamedRanges.Where(nr => nr.Ranges.Any(r => r.Contains(address.ToString()))).ToList();
            if (!names.Any()) return null;
            if (names.Count() > 1) {
                Log.Info("extractor does not support multiple names yet"); //todo
            }

            return names.First().Name;
        }

        #endregion

        #region IExcelWholeReader

        public List<string> GetSheetNames() {
            return this._wb.Worksheets.Select(ws => ws.Name).ToList();
        }

        public ExcelAddress GetUsedCells(string sheet) {
            var ws = this._wb.Worksheet(sheet);
            if (ws == null) return null;
            foreach (var pivotTable in ws.PivotTables) {
                Log.Info("WARNING found pivot table in " + sheet + " targetting " + pivotTable.TargetCell.Address.ToStringFixed());
                Log.Info("WARNING Reading values from Pivot Tables is not supported - see https://github.com/ClosedXML/ClosedXML/issues/391");
            }

            var xlRangeAddress = ws.RangeUsed(XLCellsUsedOptions.All)?.RangeAddress;
            if (xlRangeAddress == null) return null;

            return new ExcelAddress(sheet,
                xlRangeAddress.FirstAddress.ColumnLetter + xlRangeAddress.FirstAddress.RowNumber + ":" +
                xlRangeAddress.LastAddress.ColumnLetter + xlRangeAddress.LastAddress.RowNumber);

            //var firstCell = ws.FirstCellUsed();
            //var lastCell = ws.LastCellUsed();
            //
            //
            //if (firstCell == null || lastCell == null) return null;
            //
            //return new ExcelAddress(sheet,
            //    firstCell.Address.ColumnLetter + firstCell.Address.RowNumber + ":" +
            //    lastCell.Address.ColumnLetter + lastCell.Address.RowNumber);
        }

        #endregion
    }
}
