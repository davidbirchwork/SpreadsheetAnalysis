using System;
using System.ComponentModel.Composition;
using NCalcExcel.Domain;
using NCalcExcel.Functions;

namespace ExcelFunctions.ExcelFunctions {
    [Export(typeof(AFunction))]
    public class Type : AFunction {

        public Type()
            : base("Type",
                   "Type(argument)",
                   "Mimics the Type function in excel - by returning 1") {
                       this.AddParamterDescription("argument", "some cell");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Type() takes exactly 1 argument");
            
            object result = evaluator.Evaluate(function.Expressions[0]);

            if (result.GetType().IsArray) {
                return 64;
            }

            decimal isnumeric;
            if (Decimal.TryParse(result.ToString(), out isnumeric)) {
                return 1;
            }

            bool isbool;
            if (bool.TryParse(result.ToString(), out isbool)) {
                return 4;
            }
            return 2; // string
        }
    }
}
