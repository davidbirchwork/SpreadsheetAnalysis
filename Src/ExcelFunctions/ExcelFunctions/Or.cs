using System;
using System.ComponentModel.Composition;
using NCalcExcel;
using NCalcExcel.Domain;
using NCalcExcel.Functions;

namespace ExcelFunctions.ExcelFunctions {
    [Export(typeof(AFunction))]
    public class Or : AFunction {

        public Or()
            : base("Or",
                   "Or(boolean1,boolean2,boolean3...)",
                   "Returns the logical Or of a boolean of parameters") {
            this.AddParamterDescription("boolean1", "The first boolean to Or");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            bool orresult = false;
            int p = 0;
            while (!orresult && p < function.Expressions.Length) {
                LogicalExpression expression = function.Expressions[p];
                String result = evaluator.Evaluate(expression).ToString();

                if (result.Contains("{|{")) {
                    // we found a wrapped range... 
                    string range = result.Replace("{|{", "").Replace("}|}", "");
                    foreach (var parameter in range.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)) {
                        Expression expr = new Expression(parameter, evaluator.options);
                        expr.EvaluateParameter += delegate(string name, ParameterArgs args) {
                                                      args.Result = evaluator.EvaluateThisParameter(name);
                                                  };
                        object subresult = expr.Evaluate();
                        result = subresult.ToString();

                        orresult = orresult ||  EvaluateOr(result);
                    }
                } else {
                    orresult = EvaluateOr(result); // logic hack
                }

                if (evaluator.options.HasFlag(EvaluateOptions.DebugMode)) {
                    orresult = false; // make sure all parameters get eval'd
                }

                p++;
            }

            return orresult;
        }

        private bool EvaluateOr(string result) {
            result = Lookups.LookUpExtentions.CleanString(this,result);
            bool orresult;
            bool bresult;
            if (Boolean.TryParse(result, out bresult)) {
                orresult = bresult;               
            } else {
                double dresult;
                if (double.TryParse(result, out dresult)) {
                    if (dresult == 1.0) {
                        orresult = true;
                    } else if (dresult == 0.0) {
                        orresult = false;
                    } else {
                        throw new ArgumentException("Could not parse boolean" + result); 
                    }
                } else {
                    throw new ArgumentException("Could not parse boolean" + result);
                }
            }

            return orresult;
        }
    }
}
