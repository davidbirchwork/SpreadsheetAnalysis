using System;
using System.ComponentModel.Composition;
using NCalcExcel.Domain;
using NCalcExcel.Functions;

namespace ExcelFunctions.ExcelFunctions {
    //http://office.microsoft.com/en-us/access-help/iserror-function-HA001228865.aspx
    [Export(typeof(AFunction))]
    public class Iserror : AFunction {

        public Iserror()
            : base("ISERROR",
                   "ISERROR(argument)",
                   "Mimics the ISERROR function in excel - by returning false!!") {
            this.AddParamterDescription("argument", "some cell");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("ISERROR() takes exactly 1 argument");

            object res = evaluator.Evaluate(function.Expressions[0]);
            if (res == null) {
                return true;
            }
            if (ExcelErrors.IsError(res.ToString())) {
                return true;
            }

            return false; 
        }
    }
}
