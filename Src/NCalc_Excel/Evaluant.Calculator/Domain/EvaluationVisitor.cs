using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using NCalcExcel.Functions;

namespace NCalcExcel.Domain
{
    public class EvaluationVisitor : LogicalExpressionVisitor
    {
        private NumberFormatInfo numberFormatInfo;
        private EvaluateOptions _options = EvaluateOptions.None;
        public EvaluateOptions options 
    {
            get { return this._options; }
            private set { this._options = value; }
    }

        private bool IgnoreCase { get { return (options & EvaluateOptions.IgnoreCase) == EvaluateOptions.IgnoreCase; } }

        public EvaluationVisitor(EvaluateOptions options) : this(options, new ConcurrentDictionary<string, AFunction>())
        { }

        public EvaluationVisitor(EvaluateOptions options, ConcurrentDictionary<string, AFunction> aFunctions) {
            numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
            this.options = options;
            this.Functions = aFunctions;
        }

        protected ConcurrentDictionary<string, AFunction> Functions {
            get;
            set;
        }

        protected object result;       

        public object Result
        {
            get { return result; }
        }

        public object Evaluate(LogicalExpression expression)
        {
            expression.Accept(this);
            return result;
        }

        public override void Visit(LogicalExpression expression)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the the most precise type.
        /// </summary>
        /// <param name="a">Type a.</param>
        /// <param name="b">Type b.</param>
        /// <returns></returns>
        private Type GetMostPreciseType(Type a, Type b)
        {            
            foreach (Type t in new Type[] { typeof(String), typeof(Decimal), typeof(Double), typeof(Int32), typeof(Boolean) })
            {
                if (a == t || b == t)
                {
                    return t;
                }
            }

            return a;
        }

        public int CompareUsingMostPreciseType(object a, object b)
        {
            string astring = a.ToString();
            string bstring = b.ToString();
            
            // following pair of statements deals with comparing empty cells to "" eg  =A1 where A1 is blank will return zero but  IF(A1="","",A1) where A1 is blank  will take the TRUE branch and return ""            
            
            if (astring == "0" && b.GetType() == typeof(String)) {
                a = "";
            }
            
            if (a.GetType() == typeof(String) && bstring == "0") {
                b = "";
            }

            if (bstring.Contains("Person")) {
                b = bstring;
            }

            Type mpt = GetMostPreciseType(a.GetType(), b.GetType());
            if (mpt == typeof(string)) {
                if (astring.StartsWith("'")) {
                    astring = astring.Substring(1);
                }
                if (astring.EndsWith("'")) {
                    astring = astring.Substring(0, astring.Length - 1);
                }

                if (bstring.StartsWith("'")) {
                    bstring = bstring.Substring(1);
                }
                if (bstring.EndsWith("'")) {
                    bstring = bstring.Substring(0, bstring.Length - 1);
                }
                a = astring;
                b = bstring;
            }
            // do epsilon comparision...
            if (mpt == typeof(decimal)) {
                decimal ad = (decimal) Convert.ChangeType(a, mpt);                
                decimal bd = (decimal) Convert.ChangeType(b, mpt);
                decimal difference = Math.Abs(ad-bd);
                if (astring.Contains(".")) {
                    int digitsbeforedecimalpoint = astring.IndexOf(".");
                    decimal epsilon = (decimal) Math.Pow(10, Math.Min(14 - digitsbeforedecimalpoint,9));// PRECISION
                    epsilon = 1/epsilon;
                    if (difference < (epsilon)) {
                        return 0;
                    }
                }
            }
            return Comparer.Default.Compare(Convert.ChangeType(a, mpt), Convert.ChangeType(b, mpt));
        }

        public override void Visit(TernaryExpression expression)
        {
            // Evaluates the left expression and saves the value
            expression.LeftExpression.Accept(this);
            bool left = Convert.ToBoolean(result);

            if (left)
            {
                expression.MiddleExpression.Accept(this);
            }
            else
            {
                expression.RightExpression.Accept(this);
            }
        }

        public override void Visit(BinaryExpression expression)
        {
            // Evaluates the left expression and saves the value
            expression.LeftExpression.Accept(this);
            object left = result;

            // Evaluates the right expression and saves the value
            expression.RightExpression.Accept(this);
            object right = result;

            #region deal with Excel Error Propogation 
            // e.g. N/A==anything  return N/A  problem is that N/A is represented as a low int value 

            if (ExcelErrors.IsError(left.ToString())) {
                result = left;
                return;
            }
            if (ExcelErrors.IsError(right.ToString())) {
                result = right;
                return;
            }
            
            #endregion

            switch (expression.Type)
            {
                case BinaryExpressionType.And:
                    result = Convert.ToBoolean(left) && Convert.ToBoolean(right);
                    break;

                case BinaryExpressionType.Or:
                    result = Convert.ToBoolean(left) || Convert.ToBoolean(right);
                    break;

                case BinaryExpressionType.Div:
                    result = Numbers.Divide(Convert.ToDouble(Numbers.ConvertIfString(left)), right);
                    break;

                case BinaryExpressionType.Equal:
                    // Use the type of the left operand to make the comparison
                    result = CompareUsingMostPreciseType(left, right) == 0;
                    break;

                case BinaryExpressionType.Greater:
                    // Use the type of the left operand to make the comparison
                    result = CompareUsingMostPreciseType(left, right) > 0;
                    break;

                case BinaryExpressionType.GreaterOrEqual:
                    // Use the type of the left operand to make the comparison
                    result = CompareUsingMostPreciseType(left, right) >= 0;
                    break;

                case BinaryExpressionType.Lesser:
                    // Use the type of the left operand to make the comparison
                    result = CompareUsingMostPreciseType(left, right) < 0;
                    break;

                case BinaryExpressionType.LesserOrEqual:
                    // Use the type of the left operand to make the comparison
                    result = CompareUsingMostPreciseType(left, right) <= 0;
                    break;

                case BinaryExpressionType.Minus:
                    result = Numbers.Soustract(left, right);
                    break;

                case BinaryExpressionType.Modulo:
                    result = Numbers.Modulo(left, right);
                    break;

                case BinaryExpressionType.NotEqual:
                    // Use the type of the left operand to make the comparison
                    result = CompareUsingMostPreciseType(left, right) != 0;
                    break;

                case BinaryExpressionType.Plus:
                    if (left is string && !((options & EvaluateOptions.DontUseStringConcat) == EvaluateOptions.DontUseStringConcat))
                    {                        
                        result = String.Concat(left, right);
                    }
                    else
                    {
                        result = Numbers.Add(left, right);
                    }

                    break;

                case BinaryExpressionType.Times:
                    result = Numbers.Multiply(left, right);
                    break;

                case BinaryExpressionType.BitwiseAnd:
                    result = Convert.ToUInt16(left) & Convert.ToUInt16(right);
                    break;


                case BinaryExpressionType.BitwiseOr:
                    result = Convert.ToUInt16(left) | Convert.ToUInt16(right);
                    break;


                case BinaryExpressionType.BitwiseXOr:
                    //result = Convert.ToUInt16(left) ^ Convert.ToUInt16(right); FIXPOWER
                    result = Convert.ToDecimal(Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right)));
                    break;


                case BinaryExpressionType.LeftShift:
                    result = Convert.ToUInt16(left) << Convert.ToUInt16(right);
                    break;


                case BinaryExpressionType.RightShift:
                    result = Convert.ToUInt16(left) >> Convert.ToUInt16(right);
                    break;
            }

            /*if (((options & EvaluateOptions.ReduceTo15Sigfig) == EvaluateOptions.ReduceTo15Sigfig)) {
                if (result.GetType() == typeof (double)) {
                    result = RoundTo15SigFigs((double) result);
                } else if (result.GetType() == typeof (decimal)) {
                    result = RoundTo15SigFigs((decimal) result);
                }
            }*/
        }

        public static double RoundTo15SigFigs(double value) {

            //0.0 81662 59168 70416
            string s = value.ToString().Replace("-", "");
            double scalefactor = Math.Pow(10, s.IndexOf(".") - 1);
            if (scalefactor == 1 && s.StartsWith("0.")) {
                s = s.Replace("0.", "");
                scalefactor = scalefactor / 10;
                while (s.StartsWith("0")) {
                    s = s.Substring(1);
                    scalefactor = scalefactor / 10;
                }
            }
            double atzero = value / scalefactor;
            double res = Math.Round(atzero, 14);

            return scalefactor * res;
        }

        public static decimal RoundTo15SigFigs(decimal value) {

            //0.0 81662 59168 70416
            //49861 . 03045 06247 05
            string s = String.Format("{0:0.00000000000000000000}", value).Replace("-", "");
            decimal scalefactor = (decimal)Math.Pow(10, s.IndexOf(".") - 1);
            if (scalefactor == 1 && s.StartsWith("0.")) {
                s = s.Replace("0.", "");
                scalefactor = scalefactor / 10;
                while (s.StartsWith("0")) {
                    s = s.Substring(1);
                    scalefactor = scalefactor / 10;
                }
            }
            scalefactor = scalefactor == 0 ? 1 : scalefactor;
            decimal atzero = value / scalefactor;
            decimal res = Math.Round(atzero, 14);

            return scalefactor * res;

        }

        public override void Visit(UnaryExpression expression)
        {
            // Recursively evaluates the underlying expression
            expression.Expression.Accept(this);

            switch (expression.Type)
            {
                case UnaryExpressionType.Not:
                    result = !Convert.ToBoolean(result);
                    break;

                case UnaryExpressionType.Negate:
                    result = Numbers.Soustract(0, result);
                    break;

                case UnaryExpressionType.BitwiseNot:
                    result = ~Convert.ToUInt16(result);
                    break;
            }
        }

        public override void Visit(ValueExpression expression)
        {
            result = expression.Value;
        }

        public override void Visit(Function function)
        {
            FunctionArgs args = new FunctionArgs();

            // Don't call parameters right now, instead let the function do it as needed.
            // Some parameters shouldn't be called, for instance, in a if(), the "not" value might be a division by zero
            // Evaluating every value could produce unexpected behaviour
            args.Parameters = new Expression[function.Expressions.Length];
            for (int i = 0; i < function.Expressions.Length; i++ )
            {
                args.Parameters[i] =  new Expression(function.Expressions[i], options);
                args.Parameters[i].EvaluateFunction += EvaluateFunction;
                args.Parameters[i].EvaluateParameter += EvaluateParameter;

                // Assign the parameters of the Expression to the arguments so that custom Functions and Parameters can use them
                args.Parameters[i].Parameters = this.Parameters;
            }

            if ((options & EvaluateOptions.BuiltInFunctionsFirst) == EvaluateOptions.BuiltInFunctionsFirst) {
                if (this.Functions.ContainsKey(function.Identifier.Name.ToLower())) { // dupe code with below
                    AFunction func = this.Functions[function.Identifier.Name.ToLower()];
                    CheckCase(func.Name, function.Identifier.Name);
                    result = func.Evaluate(this, function);
                    return;
                }
            }

            // Calls external implementation
            OnEvaluateFunction(IgnoreCase ? function.Identifier.Name.ToLower() : function.Identifier.Name, args);

            // If an external implementation was found get the result back
            if (args.HasResult)
            {
                result = args.Result;
                return;
            }

            if ((options & EvaluateOptions.BuiltInFunctionsFirst) != EvaluateOptions.BuiltInFunctionsFirst) {
                if (this.Functions.ContainsKey(function.Identifier.Name.ToLower())) { // dupe code with above
                    AFunction func = this.Functions[function.Identifier.Name.ToLower()];
                    CheckCase(func.Name, function.Identifier.Name);
                    result = func.Evaluate(this, function);
                    return;
                }
            }

            throw new ArgumentException("Function not found", function.Identifier.Name);
                    
      
        }

        private void CheckCase(string function, string called)
        {
            if (IgnoreCase)
            {
                if (function.ToLower() == called.ToLower())
                {
                    return;
                }
                else
                {
                    throw new ArgumentException("Function not found",
                        called);
                }
            }
            else
            {
                if (function != called)
                {
                    throw new ArgumentException(String.Format("Function not found {0}. Try {1} instead.",
                        called, function));
                }
            }
        }

        public event EvaluateFunctionHandler EvaluateFunction;

        private void OnEvaluateFunction(string name, FunctionArgs args)
        {
            if (EvaluateFunction != null)
                EvaluateFunction(name, args);
        }

        public override void Visit(Identifier parameter)
        {
            if (Parameters.ContainsKey(parameter.Name))
            {
                // The parameter is defined in the hashtable
                if (Parameters[parameter.Name] is Expression)
                {
                    // The parameter is itself another Expression
                    Expression expression = (Expression)Parameters[parameter.Name];

                    // Overloads parameters 
                    foreach (var p in Parameters)
                    {
                        expression.Parameters[p.Key] = p.Value;
                    }

                    expression.EvaluateFunction += EvaluateFunction;
                    expression.EvaluateParameter += EvaluateParameter;

                    result = ((Expression)Parameters[parameter.Name]).Evaluate();
                }
                else
                    result = Parameters[parameter.Name];
            }
            else
            {
                // The parameter should be defined in a call back method
                ParameterArgs args = new ParameterArgs(this.options);
                

                // Calls external implementation
                OnEvaluateParameter(parameter.Name, args);

                if (!args.HasResult)
                    throw new ArgumentException("Parameter was not defined", parameter.Name);

                result = args.Result;
            }
        }

        public event EvaluateParameterHandler EvaluateParameter;

        private void OnEvaluateParameter(string name, ParameterArgs args)
        {
            if (EvaluateParameter != null)
                EvaluateParameter(name, args);
        }

        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Evaluates the this parameter.
        /// A hacked external method to allow custom functions to create new expressions which have access to the parent expressions evaluation event
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public object EvaluateThisParameter(string name) {
            ParameterArgs args = new ParameterArgs(this.options);

            // Calls external implementation
            OnEvaluateParameter(name, args);

            return args.Result;
        }

    }
}
