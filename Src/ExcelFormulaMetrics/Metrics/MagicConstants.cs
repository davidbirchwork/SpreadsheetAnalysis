using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Algorithms;
using Graph;
using GraphMetrics;
using NCalcExcel;
using NCalcExcel.Domain;

namespace ExcelFormulaMetrics.Metrics {
    [Metric("Magic Constant Finder","Finds Magic Constants in formulas")]
    public class MagicConstants : IMetric {

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<string> NewConstantsFound = new List<string>();
        private readonly List<string> _allConstantsFound = new List<string>();
        private readonly Dictionary<ExcelVertex, List<string>> _vertexConstants = new Dictionary<ExcelVertex, List<string>>();
        private Dictionary<ExcelVertex, int> _vertexConstantCount = new Dictionary<ExcelVertex, int>();
        private double _average;
        private Dictionary<string, int> _constantFrequancices;
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

            this._nodeCount = vertexesWithFormulas.Count();

            ConstantFinderVisitor visitor = new ConstantFinderVisitor(LogConstant);

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
                        this.NewConstantsFound = new List<string>();
                        expr.ParsedExpression.Accept(visitor);
                        if (this.NewConstantsFound.Count > 0) {
                            this._vertexConstants.Add(vertex, NewConstantsFound);
                            this._vertexConstantCount.Add(vertex, NewConstantsFound.Count);
                        }
                    }
                }
            }
            // compute some stats:
            this._constantFrequancices = this._allConstantsFound.MakeFrequencyTable(sortAscending:false,takeN:LargestN);
            this._average = this._vertexConstantCount.Values.Any() ? this._vertexConstantCount.Values.Average() : double.NaN;
            this._vertexConstantCount = (from entry in this._vertexConstantCount orderby entry.Value descending select entry).Take(LargestN).ToDictionary(pair => pair.Key, pair => pair.Value);

            return null;
        }

        private void LogConstant(string constant) {
            this.NewConstantsFound.Add(constant);
            this._allConstantsFound.Add(constant);
        }

        public string Print() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Total Constants Found: {this._allConstantsFound.Count}");            
            sb.AppendLine($"Average Constants on nodes with constants: {this._average}");
            double percent = this._vertexConstants.Count / ((double) this._nodeCount);
            sb.AppendLine($"Percent Nodes with Constants: {percent * 100:00.000}%");
            sb.AppendLine(
                $"Maximum Constants Found: {this._vertexConstantCount.First().Value} ({this._vertexConstantCount.First().Key.ID})");
            sb.AppendLine("Most common Constants ");
            foreach (var constant in this._constantFrequancices) {
                sb.AppendLine($"{constant.Key} occurred {constant.Value} times");
            }
            sb.AppendLine("Formulas with Most Constants ");
            foreach (var constant in this._vertexConstantCount) {
                sb.AppendLine($"{constant.Key.ID} has {constant.Value} constants in formula {constant.Key.Formula}");
            }

            return sb.ToString();
        }

        public List<string> PreRequisiteMetrics() {
            return new List<string>();
        }

        #endregion
    }

    public class ConstantFinderVisitor : LogicalExpressionVisitor {
        private const EvaluateOptions Options = EvaluateOptions.None;
        private readonly Action<string> _onfindConsant;
        private bool _atRootLevel = true;

        public ConstantFinderVisitor(Action<string> foundConstant) {
            this._onfindConsant = foundConstant;
        }

        #region Overrides of LogicalExpressionVisitor

        public override void Visit(LogicalExpression expression) {
            throw new Exception("The method or operation is not implemented."); //copied from Evaluator!
        }

        public override void Visit(TernaryExpression expression) {
            this._atRootLevel = false;
            expression.LeftExpression.Accept(this);
            expression.MiddleExpression.Accept(this);
            expression.RightExpression.Accept(this);
        }

        public override void Visit(BinaryExpression expression) {
            this._atRootLevel = false;
            expression.LeftExpression.Accept(this);
            expression.RightExpression.Accept(this);
        }

        public override void Visit(UnaryExpression expression) {
            this._atRootLevel = false;
            // Recursively evaluates the underlying expression
            expression.Expression.Accept(this);
        }

        public override void Visit(ValueExpression expression) {
            if (!_atRootLevel) {
                this._onfindConsant(expression.ToString().Trim());
            }
            this._atRootLevel = false;
        }

        public override void Visit(Function function) {
            this._atRootLevel = false;
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
            this._atRootLevel = false;
            // do nothing
        }

        #endregion

        public void Reset() {
            this._atRootLevel = true;
        }
    }
}