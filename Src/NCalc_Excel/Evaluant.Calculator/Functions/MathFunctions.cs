using System;
using System.ComponentModel.Composition;
using NCalcExcel.Domain;
using System.Linq;

namespace NCalcExcel.Functions {

    #region Abs
    [Export(typeof(AFunction))] public class Abs : AFunction {

        public Abs()
            : base("Abs",
                   "Abs(number)",
                   "Returns the absolute value of a number.") {
            this.AddParamterDescription("number", "The number to take the absolute value of.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Abs() takes exactly 1 argument");

            return Math.Abs(Convert.ToDecimal(
                    evaluator.Evaluate(function.Expressions[0]))
                    );
        }
    }
    #endregion

    #region Acos
    [Export(typeof(AFunction))] public class Acos : AFunction {

        public Acos()
            : base("Acos",
            "Acos(number)",
            "A number representing a cosine, where -1 ≤ number ≤ 1.") {
            this.AddParamterDescription("number", "A number representing a cosine, where -1 ≤ number ≤ 1.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Acos() takes exactly 1 argument");

            return Math.Acos(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region Asin
    [Export(typeof(AFunction))] public class Asin : AFunction {

        public Asin()
            : base("Asin",
                   "Asin(number)",
                   "Returns the angle whose sine is the specified number.") {
            this.AddParamterDescription("number", "A number representing a sine, where -1 ≤number≤ 1.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Asin() takes exactly 1 argument");

            return Math.Asin(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region Atan
    [Export(typeof(AFunction))] public class Atan : AFunction {

        public Atan()
            : base("Atan",
                   "Atan(number)",
                   "Returns the angle whose tangent is the specified number.") {
            this.AddParamterDescription("number", "A number representing a tangent.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Atan() takes exactly 1 argument");

            return Math.Atan(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region Ceiling
    [Export(typeof(AFunction))] public class Ceiling : AFunction {

        public Ceiling()
            : base("Ceiling",
                   "Ceiling(number)",
                   "Returns the smallest integer greater than or equal to the specified double-precision floating-point number.") {
            this.AddParamterDescription("number", "The double-precision floating-point number to take the ceiling of.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Ceiling() takes exactly 1 argument");

            return Math.Ceiling(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region CosR
    [Export(typeof(AFunction))] public class CosR : AFunction {

        public CosR()
            : base("CosR",
                   "CosR(angle)",
                   "Returns the cosine of the specified angle in RADIANS.") {
            this.AddParamterDescription("angle", "An angle, measured in radians.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("CosR() takes exactly 1 argument");

            return Math.Cos(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region SinR
    [Export(typeof(AFunction))] public class SinR : AFunction {

        public SinR()
            : base("SinR",
                   "SinR(angle)",
                   "Returns the sine of the specified angle in RADIANS.") {
            this.AddParamterDescription("angle", "An angle, measured in radians.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("SinR() takes exactly 1 argument");

            return Math.Sin(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region TanR
    [Export(typeof(AFunction))] public class TanR : AFunction {

        public TanR()
            : base("TanR",
                   "TanR(angle)",
                   "Returns the tangent of the specified angle in RADIANS.") {
            this.AddParamterDescription("angle", "An angle, measured in radians.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("TanR() takes exactly 1 argument");

            return Math.Tan(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region Cos
    [Export(typeof(AFunction))] public class Cos : AFunction {

        public Cos()
            : base("Cos",
                   "Cos(angle)",
                   "Returns the cosine of the specified angle in DEGREES.") {
            this.AddParamterDescription("angle", "An angle, measured in degrees.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Cos() takes exactly 1 argument");

            double radians = Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])) * (Math.PI / 180);

            return Math.Cos(radians);
        }
    }
    #endregion

    #region Sin
    [Export(typeof(AFunction))] public class Sin : AFunction {

        public Sin()
            : base("Sin",
                   "Sin(angle)",
                   "Returns the sine of the specified angle in DEGREES.") {
            this.AddParamterDescription("angle", "An angle, measured in degrees.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Sin() takes exactly 1 argument");

            double radians = Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])) * (Math.PI / 180);

            return Math.Sin(radians);
        }
    }
    #endregion

    #region Tan
    [Export(typeof(AFunction))] public class Tan : AFunction {

        public Tan()
            : base("Tan",
                   "Tan(angle)",
                   "Returns the tangent of the specified angle in DEGREES.") {
            this.AddParamterDescription("angle", "An angle, measured in degrees.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Tan() takes exactly 1 argument");

            double radians = Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])) * (Math.PI / 180);

            return Math.Tan(radians);
        }
    }
    #endregion

    #region Exp
    [Export(typeof(AFunction))] public class Exp : AFunction {

        public Exp()
            : base("Exp",
                   "Exp(number)",
                   "Returns e raised to the specified power.") {
            this.AddParamterDescription("number", "A number specifying a power.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Exp() takes exactly 1 argument");

            return Math.Exp(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region Floor
    [Export(typeof(AFunction))] public class Floor : AFunction {

        public Floor()
            : base("Floor",
                   "Floor(number)",
                   "Returns the largest integer less than or equal to the specified double-precision floating-point number.") {
            this.AddParamterDescription("number", "A double-precision floating-point number to take the floor of.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Floor() takes exactly 1 argument");

            return Math.Floor(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region IEEERemainder
    [Export(typeof(AFunction))] public class IEEERemainder : AFunction {

        public IEEERemainder()
            : base("IEEERemainder",
                   "IEEERemainder(x,y)",
                   "Returns the remainder resulting from the division of a specified number by another specified number.") {
            this.AddParamterDescription("x", "A dividend.");
            this.AddParamterDescription("y", "A divisor.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 2)
                throw new ArgumentException("IEEERemainder() takes exactly 2 arguments");

            return Math.IEEERemainder(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])), Convert.ToDouble(evaluator.Evaluate(function.Expressions[1])));
        }
    }
    #endregion

    #region Log
    [Export(typeof(AFunction))] public class Log : AFunction {

        public Log()
            : base("Log",
                   "Log(number,base)",
                   "Returns the logarithm of a specified number in a specified base.") {
            this.AddParamterDescription("number", "A number whose logarithm is to be found.");
            this.AddParamterDescription("base", "The base of the logarithm.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 2)
                throw new ArgumentException("Log() takes exactly 2 arguments");

            return Math.Log(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])), Convert.ToDouble(evaluator.Evaluate(function.Expressions[1])));
        }
    }
    #endregion

    #region Log10
    [Export(typeof(AFunction))] public class Log10 : AFunction {

        public Log10()
            : base("Log10",
                   "Log10(number)",
                   "Returns the base 10 logarithm of a specified number.") {
            this.AddParamterDescription("number", "A number whose logarithm is to be found.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Log10() takes exactly 1 argument");

            return Math.Log10(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region Pow
    [Export(typeof(AFunction))] public class Pow : AFunction {

        public Pow()
            : base("Pow",
                   "Pow(x,y)",
                   "Returns a specified number raised to the specified power.") {
            this.AddParamterDescription("x", "A double-precision floating-point number to be raised to a power.");
            this.AddParamterDescription("y", "A double-precision floating-point number that specifies a power.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 2)
                throw new ArgumentException("Pow() takes exactly 2 arguments");

            return Math.Pow(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])), Convert.ToDouble(evaluator.Evaluate(function.Expressions[1])));
        }
    }
    #endregion

    #region Round
    [Export(typeof(AFunction))] public class Round : AFunction {

        public Round()
            : base("Round",
                   "Round(value,digits)",
                   "Rounds a double-precision floating-point value to the specified precision.") {
            this.AddParamterDescription("value", "A double-precision floating-point number to be rounded.");
            this.AddParamterDescription("digits", "The number of fractional digits (precision) in the return value.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 2)
                throw new ArgumentException("Round() takes exactly 2 arguments");

            if (function.Expressions.Length != 2)
                throw new ArgumentException("Round() takes exactly 2 arguments");

            MidpointRounding rounding = (evaluator.options & EvaluateOptions.RoundAwayFromZero) == EvaluateOptions.RoundAwayFromZero ? MidpointRounding.AwayFromZero : MidpointRounding.ToEven;


            double result;
            var digits = Convert.ToInt16(evaluator.Evaluate(function.Expressions[1]));
            var value = Convert.ToDouble(evaluator.Evaluate(function.Expressions[0]));
            if (digits >= 0) {
                result = Math.Round(value, digits, rounding);
            }
            else {
                // round to the nearest 10, 100, 100 ...  this is to support excel - https://support.office.com/en-us/article/ROUND-function-c018c5d8-40fb-4053-90b1-b3e7f61a213c 
                var pow = Math.Pow(10,-digits);
                result = pow* Math.Round(value / pow, 0, rounding);
            }
            return result;

        }
    }
    #endregion

    #region RoundUp

    [Export(typeof(AFunction))]
    public class RoundUp : AFunction {

        public RoundUp()
            : base("RoundUp",
                   "RoundUp(argument)",
                   "Mimics the RoundUp function in excel - by returning 1") {
            this.AddParamterDescription("argument", "some cell");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 2)
                throw new ArgumentException("RoundUp() takes exactly 2 argument");

            double result = Convert.ToDouble(evaluator.Evaluate(function.Expressions[0]));
            int precision = Convert.ToInt32(evaluator.Evaluate(function.Expressions[1]));
            if (precision < 0) {
                throw new NotImplementedException("not implemented negative precisions");
            }

            return RoundDoubleUp(result, precision);
        }

        public static double RoundDoubleUp(double figure, int precision) {
            int sign = figure < 0 ? -1 : 1;
            double newFigure = Math.Round(figure, precision);
            double difference = (sign*figure) - (sign*newFigure); // make difference positive
            var floaterror = 5; // 
            /*
             * RoudUp(45.00000000251116071442584602201,0) gives 45 in excel and 46 in my code
             */
            double tolerance = (1/Math.Pow(10, precision + floaterror))*sign;
            if (difference > 0) // excel is pernickity
            {
                if (Math.Abs(difference) > Math.Abs(tolerance)) {
                    //Figure was rounded down 

                    double padding = (1/Math.Pow(10, precision))*sign;
                    newFigure += padding;
                }
                else {
                    // note we are deviating from Excel 
                    floaterror = 555;
                }
            }

            return newFigure;
        }
    }
    #endregion

    #region Sign
    [Export(typeof(AFunction))] public class Sign : AFunction {

        public Sign()
            : base("Sign",
                   "Sign(number)",
                   "A number indicating the sign of value.Number Description -1 value is less than zero. 0 value is equal to zero. 1 value is greater than zero.") {
            this.AddParamterDescription("number", "A signed number to get the sign of.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Sign() takes exactly 1 argument");

            return Math.Sign(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region Sqrt
    [Export(typeof(AFunction))] public class Sqrt : AFunction {

        public Sqrt()
            : base("Sqrt",
                   "Sqrt(number)",
                   "Returns the square root of a specified number.") {
            this.AddParamterDescription("number", "The number to take the square root of.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Sqrt() takes exactly 1 argument");

            return Math.Sqrt(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region Max
    [Export(typeof(AFunction))] public class Max : AFunction {

        public Max()
            : base("Max",
                   "Max(val1,val2)",
                   "Returns the larger of two decimal numbers.") {
            this.AddParamterDescription("val1", "First number to compare.");
            this.AddParamterDescription("val2", "Second number to compare.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 2)
                throw new ArgumentException("Max() takes exactly 2 arguments");

            decimal maxleft = Convert.ToDecimal(evaluator.Evaluate(function.Expressions[0]));
            decimal maxright = Convert.ToDecimal(evaluator.Evaluate(function.Expressions[1]));

            return Math.Max(maxleft, maxright);
        }
    }
    #endregion

    #region Min
    [Export(typeof(AFunction))] public class Min : AFunction {

        public Min()
            : base("Min",
                   "Min(val1,val2)",
                   "Returns the smaller of two decimal numbers.") {
            this.AddParamterDescription("val1", "First number to compare.");
            this.AddParamterDescription("val2", "Second number to compare.");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 2)
                throw new ArgumentException("Min() takes exactly 2 arguments");

            decimal minleft = Convert.ToDecimal(evaluator.Evaluate(function.Expressions[0]));
            decimal minright = Convert.ToDecimal(evaluator.Evaluate(function.Expressions[1]));

            return Math.Min(minleft, minright);
        }
    }
    #endregion

    #region PI
    [Export(typeof(AFunction))] public class PI : AFunction {

        public PI()
            : base("PI",
                   "PI()",
                   "PI() returns 3.14.....") {
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            return Math.PI;
        }
    }
    #endregion  

    #region Trunc
    [Export(typeof(AFunction))]
    public class Trunc : AFunction {
        public Trunc()
            : base("Truncate",
                   "Truncate(val)",
                   "Truncate a number to an integer") {
            this.AddParamterDescription("val", "Number to Truncate.");            
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 1)
                throw new ArgumentException("Truncate() takes exactly 1 argument");

            return Math.Truncate(Convert.ToDouble(evaluator.Evaluate(function.Expressions[0])));
        }
    }
    #endregion

    #region In
    [Export(typeof(AFunction))]
    public class InFunc : AFunction {
        public InFunc()
            : base("in",
                   "in(needle,haystack1,haystack2)",
                   "tests if needle appears in a number of haystacks, accepts an arbitrary number of arguments") {
                       this.AddParamterDescription("needle", "value to find");
                       this.AddParamterDescription("haystack1", "haystack to search");
                       this.AddParamterDescription("haystack2", "haystack to search");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length < 2)
                throw new ArgumentException("in() takes at least 2 arguments");

            object parameter = evaluator.Evaluate(function.Expressions[0]);

            bool evaluation = false;

            // Goes through any values, and stop when one is found
            for (int i = 1; i < function.Expressions.Length; i++) {
                object argument = evaluator.Evaluate(function.Expressions[i]);
                string stringargument = argument as string;
                if (stringargument !=  null) {
                    if (stringargument.Split(new[] {','}).Any(arg => evaluator.CompareUsingMostPreciseType(parameter, arg) == 0)) {
                        evaluation = true;
                        break;
                    }                
                } else if (evaluator.CompareUsingMostPreciseType(parameter, argument) == 0) {
                    evaluation = true;
                    break;
                }
            }

            return evaluation;
        }
    }
    #endregion

    #region Random
    [Export(typeof(AFunction))]
    public class RandomFunc : AFunction {
        public RandomFunc()
            : base("Random",
                    "Random(min,max)",
                    "Produces a random integer between min and max.") {
            this.AddParamterDescription("min", "Minimum of the range (inclusive)");
            this.AddParamterDescription("max", "Maximum of the range (inclusive)");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 2)
                throw new ArgumentException("Random(min,max) takes exactly 2 arguments");

            int min = Convert.ToInt32(evaluator.Evaluate(function.Expressions[0]));
            int max = Convert.ToInt32(evaluator.Evaluate(function.Expressions[1]));
            int result = int.MinValue;
            lock (Classlock) { // assume this is ok
                result = RandomFunc.RandomStore.Next(min, max);
            }
            return result;
        }

        private static readonly object Classlock = new object();

        // static random store
        private static System.Random _randomstore;
        private static System.Random RandomStore {
            get {
                if (_randomstore == null) {
                    _randomstore = new Random(DateTime.Now.Millisecond); // fairly random
                }
                return _randomstore;
            }            
        }
    }
    #endregion

    #region LimitTo
    [Export(typeof(AFunction))]
    public class LimitTo : AFunction {
        public LimitTo()
            : base("LimitTo",
                    "LimitTo(number,LowerBound,UpperBound)",
                    "Limits a number to the given bounds") {
            this.AddParamterDescription("number", "A number to limit.");
            this.AddParamterDescription("LowerBound", "Lower (inclusive) bound");
            this.AddParamterDescription("UpperBound", "Upper (inclusive) bound");
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {
            if (function.Expressions.Length != 3)
                throw new ArgumentException("LimitTo() takes exactly 3 arguments");

            double value = Convert.ToDouble(evaluator.Evaluate(function.Expressions[0]));
            double lower = Convert.ToDouble(evaluator.Evaluate(function.Expressions[1]));
            double upper = Convert.ToDouble(evaluator.Evaluate(function.Expressions[2]));

            if (value < lower)
                value = lower;
            if (value > upper)
                value = upper;

            return value;
        }
    }
    #endregion
}
