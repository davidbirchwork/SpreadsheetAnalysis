using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Algorithms;
using Graph;

namespace GraphMetrics.Metrics {
    [Metric("Colour Matrixes","Produces matrixes showing communication between colours")]
    public class ColourMatrixes : IMetric {

        public SortedDictionary<string, SortedDictionary<string, int>> DirectSheetLinkMatrix = new SortedDictionary<string, SortedDictionary<string, int>>();
        public SortedDictionary<string, SortedDictionary<string, int>> EdgeDirectSheetLinkMatrix = new SortedDictionary<string, SortedDictionary<string, int>>();
        public ConcurrentDictionary<string, ConcurrentDictionary<string, int>> InDirectSheetLinkMatrix = new ConcurrentDictionary<string, ConcurrentDictionary<string, int>>();
        private List<string> Colours;

        #region Implementation of IMetric

        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics, IEnumerable<ExcelVertex> vertices, IEnumerable<AEdge> edges, List<string> colours, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            this.Colours = colours;

            #region DirectSheetLinkMatrix - direct)

            foreach (AEdge aEdge in edges) {
                string fromsheet = aEdge.Source.GetColourID();
                string tosheet = aEdge.Target.GetColourID();

                if (!this.DirectSheetLinkMatrix.ContainsKey(fromsheet)) {
                    this.DirectSheetLinkMatrix.Add(fromsheet, new SortedDictionary<string, int>());
                }

                if (!this.DirectSheetLinkMatrix.ContainsKey(tosheet)) {
                    this.DirectSheetLinkMatrix.Add(tosheet, new SortedDictionary<string, int>());
                }

                if (!this.DirectSheetLinkMatrix[fromsheet].ContainsKey(tosheet)) {
                    this.DirectSheetLinkMatrix[fromsheet].Add(tosheet, 0);
                }

                this.DirectSheetLinkMatrix[fromsheet][tosheet] = this.DirectSheetLinkMatrix[fromsheet][tosheet] + 1;
            }

            #endregion

            #region InDirectSheetLinkMatrix

            TraceAllInDirectReferences(vertexdict);
            //Parallel.ForEach(vertexdict.Where(v => v.Value.Item1.Count == 0),
              //               vertex => TraceVertexPath(vertex.Key,vertexdict));

            #endregion

            #region EdgeDirectSheetLinkMatrix - edges only
            Dictionary<string, KeyValuePair<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>>> vertexesbyID =
                vertexdict.ToDictionary(kpv => kpv.Key.ID,
                                        kpv =>
                                        new KeyValuePair<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>>(kpv.Key, kpv.Value));            
            foreach (AEdge aEdge in edges) {
                AEdge edge = aEdge;
                KeyValuePair<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> target = vertexesbyID[edge.Target.ID];
                //    vertexdict.First(pair => pair.Key.ID == edge.Source.ID);
                if (target.Value.Item2.Count == 0) {
                    string fromsheet = aEdge.Source.GetColourID();
                    string tosheet = aEdge.Target.GetColourID();

                    if (!this.EdgeDirectSheetLinkMatrix.ContainsKey(fromsheet)) {
                        this.EdgeDirectSheetLinkMatrix.Add(fromsheet, new SortedDictionary<string, int>());
                    }
                    if (!this.EdgeDirectSheetLinkMatrix[fromsheet].ContainsKey(tosheet)) {
                        this.EdgeDirectSheetLinkMatrix[fromsheet].Add(tosheet, 0);
                    }

                    this.EdgeDirectSheetLinkMatrix[fromsheet][tosheet] =
                        this.EdgeDirectSheetLinkMatrix[fromsheet][tosheet] + 1;
                }
            }

            #endregion

            return null;
        }

        public string Print() {

            StringBuilder sb = new StringBuilder();

            #region DirectSheetLinkMatrix - in text            

            /*   foreach (KeyValuePair<string, SortedDictionary<string, int>> row in this.DirectSheetLinkMatrix) {
                foreach (KeyValuePair<string, int> col in row.Value) {
                    sb.Append("# Direct References from "+ row.Key+" to "+ col.Key + " = " +col.Value + " \n");
                }
            }*/

            #endregion

            #region DirectSheetLinkMatrix & as CSV matrix

            // headers 
            sb.Append("Direct references from one sheet to another \n");
            sb.Append("from\\to,");
            foreach (string sheet in this.Colours) {
                sb.Append(sheet + ",");
            }
            sb.Append("\n");
            // data
            foreach (string row in this.Colours) {
                sb.Append(row + ",");
                foreach (string col in this.Colours) {
                    int refs = 0;
                    if (this.DirectSheetLinkMatrix.ContainsKey(row) && this.DirectSheetLinkMatrix[row].ContainsKey(col)) {
                        refs = this.DirectSheetLinkMatrix[row][col];
                    }

                    sb.Append(refs + ",");
                }

                sb.Append("\n");
            }

            #endregion

            #region EdgeDirectSheetLinkMatrix as CSV matrix

            // headers 
            sb.Append("End values in each sheet used in other sheets \n");
            sb.Append("from\\to,");
            foreach (string sheet in this.Colours) {
                sb.Append(sheet + ",");
            }
            sb.Append("\n");
            // data
            foreach (string row in this.Colours) {
                sb.Append(row + ",");
                foreach (string col in this.Colours) {
                    int refs = 0;
                    if (this.EdgeDirectSheetLinkMatrix.ContainsKey(row) &&
                        this.EdgeDirectSheetLinkMatrix[row].ContainsKey(col)) {
                        refs = this.EdgeDirectSheetLinkMatrix[row][col];
                    }

                    sb.Append(refs + ",");
                }

                sb.Append("\n");
            }

            #endregion

            #region InDirectSheetLinkMatrix as CSV matrix

            // headers 
            sb.Append("Indirect Sheet references Table \n");
            sb.Append("from\\to,");
            foreach (string sheet in this.Colours) {
                sb.Append(sheet + ",");
            }
            sb.Append("\n");
            // data
            foreach (string row in this.Colours) {
                sb.Append(row + ",");
                foreach (string col in this.Colours) {
                    int refs = 0;
                    if (this.InDirectSheetLinkMatrix.ContainsKey(row) &&
                        this.InDirectSheetLinkMatrix[row].ContainsKey(col)) {
                        refs = this.InDirectSheetLinkMatrix[row][col];
                    }

                    sb.Append(refs + ",");
                }

                sb.Append("\n");
            }

            #endregion

            #region print matrix as graph...

            List<Tuple<string, string>> arcs = (from row in this.Colours
                                                from col in this.Colours
                                                where this.DirectSheetLinkMatrix.ContainsKey(row) && this.DirectSheetLinkMatrix[row].ContainsKey(col)
                                                select new Tuple<string, string>(row, col)).ToList();

            //this.DirectSheetLinkMatrix[row][col]);

            sb.Append(SaveGraph(this.Colours, arcs));


            #endregion

            return sb.ToString();
        }

        public List<string> PreRequisiteMetrics() {
            return  new List<string>();
        }

        #endregion

        private void TraceAllInDirectReferences(Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            Dictionary<string, Tuple<ExcelVertex, List<ExcelVertex>, List<ExcelVertex>, List<string>>> vertexes =
                vertexdict.ToDictionary(kpv => kpv.Key.ID,
                                        kpv =>
                                        new Tuple<ExcelVertex, List<ExcelVertex>, List<ExcelVertex>, List<string>>(kpv.Key, kpv.Value.Item1,
                                                                                         kpv.Value.Item2,new List<string>()));
            WorkList.ParallelRecursiveEval
                <string, Tuple<ExcelVertex, List<ExcelVertex>, List<ExcelVertex>, List<string>>,
                    Tuple<ExcelVertex, List<ExcelVertex>, List<ExcelVertex>, List<string>>>
                (vertexes, (vertex, evaluated) => {
                               IEnumerable<string> references = vertex.Value.Item3.Select(vert => vert.ID);
                               if (!references.All(evaluated.ContainsKey)) {
                                   return false; // not everything we've referenced is done
                               }
                               List<string> referencedColours = vertex.Value.Item4;
                               // add colours of our references

                               foreach (ExcelVertex referencedVertex in vertex.Value.Item3) {
                                   string referencedColour = referencedVertex.GetColourID();
                                   if (!referencedColours.Contains(referencedColour)) {
                                       referencedColours.Add(referencedColour);
                                   }
                               }

                               // then add their referenced colours

                               foreach (string reference in references) {
                                   foreach (var referencedColour in evaluated[reference].Item4) {
                                       if (!referencedColours.Contains(referencedColour)) {
                                           referencedColours.Add(referencedColour);
                                      }
                                   }
                               }

                               string colourID = vertex.Value.Item1.GetColourID();
                               foreach (string referencedColour in referencedColours) {
                                   AddReference(colourID, referencedColour);
                               }
                               
                               evaluated.AddOrUpdate(vertex.Key,vertex.Value,(key,old) => vertex.Value);
                 //   evaluated.Add(vertex.Key,vertex.Value);

                               return true;
                           });
        }

        private List<string> TraceVertexPath(ExcelVertex aVertex, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            List<string> indirectReferences = new List<string>();
            foreach (ExcelVertex target in vertexdict[aVertex].Item2) {
                indirectReferences.AddRange(TraceVertexPath(target, vertexdict));
                indirectReferences.Add(target.GetColourID());
            }
            foreach (string indirectReference in indirectReferences) {
                AddReference(aVertex.GetColourID(), indirectReference);
            }

            return indirectReferences;
        }

        private void AddReference(string fromsheet, string tosheet) {

            if (!this.InDirectSheetLinkMatrix.ContainsKey(fromsheet)) {
                this.InDirectSheetLinkMatrix.AddOrUpdate(fromsheet, new ConcurrentDictionary<string, int>(), (s, d) => d);
            }

            this.InDirectSheetLinkMatrix[fromsheet].AddOrUpdate(tosheet, 1, (k, v) => 1);
        }

        private static string SaveGraph(IEnumerable<string> nodes, IEnumerable<Tuple<string, string>> arcs) {
            XNamespace xn = "http://graphml.graphdrawing.org/xmlns";
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));


            // create gml file           
            XElement graphfile = new XElement(xn + "graphml");
            xDoc.Add(graphfile);

            // add graph meta data
            XElement graph = new XElement(xn + "graph");

            graph.Add(new XAttribute("id", "G"));
            graph.Add(new XAttribute("edgedefault", "directed"));
            graph.Add(new XAttribute("parse.nodes", nodes.Count()));
            graph.Add(new XAttribute("parse.edges", arcs.Count()));
            graph.Add(new XAttribute("parse.order", "nodesfirst"));
            graph.Add(new XAttribute("parse.nodeids", "free"));
            graph.Add(new XAttribute("parse.edgeids", "free"));
            graphfile.Add(graph);

            // add nodes
            foreach (var name in nodes) {                
                graph.Add(new XElement(xn + "node", new XAttribute("id", name)));
            }

            // add edges
            foreach (Tuple<string, string> arc in arcs) {
                string source = arc.Item1;
                string target = arc.Item2;

                graph.Add(new XElement(xn + "edge", new XAttribute("id", source + "to" + target),
                    new XAttribute("source", source),
                    new XAttribute("target", target)));
            }

            return xDoc.ToString();
        }
    }
}