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
    [Metric("Popular Functions","Lists all functions used")]
    public class PopularFunctions : IMetric {

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ConcurrentDictionary<string,int> FunctionFrequancy = new ConcurrentDictionary<string, int>();
        private int _nodeCount;
        private const int LargestN =15;

        #region Implementation of IMetric

        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics,
                              IEnumerable<ExcelVertex> vertices,
                              IEnumerable<AEdge> edges,
                              List<string> colours,
                              Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            if (vertices == null) {
                return "no vertices";
            }

            var vertexesWithFormulas = vertices.Where(v => v.isFormula).ToList();
            if (!vertexesWithFormulas.Any())
            {
                return "no meta :-/ ";
            }


            this._nodeCount = 0;

            // create a visitor and pass it all of the expressions... 
            FunctionFinderVisitor visitor = new FunctionFinderVisitor(LogFunction);

            foreach (var vertex in vertexesWithFormulas) {
                if (!string.IsNullOrEmpty(vertex.Formula)) {
                    Expression expr = new Expression(vertex.Formula);
                    if (expr.HasErrors()) {
                        // this forces parsing
                        if (vertex.Formula != "-") {
                            Log.Error("Could not parse formula " + vertex.Formula + " on vertex " + vertex.ID);
                        }
                    } else {
                        visitor.Reset();
                        expr.ParsedExpression.Accept(visitor);
                        _nodeCount++;
                    }
                }
            }            
            
            return null;
        }

        private void LogFunction(string functionid) {
            this.FunctionFrequancy.AddOrUpdate(functionid, id => 1, (id, count) => count + 1);            
        }

        public string Print() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format(Environment.NewLine+"Total Unique Functions Found: {0}", this.FunctionFrequancy.Count));
            sb.Append(this.FunctionFrequancy.Select(kpv => new {functionid = kpv.Key, count = kpv.Value}).OrderBy(kpv =>-kpv.count)
                .Aggregate("Most Used Functions: ",
                                             (acc, kpv) => acc + Environment.NewLine + kpv.count + "-" + kpv.functionid));

            return sb.ToString();
        }

        public List<string> PreRequisiteMetrics() {
            return new List<string>();
        }

        #endregion
    }

    public class FunctionFinderVisitor : LogicalExpressionVisitor {
        private const EvaluateOptions Options = EvaluateOptions.None;
        private readonly Action<string> _onfindFunction;

        public FunctionFinderVisitor(Action<string> foundFunction) {
            this._onfindFunction = foundFunction;
        }

        #region Overrides of LogicalExpressionVisitor

        public override void Visit(LogicalExpression expression) {
            throw new Exception("The method or operation is not implemented."); //copied from Evaluator!
        }

        public override void Visit(TernaryExpression expression) {
            expression.LeftExpression.Accept(this);
            expression.MiddleExpression.Accept(this);
            expression.RightExpression.Accept(this);
        }

        public override void Visit(BinaryExpression expression) {            
            expression.LeftExpression.Accept(this);
            expression.RightExpression.Accept(this);
        }

        public override void Visit(UnaryExpression expression) {            
            // Recursively evaluates the underlying expression
            expression.Expression.Accept(this);
        }

        public override void Visit(ValueExpression expression) {
            // do nout
        }

        public override void Visit(Function function) {

            // FOUND A Function!
            _onfindFunction(function.Identifier.ToString());

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

        #endregion

        public void Reset() {
        }
    }
}