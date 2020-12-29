using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using ExcelInterop.Domain;
using NCalcExcel;
using NCalcExcel.Domain;
using NCalcExcel.Functions;

namespace ExcelFunctions.ExcelFunctions.Lookups {
    /// <summary>
    /// sum if function from excel:
    /// http://office.microsoft.com/en-us/excel-help/sumif-HP005209292.aspx 
    /// </summary>
    [Export(typeof(AFunction))]
    public class SumIf : AFunction {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SumIf()
            : base("SumIf",
                   "SumIf(range,criteria,sumrange)",
                   "computes sumif") {
            this.AddParamterDescription("range", "The range to compute SumIf");
            this.AddParamterDescription("criteria", "The sumrange to compute SumIf");
            this.AddParamterDescription("sumrange", "The sumrange to compute SumIf");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            decimal sum = 0;

            if (function.Expressions.Length < 2 || function.Expressions.Length>3)
                throw new ArgumentException("SumIf() takes exactly 2 or 3 argument");

            #region prep condition
                        
            string matchexpession = "";
            String matcherresult = evaluator.Evaluate(function.Expressions[1]).ToString();

            // options - just a number
            decimal decimaltomatch;
            if (decimal.TryParse(matcherresult, out decimaltomatch)) {
                matchexpession = "value==" + decimaltomatch;
            } else {
                if (matcherresult.IndexOfAny(new char[] { '>', '=', '<' }) != -1) {
                    matchexpession = "value" + matcherresult;
                } else {
                   /* this code is so irksome i have made it defunct!
                    * bool justmatchvalue = false;
                    try {
                        matchexpession = "value" + matcherresult;
                        Expression testexpr = new Expression(matchexpession, evaluator.options);
                        string matchexpession1 = matchexpession;
                        testexpr.EvaluateParameter += delegate(string name, ParameterArgs args) {
                                                          args.Result = "1";
                                                          if (name == matchexpession1) {
                                                              justmatchvalue = true;
                                                          }
                                                      };
                        justmatchvalue = testexpr.HasErrors();
                    } catch (Exception e) {
                        justmatchvalue = true;
                    }
                    if (justmatchvalue) {*/
                        matchexpession = "value==" + "\""+matcherresult+"\"";
                    //}
                }
            }

            Expression matchexpr = new Expression(matchexpession, evaluator.options);
            matchexpr.EvaluateParameter += delegate(string name, ParameterArgs args) {
                args.Result = evaluator.EvaluateThisParameter(name);
            };
            matchexpr.Parameters = new Dictionary<string, object>() {{"value","1.0"}};

            if (matchexpr.HasErrors()) {
                Log.Error("sumif: I dont know what to do with " + matcherresult);
            }

            #endregion

            if (function.Expressions.Length == 2) {
                #region deal with simple 2 param case                
                String result = evaluator.Evaluate(function.Expressions[0]).ToString();
                if (result == "1") { // we are in dummy exec mode...
                    result = this.DoFakeExpansion(this.CleanString(function.Expressions[0].ToString())).Aggregate("{|{", (acc, next) => acc + "," + next) + "}|}";
                }
                // expand range
                if (result.Contains("{|{")) {
                    // we found a wrapped range...  so unwrap it and walk through its cells
                    string range = result.Replace("{|{", "").Replace("}|}", "");
                    foreach (var parameter in range.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)) {

                        object subresult = this.CreateAndEvalExpression(evaluator, parameter);

                        matchexpr.Parameters["value"] = subresult;
                        object matchRes = matchexpr.Evaluate();
                        if (Convert.ToBoolean(matchRes)) {
                            sum += Sum.ConvertToDecimal(subresult.ToString());
                        }
                    }
                }
                #endregion
            } else {
                // test if we need to adjust the sum_range...
 
                // get the two ranges.
                string range = this.CleanString(function.Expressions[0].ToString());
                string sumRange = this.CleanString(function.Expressions[2].ToString());
                // if there is no sheet in the address we add one to make things work..
                const string fakeSheet = "FAKESHEET!"; 
                bool addedRangeFakeSheet = !range.Contains("!");                
                if (addedRangeFakeSheet) {
                    range = fakeSheet + range;
                }
                bool addedsumRangeFakeSheet = !sumRange.Contains("!");
                if (addedsumRangeFakeSheet) {
                    sumRange = fakeSheet + sumRange;
                }
                // get their sizes
                ExcelAddress rangeAddress = new ExcelAddress(range);
                var rangeSize = rangeAddress.RangeDimensions();
                ExcelAddress sumRangeAddress = new ExcelAddress(sumRange);
                var sumRangeSize = sumRangeAddress.RangeDimensions();
                // if they do not match then reshape the sum_range. 
                if (rangeSize.Item1 != sumRangeSize.Item1 || rangeSize.Item2 != sumRangeSize.Item2) {
                    // then we need to resize the sum range... 
                    ExcelAddress newSumRangeAddress = ExcelAddress.CreateRange(sumRangeAddress.WorkSheet,
                                                                        sumRangeAddress.RangeTopLeft(), rangeSize.Item1,
                                                                        rangeSize.Item2);
                    Log.Debug("SumIf resized the sumrange from " + sumRangeAddress + " to " + newSumRangeAddress);
                    sumRangeAddress = newSumRangeAddress;
                }

                // remove the fake names...
                string realRangeAddress = rangeAddress.ToString();
                string realsumRangeAddress = sumRangeAddress.ToString();
                if (addedRangeFakeSheet) {
                    realRangeAddress = realRangeAddress.Replace(fakeSheet, "");
                }
                if (addedsumRangeFakeSheet) {
                    realsumRangeAddress = realsumRangeAddress.Replace(fakeSheet, "");
                }

                // now eval both cell ranges and run the matching over them

                
                string evaldrange = this.CreateAndEvalExpression(evaluator,realRangeAddress).ToString();               
                if (evaldrange.Contains("{|{")) {
                    // we found a wrapped range...  so unwrap it and walk through its cells
                    evaldrange = evaldrange.Replace("{|{", "").Replace("}|}", "");
                }
                string[] rangeCells = evaldrange.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                if (evaldrange == "1") {
                    rangeCells = this.DoFakeExpansion(realRangeAddress);
                }

                string evaldsumrange = this.CreateAndEvalExpression(evaluator, realsumRangeAddress).ToString();
                if (evaldsumrange.Contains("{|{")) {
                    // we found a wrapped range...  so unwrap it and walk through its cells
                    evaldsumrange = evaldsumrange.Replace("{|{", "").Replace("}|}", "");
                }
                string[] sumrangeCells = evaldsumrange.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                if (evaldrange == "1") {
                    sumrangeCells = this.DoFakeExpansion(realsumRangeAddress);
                }                

                if (rangeCells.Length != sumrangeCells.Length) {
                    Log.Error("Sum If tried but failed to construct a new sum range of the right size");
                }

                List<Tuple<string, string>> sumPairs =
                    rangeCells.Select((t, i) => new Tuple<string, string>(t, sumrangeCells[i])).ToList();

                // finally do the sum if! 

                foreach (var parameter in sumPairs) {

                    object subresult = this.CreateAndEvalExpression(evaluator, parameter.Item1);

                    matchexpr.Parameters["value"] = subresult;
                    object matchRes = matchexpr.Evaluate();
                    if (Convert.ToBoolean(matchRes)) {
                        object sumvalue = this.CreateAndEvalExpression(evaluator, parameter.Item2);

                        sum += Sum.ConvertToDecimal(sumvalue.ToString());
                    }
                }
            }

            return sum;
        }       
    }
}
