using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Graph;

namespace GraphMetrics {
    [InheritedExport(typeof(IMetric))]
    public interface IMetric {

        /// <summary>
        /// Computes the metric.
        /// </summary>
        /// <param name="computedMetrics"></param>
        /// <param name="vertices">The vertices.</param>
        /// <param name="edges">The edges.</param>
        /// <param name="colours"></param>
        /// <param name="vertexdict">The vertexdict -dict ( vertex , list uses, list used in)</param>
        /// <returns>Error message or null for success</returns>
        string Compute(ConcurrentDictionary<string, IMetric> computedMetrics, IEnumerable<ExcelVertex> vertices, IEnumerable<AEdge> edges, List<string> colours, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict);
        string Print();

        /// <summary>
        /// lists the Prerequisite metrics.
        /// </summary>
        /// <returns></returns>
        List<string> PreRequisiteMetrics();
    }
}