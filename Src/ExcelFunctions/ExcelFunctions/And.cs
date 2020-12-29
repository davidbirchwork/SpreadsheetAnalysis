using System;
using System.ComponentModel.Composition;
using NCalcExcel;
using NCalcExcel.Domain;
using NCalcExcel.Functions;

namespace ExcelFunctions.ExcelFunctions {
    [Export(typeof(AFunction))]
    public class And : AFunction {

        public And()
            : base("And",
                   "And(boolean1,boolean2,boolean3...)",
                   "Returns the logical And of a boolean of parameters") {
            this.AddParamterDescription("boolean1", "The first boolean to And");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            bool andresult = true;
            int p = 0;
            while (andresult && p < function.Expressions.Length) {
                LogicalExpression expression = function.Expressions[p];
                String result = evaluator.Evaluate(expression).ToString();
                bool bresult;
                if (Boolean.TryParse(result, out bresult)) {
                    andresult = bresult; // logical hack ;)                    
                } else {
                    double dresult;
                    if (double.TryParse(result, out dresult)) {
                        if (dresult == 1.0) {
                            andresult = true;
                        } else if (dresult == 0.0) {
                            andresult = false;
                        } else {
                            throw new ArgumentException("Could not parse boolean" + result);
                        }
                    } else {
                        throw new ArgumentException("Could not parse boolean" + result);
                    }                    
                }

                if (evaluator.options.HasFlag(EvaluateOptions.DebugMode)) {
                    andresult = true; // make sure all parameters get eval'd
                }
                p++;
            }
 
            return andresult;
        }
    }
}
