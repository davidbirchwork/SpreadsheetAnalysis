using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using ExcelInterop;
using ExcelInterop.Domain;

namespace ExcelExtractor.Domain {
    public class Extractor {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly FunctionExtractor _fe;
        private readonly Func<string,IExcelReader> _readerFactory;

        public Extractor(FunctionExtractor fe,Func<string,IExcelReader> excelReaderFactory) {
            this._fe = fe;
            this._readerFactory = excelReaderFactory;
        }

        public void StartExtraction() {
            Log.Info("Starting Extractor Thread");

            using (var excelObject = _readerFactory(this._fe.ExcelFile)) {
                Log.Info("Opened Excel File");
                string currentAddress = null;

                while (ObtainNextItem(ref currentAddress)) {
                    if (!this._fe.ProcessedCells.ContainsKey(currentAddress)) {
                        if (this._fe.ProcessedCells.Count % 1000 == 0) {
                            Log.Info("Processing:[Done:" + this._fe.ProcessedCells.Count + " | TODO: " +
                                     this._fe.CellsToExtract.Count + "] " + currentAddress);
                        }

                        ExcelAddress address = new ExcelAddress(currentAddress);

                        if (address.IsRange()) {
                            ProcessRange(address);
                        }
                        else {
                            ProcessReference(address, excelObject);
                        }
                    }
                }

                Log.Info("An extractor could not find work so its closing down");


            }


            // signal we're finished
            this._fe.RunningTasks.Release();
        }        

        private bool ObtainNextItem(ref string currentAddress) {
            // todo we should probably check this item hasnt been processed!
            string address;
            if (this._fe.CellsToExtract.TryPop(out address)) {
                currentAddress = address;
                return true;
            }
            Thread.Sleep(2000);
            if (this._fe.CellsToExtract.TryPop(out address)) {
                currentAddress = address;
                return true;
            }
            return false;
        }

        private void ProcessRange(ExcelAddress address) {
            var excelCell = new ExtractedCell(address.WorkSheet, address.CellReference, null, null,false, true);

            List<ExcelAddress> references = ExcelAddress.ExpandRangeToExcelAddresses(address);

            foreach (var reference in references) {                
                excelCell.References.Add(new CellReference(null,CellReference.CellReferenceType.RangeExpansion,reference.ToString(), reference));
                this._fe.AddWorkItem(reference.ToString());
            }

            this._fe.AddNewProcessedCell(excelCell);
        }

        private void ProcessReference(ExcelAddress address, IExcelReader excelObject) {
            
            // first we try and read the formula

            string formula;
            List<string> knownAs = new List<string>();

            try {
                formula = excelObject.ReadFormula(address);
                // test it is a valid cell name
                new CellName(address.CellReference);

                // if reading a formula fails or the cell name is wrong then we see if it is actually an excel cell name
            } catch (ArgumentException argumentException) { // its a named cell
                try {
                    knownAs.Add(address.CellReference);
                    address = excelObject.FindName(address.CellReference);                    
                    formula = excelObject.ReadFormula(address);
                } catch (Exception e) {
                    // bad times! we really do not know what this reference is!
                    throw new Exception(e+" "+argumentException);
                }
            }

            // then we read value
            string cellValue = excelObject.ReadValue(address);
            // then see if we can read any excel names...
            string excelname = excelObject.ReadGivenName(address);
            if (!string.IsNullOrEmpty(excelname)) {
                if (!knownAs.Contains(excelname)) {
                    knownAs.Add(excelname);
                }            
            }

            // now lets process the formula sanitise the formula
            bool isFormula = !string.IsNullOrEmpty(formula) && formula.StartsWith("="); 
            formula = string.IsNullOrWhiteSpace(formula) ? "'BLANK'" : ExcelFormula.SanitiseExcelFormula(formula,this._fe.Shims);           

            // create the excel cell
            ExtractedCell extractedCell = new ExtractedCell(address.WorkSheet, address.CellReference, cellValue, formula, isFormula,false);
            if (!string.IsNullOrEmpty(excelname)) {
                extractedCell.ExcelNames.Add(excelname);
            }

            // now deal with references in formulas););
            if (isFormula) {
                try {
                    List<string> references = this._fe.ExpressionFactory.ExtractVars(formula);
                    foreach (var reference in references) {
                        ExcelAddress referencedCell = SanitiseReference(address, reference, excelObject);

                        extractedCell.References.Add(new CellReference(null, CellReference.CellReferenceType.Formula, reference, referencedCell));
                        this._fe.AddWorkItem(referencedCell.ToString());
                    }
                }
                catch (Exception e) {
                    Log.Error("faile to extract variables from "+formula+" "+e);
                }

                
            }

            this._fe.AddNewProcessedCell(extractedCell);
        }

        private ExcelAddress SanitiseReference(ExcelAddress address, string reference, IExcelReader excelObject) {
            // lets remove all $'s 
            reference = reference.Replace("$", "");

            // its a local reference to append the current sheet?
            if (!reference.Contains("!")) {
                reference = address.WorkSheet + "!" + reference;
            }

            if (!reference.Contains(":")) {
                ExcelAddress newname = new ExcelAddress(reference);
                try {
                    // test we can read it
                    string formula = excelObject.ReadFormula(newname);
                    // test it is a valid cell name
                    new CellName(newname.CellReference);
                } catch (Exception e) {
                    string error = "bad name " + newname + " exception " + e;
                    Log.Debug(error);
                    reference = excelObject.FindName(newname.CellReference).ToString();                    
                }
            }

            // lets remove all $'s 
            reference = reference.Replace("$", "");

            return new ExcelAddress(reference);
        }
    }
}