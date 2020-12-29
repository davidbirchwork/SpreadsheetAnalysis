using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
// ReSharper disable StringLiteralTypo

namespace ParseTreeExtractor.Graph.Util {
    /// <summary>
    /// A class for writing out Graphml files
    /// improvements - pass in node count and edge count and allow IEnumerable for nodes and edges
    /// Written by David Birch
    /// </summary>
    public static class GraphMlUtilities {
        // ReSharper disable once InconsistentNaming
        private static readonly XNamespace xn = "http://graphml.graphdrawing.org/xmlns";

        /// <summary>
        /// Saves the graph - we don't care what class the nodes and edges actually are
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex.</typeparam>
        /// <typeparam name="TEdge">The type of the edge.</typeparam>
        /// <param name="nodes">The nodes </param>
        /// <param name="edges">The edges </param>
        /// <param name="nodeFields">The node fields and their types.</param>
        /// <param name="edgeFields">The edge fields and their types.</param>
        /// <param name="nodeId">Gets the node id, must be unique.</param>
        /// <param name="edgeId">Gets the edge id, must be unique.</param>
        /// <param name="edgeSource">Gets the node id which the edge comes from.</param>
        /// <param name="edgeTarget">Gets the node id which the edge comes goes to.</param>
        /// <param name="nodeData">The node data, must match the fields defined above.</param>
        /// <param name="edgeData">The edge data, must match the fields defined above.</param>
        /// <param name="attrsNotToShorten">attributes not to shorten</param>
        /// <returns></returns>
        public static XDocument SaveGraph<TVertex, TEdge>(IEnumerable<TVertex> nodes, IEnumerable<TEdge> edges,
            Dictionary<string, string> nodeFields,
            Dictionary<string, string> edgeFields,
            Func<TVertex, string> nodeId, Func<TEdge, string> edgeId,
            Func<TEdge, string> edgeSource, Func<TEdge, string> edgeTarget,
            Func<TVertex, Dictionary<string, string>> nodeData,
            Func<TEdge, Dictionary<string, string>> edgeData, List<string> attrsNotToShorten = null) {
            attrsNotToShorten = attrsNotToShorten ?? new List<string>();

            
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

            // create graphml file           
            XElement graphFile = new XElement(xn + "graphml");
            xDoc.Add(graphFile);

            // keep track of any shortening of the attribute names
            Dictionary<string, string> shortKeys = new Dictionary<string, string>();

            // set up data keys
            AddKeyFields(nodeFields, shortKeys, attrsNotToShorten, graphFile, "node");
            AddKeyFields(edgeFields, shortKeys, attrsNotToShorten, graphFile, "edge");

            //create the graph
            XElement graph = new XElement(xn + "graph");
            graphFile.Add(graph);

            // add nodes
            int nodeCount = 0;
            foreach (var node in nodes) {
                string id = nodeId(node);
                var nodeElem = new XElement(xn + "node", new XAttribute("id", id));

                foreach (var data in nodeData(node)) {
                    // < data key = "username" > david </ data >
                    var dataElem = new XElement(xn + "data", data.Value);
                    dataElem.Add(new XAttribute("key", GetShortKey(shortKeys, "node", attrsNotToShorten, data.Key)));
                    nodeElem.Add(dataElem);
                }

                graph.Add(nodeElem);
                nodeCount++;
            }

            // add edges
            int edgeCount = 0;
            foreach (var edge in edges) {
                string id = edgeId(edge);
                string source = edgeSource(edge);
                string target = edgeTarget(edge);

                var edgeElem = new XElement(xn + "edge", new XAttribute("id", id),
                    new XAttribute("source", source),
                    new XAttribute("target", target));

                foreach (var data in edgeData(edge)) {
                    // < data key="username">David</ data >
                    var dataElem = new XElement(xn + "data", data.Value);
                    dataElem.Add(new XAttribute("key", GetShortKey(shortKeys, "edge", attrsNotToShorten, data.Key)));
                    edgeElem.Add(dataElem);
                }

                graph.Add(edgeElem);
                edgeCount++;
            }

            // add graph meta data
            graph.Add(new XAttribute("id", "G"));
            graph.Add(new XAttribute("edgedefault", "directed"));
            graph.Add(new XAttribute("parse.nodes", nodeCount));
            graph.Add(new XAttribute("parse.edges", edgeCount));
            graph.Add(new XAttribute("parse.order", "nodesfirst"));
            graph.Add(new XAttribute("parse.nodeids", "free"));
            graph.Add(new XAttribute("parse.edgeids", "free"));

            return xDoc;
        }

        private static void AddKeyFields(Dictionary<string, string> nodeFields,
            Dictionary<string, string> shortKeys,
            List<string> attrsNotToShorten,
            XElement graphFile, string type) {

            foreach (var nodeField in nodeFields) {
                var key = new XElement(xn + "key");
                key.Add(new XAttribute("attr.name", nodeField.Key));
                key.Add(new XAttribute("attr.type", nodeField.Value));
                key.Add(new XAttribute("for", type));
                key.Add(new XAttribute("id", GetShortKey(shortKeys, type, attrsNotToShorten, nodeField.Key)));
                graphFile.Add(key);
            }
        }

        /// <summary>
        /// Graphml gets huge so its common practice to use d0, d1, d2 ... for internal field names... 
        /// </summary>
        /// <param name="shortKeys">The short keys.</param>
        /// <param name="type">node or edge</param>
        /// <param name="attrsNotToShorten"></param>
        /// <param name="keyName">The new key name.</param>
        /// <returns></returns>
        private static string GetShortKey(Dictionary<string, string> shortKeys, string type,
            List<string> attrsNotToShorten, string keyName) {
            string res;
            while (!shortKeys.TryGetValue(type + keyName, out res)) {
                string shortKey = "d" + shortKeys.Count;

                if (keyName.Length == 1) {
                    // optimization no collision detection 
                    shortKey = keyName;
                }

                if (attrsNotToShorten.Contains(keyName)) {
                    shortKey = keyName;
                }

                shortKeys.Add(type + keyName, shortKey);
            }

            return res;
        }

        //<key attr.name="weight" attr.type="double" for="edge" id="weight" />
        // here we could add the weight as follows
        // var key = new XElement(xn + "key");
        // key.Add(new XAttribute("attr.name", "weight"));
        // key.Add(new XAttribute("attr.type", "double"));
        // key.Add(new XAttribute("for", "edge"));
        // key.Add(new XAttribute("id", "weight"));
        // graphFile.Add(key);
    }
}