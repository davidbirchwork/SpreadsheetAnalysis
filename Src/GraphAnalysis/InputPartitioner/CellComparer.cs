using System;
using System.Collections;
using System.Collections.Generic;
using ExcelInterop.Domain;

namespace GraphAnalysis.InputPartitioner {
    /// <summary>
    /// Order addresses by Sheet, Row, Col
    /// (or Col the row)
    /// </summary>
    public class CellComparer : IComparer, IComparer<ExcelAddress> {

        private readonly bool _rowMajor;
        
        public CellComparer(bool colMajor = false) {
            _rowMajor = !colMajor;
        }

        public int Compare(object x, object y) => Compare(x as ExcelAddress, y as ExcelAddress);

        public int Compare(ExcelAddress a, ExcelAddress b) {
            if (ReferenceEquals(a, b)) return 0;
            if (ReferenceEquals(null, b)) return 1;
            // compare by sheet
            var sheetCompare = string.Compare(a.WorkSheet, b.WorkSheet, StringComparison.Ordinal);
            if (sheetCompare != 0) return sheetCompare;
            // search direction
            if (_rowMajor) {
                // then by row
                var row = a.IntRow;
                var rowO = b.IntRow;
                if (row < rowO) return -1;
                if (row > rowO) return 1;
            }

            // then by column
            var col = a.IntCol;
            var colO = b.IntCol;
            if (col < colO) return -1;
            if (col > colO) return 1;

            if (!_rowMajor) { // yeah i can't think of a nicer way to avoid the duplication 
                // then by row
                var row = a.IntRow;
                var rowO = b.IntRow;
                if (row < rowO) return -1;
                if (row > rowO) return 1;
            }

            return 0;
        }
    }
}