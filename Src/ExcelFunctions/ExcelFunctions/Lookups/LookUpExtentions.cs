using System.Collections.Generic;
using System.Linq;
using ExcelInterop.Domain;
using NCalcExcel;
using NCalcExcel.Domain;
using NCalcExcel.Functions;

namespace ExcelFunctions.ExcelFunctions.Lookups {

    public enum MatchValueType {
        Boolean,
        Numeric,
        String
    }

    public static class LookUpExtentions {

        public static string[] DoFakeExpansion( this AFunction func,   string range) {
            const string fakeSheet = "FAKESHEET!";
            bool addedRangeFakeSheet = !range.Contains("!");
            if (addedRangeFakeSheet) {
                range = fakeSheet + range;
            }
            ExcelAddress rangeAddress = new ExcelAddress(range);
            List<string> addresses = ExcelAddress.ExpandRangeToCellList(rangeAddress);
            if (addedRangeFakeSheet) {
                addresses = addresses.Select(addr => addr.Replace(fakeSheet, "")).ToList();
            }

            return addresses.ToArray();
        }

        /// <summary>
        /// Cleans the string from wrapping gumpf genorated by nclac
        /// </summary>
        /// <param name="func">function</param>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static string CleanString(this AFunction func, string str) {
            str = str.Trim();
            if (str.StartsWith("("))
                str = str.Substring(1);
            if (str.EndsWith(")"))
                str = str.Substring(0, str.Length - 1);
            if (str.StartsWith("["))
                str = str.Substring(1);
            if (str.EndsWith("]"))
                str = str.Substring(0, str.Length - 1);
            if (str.StartsWith("'"))
                str = str.Substring(1);
            if (str.EndsWith("'"))
                str = str.Substring(0, str.Length - 1);
            return str;
        }

        public static object CreateAndEvalExpression(this AFunction func, EvaluationVisitor evaluator, string expression, EvaluateOptions setoptions = EvaluateOptions.None) {
            var options = evaluator.options;
            if (!setoptions.HasFlag(EvaluateOptions.None)) {
                options = options | setoptions;
            }
            Expression expr = new Expression(expression, options);
            expr.EvaluateParameter += delegate(string name, ParameterArgs args) {
                args.Result = evaluator.EvaluateThisParameter(name);
            };
            return expr.Evaluate();
        }

    }
}
