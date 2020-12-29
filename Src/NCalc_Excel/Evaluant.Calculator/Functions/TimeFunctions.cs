using System;
using System.ComponentModel.Composition;
using NCalcExcel.Domain;

namespace NCalcExcel.Functions {
    public class TimeFunctions {

        #region Now

        [Export(typeof(AFunction))] public class Now : AFunction {

            public Now() :
                base("Now", "Now()", "Returns current Time") { }


            public override object Evaluate(EvaluationVisitor evaluator, Function function) {
                return DateTime.Now;
            }
        }

        #endregion

        #region Minutes

        [Export(typeof(AFunction))] public class Minutes : AFunction {

            public Minutes() :
                base("Minutes", "Minutes(datetime)", "Returns number of minutes in a datetime") {
                this.AddParamterDescription("datetime", "A date time to get the number of minutes of");
            }


            public override object Evaluate(EvaluationVisitor evaluator, Function function) {
                if (function.Expressions.Length != 1)
                    throw new ArgumentException("Minutes() takes exactly 1 argument - a date time");

                return (Convert.ToDateTime(evaluator.Evaluate(function.Expressions[0]))).Minute;
            }
        }

        #endregion

        #region Seconds

        [Export(typeof(AFunction))] public class Seconds : AFunction {

            public Seconds() :
                base("Seconds", "Seconds(datetime)", "Returns number of seconds in a datetime") {
                this.AddParamterDescription("datetime", "A date time to get the number of seconds of");
            }


            public override object Evaluate(EvaluationVisitor evaluator, Function function) {
                if (function.Expressions.Length != 1)
                    throw new ArgumentException("Seconds() takes exactly 1 argument - a date time");

                return (Convert.ToDateTime(evaluator.Evaluate(function.Expressions[0]))).Second;
            }
        }

        #endregion

        #region Hours

        [Export(typeof(AFunction))] public class Hours : AFunction {

            public Hours() :
                base("Hours", "Hours(datetime)", "Returns number of hours in a datetime") {
                this.AddParamterDescription("datetime", "A date time to get the number of hours of");
            }


            public override object Evaluate(EvaluationVisitor evaluator, Function function) {
                if (function.Expressions.Length != 1)
                    throw new ArgumentException("Hours() takes exactly 1 argument - a date time");

                return (Convert.ToDateTime(evaluator.Evaluate(function.Expressions[0]))).Hour;
            }
        }

        #endregion
    }
}
