using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ExcelInterop.Domain;
using Graph;
using GraphMetrics;

namespace ExcelFormulaMetrics.Metrics {
    [Metric("Table Finder", "finds square tables")]
    public class TableFinder : IMetric {
        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics, IEnumerable<ExcelVertex> vertices, IEnumerable<AEdge> edges, List<string> colours,
            Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {

            var bySheet = vertices.GroupBy(v => v.Sheet);

            foreach (var sheet in bySheet) {
                //todo this is actually just another graph partitioner but with a different linking rule
            }

            return null;

        }

        public string Print() {
            return "todo";
        }

        public List<string> PreRequisiteMetrics() {
            return new List<string>();
        }
    }
}