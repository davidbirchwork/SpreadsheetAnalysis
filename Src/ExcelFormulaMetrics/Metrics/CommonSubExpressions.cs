using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Graph;
using GraphMetrics;
using NCalcExcel;
using NCalcExcel.Domain;

namespace ExcelFormulaMetrics.Metrics {
    [Metric("Common Sub Expressions","Finds Frequently used Common SubExpressions - only uses string matching :-/")]
    public class CommonSubExpressions : IMetric {

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string,List<ExcelVertex>> _subExpressions = new Dictionary<string, List<ExcelVertex>>();
        private ExcelVertex _currentVertex;
        private Dictionary<string, List<ExcelVertex>> _largestSubExpressions;
        private const int LargestN = 100;

        #region Implementation of IMetric

        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics,
                              IEnumerable<ExcelVertex> vertices,
                              IEnumerable<AEdge> edges, 
                              List<string> colours,
                              Dictionary<ExcelVertex,Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            if (vertices == null) {
                return "no vertices";
            }

            var vertexesWithFormulas = vertices.Where(v => v.isFormula).ToList();
            if (!vertexesWithFormulas.Any()) {
                return "no meta :-/ ";
            }

            SubExpressionFinder visitor = new SubExpressionFinder(LogConstant);

            foreach (var vertex in vertexesWithFormulas) {
                if (!string.IsNullOrEmpty(vertex.Formula)) {
                    Expression expr = new Expression(vertex.Formula);
                    if (expr.HasErrors()) {
                        // this forces parsing
                        if (vertex.Formula != "-") {
                            Log.Error("Could not parse formula " + vertex.Formula + " on vertex " + vertex.ID);
                        }
                    } else {
                        this._currentVertex = vertex;
                        expr.ParsedExpression.Accept(visitor);
                    }
                }
            }
            
            this._subExpressions = (from entry in this._subExpressions.Where(subexpr => subexpr.Value.Count>1) orderby entry.Value.Count descending select entry).Take(LargestN).ToDictionary(pair => pair.Key, pair => pair.Value);

            this._largestSubExpressions =  (from entry in this._subExpressions.Where(subexpr => subexpr.Value.Count > 1) orderby entry.Key.Length descending select entry).Take(LargestN).ToDictionary(pair => pair.Key, pair => pair.Value);

            return null;
        }

        private void LogConstant(string subexpr) {
            if (this._subExpressions.ContainsKey(subexpr)) {
                this._subExpressions[subexpr].Add(this._currentVertex);
            } else {
                this._subExpressions.Add(subexpr,new List<ExcelVertex> {this._currentVertex});
            }            
        }

        public string Print() {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("Most Common Sub Expressions: ");
            const int numExamples = 1;
            foreach (var subExpressions in this._subExpressions) {
                sb.AppendLine(
                    $"{subExpressions.Key} appeared in {subExpressions.Value.Count} formulas - {numExamples} example(s) are {subExpressions.Value.Take(numExamples).Aggregate("", (acc, next) => acc + " |>| " + next.ID + " ->- " + next.Formula)}");
            }

            sb.AppendLine("Longest Sub Expressions: ");

            foreach (var subExpressions in this._largestSubExpressions) {
                sb.AppendLine(
                    $"{subExpressions.Key} appeared in {subExpressions.Value.Count} formulas - {numExamples} example(s) are {subExpressions.Value.Take(numExamples).Aggregate("", (acc, next) => acc + " |>| " + next.ID + " ->- " + next.Formula)}");
            }

            return sb.ToString();
        }

        public List<string> PreRequisiteMetrics() {
            return  new List<string>();
        }

        #endregion
    }

    public class SubExpressionFinder : LogicalExpressionVisitor {
        private const EvaluateOptions Options = EvaluateOptions.None;
        private readonly Action<string> _onfindsubExpression;        

        public SubExpressionFinder(Action<string> subExpressionLogger) {
            this._onfindsubExpression = subExpressionLogger;
        }

        #region Overrides of LogicalExpressionVisitor

        public override void Visit(LogicalExpression expression) {
            throw new Exception("The method or operation is not implemented."); //copied from Evaluator!
        }

        public override void Visit(TernaryExpression expression) {
            _onfindsubExpression(expression.ToString());
            expression.LeftExpression.Accept(this);
            expression.MiddleExpression.Accept(this);
            expression.RightExpression.Accept(this);
        }

        public override void Visit(BinaryExpression expression) {
            _onfindsubExpression(expression.ToString());
            expression.LeftExpression.Accept(this);
            expression.RightExpression.Accept(this);
        }

        public override void Visit(UnaryExpression expression) {
            _onfindsubExpression(expression.ToString());
            // Recursively evaluates the underlying expression
            expression.Expression.Accept(this);
        }        

        public override void Visit(Function function) {
            _onfindsubExpression(function.ToString());
            #region set up arguments

            var args = new FunctionArgs {
                Parameters = new Expression[function.Expressions.Length]
            };

            for (int i = 0; i < function.Expressions.Length; i++) {
                args.Parameters[i] = new Expression(function.Expressions[i], Options);
            }
            #endregion

            foreach (var expression in args.Parameters) {
                expression.ParsedExpression.Accept(this);
            }
        }

        public override void Visit(Identifier function) {
            // do nothing
        }

        public override void Visit(ValueExpression expression) {
            // do nothing
        }

        #endregion

    }
}