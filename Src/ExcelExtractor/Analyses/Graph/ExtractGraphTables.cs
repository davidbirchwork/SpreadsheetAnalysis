using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using ExcelExtractor.Domain;
using ExcelInterop.Domain;

namespace ExcelExtractor.Analyses.Graph {
    /// <summary>
    /// Goal here is to extract adjacent partitions 
    /// </summary>
    /// <seealso cref="ExcelExtractor.Analyses.Graph.ExtractGraphComponents" />
    public class ExtractGraphTables : ExtractGraphComponents
    {
        protected override Dictionary<int, ConcurrentBag<string>> CreatePartitions(
            out Dictionary<string, ExtractedCell> nodes, FunctionExtractor extractor,
            out List<Tuple<ExtractedCell, ExtractedCell>> arcs)
        {

            MakeNodesEdges(out nodes, extractor, out arcs);
            //todo dont just filter out ranges
            nodes = nodes.Where(n => !n.Value.IsRange).ToDictionary(n=> n.Key, n=> n.Value);
            var mynodes = nodes;



            Dictionary<string, CellName> map = new Dictionary<string, CellName>();
            foreach (var referance in mynodes.Select(n => new {
                id = n.Value.RcCell,
                Cell = new CellName(n.Value.RcCell)} )) {
                if (!map.ContainsKey(referance.id)) {
                    map.Add(referance.id, referance.Cell);
                }
            }


            // merge if all are on the same sheet
            // merge if any of the cells in the two sets are contigeious

            var partitions = PartitionAlgorithms.PartitionWithTest(nodes, node => node.Key,
                (dict,setA, setB) => {
                    if (mynodes[setA.First()].Sheet != mynodes[setB.First()].Sheet) {
                        return false;
                    }

                    var setACells = setA.Select(a => map[mynodes[a].RcCell]).ToList();
                    var setBCells = setB.Select(a => map[mynodes[a].RcCell]).ToList();
                    int cells = setBCells.Count + setBCells.Count;

                    int minRow_A = setACells.Min(c => c.RowNum);
                    int minRow_B = setBCells.Min(c => c.RowNum);
                    int maxRow_A = setACells.Max(c => c.RowNum);
                    int maxRow_B = setBCells.Max(c => c.RowNum);

                    int minCol_A = setACells.Min(c => c.ColumnNumber());
                    int minCol_B = setBCells.Min(c => c.ColumnNumber());
                    int maxCol_A = setACells.Max(c => c.ColumnNumber());
                    int maxCol_B = setBCells.Max(c => c.ColumnNumber());

                    // intersection test 
                    // https://stackoverflow.com/questions/306316/determine-if-two-rectangles-overlap-each-other?noredirect=1&lq=1
                    //if (RectA.X1 < RectB.X2 &&
                    //    RectA.X2 > RectB.X1 &&
                    //    RectA.Y1 > RectB.Y2 &&
                    //    RectA.Y2 < RectB.Y1)

                    //     RectA.X1 < RectB.X2  && 
                    //     RectB.X1 < RectA.X2  && - flip
                    //     RectB.Y2 < RectA.Y1  && - flip
                    //     RectA.Y2 < RectB.Y1  &&

                    // a < b  same as b-a >0  same as a-b < 0 
                    // <0  so adjacent boundary test is b-a<=1

                    //     RectA.X1 - RectB.X2 <= 1 && 
                    //     RectB.X1 - RectA.X2 <= 1 && 
                    //     RectB.Y2 - RectA.Y1 <= 1 && 
                    //     RectA.Y2 - RectB.Y1 <= 1 &&


                    //     minRow_A - maxRow_B <= 1 && 
                    //     minRow_B - maxRow_A <= 1 && 
                    //     maxCol_B - minCol_A <= 1 && 
                    //     maxCol_A - minCol_B <= 1 &&

                    var b = maxCol_B - minCol_A <=1 &&
                            maxCol_A - minCol_B <=1 &&
                            minRow_A - maxRow_B <=1 &&
                            minRow_B - maxRow_A <=1;

                    if (setBCells.Count > 10 && setACells.Count > 10  && !b) {
                        Console.WriteLine("bad");
                    }

                    return  b;

                });

            foreach (var partition in partitions) {
                var minRow = partition.Value.Min(v => map[mynodes[v].RcCell].RowNum);
                var maxRow = partition.Value.Max(v => map[mynodes[v].RcCell].RowNum);
                var minCol = partition.Value.Min(v => map[mynodes[v].RcCell].ColumnNumber());
                var maxCol = partition.Value.Max(v => map[mynodes[v].RcCell].ColumnNumber());
                Log.Info("partition of "+partition.Value.Count+"cells on sheet"+partition.Value.First()+" rows "+minRow+" - "+maxRow + " cols "+minCol+" - "+maxCol);
            }

            return partitions;

        }
    }
}