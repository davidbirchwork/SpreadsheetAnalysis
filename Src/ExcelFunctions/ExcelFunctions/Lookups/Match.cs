using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using NCalcExcel.Domain;
using NCalcExcel.Functions;

namespace ExcelFunctions.ExcelFunctions.Lookups {
    /// <summary>
    /// http://office.microsoft.com/en-us/excel-help/match-HP005209168.aspx
    /// </summary>
    [Export(typeof(AFunction))]
    public class Match : AFunction {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public enum MatchType {
            MaxSmaller,//1
            Exact,//0
            MinLarger//-1
        }        

        public Match()
            : base("Match",
                   "Match(lookup_value,lookup_array,matchtype)",
                   "computes Match") {
                       this.AddParamterDescription("lookup_value", "The value you use to find the value you want in a table");
                       this.AddParamterDescription("lookup_array", "Is a contiguous range of cells containing possible lookup values");
                       this.AddParamterDescription("matchtype", "Is the number -1, 0, or 1. Match_type specifies how Microsoft Excel matches lookup_value with values in lookup_array");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            int result = 0;

            if (function.Expressions.Length < 2 || function.Expressions.Length >3)
                throw new ArgumentException("Match() takes 2 or 3 arguments");

            #region look up value

            object lookupValueObject = evaluator.Evaluate(function.Expressions[0]);                        

            string lookupValueString = this.CleanString(lookupValueObject.ToString().ToLower());
            MatchValueType lookValueType = MatchValueType.String;

            bool lookupvalueBool;            
            if (bool.TryParse(lookupValueString,out lookupvalueBool)) {
                lookValueType = MatchValueType.Boolean;
            }

            decimal lookupvalueNumeric;
            if (decimal.TryParse(lookupValueString, out lookupvalueNumeric)) {
                lookValueType = MatchValueType.Numeric;
            }

            #endregion

            #region match type
            
            int matchtype = 1;
            if (function.Expressions.Length == 3) {
                object matchobject = evaluator.Evaluate(function.Expressions[2]);
                matchtype = Convert.ToInt32(matchobject);                
            }

            MatchType matchType = matchtype == -1
                                      ? MatchType.MinLarger
                                      : matchtype == 1 ? MatchType.MaxSmaller : MatchType.Exact;
            #endregion

            String lookupRange = evaluator.Evaluate(function.Expressions[1]).ToString();
            bool indummyExecMode = false;
            if (lookupRange == "1") { // we are in dummy exec mode...
                lookupRange = this.DoFakeExpansion(this.CleanString(function.Expressions[1].ToString())).Aggregate("{|{", (acc, next) => acc + "," + next) + "}|}";
                indummyExecMode = true;
            }

            List<string> testvalues = new List<string>();
            // expand range
            if (lookupRange.Contains("{|{")) {
                // we found a wrapped range...  so unwrap it and walk through its cells
                string range = lookupRange.Replace("{|{", "").Replace("}|}", "");
                string[] lookupCells = range.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                bool foundValue = false;
                int i;
                for (i = 0;
                     i < lookupCells.Length && !foundValue;
                     i++) {

                    string testValue = this.CleanString(this.CreateAndEvalExpression(evaluator, lookupCells[i]).ToString().ToLower());

                    
                    testvalues.Add(testValue);// debug
                
                    switch (lookValueType) {
                        case MatchValueType.Boolean:
                            #region boolean                            
                            bool testboolean = Boolean.Parse(testValue);
                            switch (matchType) {
                                case MatchType.MaxSmaller:
                                    if (!testboolean) {
                                        result = i + 1;
                                    } else {
                                        foundValue = true;
                                    }
                                    break;
                                case MatchType.Exact:
                                    if (lookupvalueBool.Equals(testboolean)) {
                                        foundValue = true;
                                        result = i + 1;
                                    }
                                    break;
                                case MatchType.MinLarger:
                                    if (testboolean) {
                                        result = i + 1;
                                    } else {
                                        foundValue = true;
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            #endregion
                            break;
                        case MatchValueType.Numeric:
                            #region Numeric

                            decimal testDecimal = decimal.Parse(testValue);

                            switch (matchType) {
                                case MatchType.MaxSmaller:
                                    if (testDecimal <= lookupvalueNumeric) {
                                        result = i + 1;
                                    } else {
                                        foundValue = true;
                                    }
                                    break;
                                case MatchType.Exact:
                                    if (lookupvalueNumeric.Equals(testDecimal)) {
                                        foundValue = true;
                                        result = i + 1;
                                    }
                                    break;
                                case MatchType.MinLarger:
                                    if (testDecimal >= lookupvalueNumeric) {
                                        result = i + 1;
                                    } else {
                                        foundValue = true;
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            #endregion
                            break;
                        case MatchValueType.String:
                            #region String
                            
                            string testString = testValue.ToLower();
                            var sorted = new List<string> {testString, lookupValueString};

                            switch (matchType) {
                                case MatchType.MaxSmaller:
                                    sorted.Sort();

                                    if (sorted.IndexOf(testString)==1) {
                                        result = i + 1;
                                    } else {
                                        foundValue = true;
                                    }
                                    break;
                                case MatchType.Exact:
                                    if (lookupValueString.Equals(testString)) {
                                        foundValue = true;
                                        result = i + 1;
                                    }
                                    break;
                                case MatchType.MinLarger:
                                    sorted.Sort();                                    
                                    
                                    if (sorted.IndexOf(testString)==0) {
                                        result = i + 1;
                                    } else {
                                        foundValue = true;
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            #endregion
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (indummyExecMode) {
                    // we must call all cells we might *possibly* reference... 
                    for (int j = i; j < lookupCells.Length; j++) {
                         string testvalue = this.CreateAndEvalExpression(evaluator, lookupCells[j]).ToString().ToLower();
                    } 
                }

                if (!foundValue) {
                    result = 0;
                }
            } else {
                Log.Error("panic we dont know how to use the range passed " + lookupRange);
            }


            if (result == 0)
                return ExcelErrors.NA;
            else 
                return result;
        }        
    }
}
