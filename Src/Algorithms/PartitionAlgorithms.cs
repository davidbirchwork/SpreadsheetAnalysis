using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Algorithms {
    public static class PartitionAlgorithms {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);        

        public static Dictionary<int, ConcurrentBag<string>> PartitionOnArcs<TV>(IDictionary<string, TV> nodes,
            IEnumerable<Tuple<TV, TV>> arcs,Func<TV,string> nodeId) {
            Log.Info("about to start partitioning");

            ConcurrentDictionary<string, int> nodePartitionLabels = new ConcurrentDictionary<string, int>();
            Dictionary<int, ConcurrentBag<string>> partitions = new Dictionary<int, ConcurrentBag<string>>();
            int p = 0;
            foreach (var n in nodes) {
                nodePartitionLabels.AddOrUpdate(n.Key, k => p, (k, v) => p);
                partitions.Add(p, new ConcurrentBag<string> {n.Key});

                p++;
            }

            foreach (var arc in arcs) {
                var s = nodeId(arc.Item1);
                var t = nodeId(arc.Item2);
                if (nodePartitionLabels[s] != nodePartitionLabels[t]) {
                    int newPart = Math.Min(nodePartitionLabels[s], nodePartitionLabels[t]);
                    int oldPart = Math.Max(nodePartitionLabels[s], nodePartitionLabels[t]);
                    nodePartitionLabels.AddOrUpdate(s, k => newPart, (k, op) => Math.Min(op, newPart));
                    nodePartitionLabels.AddOrUpdate(t, k => newPart, (k, op) => Math.Min(op, newPart));

                    foreach (var node in partitions[oldPart]) {
                        nodePartitionLabels.AddOrUpdate(node, k => newPart, (k, op) => Math.Min(op, newPart));
                        partitions[newPart].Add(node);
                    }

                    partitions.Remove(oldPart);
                }
            }

            Log.Info("Finished Partitioning - Partitioned " + nodes.Count + " into " + partitions.Count +
                     " partitions");
            return partitions;
        }

        /// <summary>
        /// Partitions a set of objects with a supplied test for whether nodes are in the same partition
        /// Assumes greedy partitioning - if any pair of nodes from two partitions are linked then the whole partition will be merged
        /// todo implement a version which has proportional merge strategy - e.g. if 50% of one partition matches another then merge 
        /// </summary>
        /// <typeparam name="TV">vertex ttype</typeparam>
        /// <param name="nodes">The nodes</param>
        /// <param name="idFunc">The identifier function.</param>
        /// <param name="testInSamePartition">The test for whether two sets should be merged - must be symmetric (A,B) = (B,A)</param>
        /// <returns>the partition set</returns>
        public static Dictionary<int, ConcurrentBag<string>> PartitionWithTest<TV>(
            IEnumerable<TV> nodes, Func<TV, string> idFunc,
            Func<Dictionary<string, TV>, IEnumerable<string>, IEnumerable<string>, bool> testInSamePartition) {

            Dictionary<int, ConcurrentBag<string>> partitions = new Dictionary<int, ConcurrentBag<string>>();
            Dictionary<string, TV> idDict = nodes.ToDictionary(idFunc, n => n);
            // set everything into its own partition
            int p = 0;
            foreach (var v in idDict) {
                partitions.Add(p, new ConcurrentBag<string> {v.Key });
                p++;
            }
            
            bool change = true;
            int iteration = 0;

            while (change) {
                change = false;
                iteration++;

                List<int> ps = partitions.Keys.ToList();
                ps.Sort();

                foreach (var a in ps) {
                    //todo parallelise? ? ? 
                    if (!partitions.ContainsKey(a)) continue;
                    foreach (var b in ps) {
                        if (b >= a) break; // do lower triangle of matrix
                        if (!partitions.ContainsKey(b)) continue;
                        
                        if (!partitions.ContainsKey(a) || !partitions.ContainsKey(b))
                            continue; // not already been merged

                        if (testInSamePartition(idDict,partitions[a], partitions[b])) {
                            change = true;
                            int newPart = Math.Min(a, b);
                            int oldPart = Math.Max(a, b);


                            foreach (var node in partitions[oldPart]) {
                                partitions[newPart].Add(node);
                            }

                            partitions.Remove(oldPart);
                        }

                    }
                }

                if (iteration % 10 == 0) {
                    Log.Info("Partitioning iteration " + iteration);
                }

            }

            return partitions;
        }
    }
}
