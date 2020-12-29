using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Graph;

namespace GraphMetrics.Metrics {

    [Metric("ChainLength", "Counts number of nodes in the longest chain")]
    public class ChainLength : IMetric {

        private ConcurrentDictionary<ExcelVertex,int> chainLength = new ConcurrentDictionary<ExcelVertex, int>();

        #region Implementation of IMetric

        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics, IEnumerable<ExcelVertex> vertices, IEnumerable<AEdge> edges, List<string> colours, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            if (vertices != null) {
                // this algorithm is easy if we start from the root but at the moment we dont know the root...
            }
            return "no vertices";
        }

        public string Print() {
            return "not implemented";
        }

        public List<string> PreRequisiteMetrics() {
            return new List<string>();
        }

        #endregion
    }
}
