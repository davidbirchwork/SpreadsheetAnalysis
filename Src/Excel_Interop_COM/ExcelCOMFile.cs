using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ExcelInterop;
using ExcelInterop.Domain;
using Microsoft.Office.Interop.Excel;

namespace Excel_Interop_COM {
    /// <summary>
    /// Class to read from Excel
    /// </summary>
    [Serializable]
    public class ExcelCOMFile : IExcelReaderWriter
    {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private Application xlApp;
        private Workbook xlWorkBook;    
        private Workbooks xlworkbooks;
        private object misValue = Missing.Value;
        private Sheets worksheets;
        private string FileName;

        public ExcelCOMFile(string fileName) {
            this.FileName = fileName;
            if (!File.Exists(this.FileName)) {
                Log.Fatal("Could not find Excel File " + this.FileName);
                throw new FileNotFoundException("Excel Services could not find file " + this.FileName);
            }
            try {

                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlApp.AlertBeforeOverwriting = false;
                xlApp.DisplayAlerts = false;
                xlworkbooks = xlApp.Workbooks;
                xlWorkBook = xlworkbooks.Open(this.FileName, // filename
                                                  0, //updatelinks
                                                  false, //readonly 
                                                  5, //format
                                                  "", //Password 
                                                  "", //writeResPass
                                                  true, //ignoreReadOnly
                                                  XlPlatform.xlWindows, //origin
                                                  "\t", //delimiter
                                                  true, //editable
                                                  false, //Notify
                                                  0, //converter
                                                  false, //AddToMru
                                                  1, //Local
                                                  0); //corruptLoad
                 worksheets = xlWorkBook.Worksheets;                

                this.Shims = new Dictionary<string, string>();
                for (int ws = 1; ws <= worksheets.Count; ws++) {
                    dynamic worksheet = worksheets[ws];                
                    dynamic sheetname = worksheet.Name;
                    // {"'Sum Resources'","Sum_Resources"}, 
                    string originalname = "'"+sheetname.ToString()+"'";
                    string newname = originalname.Replace("'", "");
                    newname = newname.Replace(" ", "_");
                    this.Shims.Add(originalname,newname);
                    //log.Debug("Renamed>"+originalname+"->"+ newname);
                    ReleaseObject(worksheet);
                   // ReleaseObject(sheetname);dont thinks tihs need releasing
                }
                 this.xlApp.Calculation = XlCalculation.xlCalculationManual;
            } catch (Exception e) {
                Log.Fatal("Could not open ExcelFile '" + this.FileName + "' - failed with message: " + e.Message);
                Dispose(true);
            }
        }

        /// <summary>
        /// Write a set of values to a excel sheet
        /// </summary>
        /// <param name="cellstoUpdate">SheetNames,cell names , values</param>
        public void SetValues(Dictionary<ExcelAddress, string> cellstoUpdate) {
            Dictionary<string, Dictionary<ExcelAddress, string>> indexedbySheet = new Dictionary<string, Dictionary<ExcelAddress, string>>();

            foreach (KeyValuePair<ExcelAddress, string> pair in cellstoUpdate) {
                if (!indexedbySheet.ContainsKey(pair.Key.WorkSheet)) {
                    indexedbySheet.Add(pair.Key.WorkSheet, new Dictionary<ExcelAddress, string>());
                }
                indexedbySheet[pair.Key.WorkSheet].Add(pair.Key, pair.Value);
            }

            foreach (string worksheet in indexedbySheet.Keys) {
                try {
                    
                    Worksheet xlWorkSheet = (Worksheet)worksheets.Item[worksheet];

                    foreach (KeyValuePair<ExcelAddress, string> cellpair in indexedbySheet[worksheet]) {
                        try {
                            Range range  = xlWorkSheet.Range[cellpair.Key.CellReference, cellpair.Key.CellReference];
                            Range cells = range.Cells;
                            cells.Value2 = cellpair.Value;
                            ReleaseObject(cells);
                            cells = null;
                            ReleaseObject(range);
                            range = null;
                        } catch (Exception e) {
                            Log.Fatal(" Could not find Cell. '" + worksheet + "' '" + cellpair.Key.CellReference + "' - failed with message: " + e.Message);                            
                        }
                    }

                    ReleaseObject(xlWorkSheet);
                    xlWorkSheet = null;
                } catch (Exception e) {
                    Log.Fatal(" Could not find worksheet.  '" + worksheet  +"' in " + this.FileName + " - failed with message: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Read a set of values from the excel sheet
        /// </summary>
        /// <param name="cells">list of cells</param>
        /// <returns>list of string values</returns>
        public Dictionary<ExcelAddress, string> ReadValues(IEnumerable<ExcelAddress> cells) {
            Dictionary<string, List<ExcelAddress>> indexedbySheet = new Dictionary<string, List<ExcelAddress>>();
            foreach (ExcelAddress cell in cells) {
                string workSheet = UndoShims(cell.WorkSheet);
                if (!indexedbySheet.ContainsKey(workSheet)) {
                    indexedbySheet.Add(workSheet, new List<ExcelAddress>());
                }
                indexedbySheet[workSheet].Add(cell);
            }

            Dictionary<ExcelAddress, string> results = new Dictionary<ExcelAddress, string>();

            foreach (string worksheet in indexedbySheet.Keys) {
                try {
                    Worksheet xlWorkSheet = (Worksheet)this.worksheets.Item[worksheet];

                    foreach (ExcelAddress cell in indexedbySheet[worksheet]) {
                        try {
                            Range range = xlWorkSheet.Range[cell.CellReference, cell.CellReference];
                            Range cellsrange = range.Cells;
                            dynamic value2 = cellsrange.Value2;
                            string result = (value2 == null) ? "BLANK" : value2.ToString();
                            result = ApplyShims(result);
                            results.Add(cell,result);
                            ReleaseObject(cellsrange); 
                            ReleaseObject(range);                            
                        } catch (Exception e) {
                            Log.Fatal(" Could not find Cell. '" + worksheet + "' '" + cell.CellReference + "' in excel file '"+this.FileName+"' - failed with message: " + e.Message);
                        }
                    }

                    ReleaseObject(xlWorkSheet);
                    xlWorkSheet = null;
                } catch (Exception e) {
                    Log.Fatal("Could not find worksheet.  '" + worksheet + "' in "+this.FileName+" - failed with message: " + e.Message);
                }
            }

            return results;
        }

        public Dictionary<string, string> ReadValues(Dictionary<string, ExcelAddress> excelReadValues, string notFoundValue = null) {
            Dictionary<ExcelAddress, string> redCells = ReadValues(excelReadValues.Values);
            return excelReadValues.ToDictionary(pair => pair.Key, pair => {
                                                                 if (redCells.ContainsKey(pair.Value)) {
                                                                     return redCells[pair.Value];
                                                                 }   else {
                                                                     return notFoundValue ?? redCells[pair.Value]; // we throw exception unless a not found value is supplied
                                                                 }
                                                                });
        }

        public string ReadFormula(ExcelAddress address) {
            string workSheet = UndoShims(address.WorkSheet);
            
            string result = null;
            try {
                Worksheet xlWorkSheet = (Worksheet) this.worksheets.Item[workSheet];
                try {
                    Range range = xlWorkSheet.Range[address.CellReference, address.CellReference];
                    Range cellsrange = range.Cells;
                    result = cellsrange.Formula.ToString();
                    ReleaseObject(cellsrange);
                    cellsrange = null;
                    ReleaseObject(range);
                    range = null;
                } catch (Exception e) {
                 //   log.Fatal(" Could not find Cell. '" + address.WorkSheet + "' '" + address.CellName +
                   //                "' - failed with message: " + e.Message);
                    throw new ArgumentException("BadCellName" + address + e.Message);
                }
                ReleaseObject(xlWorkSheet);
                xlWorkSheet = null;
            } catch (Exception e) {                
             //   log.Fatal(" Could not find worksheet.  '" + address.WorkSheet + "' - failed with message: " +
               //                e.Message);
                throw new ArgumentException("BadSheetName" + address + e.Message);
            }

            result = ApplyShims(result);

            return result;
        }

        private Dictionary<string, string> Shims = new Dictionary<string, string> {
               {"'Resource Output Comparison'","Resource_Output_Comparison"} 
            };

        private string ApplyShims(string result) {
            foreach (var shim in Shims) {
                result = result.Replace((string) shim.Key, shim.Value);
            }
            return result;
        }

        private string UndoShims(string result) {
            foreach (var shim in Shims) {
                result = result.Replace((string) shim.Value, shim.Key);

            }
            result = result.Replace("'", "");
            return result;
        }

        /// <summary>
        /// Close the file and dispose of all the resources
        /// </summary>
        /// <param name="saveResults">tick to overwrite the results into the excel file</param>
        public void Close(bool saveResults) {
            // now try and save... 
            if (xlWorkBook != null) {
                xlWorkBook.Close(saveResults, this.FileName, misValue);
            }
            xlApp.Quit();
            Dispose(true); // clean up
        }

        #region destructors and disposal

        private static void ReleaseObject(object obj) {
            // ReSharper disable RedundantAssignment 
            //we allow it to be garbage collected?
            if (obj != null) {
                try {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(obj);
                    obj = null;
                } catch (Exception ex) {
                    obj = null;
                    Log.Debug("Unable to release the Object " + ex.Message);
                }
            }
            // ReSharper restore RedundantAssignment
        }

        private bool _IsDisposed; // defaults to false

        ~ExcelCOMFile() {
            Dispose(false);
        }

        protected void Dispose(bool disposing) {
            if (disposing && !_IsDisposed) {

                // nasty stuff since com can screw up in so many ways!                              
                if (worksheets != null) ReleaseObject(worksheets);
                worksheets = null;
                if (xlworkbooks != null) ReleaseObject(xlworkbooks);
                xlworkbooks = null;
                if (xlWorkBook != null) ReleaseObject(xlWorkBook);
                xlWorkBook = null;
                if (xlApp != null) ReleaseObject(xlApp);
                xlApp = null;

                // GC.Collect(); dont think this is necessary 
            }

            _IsDisposed = true;
        }

        public void Dispose() {
            if (xlWorkBook != null) xlWorkBook.Close(false, misValue, misValue);
            if (xlApp != null) xlApp.Quit();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public void Calculate() {
            this.xlApp.CalculateFullRebuild();//recalculate the whole spreadsheet
            //this.xlApp.Calculate();
            //this.xlApp.CalculateFullRebuild(); even bigger           
            
        }

        public void SetManualCalculation() {
            this.xlApp.Calculation = XlCalculation.xlCalculationManual;
            this.xlApp.MultiThreadedCalculation.Enabled = false;
        }

        /// <summary>
        /// Finds the Excel address a name refers to
        /// </summary>
        /// <param name="globalName">Name of the global.</param>
        /// <returns></returns>
        public ExcelAddress FindName(string globalName) {
            ExcelAddress result = new ExcelAddress("Fatal", "Fatal");
            try {
                Names names = this.xlWorkBook.Names;
                Name name = names.Item(globalName);
                string range = (name.RefersTo) as string;
                if (range != null) {
                    range = range.Substring(1); // remove the =                    
                    result = new ExcelAddress(range);
                }
                ReleaseObject(name);
                name = null;
                ReleaseObject(names);
                names = null;
            } catch (Exception e) {
                Log.Fatal("tried to find a bad name " + globalName+ " "+e);
            }
            return result;
        }

        /// <summary>
        /// Cleans the excel names by replacing .'s with _'s
        /// </summary>
        /// <returns></returns>
        public List<string> CleanExcelNames() {
            List<string> changednames = new List<string>();
            try {
                Names names = this.xlWorkBook.Names;
                for (int n = 0; n < names.Count; n++) {
                    Name name = names.Item(n, misValue, misValue);
                    string tname = name.Name;
                    if (tname.Contains(".")) {
                        changednames.Add(tname);
                        name.Name = tname.Replace(".", "_");
                    }
                    ReleaseObject(name);
                    name = null;
                }                
                ReleaseObject(names);
                names = null;
            } catch (Exception e) {
                Log.Fatal("Fatal trying to clean names " + e);
            }
            return changednames;
        }

        public string ReadValue(ExcelAddress address) {          
            string result = null;
            try {
                string workSheet = UndoShims(address.WorkSheet);
                Worksheet xlWorkSheet = (Worksheet)this.worksheets.Item[workSheet];
                try {
                    Range range = xlWorkSheet.Range[address.CellReference, address.CellReference];
                    Range cellsrange = range.Cells;
                    object objresult = cellsrange.Value2;
                    if (objresult == null) {
                        result = string.Empty;
                    } else if (objresult.GetType() == typeof(double)) {
                        result = ((double)objresult).ToString("G17");
                    } else {
                        result = objresult.ToString();
                    }
                    ReleaseObject(cellsrange);
                    cellsrange = null;
                    ReleaseObject(range);
                    range = null;
                } catch (Exception e) {
                    //   log.Fatal(" Could not find Cell. '" + address.WorkSheet + "' '" + address.CellName +
                    //                "' - failed with message: " + e.Message);
                    throw new ArgumentException("BadCellName" + address + e.Message);
                }
                ReleaseObject(xlWorkSheet);
                xlWorkSheet = null;
            } catch (Exception e) {
                //   log.Fatal(" Could not find worksheet.  '" + address.WorkSheet + "' - failed with message: " +
                //                e.Message);
                throw new ArgumentException("BadSheetName" + address + e.Message);
            }

            result = ApplyShims(result);

            return result;
        }

        public void SaveTo(string filename) {
            const int maxcompathlength = 218;
            //const string longpathprefix = @"\\?\";

            if (filename.Length > maxcompathlength) {                
                string directory = Path.GetDirectoryName(filename);
                string newFname = Guid.NewGuid() + "." + Path.GetExtension(filename);
               
                string shortfname = Path.Combine(directory + newFname);
                try {
                    xlWorkBook.SaveAs(shortfname);
                    File.WriteAllText(Path.ChangeExtension(shortfname, "txt"),"Filename was too long - should have been: "+Environment.NewLine+filename);                    
                } catch (Exception e) {
                    Log.Fatal("Failed to save excel file " + e.Message);
                }                
                
            } else {
                if (File.Exists(filename)) {
                    Log.Fatal("can not over write file " + filename);
                    xlWorkBook.SaveAs(Path.Combine(Path.GetDirectoryName(filename),
                                 Path.GetFileName(filename) + "_2" + Path.GetExtension(filename)));
                } else {
                    lock (saveaslockobject) {
                        xlWorkBook.SaveAs(filename);
                    }
                }
            }
        }

        private static object  saveaslockobject = new object();

        public string ReadGivenName(ExcelAddress address) {
            string result = null;
            try {
                string workSheet = UndoShims(address.WorkSheet);
                Worksheet xlWorkSheet = (Worksheet)this.worksheets.Item[workSheet];
                try {
                    Range range = xlWorkSheet.Range[address.CellReference, address.CellReference];
                    Range cellsrange = range.Cells;

                    try {

                        dynamic objresult = cellsrange.Name;
                        try {
                            Object res = objresult.NameLocal;
                            result = res == null ? string.Empty : res.ToString();
                        } catch (Exception) {
                            return string.Empty;
                        }
                        ReleaseObject(objresult);
                        objresult = null;
                    } catch (Exception) {
                        return string.Empty;
                    }
                    ReleaseObject(cellsrange);
                    cellsrange = null;
                    ReleaseObject(range);
                    range = null;
                } catch (Exception e) {
                    //   log.Fatal(" Could not find Cell. '" + address.WorkSheet + "' '" + address.CellName +
                    //                "' - failed with message: " + e.Message);
                    throw new ArgumentException("BadCellName" + address + e.Message);
                }
                ReleaseObject(xlWorkSheet);
                xlWorkSheet = null;
            } catch (Exception e) {
                //   log.Fatal(" Could not find worksheet.  '" + address.WorkSheet + "' - failed with message: " +
                //                e.Message);
                throw new ArgumentException("BadSheetName" + address + e.Message);
            }

            result = ApplyShims(result);

            return result;
        }

        public List<string> GetLinkedFiles() {
            List<string> links = new List<string>();
            if (this.xlWorkBook != null) {
                try {
                    dynamic obj = this.xlWorkBook.LinkSources(XlLink.xlExcelLinks);
                    Array arr = (obj as object) as Array;
                    if (arr != null){                        
                        object[] objs = ConvertArray(arr);
                        foreach (object link in objs) {
                            links.Add(link.ToString());
                        }
                    }
                    ReleaseObject(obj);
                    obj = null;
                } catch (Exception e) {
                    Log.Fatal("Problems read Excel Links " + e);
                }
            }

            return links;
        }

        public bool UpdateLinkedFile(string link,string newLink) {
            if (this.xlWorkBook == null)
                return false;
            try {
                this.xlWorkBook.ChangeLink(link, newLink);
            } catch (Exception e) {
                Log.Fatal($"Tried to update link '{link}' to '{newLink}' but failed with message '{e}'");
                return false;
            }
            
            return true;

        }

        private static object[] ConvertArray(Array arr) {
            int lb = arr.GetLowerBound(0);
            var ret = new object[arr.GetUpperBound(0) - lb + 1];
            for (int ix = 0; ix < ret.Length; ++ix) {
                ret[ix] = arr.GetValue(ix + lb);
            }
            return ret;
        }

        public bool RunMacro(string macroName, IEnumerable<object> arguments = null) {
            if (this.xlApp != null) {
                List<object> args = arguments == null? new List<object>(): arguments.ToList() ;
                args.Insert(0,macroName);
                try {
                    this.xlApp.GetType().InvokeMember("Run",
                                                      BindingFlags.Default |
                                                      BindingFlags.InvokeMethod,
                                                      null, this.xlApp, args.ToArray());
                } catch (Exception e) {
                    Log.Fatal("Could not run macro " + macroName + " in excel file " + this.FileName+ " Fatal was "+e);
                    return false;
                }
                return true;
            }
            return false;
        }

        public static IExcelReader Factory(string fname) {
            return new ExcelCOMFile(fname);
        }
    }
}