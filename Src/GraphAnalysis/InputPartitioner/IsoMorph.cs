using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Algorithms;
using ExcelInterop.Domain;
using log4net;
using ParseTreeExtractor.Domain;

namespace GraphAnalysis.InputPartitioner {
    public static class IsoMorph {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public class NodeClass {
            public string NodeId { get; }
            public double EClass { get; }
            public int Children { get; }
            public string CellId { get; }

            public NodeClass(string nodeId, double eClass, int children, string cellId) {
                NodeId = nodeId;
                EClass = eClass;
                Children = children;
                CellId = cellId;
            }
        }

        public class SubGraphEqClasses {
            public string CellNodeId { get; }
            public List<Node> Nodes { get; }
            public List<NodeClass> EqClasses { get; }

            public SubGraphEqClasses(string cellNodeId, List<Node> nodes, List<NodeClass> eqClasses) {
                CellNodeId = cellNodeId;
                Nodes = nodes;
                EqClasses = eqClasses;
            }
        }

        /// <summary>
        /// Record a match between two nodes in the graph
        /// Record also their cells. 
        /// </summary>
        public class IsoMatch {
            public string INode { get; }
            public string JNode { get; }
            public string IAddress { get; }
            public string JAddress { get; }

            public IsoMatch(string iNode, string jNode, string iAddress, string jAddress) {
                INode = iNode;
                JNode = jNode;
                IAddress = iAddress;
                JAddress = jAddress;
            }
        }

        public static IsoMorphResults Partition(SpreadGraph graph, Extraction extraction,
            bool usePartitioning = false,bool subexpr = false) {
            string ClassPartitioner(string nodeId) {
                return graph.Nodes[nodeId].NodeType;
            }

            Log.Info("Starting Isomorph");

            // 0) Figure out equivalence classes
            Dictionary<int, ConcurrentBag<string>> nodesByType = PartitionAlgorithms.PartitionWithTest(graph.Nodes,
                i => i.Key,
                (dict, left, right) => ClassPartitioner(left.First()) == ClassPartitioner(right.First()));

            EquivalenceClassLabeller<string>
                equivalenceClasses = new EquivalenceClassLabeller<string>(ClassPartitioner);

            string typeTable = string.Join(Environment.NewLine,
                nodesByType.Select(n =>
                    "P=" + n.Key + "=" + ClassPartitioner(n.Value.First()) + " size =" + n.Value.Count + " Class = " +
                    equivalenceClasses.FindClass(n.Value.First())));
            Log.Info(typeTable);

            var results = new IsoMorphResults();

            // 1) partition graph
            if (usePartitioning) {
                Log.Info("Partitioning the graph");
                var partitions = PartitionAlgorithms.PartitionOnArcs(graph.Nodes,
                        graph.Edges.Select(e => new Tuple<Node, Node>(e.Source, e.Target)), n => n.Id)
                    .Where(p => p.Value.Count(v => graph.Nodes[v].NodeType == "Cell") > 1).ToList();
                Log.Info("Partitioned the graph into " + partitions.Count +
                         " pieces of more than one cell ready for isomorph");

                foreach (var partition in partitions) {
                    var partitionNodes = partition.Value;
                    Log.Debug("#Partition id " + partition.Key + " with " + partitionNodes.Count + " nodes");

                    RunIsoMorphs(graph, extraction, equivalenceClasses, results,
                        graph.Nodes.Values.Where(n => partitionNodes.Contains(n.Id) && n.NodeType == "Cell").ToList(),subexpr);
                }
            }
            else {
                RunIsoMorphs(graph, extraction, equivalenceClasses, results,
                    graph.Nodes.Values.Where(n => n.NodeType == "Cell").ToList(),subexpr);
            }

            return results;
        }

        private static void RunIsoMorphs(SpreadGraph graph, Extraction extraction,
            EquivalenceClassLabeller<string> equivalenceClasses, IsoMorphResults results, List<Node> cellNodes,
            bool subexpr) {
// 2) find cell sub graphs 
            var cells = cellNodes;

            Dictionary<string, List<Node>> cellAstNodes = cells.ToDictionary(cell => cell.Id,
                cell => GraphAlgorithms.WalkUntilTrue(cell,
                        c => c.LinksTo,
                        c => c.NodeType != "Cell",
                        c => true, includeRoot: true)
                    .ToList()
                // cut out input cells
            ).Where(c => c.Value.Any(n => n.NodeType == "Reference")).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (!cellAstNodes.Any()) return;
            Log.Info("Average size of Cell AST graph is " + cellAstNodes.Average(a => a.Value.Count));

            // 3) generate node labels for each cell
            List<SubGraphEqClasses> subGraphs = new List<SubGraphEqClasses>();
            foreach (var cellGraph in cellAstNodes) {
                Dictionary<string, NodeClass> nl = new Dictionary<string, NodeClass>();
                List<Node> nodes = cellGraph.Value;
                string root = cellGraph.Key;
                RecursivelySetNodeHashes(equivalenceClasses, nl, nodes, root, nodes.First(n => n.Id == root));

                subGraphs.Add(new SubGraphEqClasses(root, nodes,
                    nl.Values.Where(ec => ec.Children > 1) // do not match single nodes
                        .OrderByDescending(eq => eq.EClass)
                        .ToList()));
            }

            // 4) compare all to all to find maximal sub graph isomorphisms...

            List<IsoMatch> ms = new List<IsoMatch>();

            for (int i = 0; i < subGraphs.Count; i++) {
                for (int j = 0; j <= i; j++) {
                    var iGraph = subGraphs[i];
                    var jGraph = subGraphs[j];

                    // walk both lists and find matches... 

                    ms.AddRange(MatchSubGraphs(iGraph, jGraph));
                }
            }

            Log.Info("Found " + ms.Count + " matches (that is a cross product)");

            // 5) test matches

            // 6) consider maximal common matches over whole ast graph... 
            var globalMatches = subGraphs.SelectMany(sg => sg.EqClasses)
                .GroupBy(sg => sg.EClass)
                .Where(g => g.Count() > 1)
                .OrderByDescending(g => g.Count());

            //todo this is a sub expression matcher 

            Log.Info(globalMatches.Aggregate("Global Matches: ",
                (acc, next) => acc + Environment.NewLine + next.Key + " has " + next.Count() +
                               " matches - example is " + next.First().NodeId +
                               " a " + graph.Nodes[next.First().NodeId].NodeType +
                               " with a tree of " + next.First().Children + " nodes " +
                               "in cell " + next.First().CellId));

            // 7) find cell > cell matches
            var wholeCellMatches = subGraphs.SelectMany(sg => sg.EqClasses)
                .Where(a => a.NodeId == a.CellId) // only cells
                .GroupBy(sg => sg.EClass)
                .Where(g => g.Count() > 1)
                .OrderByDescending(g => g.Count()).ToList();

            Log.Info(wholeCellMatches.Aggregate("Whole Cell Matches: ",
                (acc, next) => acc + Environment.NewLine + next.Key + " has " + next.Count() +
                               " matches - example is " + next.First().NodeId +
                               " a " + graph.Nodes[next.First().NodeId].NodeType +
                               " node with a tree of " + next.First().Children + " nodes " +
                               "in cell " + next.First().CellId + " its formula is " +
                               extraction.CellFormulas[new ExcelAddress(next.First().CellId)]));

            //8) Vectorize cells via the greedy rectangles algorithm 

            List<Tuple<ExcelAddress, List<NodeClass>>> structurallyIsoCellVectors =
                new List<Tuple<ExcelAddress, List<NodeClass>>>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Vectors of structurally isomorphic cells:");
            foreach (var group in wholeCellMatches) {
                sb.AppendLine("Class " + @group.Key);
                var vectors = Vectorizer.Vectorize(@group.ToList(), nc => new ExcelAddress(nc.CellId));
                foreach (var vector in vectors) {
                    sb.AppendLine(vector.Item1 + " [" + vector.Item2.Count + "] = " +
                                  extraction.CellFormulas[vector.Item1.RangeTopLeftCell()]);
                }

                // filter out single cells 
                structurallyIsoCellVectors.AddRange(vectors.Where(v => v.Item2.Count > 1));
            }

            Log.Info(sb);

            //9) test reference isomorphic cells - based on structurally isomorphic ones

            var structureAndRelativeReferenceIsomorphic = new List<Tuple<ExcelAddress, List<string>>>();

            sb = new StringBuilder();
            sb.AppendLine("Structural and Reference Isomorphic:");

            foreach (var vector in structurallyIsoCellVectors) {
                // get the cell and the cells it references... 
                // calculate relative offset for each reference
                // ranges are tricky :-/ 
                var vectorCells = vector.Item2.Select(c => c.CellId);
                var splits = CellReferenceIsomorph.Test(vectorCells, graph);
                // note that we'd like to use this to partition - somehow! 
                foreach (var split in splits) {
                    foreach (var v in Vectorizer.Vectorize(split, id => new ExcelAddress(id)).Where(a => a.Item2.Count > 1)) {
                        structureAndRelativeReferenceIsomorphic.Add(v);
                        sb.AppendLine(v.Item1 + " [" + v.Item2.Count + "] = " +
                                      extraction.CellFormulas[v.Item1.RangeTopLeftCell()]);
                    }
                }
            }

            Log.Info(sb);

            //10) Numeric Isomorphism 

            sb = new StringBuilder();
            sb.AppendLine("Numeric, Structural and Reference Isomorphic:");
            var numericStructureAndRelativeReferenceIsomorphic = new List<Tuple<ExcelAddress, List<string>>>();

            foreach (var vector in structureAndRelativeReferenceIsomorphic) {
                // check numeric isomorphism 
                var splits = ConstantIsomorph.Test(vector.Item2, graph);

                foreach (var split in splits) {
                    foreach (var v in Vectorizer.Vectorize(split, id => new ExcelAddress(id))
                        .Where(a => a.Item2.Count > 1)) {
                        numericStructureAndRelativeReferenceIsomorphic.Add(v);
                        sb.AppendLine(v.Item1 + " [" + v.Item2.Count + "] = " +
                                      extraction.CellFormulas[v.Item1.RangeTopLeftCell()]);
                    }
                }
            }

            Log.Info(sb);

            //11) Referenced by Isomorphism  << Hard one 

            sb = new StringBuilder();
            sb.AppendLine("Referenced by, Numeric, Structural and Reference Isomorphic:");
            var referencedbyNumericStructureAndRelativeReferenceIsomorphic = new List<Tuple<ExcelAddress, List<string>>>();

            foreach (var vector in numericStructureAndRelativeReferenceIsomorphic) {
                // check numeric isomorphism 
                var splits = CellReferenceIsomorph.Test(vector.Item2, graph, true);

                foreach (var split in splits) {
                    foreach (var v in Vectorizer.Vectorize(split, id => new ExcelAddress(id))
                        .Where(a => a.Item2.Count > 1)) {
                        referencedbyNumericStructureAndRelativeReferenceIsomorphic.Add(v);
                        sb.AppendLine(v.Item1 + " [" + v.Item2.Count + "] = " +
                                      extraction.CellFormulas[v.Item1.RangeTopLeftCell()]);
                    }
                }
            }

            Log.Info(sb);

            results.FoundVectors.AddRange(numericStructureAndRelativeReferenceIsomorphic);
        }

        private static List<IsoMatch> MatchSubGraphs(SubGraphEqClasses iGraph, SubGraphEqClasses jGraph) {
            List<IsoMatch> matches = new List<IsoMatch>();
            var tolerance = 1;
            var iInd = 0;
            var jInd = 0;
            while (iInd < iGraph.EqClasses.Count && jInd < jGraph.EqClasses.Count) {
                var iVal = iGraph.EqClasses[iInd].EClass;
                var jVal = jGraph.EqClasses[jInd].EClass;

                
                if (Math.Abs(iVal - jVal) < tolerance) {
                    // do we advance i or j? 
                    // 4 4
                    // 4 3 

                    // 4 4 3
                    // 4 4 4 
                    List<int> identicalIs = new List<int>();
                    List<int> identicalJs = new List<int>();
                    while (iInd < iGraph.EqClasses.Count && Math.Abs(iGraph.EqClasses[iInd].EClass - iVal) < tolerance) {
                        identicalIs.Add(iInd);
                        iInd++;
                    }

                    while (jInd < jGraph.EqClasses.Count && Math.Abs(jGraph.EqClasses[jInd].EClass - jVal) < tolerance) {
                        identicalJs.Add(jInd);
                        jInd++;
                    }

                    // record cross product 

                    foreach (var ii in identicalIs) {
                        foreach (var jj in identicalJs) {
                            var iClass = iGraph.EqClasses[ii];
                            var jClass = jGraph.EqClasses[jj];
                            if (iClass.Children == jClass.Children) {
                                matches.Add(new IsoMatch(iClass.NodeId, jClass.NodeId,
                                    iClass.CellId, jClass.CellId));
                            } else {
                                Log.Error("equiv classes are not being aggregated uniquely");
                            }
                        }
                    }
                }

                if (iVal < jVal) {
                    jInd++;
                }

                if (iVal > jVal) {
                    jInd++;
                }
            }

            return matches;
        }

        private static void RecursivelySetNodeHashes(EquivalenceClassLabeller<string> classes,
            Dictionary<string, NodeClass> nodeClasses, List<Node> nodes, string root, Node node) {
            var children = node.LinksTo.Where(nodes.Contains).ToList();
            foreach (var child in children) {
                RecursivelySetNodeHashes(classes, nodeClasses, nodes, root, child);
            }

            var eClass = children.Aggregate( (double) classes.FindClass(node.Id),
                (acc, next) => acc * nodeClasses[next.Id].EClass); // multiply equiv classes...

            var treeNodeCount = 1+ children.Select(c => nodeClasses[c.Id].Children).Sum();
            if (nodeClasses.TryGetValue(node.Id, out var val)) {
                if (val.EClass != eClass || val.Children != treeNodeCount) {
                    Log.Error("hash collision");
                }
            }
            else {
                nodeClasses.Add(node.Id, new NodeClass(node.Id, eClass,
                    treeNodeCount,
                    root));
            }
        }
    }
}