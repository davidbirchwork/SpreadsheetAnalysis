using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Text;
using NCalcExcel.Functions;
using System.Linq;

namespace NCalcExcel {
    public class ExpressionFactory {

        public EvaluateOptions Options = EvaluateOptions.None;

        [ImportMany(AllowRecomposition = true)]
        public AFunction[] Functions { get; set; }

        public ConcurrentDictionary<string, AFunction> FunctionDictionary = new ConcurrentDictionary<string, AFunction>();

        #region ctors

        public ExpressionFactory() : this(new AggregateCatalog()){            
        }

        public ExpressionFactory(AggregateCatalog catalog) {
            catalog.Catalogs.Add(new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly()));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            foreach (AFunction function in Functions) {
                FunctionDictionary.GetOrAdd(function.Name.ToLower(), function);
            }

            Expression.FunctionsMap = this.FunctionDictionary;
        }

        #endregion

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="onparameter">The onparameter.</param>
        /// <returns>evaluated result</returns>
        public object Evaluate(string expression, Dictionary<string, object> parameters, EvaluateParameterHandler onparameter = null) {
             Expression expr = new Expression(expression,this.Options) {Parameters = parameters};
             if (onparameter != null) {
                 expr.EvaluateParameter += onparameter;
             }
            return expr.Evaluate();
        }

        /// <summary>
        /// Evaluates the specified expression with specific evaluation options
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">evaluation options to use</param>
        /// <param name="onparameter">The onparameter.</param>
        /// <returns>evaluated result</returns>
        public object Evaluate(string expression, Dictionary<string, object> parameters, EvaluateOptions options, EvaluateParameterHandler onparameter = null) {
            Expression expr = new Expression(expression, options) { Parameters = parameters };
            if (onparameter != null) {
                expr.EvaluateParameter += onparameter;
            }
            return expr.Evaluate();
        }

        #region Help

        public IEnumerable<string> FunctionNames() {
            return from func in this.FunctionDictionary.Values select func.Name;   
        }

        public string GetHelpOnUsage(string functionname) {
            string funcname = functionname.Substring(0, functionname.IndexOf("("));
            if (!this.FunctionDictionary.ContainsKey(funcname)) {
                return "Unknown Function '" + funcname + "'";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.FunctionDictionary[funcname].Description);
            foreach (KeyValuePair<string, string> param in this.FunctionDictionary[funcname].ParameterHelp) {
                sb.AppendLine("Param " + param.Key + " : " + param.Value);
            }
            return sb.ToString();
        }

        public string PrintLanguage() {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Operators:");
            sb.AppendLine(" Logical:");
            sb.AppendLine("    or, ||");
            sb.AppendLine("    and, &&");
            sb.AppendLine("    not, !");
            sb.AppendLine(" Relational:");
            sb.AppendLine("    =, ==");
            sb.AppendLine("    !=, <>");
            sb.AppendLine("    <, <=, >, >= ");
            sb.AppendLine(" Arithmetic:");
            sb.AppendLine("    +, - *, /");
            sb.AppendLine("    mod: %");
            sb.AppendLine("    pow: ^");
            sb.AppendLine(" Brackets ( )");
            sb.AppendLine();

            sb.AppendLine("List of Language Supported Functions:");
            sb.AppendLine();
            foreach (AFunction func in this.FunctionDictionary.Values.OrderBy( f=> f.Name)) {
                sb.AppendLine(func.Usage + " : " + func.Description);
            }

            return sb.ToString();
        }
        #endregion

        #region Other Routines

        /// <summary>
        /// Extract the variables from an expression.
        /// Search is case insensitive!
        /// </summary>
        /// <param name="expression">expression to extract from</param>        
        /// <returns>list of variables found</returns>
        public List<string> ExtractVars(string expression) {
            //optimisation - probably lots within this method - eg get list first then sort it out 
            List<string> foundVariables = new List<string>();
           // Random random = new Random();
            try {
                Expression expr = new Expression(expression, EvaluateOptions.IgnoreCase | EvaluateOptions.DebugMode);
                expr.EvaluateParameter += delegate(string name, ParameterArgs args) {
                    if (foundVariables.FindAll(v => string.Compare(v, name, StringComparison.OrdinalIgnoreCase) == 0).Count == 0) {
                        foundVariables.Add(name);
                    }
                    //add a dummy result so expression keeps parsing ;) 
                    args.Result = 1;
                    args.HasResult = true;
                };
                expr.Evaluate();
            } catch (DivideByZeroException) {
                // ignore this...
            } catch (Exception e) {
                // log an invalid expr? - but what about division by zero ??                    
                throw e;
            }
          
            return foundVariables;
        }

        /// <summary>
        /// Tries to parse the expression
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>true if no errors</returns>
        public bool TryParse(string expression) {
            try {
                Expression expr = new Expression(expression, this.Options);
                return !expr.HasErrors();
            } catch (Exception) {
                return false;                
            }
        }

        #endregion

        /// <summary>
        /// Tries to parse the expression
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>error message or null if no errors</returns>
        public string TryParseWithErrors(string expression) {
            try {
                Expression expr = new Expression(expression, this.Options);
                return !expr.HasErrors() ? null : "Error in Expression: " + expr.Error;
            } catch (Exception e) {
                return "Error in Expression: " + e.Message;
            }
        }
    }
}
