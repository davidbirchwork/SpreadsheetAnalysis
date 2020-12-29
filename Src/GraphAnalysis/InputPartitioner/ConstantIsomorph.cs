using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Algorithms;
using ExcelInterop.Domain;
using log4net;
using ParseTreeExtractor.Domain;

namespace GraphAnalysis.InputPartitioner {
    /// <summary>
    /// enables groups of cells to be tested for having the same set of constants 
    /// </summary>
    public static class ConstantIsomorph {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<List<string>> Test(IEnumerable<string> vectorCells, SpreadGraph graph) {

            var references = vectorCells.Select(c => {
                var cell = graph.Nodes[c];
                ExcelAddress address = new ExcelAddress(c);
                bool IsConstant(Node n) => n.NodeType == "NumberToken" || n.NodeType == "TextToken";
                var refs = GraphAlgorithms.WalkUntilTrue(cell, n => n.LinksTo, n => n.NodeType != "Cell", IsConstant);
                var constants = refs.Select(r => r.Label).ToList();
                return new {
                    id = c,
                    cell, refs, constants
                };
            }).ToList();
            
            // partition comparing the relative references... 
            
            var partitioned = PartitionAlgorithms.PartitionWithTest(references, i => i.id,
                (dict, a, b) => {
                    var l = dict[a.First()];
                    var r = dict[b.First()];

                    // note that we should be concerned with the order of equality
                    // because we cannot be sure of equivalence under commutativity. 
                    return l.constants.Count == r.constants.Count &&
                        l.constants.Zip(r.constants, (x, y) => x.Equals(y)).All(t => t);                    
                }
            );

            return partitioned.Select(p => p.Value.ToList()).ToList();

        }
    }
}
