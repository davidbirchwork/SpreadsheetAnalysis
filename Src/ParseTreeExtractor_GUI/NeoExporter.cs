using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExcelInterop.Domain;
using Irony.Parsing;
using Neo4jClient;
using ParseTreeExtractor.AST;
using ParseTreeExtractor.Domain;

namespace ParseTreeExtractor {
    internal static class NeoExporter {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal static void ExportToNeo(Extraction extraction) {
            var gc = Neo4JConnect();
            ClearNeo4J(gc);


            CreateIndexes(gc);
            CreateNeo4j_CellNodes(gc,extraction);
            CreateNeo4j_ParseTrees(gc, extraction);
            CreateNeo4j_CellRootsLinks(gc, extraction);
            // todo create named range cells 
            // todo create named range cell references 
            
            // make range nodes: 
            CreateNeo4j_Nodes(gc, extraction.Ranges);

            // make named range nodes 
            CreateNeo4j_Nodes(gc, extraction.NamedRanges);

            // now make all references 
           // CreateNeo4j_Links(gc, extraction.References, extraction.CellNodes);


            Log.Info("Completed Neo4J export");
        }


        private static void CreateNeo4j_Nodes<T>(GraphClient gc, ICollection<T> ranges) {
            Log.Info($"Creating {nameof(T)}");

            var start = DateTime.Now;

            foreach (var batch in GetBatches(ranges, 500)) {
                Log.Info($"Creating a batch of {nameof(T)}");
                var query = gc.Cypher
                    .Unwind(batch, "node")
                    .Merge($"(n:{nameof(T)} {{Name: node.Name}})")
                    .Set("n = node");

                query.ExecuteWithoutResults();
                Log.Info($"Finished creating a batch of {nameof(T)}");
            }

            Log.Info($"Creating {nameof(T)} Total took: {(DateTime.Now - start).TotalMilliseconds} ms");
        }

        private static void CreateNeo4j_Links(GraphClient gc, List<Tuple<string, ExcelAddress>> references,
            Dictionary<ExcelAddress, CellNode> nodes) {
            Log.Info("about to create " + references.Count + " references");
            int c = 0;

            foreach (var reference in references) {
                string fromId = reference.Item1;
                string toId = nodes[reference.Item2].Id; // epic naming convention
                var query = gc.Cypher.Match("(n1 {Name: \"" + fromId + "\"})")
                    .Match("(n2 {Name: \"" + toId + "\"})")
                    .Create("(n1)-[:Ref]->(n2)");

                query.ExecuteWithoutResults();
                c++;
                if (c % 1000 == 0) {
                    Log.Info("Created " + c + " relationships");
                }
            }

            Log.Info("Finished creating references");
        }

        private static void CreateIndexes(GraphClient gc) {
            // todo 
            //gc.CreateIndex("Id",new IndexConfiguration(){ Provider = IndexProvider.lucene,Type = IndexType.exact}, IndexFor.Node);
        }

        private static void CreateNeo4j_CellRootsLinks(GraphClient gc, Extraction extraction) {
            Log.Info("about to create " + extraction.CellNodes.Count + " cell nodes references");
            int c = 0;

            foreach (var cellNode in extraction.CellNodes) {
                string fromId = cellNode.Value.Id;
                string toId = fromId + "_1"; // epic naming convention
                var query = gc.Cypher.Match("(n1:CellNode {Name: \"" + fromId + "\"})")
                    .Match("(n2:node {Name: \"" + toId + "\"})")
                    .Create("(n1)-[:CellRef]->(n2)");

                query.ExecuteWithoutResults();
                c++;
                if (c % 1000 == 0) {
                    Log.Info("Created " + c + " relationships");
                }
            }

            Log.Info("Finished creating Cell root nodes");
        }

        private static void CreateNeo4j_ParseTrees(GraphClient gc, Extraction extraction) {
            Log.Info("about to create parse trees nodes in C#");
            List<ASTNode> nodes = new List<ASTNode>();
            foreach (var tree in extraction.ParseTrees) {
                if (tree.Value == null) continue;

                // recurse down the tree and build a list of nodes with relationships
                Queue<ParseTreeNode> todo = new Queue<ParseTreeNode>();

                todo.Enqueue(tree.Value);

                int id = 0;

                while (todo.Any()) {
                    var next = todo.Dequeue();

                    id++;
                    var nodeId = tree.Key.FullName + "_" + id;
                    nodes.Add(new ASTNode {
                        Id = nodeId,
                        Type = next.AstNode == null ? next.Term.ToString() : next.AstNode.ToString(),
                        Children = next.ChildNodes.Select((n, i) => tree.Key.FullName + "_" + (id + todo.Count + i + 1))
                            .ToList(), // some magic 
                        Label = next.ToString(),
                        ParentCell = tree.Key.FullName
                    });
                    
                    foreach (var childNode in next.ChildNodes) {
                        todo.Enqueue(childNode);
                    }
                }
            }

            if (!nodes.Any()) return;


            var batches = GetBatches(nodes, 500).ToList();
            Log.Info("Starting to create nodes in Neo4j");

            var now = DateTime.Now;
            foreach (var batch in batches) {
                DateTime bStart = DateTime.Now;
                Log.Info("Starting batch of nodes at " + bStart);
                var query = gc.Cypher
                    .Unwind(batch, "node")
                    .Merge($"(n:node {{Name: node.Name}})")
                    .Set("n = node")
                    .With("n, node")
                    .Unwind("node.Children", "linkTo")
                    .Merge($"(n1:node {{Name: linkTo}})")
                    .With("n, n1")
                    .Merge("(n)-[:LINKED_TO]->(n1)");

                query.ExecuteWithoutResults();
                Log.Info("Batch " + bStart + $"took: {(DateTime.Now - bStart).TotalMilliseconds} ms");
            }

            Log.Info($"Total took: {(DateTime.Now - now).TotalMilliseconds} ms");

            return;
        }

        private static ICollection<ICollection<T>> GetBatches<T>(ICollection<T> toBatch, int sizeOfBatch) {
            var output = new List<ICollection<T>>();
            if (!toBatch.Any()) {
                return output;
            }

            if (sizeOfBatch > toBatch.Count) sizeOfBatch = toBatch.Count;

            var numBatches = toBatch.Count / sizeOfBatch;
            for (int i = 0; i < numBatches; i++) {
                output.Add(toBatch.Skip(i * sizeOfBatch).Take(sizeOfBatch).ToList());
            }

            return output;
        }

        private static void CreateNeo4j_CellNodes(GraphClient gc, Extraction extraction) {
            Log.Info("Creating Cell nodes");
            

            //if (cells.ContainsKey(new ExcelAddress("SHL_Correction_2016!I10"))) {
            //    Log.Info("ok");
            //}

            var start = DateTime.Now;

            foreach (var batch in GetBatches(extraction.CellNodes.Values, 500)) {
                Log.Info("Creating a batch of cell nodes");
                var query = gc.Cypher
                    .Unwind(batch, "node")
                    .Merge($"(n:{nameof(CellNode)} {{Name: node.Name}})")
                    .Set("n = node");

                query.ExecuteWithoutResults();
                Log.Info("Finished creating a batch of cell nodes");
            }

            Log.Info($"Creating cell nodes Total took: {(DateTime.Now - start).TotalMilliseconds} ms");            
        }

        private static GraphClient Neo4JConnect() {
            var gc = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "access");
            gc.Connect();

            Log.Info("connected to neo4j");

            return gc;
        }

        private static void ClearNeo4J(GraphClient gc) {
            Log.Info("About to clear Neo4j");
            var queryRels = gc.Cypher.Match("()-[r]->()").Delete("r");
            queryRels.ExecuteWithoutResults();
            Log.Info("Cleared Neo4j relations");
            var queryNodes = gc.Cypher.Match("(n)").Delete("n");
            queryNodes.ExecuteWithoutResults();
            Log.Info("Cleared Neo4j nodes");
            Log.Info("Cleared Neo4j node");
        }
    }
}