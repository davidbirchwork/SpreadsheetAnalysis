using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Graph;

namespace GraphMetrics.Metrics {
    [Metric("Colouring Metrics", "Counts number of colours and the number of cells they contain")]
    public class ColouringMetrics : IMetric {

        private readonly SortedDictionary<string, double> _sheetNodeCount = new SortedDictionary<string, double>();
        private readonly SortedDictionary<string, int> _sheetEndVariableCount = new SortedDictionary<string, int>();      

        #region Implementation of IMetric

        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics, IEnumerable<ExcelVertex> vertices, IEnumerable<AEdge> edges, List<string> colours, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {

            if (vertices == null || vertexdict == null) {
                return "no vertices";
            }

            #region SheetNodeCount

            foreach (ExcelVertex aVertex in vertices) {
                string sheet = aVertex.GetColourID();
                if (!_sheetNodeCount.ContainsKey(sheet)) {
                    this._sheetNodeCount.Add(sheet, 0);
                }
                this._sheetNodeCount[sheet] = this._sheetNodeCount[sheet] + 1;
            }

            #endregion

            #region SheetEndVariableCount

            foreach (KeyValuePair<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertex in vertexdict) {
                var uses = vertex.Value.Item2;
                //var usedin = vertex.Value.Item2;
                if (uses.Count == 0) {
                    // its an end value
                    string sheet = vertex.Key.GetColourID();
                    if (!this._sheetEndVariableCount.ContainsKey(sheet)) {
                        this._sheetEndVariableCount.Add(sheet, 0);
                    }
                    this._sheetEndVariableCount[sheet] = this._sheetEndVariableCount[sheet] + 1;
                }
            }

            #endregion

            return null;
        }

        public string Print() {
            string text = this._sheetNodeCount.Aggregate("", (current, sheetnode) => current + ("Node Count (" + sheetnode.Key + "): " + sheetnode.Value + "\n"));
            return this._sheetEndVariableCount.Aggregate(text, (current, sheetnode) => current + ("End Value Count (" + sheetnode.Key + "): " + sheetnode.Value + "\n"));
        }

        public List<string> PreRequisiteMetrics() {
            return new List<string>();
        }

        #endregion
    }
}