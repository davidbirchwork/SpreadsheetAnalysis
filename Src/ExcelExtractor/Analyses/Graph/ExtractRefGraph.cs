using System;
using System.Collections.Generic;
using System.Linq;
using ExcelExtractor.Domain;
using NCalcExcel;

namespace ExcelExtractor.Analyses.Graph {
    public class ExtractRefGraph {//todo make static
        public void ExtractGraph(string friendlynamesfile, bool includeBlanksChecked, bool usesheetPrefix,
            string fileName, bool useKnownNames, FunctionExtractor extractor,ExpressionFactory factory) { 
            Dictionary<string, string> friendlyNames = new Dictionary<string, string>();
            if (friendlynamesfile != null) {
                CellMap[] mappings = CellMap.ReadTSV(friendlynamesfile);
                if (mappings != null) {
                    List<CellMap> distinct = new List<CellMap>();
                    foreach (CellMap cellMap in mappings) {
                        string name = cellMap.Sheet + "!" + cellMap.Cell;
                        if (!distinct.Any(map => {
                            string othername = map.Sheet + "!" + map.Cell;
                            return name == othername;
                        }
                        )) {
                            distinct.Add(cellMap);
                        }
                    }

                    friendlyNames = distinct.ToDictionary(mapping => (mapping.Sheet + "!" + mapping.Cell),
                        mapping => mapping.FriendlyName);
                }
            }

            if (useKnownNames) {
                foreach (var cell in extractor.ProcessedCells) {
                    foreach (var friendlyName in cell.Value.ExcelNames) {
                        friendlyNames.Add(cell.ToString(), friendlyName);
                    }
                }
            }

            // get the data
            _arcs = new List<Tuple<string, string>>();
            _nodes = new Dictionary<string, string>();
            ExtractNodesandEdges(extractor.ProcessedCells.Values.AsEnumerable(),
                new Dictionary<string, Tuple<ExtractedCell, object>>());
            // create a .gml and save it

            if (!includeBlanksChecked) {
                RemoveFromGraph(
                    extractor.ProcessedCells.Values.Where(cell => cell.IsBlank)
                        .Select(cell => cell.ToString()).ToList());
            }


            GraphMlUtilities_Old.SaveGraph(this._nodes, this._arcs, friendlyNames, usesheetPrefix, fileName);
        }

        private void RemoveFromGraph(IEnumerable<string> nodesToRemove) {
            this._nodes = this._nodes.Where(kpv => !nodesToRemove.Contains(kpv.Key)).ToDictionary(kpv => kpv.Key,
                                                                                                  kpv => kpv.Value);
            // ReSharper disable PossibleMultipleEnumeration
            this._arcs = this._arcs.Where(arc => !nodesToRemove.Contains(arc.Item1) && !nodesToRemove.Contains(arc.Item2)).ToList();
            // ReSharper enable PossibleMultipleEnumeration
        }

        private List<Tuple<string, string>> _arcs;
        private Dictionary<string, string> _nodes; // nodename vs meta

        private void ExtractNodesandEdges(IEnumerable<ExtractedCell> cells, Dictionary<string, Tuple<ExtractedCell, object>> parameters) {

            foreach (var cell in cells) {
                _nodes.Add(cell.ToString(), cell.GenerateFormula(parameters));
                foreach (var reference in cell.References) {
                    _arcs.Add(new Tuple<string, string>(cell.ToString(), reference.Cell.ToString()));
                }
            }
        }
    }
}
