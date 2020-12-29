using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ExcelInterop.Domain;
using ParseTreeExtractor.AST;

namespace ParseTreeExtractor.Domain {
    public class SpreadGraph {
        public readonly ConcurrentDictionary<string,Node> Nodes = new ConcurrentDictionary<string, Node>();
        public ConcurrentBag<Edge> Edges = new ConcurrentBag<Edge>();

        public bool AddNode(Node node) => Nodes.TryAdd(node.Id, node);

        public void AddNodes(IEnumerable<Node> nodes) {
            foreach (var node in nodes) {
                AddNode(node);
            }
        }

        public void AddEdge(string sourceId, string targetId) {
            if (!Nodes.ContainsKey(sourceId))
                throw new ArgumentOutOfRangeException(nameof(sourceId)," no known source node " + sourceId + " in edge " + sourceId + " to " + targetId);
            if (!Nodes.ContainsKey(targetId)) {
                // here we should handle empty nodes by creating them - we must ensure that targetid's are valid single cells which do not otherwise exist. 
                try {// todo this is not populating the extraction lists so need to be careful! 
                    var split = targetId.Split(new[] {"!"}, StringSplitOptions.None);
                    if (split.Length != 2 || string.IsNullOrWhiteSpace(split[0]) ||
                        string.IsNullOrWhiteSpace(split[1]) || split[1].Contains("_"))
                        throw new ArgumentException("bad target " + targetId);
                    ExcelAddress addr = new ExcelAddress(targetId);
                    if(addr.IsRange() || string.IsNullOrWhiteSpace(addr.WorkSheet)) throw new ArgumentException("bad address");

                    //addn
                    Node n = new CellNode {ColNo = addr.IntCol,Id = targetId, NodeType = "Cell",RowNo = addr.IntRow,Sheet = addr.WorkSheet};
                    Node v = new ASTNode {
                        Children = new List<string>(), Id = targetId + "_0", IsOperator = false, Label = "0",
                        ParentCell = targetId, Type = "NumberToken"
                    };
                    this.AddNode(n);
                    this.AddNode(v);
                    this.AddEdge(n.Id,v.Id);

                } catch (Exception e) {
                    throw new ArgumentOutOfRangeException(nameof(targetId),
                        "no known target node " + targetId + " in edge " + sourceId + " to " + targetId+ " exception "+e);
                }
            }

            Edges.Add(new Edge {
                Id = sourceId + ">" + targetId,
                Source = Nodes[sourceId],
                Target = Nodes[targetId]
            });
        }
    }
}