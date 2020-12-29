using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Algorithms;
using ExcelExtractor.Domain;

namespace ExcelExtractor.Analyses.Graph
{
    /// <summary>
    /// partition the whole graph into smaller partitions based on the reference structure 
    /// </summary>
    public class ExtractGraphComponents {

        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);        

        public List<ExcelGraph> PartitionGraph(FunctionExtractor extractor, string fileName, int minimumSubgraphSize = 1) {
            Log.Info("about to extract sub components");

            Log.Info("about to start evaluation");

            if (extractor.Evaluations == null) {
                extractor.EvaluateAll();
            }

            Log.Info("About to start building graph");

            var partitions = CreatePartitions(out var nodes, extractor, out var arcs);

            var subcomponents = partitions.Select(kvp => new {
                Pid = kvp.Key,
                Count = kvp.Value.Count
            }).OrderByDescending(k => k.Count);

            var distributionSize = subcomponents.GroupBy(k => k.Count);
            
            var sb = new StringBuilder();
            sb.AppendLine("Size\tCount");
            foreach (var a in distributionSize) {
                sb.AppendLine(a.Key+"\t"+a.Count());
            }
            Log.Info("Distribution of Partition Sizes ");
            Log.Info(sb);

            var distfname = Path.ChangeExtension(fileName, ".__dist.csv")?.Replace(".__", "_");
            if (distfname == null) {
                Log.Error("bad file name");
                return null;
            }
            File.WriteAllText(distfname,sb.ToString());

            // save indeivdual graphs

            int pid = 1;
            var graphs = new List<ExcelGraph>();
            foreach (var partition in partitions.Where(pt => pt.Value.Count > minimumSubgraphSize).OrderByDescending(a=> a.Value.Count)) {
                var newfname = Path.ChangeExtension(fileName, ".__" + pid + "_" + partition.Value.Count + ".graphml")?.Replace(".__", "_");
                if (newfname == null) {
                    Log.Error("bad file name");
                    return null;
                }
                Log.Info("writing " + newfname);

                Dictionary<string, ExtractedCell> pnodes = partition.Value.ToDictionary(k => k,k=> nodes[k]);
                List<Tuple<ExtractedCell, ExtractedCell>> parcs = arcs.Where(a =>
                        partition.Value.Contains(a.Item1.ToString()) && partition.Value.Contains(a.Item2.ToString()))
                    .ToList();

                graphs.Add( new ExcelGraph(pid, partition.Value.Count, pnodes, parcs));


                var doc = GraphMlUtilities.SaveGraph(pnodes, parcs,
                    ExtractedCell.GetDataDictionary(),
                    new Dictionary<string, string> {{"weight", "double"}},
                    n => n.Key,
                    e => e.Item1 + "|" + e.Item2,
                    e => e.Item1.ToString(),
                    e => e.Item2.ToString(),
                    n => n.Value.GetData(),
                    e => new Dictionary<string, string>()
                );
                
                doc.Save(newfname);
                pid++;
            }

            // save a whole graph 
            {
                //partitions

                var newfname = Path.ChangeExtension(fileName, ".__AllPartitions_.graphml")?.Replace(".__", "_");
                if (newfname == null)
                {
                    Log.Error("bad file name");
                    return null;
                }
                Log.Info("writing " + newfname);

                Dictionary<string, ExtractedCell> pnodes = partitions.SelectMany(p => p.Value).ToDictionary(k => k, k => nodes[k]);
                var pdict = new Dictionary<string,int>();
                foreach (var partition in partitions) {
                    foreach (var node in partition.Value) {
                        pdict.Add(node,partition.Key);
                    }
                }

                List<Tuple<ExtractedCell, ExtractedCell>> parcs = arcs.Where(a=> pdict.ContainsKey(a.Item1.ToString()) && pdict.ContainsKey(a.Item2.ToString())) .ToList();

                graphs.Add(new ExcelGraph(0, pnodes.Count, pnodes, parcs));


                var nodeDictionary = ExtractedCell.GetDataDictionary();
                nodeDictionary.Add("Partition","int");

                var doc = GraphMlUtilities.SaveGraph(pnodes, parcs,
                    nodeDictionary,
                    new Dictionary<string, string> { { "weight", "double" } },
                    n => n.Key,
                    e => e.Item1 + "|" + e.Item2,
                    e => e.Item1.ToString(),
                    e => e.Item2.ToString(),
                    n => {
                         var d = n.Value.GetData();
                        d.Add("Partition", pdict[n.Key].ToString());
                        return d;
                    },
                    e => new Dictionary<string, string>()
                );

                doc.Save(newfname);
            }


            return graphs;
        }
        protected virtual Dictionary<int, ConcurrentBag<string>> CreatePartitions(out Dictionary<string, ExtractedCell> nodes, FunctionExtractor extractor, out List<Tuple<ExtractedCell, ExtractedCell>> arcs) {
            MakeNodesEdges(out nodes, extractor, out arcs);

            Dictionary<int, ConcurrentBag<string>> partitions = PartitionAlgorithms.PartitionOnArcs(nodes, arcs, n =>n.ToString() );
            return partitions;
        }

        protected static void MakeNodesEdges(out Dictionary<string, ExtractedCell> nodes, FunctionExtractor extractor, out List<Tuple<ExtractedCell, ExtractedCell>> arcs) {
            nodes = extractor.Evaluations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            arcs = new List<Tuple<ExtractedCell, ExtractedCell>>();


            foreach (var cell in nodes) {
                foreach (var reference in cell.Value.References) {
                    arcs.Add(new Tuple<ExtractedCell, ExtractedCell>(cell.Value, nodes[reference.Cell.ToString()]));
                }
            }
        }
    }
}
