using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Algorithms;
using ExcelInterop.Domain;
using log4net;
using ParseTreeExtractor.Domain;

namespace GraphAnalysis.InputPartitioner {
    /// <summary>
    /// enables groups of cells to be tested for reference isomorphism
    /// uses relative offsets for references 
    /// </summary>
    public static class CellReferenceIsomorph {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public class RefMatrix {
            public string id { get; }
            public Node cell { get; }
            public List<Node> refs { get; }
            public List<ExcelAddress> addresses { get; }
            public List<RelativeReference> relativeRefs { get; }
            public List<ExcelAddress> ranges { get; }

            public RefMatrix(string id, Node cell, List<Node> refs, List<ExcelAddress> addresses, List<RelativeReference> relativeRefs, List<ExcelAddress> ranges) {
                this.id = id;
                this.cell = cell;
                this.refs = refs;
                this.addresses = addresses;
                this.relativeRefs = relativeRefs;
                this.ranges = ranges;
            }

            public override string ToString() {
                return $"{{ id = {id}, cell = {cell}, refs = {refs}, addresses = {addresses}, relativeRefs = {relativeRefs}, ranges = {ranges} }}";
            }

            public override bool Equals(object value) {
                var type = value as RefMatrix;
                return (type != null) && EqualityComparer<string>.Default.Equals(type.id, id) && EqualityComparer<Node>.Default.Equals(type.cell, cell) && EqualityComparer<List<Node>>.Default.Equals(type.refs, refs) && EqualityComparer<List<ExcelAddress>>.Default.Equals(type.addresses, addresses) && EqualityComparer<List<RelativeReference>>.Default.Equals(type.relativeRefs, relativeRefs) && EqualityComparer<List<ExcelAddress>>.Default.Equals(type.ranges, ranges);
            }

            public override int GetHashCode() {
                int num = 0x7a2f0b42;
                num = (-1521134295 * num) + EqualityComparer<string>.Default.GetHashCode(id);
                num = (-1521134295 * num) + EqualityComparer<Node>.Default.GetHashCode(cell);
                num = (-1521134295 * num) + EqualityComparer<List<Node>>.Default.GetHashCode(refs);
                num = (-1521134295 * num) + EqualityComparer<List<ExcelAddress>>.Default.GetHashCode(addresses);
                num = (-1521134295 * num) + EqualityComparer<List<RelativeReference>>.Default.GetHashCode(relativeRefs);
                return (-1521134295 * num) + EqualityComparer<List<ExcelAddress>>.Default.GetHashCode(ranges);
            }
        }

        public static List<List<string>> Test(IEnumerable<string> vectorCells, SpreadGraph graph, bool referencedBy = false) {
            var references = GenerateRefMatrices(vectorCells, graph, referencedBy);
            
            // partition comparing the relative references... 
            
            var partitioned = PartitionAlgorithms.PartitionWithTest(references, i => i.id,
                (dict, a, b) => {
                    var l = dict[a.First()];
                    var r = dict[b.First()];

                    // note that we should be concerned with the order of reference equality
                    // e.g. a-b != b-a although they are structurally isomorphic
                    return l.relativeRefs.Count == r.relativeRefs.Count &&
                        l.relativeRefs.Zip(r.relativeRefs, (x, y) => x.Equals(y)).All(t => t);
                    //return l.relativeRefs.All( rr => r.relativeRefs.Contains(rr));
                }
            );

            return partitioned.Select(p => p.Value.ToList()).ToList();
        }

        private static List<RefMatrix> GenerateRefMatrices(IEnumerable<string> vectorCells, SpreadGraph graph, bool referencedBy = false, bool incRanges = false) {
            var references = vectorCells.Select(c => {
                var cell = graph.Nodes[c];
                ExcelAddress address = new ExcelAddress(c);

                bool IsRef(Node n) =>
                    n.NodeType == "Cell" || (incRanges && ( n.NodeType == "Range" || n.NodeType == "NamedRange"));
                //if we just keep following the graph Range and Named Ranges become cells :-)

                var refs = GraphAlgorithms.WalkUntilTrue(cell,
                    n => referencedBy ? n.LinkedFrom : n.LinksTo, n => !IsRef(n), IsRef);
                var addresses = refs.Select(r => new ExcelAddress(r.Id)).ToList();
                return new RefMatrix(c, cell, refs, addresses.Where(a => !a.IsRange()).ToList(), addresses.Where(a=> !a.IsRange()).Select(a => a.RelativeFrom(address)).ToList(),
                    addresses.Where(a => a.IsRange()).ToList());
            }).ToList();
            return references;
        }

        public static Node CreateVectorisedVersion(string key, SpreadGraph graph) {

            // todo use the first half of this to make a vectorized version of the above. 
            var range = new ExcelAddress(key);
            var vectorCells = ExcelAddress.ExpandRangeToExcelAddresses(range).Select(r => r.ToString()).ToList();

            var references = GenerateRefMatrices(vectorCells, graph, false, true);

            var template = references.First();
            var start = template.cell;
            
            // create vectors maps 

            Dictionary<ExcelAddress,ExcelAddress> vectorReferenceTranslation = new Dictionary<ExcelAddress, ExcelAddress>();
            // when we encounter a reference we must find its index and then pull all of the indexes out of the other cells. 
            // these references will have to be ordered (if ranges by TL cell)
            // if the reference is always identical then we can keep it as is. 
            // if we can form a contigeous range out of these then we can proceed and replace 
            // if we cannot then we need to figure out how to form a reference - e.g. referencing every other cell or every 2nd... 

            for (var i = 0; i < template.addresses.Count; i++) {
                var templateAddress = template.addresses[i];
                if (references.All(a => a.addresses[i] == templateAddress)) { 
                    // static reference 
                    vectorReferenceTranslation[templateAddress]=templateAddress;
                    continue;
                }

                var allRefs = references.Select(r => r.addresses[i]).ToList();
                var res = Vectorizer.Vectorize(allRefs, a => a);
                if (res.Count != 1) {
                    Log.Warn("Failed to vectorize - a vectors references are not contigeous");
                }
                vectorReferenceTranslation[templateAddress] =res.First().Item1;
            }

            for (var i = 0; i < template.ranges.Count; i++) {
                var templateRange = template.ranges[i];
                if (references.All(a => a.ranges[i] == templateRange))
                {
                    // static reference 
                    vectorReferenceTranslation[templateRange] = templateRange;
                    continue;
                }
                var allRefs = references.SelectMany(r => ExcelAddress.ExpandRangeToExcelAddresses(r.ranges[i])).ToList();
                var res = Vectorizer.Vectorize(allRefs, a => a);
                if (res.Count != 1)
                {
                    Log.Warn("Failed to vectorize - a vectors references are not contigeous");
                }
                vectorReferenceTranslation[templateRange] = res.First().Item1;
            }

            // walk the first tree creating a clone ... 
            // every time we encounter a reference node then find all of the corresponding references 
            vectorReferenceTranslation.Add(new ExcelAddress(template.id), new ExcelAddress(key)); // rename the tree
            Node vector = CloneTree(template.cell, vectorReferenceTranslation,true);

            // todo the ids of the clone tree need to be reset
            return vector;

            // todo the problem with this is then that its is not guaranteed that the range references we create will be in the refMap ... 
            // todo so the solution is for the refMAp to become of a list of things which can test and accept references
        }

        private static Node CloneTree(Node cell, Dictionary<ExcelAddress, ExcelAddress> vectorReferenceTranslation, bool root = false) {
            // todo is it actually cloning that we want todo? would we be better to edit the spread-graph? 
            // in an ideal world we'd have a hypergraph abstraction and be able to figure out what edges cross the boundary into a hyperedge. 

            Node clone = null;

            if (cell is RangeNode r) {
                if (vectorReferenceTranslation.TryGetValue(new ExcelAddress(r.Id), out var address)) {
                    clone = new RangeNode(address.ToString(),address.WorkSheet,address.RangeCellCount);
                    if (!root) return clone;
                }
                else {
                    Log.Warn("Failed to translate Range");
                }
            }

            if (cell is CellNode c) {
                if (vectorReferenceTranslation.TryGetValue(new ExcelAddress(c.Id), out var address)) {
                    if (address.IsRange()) {
                        clone= new RangeNode(address.ToString(), address.WorkSheet, address.RangeCellCount);
                        if (!root) return clone;
                    }
                    else {
                        clone = new CellNode {
                            ColNo = address.IntCol, Id = address.ToString(), NodeType = "Cell", RowNo = address.IntRow,
                            Sheet = address.WorkSheet
                        };
                        if (!root) return clone;
                    }
                }
                else {
                    Log.Warn("Failed to translate Cell");
                }
            }

            // note named ranges must just be cloned...
            if (clone == null) {
                clone = cell.Clone() as Node;
            }

            if (clone == null) {
                Log.Error("failed to clone node");
                return null;
            }

            foreach (var child in cell.LinksTo) {
                var twin = CloneTree(child,vectorReferenceTranslation);
                clone.LinksTo.Add(twin);
            }

            return clone;
        }
    }
}
