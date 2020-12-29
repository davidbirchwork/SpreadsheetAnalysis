using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Graph;

namespace GraphMetrics.Metrics {    
    [Metric("Average Valency", "Average Valency of all nodes")]
    public class AverageValencies : IMetric {

        private double _averageValency;
        private double _averageIncomingValency;
        private double _averageOutGoingValency;

        #region Implementation of IMetric

        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics, IEnumerable<ExcelVertex> vertices, IEnumerable<AEdge> edges, List<string> colours, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            if (vertices == null || vertexdict == null) {
                return "no vertices";
            }
            foreach (var vertex in vertexdict) {
                _averageValency += vertex.Value.Item1.Count + vertex.Value.Item2.Count;
                _averageIncomingValency += vertex.Value.Item1.Count;
                _averageOutGoingValency += vertex.Value.Item2.Count;
            }
            int vertexCount = vertices.Count();
            _averageValency = _averageValency / vertexCount;
            _averageIncomingValency = _averageIncomingValency / vertexCount;
            _averageOutGoingValency = _averageOutGoingValency / vertexCount;
            return null;
        }

        public string Print() {
            return string.Format("Average Valency: {0:##.00} \n", this._averageValency) + "\n" +
                   string.Format("Average Incoming Valency: {0:##.00} \n", this._averageIncomingValency) + "\n" +
                   string.Format("Average Outgoing Valency: {0:##.00} \n", this._averageOutGoingValency) + "\n";

        }

        public List<string> PreRequisiteMetrics() {
            return new List<string>();
        }

        #endregion
    }
}