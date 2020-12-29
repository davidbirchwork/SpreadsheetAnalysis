using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExcelExtractor.Domain;
using NCalcExcel;

namespace ExcelExtractor.Analyses.Graph {
    public static class ExtractEvalGraph {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void SaveGraph(this FunctionExtractor extractor, string fileName, bool sheetPrefix,
            ExpressionFactory factory) {
            List<Tuple<ExtractedCell, ExtractedCell>> arcs = new List<Tuple<ExtractedCell, ExtractedCell>>();
            Dictionary<string, ExtractedCell> nodes = new Dictionary<string, ExtractedCell>();


            ExtractedCell rootcell =
                extractor.EvaluationGraph.Keys.FirstOrDefault(cell =>
                    cell.ToString().Equals(extractor.RootCell));
            if (rootcell == null) {
                Log.Error("could not find root evaluation cell " + extractor.RootCell);
            }

            ChaseEvaluatedGraph(extractor.EvaluationGraph, nodes, arcs, rootcell);
            IDictionary<string, string> friendlyNames = new Dictionary<string, string>();
            foreach (var key in extractor.EvaluationGraph.Keys.Where(key => key.ExcelNames.Count > 0)) {
                friendlyNames.Add(key.ToString(), key.ExcelNames.First());
            }


            //   GraphMlUtilities.SaveGraph(nodes, arcs, friendlyNames, sheetPrefix, fileName);
            var doc = GraphMlUtilities.SaveGraph(nodes, arcs,
                ExtractedCell.GetDataDictionary(),
                new Dictionary<string, string> {{"weight", "double"}},
                n => n.Key,
                e => e.Item1 + "|" + e.Item2,
                e => e.Item1.ToString(),
                e => e.Item2.ToString(),
                n => n.Value.GetData(),
                e => new Dictionary<string, string>()
            );
            doc.Save(fileName);
        }

        private static void ChaseEvaluatedGraph(
            ConcurrentDictionary<ExtractedCell, List<ExtractedCell>> extractionGraph,
            Dictionary<string, ExtractedCell> nodes,
            List<Tuple<ExtractedCell, ExtractedCell>> arcs,
            ExtractedCell cell,
            bool removetransitivewholesetreferences = true) {

            string cellname = cell.ToString();
            if (nodes.ContainsKey(cellname)) {
                return;
            }

            nodes.Add(cellname, cell);//.GenerateFormula(parameters, 0, exprfactory)

            if (removetransitivewholesetreferences && cell.IsRange && !cell.RangeIsFullyEvaluated) {
                Log.Info("not expanding range" + cell);
                return; // dont expand ranges which are not entirely referenced...
            }

            if (cell.IsRange) {
                // its vaguely possible that ranges when used do not end up with the cells they reference in the Evaluated list...
                // so lets take the actual children
                if (extractionGraph[cell].Count == 0) {
                    foreach (var extractedCell in cell.References.Select(r => r.Cell)) {
                        arcs.Add(new Tuple<ExtractedCell, ExtractedCell>(cell, extractedCell));
                        ChaseEvaluatedGraph(extractionGraph, nodes, arcs, extractedCell);
                    }
                } else {
                    Log.Info(cellname + " has " + extractionGraph[cell].Count + " references");
                }
            } else {
                foreach (var extractedCell in extractionGraph[cell]) {
                    arcs.Add(new Tuple<ExtractedCell, ExtractedCell>(cell, extractedCell));
                    ChaseEvaluatedGraph(extractionGraph, nodes, arcs, extractedCell);
                }
            }
        }

    }
}
