using System;
using System.Collections.Generic;
using Graph;

namespace GraphViewer {
    internal static class Abstractor {

        /// <summary>
        /// Calculates the specified root.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="vertexdict">The vertexdict [incoming edges, outgoing edges].</param>
        /// <param name="vertices">The vertices.</param>
        public static List<ExcelVertex> Calculate(ExcelVertex root, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict, IEnumerable<ExcelVertex> vertices) {

            List<ExcelVertex> visitedNodes = new List<ExcelVertex> { root };

            foreach (ExcelVertex child in vertexdict[root].Item1) {
                if (!TraceChild(child, vertexdict, visitedNodes)) {
                    return null;
                }
            }
            return visitedNodes;
        }

        private static bool TraceChild(ExcelVertex child, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict, List<ExcelVertex> visited) {
            // chase all downward links
            if (visited.Contains(child)) {
                return true;
            }
            if (!visited.Contains(child)) {
                visited.Add(child);
            }

            foreach (ExcelVertex grandchild in vertexdict[child].Item1) {
                if (!TraceChild(grandchild, vertexdict, visited)) {
                    return false;
                }
            }

            // follow all upward links and hope they meet a vertex we've already visited
            foreach (ExcelVertex parent in vertexdict[child].Item2) {
                if (!TraceParent(parent, vertexdict, visited)) {
                    return false;
                }
            }

            return true;
        }

        private static bool TraceParent(ExcelVertex parent, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict, List<ExcelVertex> visited) {
            if (visited.Contains(parent)) {
                return true;
            }
            visited.Add(parent);

            foreach (ExcelVertex grandParent in vertexdict[parent].Item2) {
                if (!TraceChild(grandParent, vertexdict, visited)) {
                    return false;
                }
            }

            foreach (ExcelVertex child in vertexdict[parent].Item1) {
                if (!TraceChild(child, vertexdict, visited)) {
                    return false;
                }
            }

            return true;
        }
    }
}