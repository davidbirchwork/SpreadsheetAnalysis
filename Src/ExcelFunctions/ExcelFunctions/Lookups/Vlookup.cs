using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using ExcelInterop.Domain;
using NCalcExcel;
using NCalcExcel.Domain;
using NCalcExcel.Functions;
using ExcelErrors = NCalcExcel.Domain.ExcelErrors;

namespace ExcelFunctions.ExcelFunctions.Lookups {
    [Export(typeof(AFunction))]
    public class Vlookup : AFunction {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Vlookup()
            : base("Vlookup",
                   "Vlookup(lookup_value,table_array,col_index_num,range_lookup)",
                   "computes Vlookup") {
                       this.AddParamterDescription("lookup_value", "The value to search in the first column of the table");
                       this.AddParamterDescription("table_array", "Two or more columns of data");
                       this.AddParamterDescription("col_index_num", "  The column number in table_array from which the matching value must be returned");
                       this.AddParamterDescription("range_lookup", " A logical value that specifies whether you want VLOOKUP to find an exact match or an approximate match");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            

            if (function.Expressions.Length < 3 || function.Expressions.Length > 4)
                throw new ArgumentException("Vlookup() takes 3 or 4 arguments");

            #region match type

            bool inexactLookup = true;
            if (function.Expressions.Length == 4) {
                inexactLookup = Convert.ToBoolean(evaluator.Evaluate(function.Expressions[3]));
            }

            #endregion

            #region col index

            int anint;
            bool staticColumn = int.TryParse(this.CleanString(function.Expressions[2].ToString()), out anint);

            object value = evaluator.Evaluate(function.Expressions[2]);
            int colIndex = Convert.ToInt32(this.CleanString(value.ToString()));
            if (colIndex < 1) {
                Log.Error("vlookup invalid column index " + colIndex);
                return ExcelErrors.VALUE;
            }
            #endregion

            #region look up value

            object lookupValueObject = evaluator.Evaluate(function.Expressions[0]);

            string lookupValueString = this.CleanString(lookupValueObject.ToString().ToLower());
            MatchValueType lookValueType = MatchValueType.String;

            bool lookupvalueBool;
            if (bool.TryParse(lookupValueString, out lookupvalueBool)) {
                lookValueType = MatchValueType.Boolean;
            }

            decimal lookupvalueNumeric;
            if (decimal.TryParse(lookupValueString, out lookupvalueNumeric)) {
                lookValueType = MatchValueType.Numeric;
            }

            #endregion

            #region get Table & do loook up

            bool hasWorksheet = false;
            string worksheet = "";
            CellName firstLookup = new CellName();
            CellName firstValue = new CellName();
            int rows = 0; // num look up pairs

            string lookupRange = this.CleanString(function.Expressions[1].ToString());
            if (lookupRange.Contains("!")) {
                var sheetsplit = lookupRange.Split(new string[] { "!" }, StringSplitOptions.RemoveEmptyEntries);
                worksheet = sheetsplit[0];
                lookupRange = sheetsplit[1];
                hasWorksheet = true;
            }


            if (staticColumn) {                
                var split = lookupRange.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                string topleft = split[0];
                string bottomright = split[1];
                CellName topleftcell = new CellName(topleft);
                CellName bottomrightcell = new CellName(bottomright);

                //get #rows
                rows = 1;
                CellName current = new CellName(topleft);
                for (; current.Row != bottomrightcell.Row; rows++) {
                    current = current.OneDown();
                }

                //get #cols
                int cols = 1;
                firstValue = new CellName(topleft);
                for (; firstValue.Column != bottomrightcell.Column; cols++) {
                    firstValue = firstValue.OneRight();
                }
                firstLookup = topleftcell;

                if (colIndex > cols) {
                    Log.Error("VLOOKUP col index larger than range");
                    return "#REF!";
                }

                firstValue = new CellName(topleft);
                for (int c = 1; c != colIndex; c++) {
                    firstValue = firstValue.OneRight();
                }

            } else {
                lookupRange = this.CleanString( this.CreateAndEvalExpression(evaluator,function.Expressions[1].ToString()).ToString());

                if ((evaluator.options & EvaluateOptions.DebugMode) == EvaluateOptions.DebugMode) {
                    lookupRange = this.CleanString(function.Expressions[1].ToString());
                    string aworksheet = worksheet;
                    if (lookupRange.Contains("!")) {
                        aworksheet = lookupRange.Split(new[] {"!"}, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                    ExcelAddress ad = hasWorksheet ? new ExcelAddress(lookupRange) : new ExcelAddress(aworksheet, lookupRange);
                    lookupRange = ExcelAddress.ExpandRangeToCellList(ad).Aggregate("",(acc, next) => acc + "," + next);

                    lookupRange = lookupRange.Substring(1);
                    lookupRange = "{|{" + lookupRange + "}|}";
                }

                if (lookupRange.Contains("{|{")) {
                    // we found a wrapped range...  so unwrap it and walk through its cells
                    string range = lookupRange.Replace("{|{", "").Replace("}|}", "");
                    string[] lookupTableCells = range.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    string topleft = this.CleanString(lookupTableCells.First());
                    if (topleft.Contains("!")) {
                        topleft = topleft.Split(new[] {"!"}, StringSplitOptions.RemoveEmptyEntries)[1];
                    }
                    string bottomright = this.CleanString(lookupTableCells.Last());
                    if (bottomright.Contains("!")) {
                        bottomright = bottomright.Split(new[] {"!"}, StringSplitOptions.RemoveEmptyEntries)[1];
                    }

                    CellName topleftcell = new CellName(topleft);
                    CellName bottomrightcell = new CellName(bottomright);

                    //get #rows
                    rows = 1;
                    CellName current = new CellName(topleft);
                    for (; current.Row != bottomrightcell.Row; rows++) {
                        current = current.OneDown();
                    }

                    //get #cols
                    int cols = 1;
                    firstValue = new CellName(topleft);
                    for (; firstValue.Column != bottomrightcell.Column; cols++) {
                        firstValue = firstValue.OneRight();
                    }
                    firstLookup = topleftcell;

                    if (colIndex > cols) {
                        Log.Error("VLOOKUP col index larger than range");
                        return "#REF!";
                    }

                    firstValue = new CellName(topleft);
                    for (int c = 1; c != colIndex; c++) {
                        firstValue = firstValue.OneRight();
                    }

                    if ((evaluator.options & EvaluateOptions.DebugMode) == EvaluateOptions.DebugMode) {
                        // we must reference each column which we might reference...
                          CellName topcell = new CellName(firstLookup.ToString());
                          CellName bottomcell = new CellName(firstLookup.ToString());
                          for (int r = 0; r < rows; r++) {
                              bottomcell = bottomcell.OneDown();
                          }

                        for (int c = 0; c < cols; c++) {
                            string cellrange = topcell + ":" + bottomcell;

                            this.CreateAndEvalExpression(evaluator, hasWorksheet ? worksheet + "!" + cellrange : cellrange).ToString().ToLower();

                            topcell = topcell.OneRight();
                            bottomcell = bottomcell.OneRight();
                        }
                    }

                } else {
                    Log.Error("vlookup couldn't handle range :(");
                }
            }
            #endregion

            #region get the two columns of values

            CellName lastLookup = new CellName(firstLookup.ToString());
            CellName lastvalue = new CellName(firstValue.ToString());
            for (int r = 0; r < rows; r++) {
                lastLookup = lastLookup.OneDown();
                lastvalue = lastvalue.OneDown();
            }

            string mylookupRange = firstLookup + ":" + lastLookup;
            string myvalueRange = firstValue + ":" + lastvalue;

            string lookupcellsstring = this.CreateAndEvalExpression(evaluator, hasWorksheet ? worksheet + "!" + mylookupRange : mylookupRange).ToString();
            string valuecellsstring = this.CreateAndEvalExpression(evaluator, hasWorksheet ? worksheet + "!" + myvalueRange : myvalueRange).ToString();

            if ((evaluator.options & EvaluateOptions.DebugMode) == EvaluateOptions.DebugMode) {
                return "1";
            }

            // we found a wrapped range...  so unwrap it and walk through its cells
            lookupcellsstring = lookupcellsstring.Replace("{|{", "").Replace("}|}", "");
            string[] lookupCells = lookupcellsstring.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            valuecellsstring = valuecellsstring.Replace("{|{", "").Replace("}|}", "");
            string[] valueCells = valuecellsstring.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            #endregion

            #region finally do the VLOOKUP

            bool foundValue = false;
            string resultCell = "";            
            for (int r = 0; r < rows; r++) {
                string currentTestCell = this.CleanString(lookupCells[r]);
                string currentValueCell = this.CleanString(valueCells[r]);
                var testValue = this.CreateAndEvalExpression(evaluator, currentTestCell,EvaluateOptions.DonotHydrateBlanksToZero).ToString().ToLower(); 
                    // pass in an eval option which hydrates empty strings as empty strings and NOT as zero's
                //var value = this.CreateAndEvalExpression(evaluator, hasWorksheet ? worksheet + "!" + currentTestCell : currentTestCell.ToString()).ToString().ToLower();

                if (inexactLookup) {
                    switch (lookValueType) {
                        case MatchValueType.Boolean:
                          bool testBool;
                          if (Boolean.TryParse(testValue, out testBool)) {
                              if (!testBool) {
                                  resultCell = currentValueCell;
                              } else {
                                  foundValue = true;
                              }
                          }
                            break;
                        case MatchValueType.Numeric:
                            decimal testdecimal;
                            if (decimal.TryParse(testValue, out testdecimal)) {
                                if (testdecimal <= lookupvalueNumeric) {
                                    resultCell = currentValueCell;
                                } else {
                                    foundValue = true;
                                }
                            }
                            break;
                        case MatchValueType.String:
                              string testString = this.CleanString(testValue);
                            var sorted = new List<string> { testString, lookupValueString };
                            sorted.Sort();

                            if (testString == lookupValueString || sorted.IndexOf(lookupValueString) == 1) {
                                resultCell = currentValueCell;
                            } else {
                                foundValue = true;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (foundValue) {
                        if (resultCell == "") {
                            // value  less than the smallest value 
                            return ExcelErrors.NA; // equiv of #N/A
                        } else {
                            return this.CreateAndEvalExpression(evaluator, resultCell);
                        }
                    } else if (r == rows-1) { // last index - return last value
                        return this.CreateAndEvalExpression(evaluator, resultCell);
                    }
                } else {
                    resultCell = currentValueCell;
                    switch (lookValueType) {
                        case MatchValueType.Boolean:
                           bool testBool;
                           if (Boolean.TryParse(testValue, out testBool)) {
                               if (lookupvalueBool == testBool) {
                                   return this.CreateAndEvalExpression(evaluator, resultCell);
                               }
                           }
                            break;
                        case MatchValueType.Numeric:
                            decimal testdecimal;
                            if (decimal.TryParse(testValue, out testdecimal)) {
                                if (lookupvalueNumeric == testdecimal) {
                                    return this.CreateAndEvalExpression(evaluator, resultCell);
                                }
                            }
                            break;
                        case MatchValueType.String:
                            string teststring = this.CleanString(testValue);
                            if (lookupValueString == teststring) {
                                return this.CreateAndEvalExpression(evaluator, resultCell);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }                    
                }                
            }

            #endregion

            Log.Error("VLOOKUP Didn't find value!");
            return ExcelErrors.NA; // equiv of #N/A
        }
    }
}