using System;
using System.Collections.Generic;
using System.Linq;
using ExcelInterop.Domain;

namespace GraphAnalysis.InputPartitioner {
    public static class Vectorizer {
        public static List<Tuple<ExcelAddress,List<T>>> Vectorize<T>(List<T> objs, Func<T,ExcelAddress> addresser) {
            // a greedy rectangle algorithm from https://www.microsoft.com/en-us/research/uploads/prod/2018/10/sarkar_2018_calcview.pdf
            /*
             *  maximal rectangular ranges (called ‘blocks’) are detected using a greedy flood filling operation:
             * the top-left cell in the class is chosen to ‘seed’ the block.
             * The cell to the right of the seed is checked;
             * if it belongs to the same class,
             * then the block is grown to include it.
             * This is repeated until the block has achieved a maximal left-right extent.
             * The block is now grown vertically by checking
             * if the corresponding cells in the row below are also part of the equivalence class.
             * Once it can no longer be grown vertically,
             * this maximal block is then ‘removed’ from the equivalence class.
             * A new top-left seed is picked and grown,
             * and the process is repeated until all the cells in the equivalence class have been assimilated as part of a block
             */

            //todo this breaks in the case that:
            // A B
            // A _
            // A _

            // as AB will become a vector rather than AAA
            

            List<Tuple<ExcelAddress, List<T>>> addressedResults = new List<Tuple<ExcelAddress, List<T>>>();

            var d = new SortedDictionary<ExcelAddress,T>(new CellComparer());
            foreach (var o in objs) {
                d.Add(addresser(o),o);
            }

            while (d.Any()) {
                // take top left
                ExcelAddress p = d.First().Key;
                ExcelAddress topLeft = p;
                ExcelAddress bottomRight = p;
                List <T> block = new List<T>();
                int width = 0;

                while (d.ContainsKey(p)) { // work down rows
                    // find a row
                    List<ExcelAddress> row = new List<ExcelAddress>();
                    ExcelAddress next = p; 
                    while (d.ContainsKey(next)) { // but first work across columns 
                        row.Add(next);
                        next = next.OneRight();
                    }

                    // only count full rows after the first row
                    if (row.Count == (width != 0 ? width : width = row.Count)) {
                        // add it to the block
                        foreach (var r in row) {
                            block.Add(d[r]);
                            d.Remove(r);
                            bottomRight = r;
                        }
                    }
                    // start next row
                    p = p.OneDown();
                }

                // now lets find the range of these blocks
                addressedResults.Add(new Tuple<ExcelAddress, List<T>>(
                    new ExcelAddress(topLeft.WorkSheet,
                        topLeft.Column() + topLeft.Row() + ":" + bottomRight.Column() + bottomRight.Row()),
                    block
                ));
            }

            return addressedResults;
        }
    }
}