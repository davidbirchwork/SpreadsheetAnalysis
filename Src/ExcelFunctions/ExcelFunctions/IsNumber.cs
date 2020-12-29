using System;
using System.ComponentModel.Composition;
using NCalcExcel.Domain;
using NCalcExcel.Functions;

namespace ExcelFunctions.ExcelFunctions {
    //http://office.microsoft.com/en-us/access-help/IsNumber-function-HA001228865.aspx
    [Export(typeof(AFunction))]
    public class IsNumber : AFunction {

        public IsNumber()
            : base("IsNumber",
                   "IsNumber(argument)",
                   "Mimics the IsNumber function in excel") {
            this.AddParamterDescription("argument", "some cell");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("IsNumber() takes exactly 1 argument");

            object res = evaluator.Evaluate(function.Expressions[0]);
            if (res == null) {
                return false;
            }
            if (ExcelErrors.IsError(res.ToString())) {
                return false;
            }

            string arg = Lookups.LookUpExtentions.CleanString(this, function.Expressions[0].ToString());
            if (evaluator.Parameters.ContainsKey(arg)) {
                bool isblank = Convert.ToBoolean(evaluator.Parameters["A_BLANK_A" + arg]);
                if (isblank) {
                    return false;
                }
            }

            string ress = res.ToString();
            decimal adecimal;
            if (decimal.TryParse(ress, out adecimal)) {
                return true;
            }


            return false;
        }
    }
}
