using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using ExcelFunctions.ExcelFunctions.Lookups;
using NCalcExcel.Domain;
using NCalcExcel.Functions;

namespace ExcelFunctions.ExcelFunctions.Range {
    /// <summary>
    /// sum product function from excel:
    /// https://support.office.com/en-gb/article/SUMPRODUCT-function-16753e75-9f68-4874-94ac-4d2145a2fd2e
    /// </summary>
    [Export(typeof(AFunction))]
    public class SumProduct : AFunction {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SumProduct()
            : base("SumProduct",
                   "SumProduct(rangeA,rangeB)",
                   "computes SumProduct") {
            this.AddParamterDescription("rangeA", "The first range to compute SumProduct");
            this.AddParamterDescription("rangeB", "The Second range to compute SumProduct");            
        }

        public override object Evaluate(EvaluationVisitor evaluator, Function function) {

            if (function.Expressions.Length != 2 )
                throw new ArgumentException("SumProduct() currently takes exactly 2 arguments, taking further is an extension"); // todo we need to expand to support further arrays 


            List<decimal> firstArray  = EvalArray(evaluator, function, function.Expressions[0]);
            List<decimal> secondArray = EvalArray(evaluator, function, function.Expressions[1]);

            if (firstArray.Count != secondArray.Count) {
                Log.Error("SumProduct Arrays are not of equal size");
                return 0;
            }

            var sum = firstArray.Zip(secondArray, (a, b) => a*b).Sum();

            return sum;

    
        }

        private List<decimal>  EvalArray(EvaluationVisitor evaluator, Function function, LogicalExpression functionExpression) {
            String result = evaluator.Evaluate(functionExpression).ToString();
            List<decimal> firstArray = new List<decimal>();
            if (result == "1") {
                // we are in dummy exec mode...
                result =
                    this.DoFakeExpansion(this.CleanString(functionExpression.ToString()))
                        .Aggregate("{|{", (acc, next) => acc + "," + next) + "}|}";
                firstArray.Add(1);
            }
            // expand range
            if (result.Contains("{|{")) {
                // we found a wrapped range...  so unwrap it and walk through its cells
                string range = result.Replace("{|{", "").Replace("}|}", "");
                foreach (var parameter in range.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)) {
                    object subresult = this.CreateAndEvalExpression(evaluator, parameter);
                    decimal val = Sum.ConvertToDecimal(subresult.ToString());
                    firstArray.Add(val);
                }
            }

            return firstArray;
        }
    }
}
