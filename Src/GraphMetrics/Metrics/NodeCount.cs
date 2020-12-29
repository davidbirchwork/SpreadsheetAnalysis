using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Graph;

namespace GraphMetrics.Metrics {
    [Metric("Node Count","Counts number of nodes")]
    public class NodeCount : IMetric {

        private int _nodeCount;

        #region Implementation of IMetric

        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics, IEnumerable<ExcelVertex> vertices, IEnumerable<AEdge> edges, List<string> colours, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            if (vertices != null) {
                this._nodeCount = vertices.Count();
                return null;
            }
            return "no vertices";
        }

        public string Print() {
            return "Nodes in Graph: " + this._nodeCount;
        }

        public List<string> PreRequisiteMetrics() {
            return  new List<string>();
        }

        #endregion
    }
}