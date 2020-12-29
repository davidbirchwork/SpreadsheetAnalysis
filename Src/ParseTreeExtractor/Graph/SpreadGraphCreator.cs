using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Irony.Parsing;
using log4net;
using ParseTreeExtractor.AST;
using ParseTreeExtractor.Domain;

namespace ParseTreeExtractor.Graph {
    public static class SpreadGraphCreator {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Objective here is to take the results of extraction and create a .net object graph
        /// </summary>
        /// <param name="extraction">results of extracting the spreadsheet</param>
        public static SpreadGraph Create(Extraction extraction) {
            // todo note that due to the AST refactoring the AST Nodes for a cell will not be consecutively numbered... re-labeling could e done
            Log.Info("Starting to create a SpreadGraph");

            SpreadGraph graph = new SpreadGraph();
            // add cell nodes
            Log.Info("Adding " + extraction.CellNodes.Count + " Cell nodes to the graph");
            graph.AddNodes(extraction.CellNodes.Values);
            // create Parse Trees 
            CreateParseTrees(graph, extraction);
            //Link Cells and their Parse Trees
            LinkCellsAndParseTrees(graph, extraction);
            // add Range nodes
            Log.Info("Adding " + extraction.Ranges.Count + " Range nodes to the graph");
            graph.AddNodes(extraction.Ranges);
            // add Named Range nodes
            Log.Info("Adding " + extraction.NamedRanges.Count + " Named Range nodes to the graph");
            graph.AddNodes(extraction.NamedRanges);

            // create internal graph references
            CreateEdges(graph, extraction);

            Log.Info("Finished creating a SpreadGraph");

            Log.Info($"Cleaned {CleanOrphanNodes(graph)} Orphan Nodes");

            Log.Info("Creating Valency Map");
            CreateNodeLists(graph);

            return graph;
        }

        private static void CreateNodeLists(SpreadGraph graph) {
            foreach (var edge in graph.Edges) {
                edge.Source.LinksTo.Add(edge.Target);
                edge.Target.LinkedFrom.Add(edge.Source);
            }
        }

        private static int CleanOrphanNodes(SpreadGraph graph) {
            ConcurrentDictionary<string,int> nodeValency= new ConcurrentDictionary<string, int>();
            foreach (var edge in graph.Edges) {
                nodeValency.AddOrUpdate(edge.Source.Id, 1, (k, v) => v+1);
                nodeValency.AddOrUpdate(edge.Target.Id, 1, (k, v) => v + 1);
            }
            var orphans = graph.Nodes.Where(n => !nodeValency.ContainsKey(n.Key)).ToList();

            foreach (var orphan in orphans) {
                graph.Nodes.TryRemove(orphan.Key, out _);
            }

            return orphans.Count;
        }

        /// <summary>
        /// See ExcelExtractor.GenerateReferences
        /// this creates graph edges for:
        /// > AST Reference links 
        /// > Range > Cell links
        /// > Named Cell > Cells links
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="extraction"></param>
        private static void CreateEdges(SpreadGraph graph, Extraction extraction) {
            Log.Info("about to create " + extraction.References.Count + " references");
            
            foreach (var reference in extraction.References) {
                graph.AddEdge(reference.Item1, 
                              reference.Item2.ToString());
            }

            Log.Info("Finished creating references");
        }

        private static string GetName(ParseTreeNode n) => ((Tuple<string, object>)n.Tag).Item1;

        private static void LinkCellsAndParseTrees(SpreadGraph graph, Extraction extraction) {
            int count = 0;
            foreach (var tree in extraction.ParseTrees) {
                if (tree.Value == null) continue;
                graph.AddEdge(tree.Key.FullName,
                       GetName(tree.Value)); 
                count++;
            }

            Log.Info("Just added "+count+" Edges between Cells and their ASTs");
        }

        private static void CreateParseTrees(SpreadGraph graph, Extraction extraction) {
            Log.Info("about to create parse trees nodes in C#");

            List<ASTNode> nodes = new List<ASTNode>();

            #region Create Parse Trees
            

            foreach (var tree in extraction.ParseTrees) {
                if (tree.Value == null) continue;

                // recurse down the tree and build a list of nodes with relationships
                Queue<ParseTreeNode> todo = new Queue<ParseTreeNode>();

                todo.Enqueue(tree.Value);

                while (todo.Any()) {
                    var next = todo.Dequeue();
                    
                    nodes.Add(new ASTNode {
                        Id = GetName(next),
                        Type = next.AstNode == null ? next.Term.ToString() : next.AstNode.ToString(),
                        IsOperator = next.IsOperator(),
                        Children = next.ChildNodes.Select((n, i) => GetName(n))
                            .ToList(), // some magic Breadth First Search numbering
                        Label = next.ToString(),
                        ParentCell = tree.Key.FullName
                    });

                    foreach (var childNode in next.ChildNodes) {
                        todo.Enqueue(childNode);
                    }
                }
            }

            #endregion

            if (!nodes.Any()) return;

            Log.Info("Adding "+nodes.Count+" AST nodes to the graph");

            #region Add Internal Edges

            graph.AddNodes(nodes);
            int edgeCount = 0;
            foreach (var astNode in nodes) {
                foreach (var child in astNode.Children) {
                    graph.AddEdge(astNode.Id, child);
                    edgeCount++;
                }
            }

            #endregion

            Log.Info("Just added "+edgeCount+ " internal AST edges");
        }
    }
}