using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Graph;

namespace GraphMetrics.Metrics {
    [Metric("Node Valencies","Finds top referred to cells")]
    public class NodeValencies : IMetric {

        private Dictionary<string, int> _valiencies = new Dictionary<string, int>();
        private Dictionary<string, int> _endValiencies = new Dictionary<string, int>();
        private readonly Dictionary<string, double> _sheetValencies = new Dictionary<string, double>();
        private readonly Dictionary<string,int> _sheetNodeCount = new Dictionary<string, int>();

        #region Implementation of IMetric

        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics, IEnumerable<ExcelVertex> vertices, IEnumerable<AEdge> edges, List<string> colours, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            #region Valiencies

            foreach (KeyValuePair<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertex in vertexdict) {
                string vertexname = vertex.Key.ID;
                int valiency = vertex.Value.Item1.Count + vertex.Value.Item2.Count;
                this._valiencies.Add(vertexname, valiency);

                string colour = vertex.Key.GetColourID();
                if (!this._sheetNodeCount.ContainsKey(colour)) {
                    this._sheetNodeCount.Add(colour,0);
                }
                this._sheetNodeCount[colour] = this._sheetNodeCount[colour] + 1;
            }
            // now sort them
            {
                var list = this._valiencies.ToList();
                list.Sort((first, second) => first.Value.CompareTo(second.Value) * -1);
                this._valiencies = list.ToDictionary(kp => kp.Key, kp => kp.Value);
            }

            #endregion

            #region end node valiencies

            foreach (
                KeyValuePair<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertex in
                    vertexdict.Where(n => n.Value.Item2.Count == 0)) {
                string vertexname = vertex.Key.ID;
                int valiency = vertex.Value.Item1.Count + vertex.Value.Item2.Count;
                this._endValiencies.Add(vertexname, valiency);
            }
            // now sort them
            {
                var list = this._endValiencies.ToList();
                list.Sort((first, second) => first.Value.CompareTo(second.Value) * -1);
                this._endValiencies = list.ToDictionary(kp => kp.Key, kp => kp.Value);
            }

            #endregion

            #region average valiencies

            foreach (var vertex in vertexdict) {
                string sheet = vertex.Key.GetColourID();
                if (!_sheetValencies.ContainsKey(sheet)) {
                    _sheetValencies.Add(sheet, 0);
                }
                _sheetValencies[sheet] = _sheetValencies[sheet] + vertex.Value.Item1.Count + vertex.Value.Item2.Count;
            }

            foreach (string sheet in this._sheetValencies.Keys.ToArray()) {
                this._sheetValencies[sheet] = this._sheetValencies[sheet] / this._sheetNodeCount[sheet];
            }

            #endregion

            return null;
        }

        public string Print() {
            string result = "";            

            result+= "Top 15 most referred to cells \n";
            result = this._valiencies.Take(15).Aggregate(result, (current, node) => current + ("Valency Count " + node.Key + " : " + node.Value + "\n"));            

            result+= "Top 15 most referred to end values \n";
            result = this._endValiencies.Take(15).Aggregate(result, (current, node) => current + ("End Node Valency Count " + node.Key + " : " + node.Value + "\n"));

            result += "Sheet Average Valencies \n";
            return this._sheetValencies.Aggregate(result, (current, sheetnode) => current + ("Sheet Average Valency (" + sheetnode.Key + ") : " + string.Format("{0:00.00} \n", sheetnode.Value)));
        }

        public List<string> PreRequisiteMetrics() {
            return new List<string>();
        }

        #endregion
    }
}