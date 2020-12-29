using System.ComponentModel.Composition;
using NCalcExcel.Domain;

namespace NCalcExcel.Functions {
    using System;

    namespace Evaluant.Calculator.Domain {
        #region if
        [Export(typeof(AFunction))]
        public class ControlIf : AFunction {

                public ControlIf()
                    : base("if",
                           "if(condition,trueExpression,falseExpression)",
                           "Represents a choice in the expression evaluation.") {
                    this.AddParamterDescription("condition", "Condition to evaluate.");
                    this.AddParamterDescription("trueExpression", "Expression to evaluate if condition is true.");
                    this.AddParamterDescription("falseExpression", "Expression to evaluate if condition is false.");
                }

                public override object Evaluate(EvaluationVisitor evaluator, Function function) {                    
                    if (function.Expressions.Length != 3)
                        throw new ArgumentException("if() takes exactly 3 arguments");

                    object conditionValue = evaluator.Evaluate(function.Expressions[0]);

                    if (ExcelErrors.IsError(conditionValue.ToString())) {
                        return conditionValue;                        
                    }

                    bool cond = Convert.ToBoolean(conditionValue);

                    if ((evaluator.options & EvaluateOptions.DebugMode) == EvaluateOptions.DebugMode) { // todo we *could* be lazy here
                        object truearg = evaluator.Evaluate(function.Expressions[1]);
                        object falsearg = evaluator.Evaluate(function.Expressions[2]);
                        return cond ? truearg : falsearg;
                    }

                    return cond ? evaluator.Evaluate(function.Expressions[1]) : evaluator.Evaluate(function.Expressions[2]);

                }
            }
            #endregion
        }
}
