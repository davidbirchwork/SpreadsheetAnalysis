using System;
using System.Collections.Generic;
using ExcelExtractor.Domain;

namespace ExcelExtractor.Analyses.Graph {
    /// <summary>
    /// holds a calculation sub graph of extracted Cells
    /// </summary>
    public class ExcelGraph {
        public int Pid { get; }
        public int Size { get; }
        public Dictionary<string, ExtractedCell> Nodes { get; }
        public List<Tuple<ExtractedCell, ExtractedCell>> Arcs { get; }

        public ExcelGraph(int pid, int size, Dictionary<string, ExtractedCell> nodes,
            List<Tuple<ExtractedCell, ExtractedCell>> arcs) {
            this.Pid = pid;
            this.Size = size;
            this.Nodes = nodes;
            this.Arcs = arcs;
        }
    }
}