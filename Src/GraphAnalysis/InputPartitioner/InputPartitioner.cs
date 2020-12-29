using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Algorithms;
using ExcelInterop.Domain;
using log4net;
using ParseTreeExtractor.Domain;

namespace GraphAnalysis.InputPartitioner {
    public static class InputPartitioner {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

       public class InputCell : IComparable<InputCell> {
            public string Id { get; }
            public Node Node { get; }
            public ExcelAddress Address { get; }

            public InputCell(string id, Node node, ExcelAddress address) {
                Id = id;
                Node = node;
                Address = address;
            }

            public int CompareTo(InputCell other) {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                // compare by sheet
                var sheetCompare = string.Compare(Address.WorkSheet, other.Address.WorkSheet, StringComparison.Ordinal);
                if (sheetCompare != 0) return sheetCompare;
                // search direction
                // then by row
                var row = Address.IntRow;
                var rowO = other.Address.IntRow;
                if (row < rowO) return -1;
                if (row > rowO) return 1;

                // then by column
                var col = Address.IntCol;
                var colO = other.Address.IntCol;
                if (col < colO) return -1;
                if (col > colO) return 1;
                return 0;
            }

            public override string ToString() => Id;
        }

        public static List<LabeledInput> PartitionInputs(SpreadGraph graph, Extraction extraction) {
            // inputs are cells which do not contain references 
            // labels are inputs which are not referenced 
            var cells = graph.Nodes.Where(n => n.Value.NodeType == "Cell").ToList();
            var referencedCells = cells.Where(c=> c.Value.LinkedFrom.Any()).ToList();
            var labels = cells.Where(c => !c.Value.LinkedFrom.Any()).ToList();

            Dictionary<string, Node> inputs = new Dictionary<string, Node>();
          //  var unreferencedCells = cells.Where(c => !c.Value.LinkedFrom.Any());
            foreach (var cell in referencedCells) {
                var cellGraph = GraphAlgorithms.WalkUntilTrue(cell.Value,
                    c => c.LinksTo,
                    c => c.NodeType != "Cell",
                    c => true).Select(c => c.Id);
                var hasReferences = cellGraph.Any(c => graph.Nodes[c].NodeType == "Reference");

                // check cell has contents 
                var excelAddress = new ExcelAddress(cell.Key);
                if (extraction.CellFormulas.ContainsKey(excelAddress) && !string.IsNullOrWhiteSpace(extraction.CellFormulas[excelAddress])
                    && !hasReferences) {
                    inputs.Add(cell.Key, cell.Value);
                }
            }

            var inputList = inputs.Select(i => new InputCell(i.Key, i.Value, new ExcelAddress(i.Key))).ToList();

            // todo here we are trying the greedy rectangles algorithm - kill off the next line to make switch 
            var vectors = SortedListPartitioner(inputList, extraction);
            var outputs = Vectorizer.Vectorize(inputList, o => o.Address);
            CompareApproaches(vectors, outputs);
            vectors = outputs.Select(v => v.Item1).ToList();
            

            var labeledVectors = SeekLabels(vectors, labels,extraction);
            
            foreach (var input in labeledVectors) {
                input.AcquireValues(extraction);
            }

            return labeledVectors;
        }

        private static void CompareApproaches(List<ExcelAddress> vectors,
            List<Tuple<ExcelAddress, List<InputCell>>> outputs) {

            foreach (var greedy in outputs.Where(o => !vectors.Contains(o.Item1))) {
                Log.Info("GreedyFound " + greedy.Item1);
            }

            foreach (var vec in vectors.Where(v => !outputs.Any(o => o.Item1 == v))) {
                Log.Info("Greedy DIDNT Find " + vec);
            }

        }

        private static List<ExcelAddress> SortedListPartitioner(List<InputCell> inputList, Extraction extraction) {
            var orderedInputs = inputList.OrderBy(i => i).ToList(); // column major

            var arrays = orderedInputs.ContiguousPartitioner((a, b) => {
                if (a.First().Address.WorkSheet != b.Address.WorkSheet) return false;
                int p = a.Count - 1;
                int rGap = 0;
                while (p >= 0 && rGap <= 1) {
                    /* column major list then stop listening 
                    var rowGap = b.Address.IntRow - a[p].Address.IntRow;
                    colGap = b.Address.IntCol - a[p].Address.IntCol;
                    if (rowGap>=0 && rowGap <= 1 && colGap <= 1  && !(colGap == 1 && rowGap ==1)) {
                        // first and last conditions force rectangular and not ragged tables
                        return true;
                    }
                    */
                    // Row major
                    var cGap = b.Address.IntCol - a[p].Address.IntCol;
                    rGap = b.Address.IntRow - a[p].Address.IntRow;
                    if (cGap >= 0 && cGap <= 1 && rGap <= 1 && !(rGap == 1 && cGap == 1)) {
                        // first and last conditions force rectangular and not ragged tables
                        return true;
                    }

                    p--;
                }

                return false;
            });

            var vectors = CellsToRanges(arrays, extraction);
            return vectors;
        }

        private static List<LabeledInput> SeekLabels(List<ExcelAddress> vectors, List<KeyValuePair<string, Node>> cells,
            Extraction extraction) {

            List<LabeledInput> res = new List<LabeledInput>();
            var labelMap = cells.ToDictionary(d => new ExcelAddress(d.Value.Id), d => d.Value);
            int maxOffset = 5;

            CellLabel SeekLabels(Direction d, ExcelAddress testCell, int dimension, Func<ExcelAddress, ExcelAddress> searchFunc,
                Func<ExcelAddress, ExcelAddress> arrayFunc, bool reverse = false) {
                for (int offset = 1; offset <= maxOffset; offset++) {
                    // test if the whole vector has inputs or not?
                    testCell = searchFunc(testCell);
                    var labels = new List<ExcelAddress> {testCell};
                    for (int h = 1; h < dimension; h++) {
                        labels.Add(arrayFunc(labels.Last()));
                    }

                    if (labels.All(labelMap.ContainsKey)) {
                        //if (labels.Count == 1 && double.TryParse(cellValues[labels.First()], out _)) {
                        //    continue; // do not allow single numeric labels for an axis... 
                        //}

                        //todo could seek partial
                            if (reverse) {
                                // ensure canonical ordering
                                labels.Reverse();
                            }

                            var address = TestOrderedRange(labels,extraction);
                            return address == null ? null : new CellLabel(d,address);
                    }
                }

                return null;
            }

            foreach (var vector in vectors) {
                // find width and height of vector
                // search left and up for matching cells which are inputs. 
                var dims = vector.RangeDimensions();

                var input = new LabeledInput(vector,
                    SeekLabels(Direction.Left, vector.RangeTopLeftCell(), dims.Item2, c => c.OneLeft(), c => c.OneDown()),
                    SeekLabels(Direction.Top,vector.RangeTopLeftCell(), dims.Item1, c => c.OneUp(), c => c.OneRight()),
                    SeekLabels(Direction.Bottom, vector.RangeBottomRightCell(), dims.Item1, c => c.OneDown(), c => c.OneLeft(), true),
                    SeekLabels(Direction.Right, vector.RangeBottomRightCell(), dims.Item2, c => c.OneRight(), c => c.OneUp(), true)
                );

                #region Seek meta label

                input.SeekMetaLabel(labelMap, extraction, Direction.Top, Direction.Left);
                input.SeekMetaLabel(labelMap, extraction, Direction.Top, Direction.Right);
                input.SeekMetaLabel(labelMap, extraction, Direction.Bottom, Direction.Left);
                input.SeekMetaLabel(labelMap, extraction, Direction.Bottom, Direction.Right);
                #endregion

                res.Add(input);
            }

            return res;
        }

        /// <summary>
        /// transform a list of cells into Ranges
        /// requires complete rectangles - others will be dropped. 
        /// </summary>
        /// <param name="arrays"></param>
        /// <param name="extraction"></param>
        /// <returns></returns>
        private static List<ExcelAddress> CellsToRanges(List<List<InputCell>> arrays, Extraction extraction) {
            List<ExcelAddress> vectors = new List<ExcelAddress>(); 
            foreach (var array in arrays) {// todo here we can skip out single cell ranges...
                var res = TestOrderedRange( array.Select(a => a.Address).ToList(),extraction);
                if (res != null) {
                    vectors.Add(res);
                }
            }

            return vectors;
        }

        private static ExcelAddress TestOrderedRange(List<ExcelAddress> cells, Extraction extraction) {
            return TestRange(cells.First(), cells.Last(), cells,extraction);
        }

        private static ExcelAddress TestRange(ExcelAddress topLeft, ExcelAddress bottomRight, List<ExcelAddress> cells,
            Extraction extraction) {
            var range = new ExcelAddress(topLeft.WorkSheet,
                topLeft.Column() + topLeft.Row() + ":" + bottomRight.Column() + bottomRight.Row());
            if (topLeft.IntRow > bottomRight.IntRow) {
                Log.Warn("found incomplete input partition");
                return null;
            }

            // acid test
            bool completeRange = true;
            bool completeRangeBarEmpties = true;
            foreach (var cell in ExcelAddress.ExpandRangeToExcelAddresses(range)) {
                if (!cells.Contains(cell)) {
                    completeRange = false;
                    // do the missing cells actually exist? 
                    if (!extraction.CellNodes.ContainsKey(cell)) {
                        completeRangeBarEmpties = false;
                    }
                    
                }
            }

            if (completeRange) {
                Log.Info("found input partition " + range);
                return range;
            }
            else {
                if (completeRangeBarEmpties) {
                    return range;
                }

                Log.Warn("found incomplete input partition " + range);
                
                // todo cut range back so it has fewer columns 
                
                return null;
            }
          }

        public static void PrintInputs(List<LabeledInput> inputs, string fName) {
            File.WriteAllText(Path.ChangeExtension(fName, "_.inputs.csv")?.Replace("._", "_") ?? throw new ArgumentException("no filename",nameof(fName)),
                string.Join(Environment.NewLine, inputs));
            foreach (var input in inputs) {
                Log.Info("Found" + input);
            }
        }
    }
}
