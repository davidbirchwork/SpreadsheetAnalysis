using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using log4net;
using Microsoft.Msagl.Drawing;

namespace HyperGraphViewer {
    public static class GraphmlReader {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private class DataKey {
            public string Id { get; }
            public string Name { get; }
            public string ValueType { get; }
            public string AppliesTo { get; }

            public DataKey(string id, string name, string valueType, string appliesTo) {
                Id = id;
                Name = name;
                ValueType = valueType;
                AppliesTo = appliesTo;
            }
        }

        public static Graph ReadGraphml(string fileName, bool hyperGraph= false, bool nestedHyperGraph = false) {
            Log.Info("about to start parsing Graphml from "+fileName);
            var graph = new Graph();

            XDocument xGraph = XDocument.Load(fileName);

            XNamespace ns = XNamespace.Get(@"http://graphml.graphdrawing.org/xmlns");

            // read headers 
            var keys = xGraph.Root?.Elements(ns + "key").Select(k =>
                new DataKey(
                    k.Attribute("id")?.Value.ToLowerInvariant(),
                    k.Attribute("attr.name")?.Value.ToLowerInvariant(),
                    k.Attribute("attr.type")?.Value,
                    k.Attribute("for")?.Value)
            ).ToList();
            if (keys == null) {
               Log.Warn("No meta keys in this graphml");
                return graph;
            }

            var nodeKeys = keys.Where(n => n.AppliesTo == "node").ToList();
            var edgeKeys = keys.Where(n => n.AppliesTo == "edge").ToList();

            var graphRoot = xGraph.Root?.Element(ns + "graph");
            if (graphRoot == null) {
               Log.Warn("No graph element in this graphml");
                return graph;
            }
            Dictionary<string,Node> nodes = new Dictionary<string, Node>();

            foreach (XElement node in graphRoot.Elements(ns + "node")) {
                var id = node.Attribute("id")?.Value;
                if (id == null) {
                    Log.Warn("Node with no id");
                    continue;
                }

                // read data
                var data = node.Elements(ns + "data").ToDictionary(
                    xdata => xdata.Attribute("key")?.Value.ToLowerInvariant(),
                    xdata => xdata.Value);
                var translateData = TranslateData(data, nodeKeys);

                var label = id;
                if (translateData.TryGetValue("label", out var lbl)) {
                    label = lbl.ToString();
                }

                if (hyperGraph && translateData.ContainsKey("type") && translateData["type"].ToString() == "Cell") {
                    var subgraph = new Subgraph(id + "_sub")
                        {LabelText = label, UserData = translateData, Attr = {Color = Color.Blue}};
                    graph.RootSubgraph.AddSubgraph(subgraph);
                }
                if (nestedHyperGraph && translateData.ContainsKey("type") && translateData["type"].ToString() == "Range") {
                    var subgraph = new Subgraph(id + "_sub")
                        { LabelText = label, UserData = translateData, Attr = { Color = Color.Blue } };
                    graph.RootSubgraph.AddSubgraph(subgraph);
                }

                var newNode = new Node(id) {LabelText = label, UserData = translateData, Attr = {Color = Color.Black}};
                graph.AddNode(newNode);
                nodes.Add(id, newNode);
            }

            foreach (XElement edge in graphRoot.Elements(ns + "edge")) {
                var id = edge.Attribute("id")?.Value;
                var source = edge.Attribute("source")?.Value;
                var target = edge.Attribute("target")?.Value;
                if (id == null || source == null || target == null) {
                   Log.Warn("Bad edge");
                    continue;
                }

                // read data
                var data = edge.Elements(ns + "data").ToDictionary(
                    xdata => xdata.Attribute("key")?.Value,
                    xdata => xdata.Value);
                var translateData = TranslateData(data, edgeKeys);

                if (translateData.TryGetValue("label", out var lbl)) {
                    var e = graph.AddEdge(source, lbl.ToString(), target);
                    e.UserData = translateData;
                }
                else {
                    var e = graph.AddEdge(source, target);
                    e.UserData = translateData;
                }
            }

           if (hyperGraph) {
                // build a node map
                var dict = new ConcurrentDictionary<string,List<string>>();
                foreach (var edge in graph.Edges) {
                    dict.AddOrUpdate(edge.Source, i => new List<string> {edge.Target},
                        (s, targets) => {
                            targets.Add(edge.Target);
                            return targets;
                        });
                }
                // find cell nodes 
                foreach (var subgraph in graph.SubgraphMap.Values) {
                    if (subgraph.Id == "the root subgraph's boundary") continue;
                    // follow them through until we hit a reference node
                    var nodeId = subgraph.Id.Replace("_sub", "");
                    List<Node> toAdd = new List<Node> {graph.FindNode(nodeId)};
                    Stack<string> fringe = new Stack<string>();
                    if (dict.ContainsKey(nodeId)) {
                        foreach (var child in dict[nodeId]) {
                            fringe.Push(child);
                        }
                    }

                    while (fringe.Count != 0) {
                        string id = fringe.Pop();
                        var n = nodes[id];
                        toAdd.Add(n);
                        if (!n.NodeTypeIs("Reference")) {
                            if (dict.ContainsKey(id)) {
                                foreach (var child in dict[id]) {
                                    fringe.Push(child);
                                }
                            }
                        }
                    }
                    // add them to the sub graph
                    foreach (var subgraphNode in toAdd) {
                        subgraph.AddNode(subgraphNode);
                        if (nestedHyperGraph && subgraphNode.NodeTypeIs("Cell") && subgraphNode.Id+"_sub" != subgraph.Id) {// put range references in as well
                            subgraph.AddSubgraph(graph.SubgraphMap[subgraphNode.Id+"_sub"]);
                        }
                    }
                }
            }

            Log.Info("finished parsing Graphml from " + fileName);

            return graph;
        }

        private static bool NodeTypeIs(this Node n, string type) {
            return n.UserData is Dictionary<string, object> data && data.ContainsKey("type") && data["type"].ToString() == type;
        }

        private static Dictionary<string, object>
            TranslateData(Dictionary<string, string> data, List<DataKey> nodeKeys) {
            var translatedData = new Dictionary<string, object>();

            foreach (var kvp in data) {
                var key = nodeKeys.FirstOrDefault(k => k.Id == kvp.Key);
                if (key == null) {
                    translatedData.Add(kvp.Key, kvp.Value);
                    continue;
                }

                switch (key.ValueType) {
                    case "string":
                        translatedData.Add(key.Name, kvp.Value);
                        break;
                    case "int":
                        translatedData.Add(key.Name, int.Parse(kvp.Value));
                        break;
                    case "float":
                        translatedData.Add(key.Name, float.Parse(kvp.Value));
                        break;
                    default:
                        translatedData.Add(kvp.Key, kvp.Value);
                        break;
                }
            }

            return translatedData;
        }
    }
}