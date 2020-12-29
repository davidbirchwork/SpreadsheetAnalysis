using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ExcelExtractor.Analyses.Graph {
    /// <summary>
    /// A class for writing out Graphml files
    /// improvements - pass in node count and edge count and allow IEnumerable for nodes and edges 
    /// </summary>
    public static class GraphMlUtilities {
        /// <summary>
        /// Saves the graph - we dont care what class the nodes and edges actually are
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
        /// <returns></returns>
        public static XDocument SaveGraph<TVertex, TEdge>(IEnumerable<TVertex> nodes, IEnumerable<TEdge> edges,
            Dictionary<string, string> nodeFields,
            Dictionary<string, string> edgeFields,
            Func<TVertex, string> nodeId, Func<TEdge, string> edgeId,
            Func<TEdge, string> edgeSource, Func<TEdge, string> edgeTarget,
            Func<TVertex, Dictionary<string, string>> nodeData,
            Func<TEdge, Dictionary<string, string>> edgeData) {
            return SaveGraph(nodes, edges, nodeFields, edgeFields, nodeId, edgeId, edgeSource, edgeTarget, nodeData,
                edgeData, new List<string>());

        }

        
        /// <summary>
        /// Saves the graph - we dont care what class the nodes and edges actually are
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
        /// <param name="attbsToShorten"></param>
        /// <returns></returns>
        public static XDocument SaveGraph<TVertex, TEdge>(IEnumerable<TVertex> nodes, IEnumerable<TEdge> edges,
            Dictionary<string, string> nodeFields,
            Dictionary<string, string> edgeFields,
            Func<TVertex, string> nodeId, Func<TEdge, string> edgeId,
            Func<TEdge, string> edgeSource, Func<TEdge, string> edgeTarget,
            Func<TVertex, Dictionary<string, string>> nodeData,
            Func<TEdge, Dictionary<string, string>> edgeData, List<string> attbsToShorten) {


            XNamespace xn = "http://graphml.graphdrawing.org/xmlns";
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

            // create graphml file           
            XElement graphfile = new XElement(xn + "graphml");
            xDoc.Add(graphfile);
            
            // set up the headers
            Dictionary<string,string> shortKeys = new Dictionary<string, string>();

            AddKeyFields(nodeFields, xn, shortKeys,attbsToShorten, graphfile,"node");
            AddKeyFields(edgeFields, xn, shortKeys,attbsToShorten, graphfile,"edge");

           // add graph meta data
            XElement graph = new XElement(xn + "graph");

            graph.Add(new XAttribute("id", "G"));
            graph.Add(new XAttribute("edgedefault", "directed"));
            graph.Add(new XAttribute("parse.nodes", value: nodes.Count()));
            graph.Add(new XAttribute("parse.edges", edges.Count()));
            graph.Add(new XAttribute("parse.order", "nodesfirst"));
            graph.Add(new XAttribute("parse.nodeids", "free"));
            graph.Add(new XAttribute("parse.edgeids", "free"));
            graphfile.Add(graph);

            // add nodes
            foreach (var node in nodes) {
                string id = nodeId(node);
                var nodeElem = new XElement(xn + "node", new XAttribute("id", id));
                
                foreach (var data in nodeData(node)) {
                    // < data key = "username" > _Paul_Tobin_ </ data >
                    var dataElem = new XElement(xn + "data", data.Value);
                    dataElem.Add(new XAttribute("key", GetShortKey(shortKeys, "node" ,attbsToShorten, data.Key)));
                    nodeElem.Add(dataElem);
                }

                graph.Add(nodeElem);
            }

            // add edges
            foreach (var edge in edges) {
                string id = edgeId(edge);
                string source = edgeSource(edge);
                string target = edgeTarget(edge);
                
                var edgeElem = new XElement(xn + "edge", new XAttribute("id", id),
                    new XAttribute("source", source),
                    new XAttribute("target", target));

                foreach (var data in edgeData(edge)) {
                    // < data key = "username" > _Paul_Tobin_ </ data >
                    var dataElem = new XElement(xn + "data", data.Value);
                    dataElem.Add(new XAttribute("key", GetShortKey(shortKeys, "edge" ,attbsToShorten, data.Key)));
                    edgeElem.Add(dataElem);
                }
             
                graph.Add(edgeElem);
            }

            return xDoc;
        }

        private static void AddKeyFields(Dictionary<string, string> nodeFields, XNamespace xn,
            Dictionary<string, string> shortKeys,
            List<string> attbsNotToShorten,
            XElement graphfile, string type) {
            foreach (var nodeField in nodeFields) {
                var akey = new XElement(xn + "key");
                akey.Add(new XAttribute("attr.name", nodeField.Key));
                akey.Add(new XAttribute("attr.type", nodeField.Value)); 
                akey.Add(new XAttribute("for", type));
                akey.Add(new XAttribute("id", GetShortKey(shortKeys, type , attbsNotToShorten, nodeField.Key)));
                graphfile.Add(akey);
            }
        }

        /// <summary>
        /// Graphml gets huge so its common practice to use d0, d1, d2 ... for internal field names... 
        /// </summary>
        /// <param name="shortKeys">The short keys.</param>
        /// <param name="type">node or edge</param>
        /// <param name="attbsToShorten"></param>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        private static string GetShortKey(Dictionary<string, string> shortKeys, string type,
            List<string> attbsToShorten, string s) {
            string res;
            while (!shortKeys.TryGetValue(type + s, out res)) { 
                string shortkey = "D" + shortKeys.Count;
                
                if (s.Length == 1) {// optimisation no collision detection 
                    shortkey = s; 
                }

                if (!attbsToShorten.Contains(s)) {
                    shortkey = s;
                }

                shortKeys.Add(type+s, shortkey);
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
        // graphfile.Add(key);
    }
}
