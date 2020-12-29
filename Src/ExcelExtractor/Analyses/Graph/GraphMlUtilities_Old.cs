using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ExcelExtractor.Analyses.Graph {
    public static class GraphMlUtilities_Old {
        public static void SaveGraph(Dictionary<string, string> nodes, List<Tuple<string, string>> arcs, IDictionary<string, string> friendlyNames, bool chkSheetPrefix, string fileName) {
            XNamespace xn = "http://graphml.graphdrawing.org/xmlns";
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

            // create gml file           
            XElement graphfile = new XElement(xn + "graphml");
            xDoc.Add(graphfile);

            // add graph meta data
            XElement graph = new XElement(xn + "graph");

            graph.Add(new XAttribute("id", "G"));
            graph.Add(new XAttribute("edgedefault", "directed"));
            graph.Add(new XAttribute("parse.nodes", nodes.Count));
            graph.Add(new XAttribute("parse.edges", arcs.Count));
            graph.Add(new XAttribute("parse.order", "nodesfirst"));
            graph.Add(new XAttribute("parse.nodeids", "free"));
            graph.Add(new XAttribute("parse.edgeids", "free"));
            graphfile.Add(graph);

            // add nodes
            foreach (var node in nodes) {
                string name = GetName(friendlyNames, node.Key, chkSheetPrefix);
                graph.Add(new XElement(xn + "node", new XAttribute("id", name), new XAttribute("meta",node.Value ?? "") ));
            }

            // add edges
            foreach (Tuple<string, string> arc in arcs) {
                string source = GetName(friendlyNames, arc.Item1, chkSheetPrefix);
                string target = GetName(friendlyNames, arc.Item2, chkSheetPrefix);

                graph.Add(new XElement(xn + "edge", new XAttribute("id", source + "to" + target),
                                       new XAttribute("source", source),
                                       new XAttribute("target", target)));
            }
          
            xDoc.Save(fileName);          
        }

        public static string GetName(IDictionary<string, string> friendlyNames, string node, bool chkSheetPrefix) {
            if (friendlyNames == null || ! friendlyNames.ContainsKey(node)) {
                return node;
            }
            string name = friendlyNames[node];
            name = name.Trim();
            if (chkSheetPrefix) {
                name = node.Split(new[] {'!'})[0] + "!" + name; // ensure it has a sheet modifier
            }
            return name;
        }
    }
}
