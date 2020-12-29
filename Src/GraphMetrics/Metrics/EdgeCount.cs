using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Graph;

namespace GraphMetrics.Metrics {
    [Metric("Edge Count", "Counts number of Edges")]
    public class EdgeCount : IMetric {

        private int _edgeCount;

        #region Implementation of IMetric

        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics, IEnumerable<ExcelVertex> vertices, IEnumerable<AEdge> edges, List<string> colours, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            if (edges != null) {
                this._edgeCount = edges.Count();
                return null;
            }
            return "no edges";
        }

        public string Print() {
            return "Edges in Graph: " + this._edgeCount;
        }

        public List<string> PreRequisiteMetrics() {
            return new List<string>();
        }

        #endregion
    }
}