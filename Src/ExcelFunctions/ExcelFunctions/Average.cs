using System;
using System.ComponentModel.Composition;
using NCalcExcel;
using NCalcExcel.Domain;
using NCalcExcel.Functions;

namespace ExcelFunctions.ExcelFunctions {
    [Export(typeof(AFunction))]
    public class Average : AFunction {

        public Average()
            : base("Average",
                   "Average(number1,number2,number3...)",
                   "Returns the Average of a number of parameters") {
            this.AddParamterDescription("number1", "The first number to Average");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            decimal sum = 0;
            int count = 0;      

            foreach (LogicalExpression expression in function.Expressions) {              
                String result = evaluator.Evaluate(expression).ToString();

                if (result.Contains("{|{")) {
                    // we found a wrapped range... 
                    string range = result.Replace("{|{", "").Replace("}|}", "");
                    foreach (var parameter in range.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)) {
                        Expression expr = new Expression(parameter, evaluator.options);
                        expr.EvaluateParameter += delegate(string name, ParameterArgs args) {
                                                      args.Result = evaluator.EvaluateThisParameter(name);
                                                  };
                        object subresult = expr.Evaluate();

                        sum += ConvertToDecimal(subresult.ToString());
                        count++;
                    }
                } else {
                    sum += ConvertToDecimal(result);
                    count++;
                }
                
            }

            return (double) sum / (double) count;
        }

        public static decimal ConvertToDecimal(string result) {
            if (!string.IsNullOrWhiteSpace(result) && !string.IsNullOrEmpty(result) && result != "\"\"" && result != "-" && result != "'-'" && !result.StartsWith("{|{")) { // a dash is fine.... apparently!
                return Convert.ToDecimal(result);
            }
            return 0;
        }
    }
}
