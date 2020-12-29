using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExcelExtractor.Domain;

namespace ExcelExtractor.Analyses.Graph {
    public static class ExtractWholeGraph {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Save(FunctionExtractor extractor, string fileName) {
            
            if (extractor.Evaluations == null) {
                extractor.EvaluateAll();
            }

            Dictionary<string, ExtractedCell> nodes = extractor.Evaluations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            List<Tuple<ExtractedCell, ExtractedCell>> arcs = new List<Tuple<ExtractedCell, ExtractedCell>>();

            foreach (var cell in nodes) {
                foreach (var reference in cell.Value.References) {
                    arcs.Add(new Tuple<ExtractedCell, ExtractedCell>(cell.Value, nodes[reference.Cell.ToString()]));
                }
            }

            var doc = GraphMlUtilities.SaveGraph(nodes, arcs,
                ExtractedCell.GetDataDictionary(),
                new Dictionary<string, string> { { "weight", "double" } },
                n => n.Key,
                e => e.Item1 + "|" + e.Item2,
                e => e.Item1.ToString(),
                e => e.Item2.ToString(),
                n => n.Value.GetData(),
                e => new Dictionary<string, string>()
            );
            doc.Save(fileName);
        }
    }
}
