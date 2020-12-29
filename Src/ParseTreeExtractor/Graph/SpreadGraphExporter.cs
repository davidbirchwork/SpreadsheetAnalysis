using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using log4net;
using ParseTreeExtractor.AST;
using ParseTreeExtractor.Domain;
using ParseTreeExtractor.Graph.Util;

namespace ParseTreeExtractor.Graph {
    /// <summary>
    /// Objective - export a graph to graphml 
    /// </summary>
    public static class SpreadGraphExporter {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Export(SpreadGraph graph,string filename) {
            var graphml = Export(graph);

            Log.Info("About to write graphml to "+filename);
            using (XmlWriter writer = new XmlTextWriter(filename, Encoding.UTF8)) {
                graphml.WriteTo(writer);
            }
        }

        public static XDocument Export(SpreadGraph graph) {
// figure out the types of the data exported - yep this is ugly reflection might make it nicer...
            Dictionary<string, string> nodeFields = new CellNode().GetDataTypes()
                .Union(new NamedRange().GetDataTypes())
                .Union(new RangeNode().GetDataTypes())
                .Union(new ASTNode().GetDataTypes())
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Dictionary<string, string> edgeFields = new Edge().GetDataTypes();

            var graphml = GraphMlUtilities.SaveGraph(graph.Nodes.Values, graph.Edges,
                nodeFields,
                new Dictionary<string, string>(), // edge fields
                n => n.Id,
                e => e.Id,
                e => e.Source.Id,
                e => e.Target.Id,
                n => n.GetData(),
                e => e.GetData(),
                new List<string>()
            );
            return graphml;
        }
    }
}
