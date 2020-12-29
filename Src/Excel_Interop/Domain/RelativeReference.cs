using System.Collections.Generic;

namespace ExcelInterop.Domain {
    public class RelativeReference {
        public ExcelAddress Target { get; }
        public bool DifferentSheet { get; }
        public int RowDiff { get; }
        public int ColDiff { get; }

        public RelativeReference(ExcelAddress target, bool differentSheet, int rowDiff, int colDiff) {
            Target = target;
            this.DifferentSheet = differentSheet;
            this.RowDiff = rowDiff;
            this.ColDiff = colDiff;
        }

        public override string ToString() {
            return $"{{ differentSheet = {DifferentSheet}, rowDiff = {RowDiff}, colDiff = {ColDiff} }}";
        }

        public override bool Equals(object value) {
            return value is RelativeReference type
                 && ( type.Target == this.Target || //note this line is really important
                   EqualityComparer<bool>.Default.Equals(type.DifferentSheet, DifferentSheet) &&
                   EqualityComparer<int>.Default.Equals(type.RowDiff, RowDiff) &&
                   EqualityComparer<int>.Default.Equals(type.ColDiff, ColDiff));
        }

        public override int GetHashCode() {
            int num = 0x7a2f0b42;
            num = (-1521134295 * num) + EqualityComparer<bool>.Default.GetHashCode(DifferentSheet);
            num = (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(RowDiff);
            return (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(ColDiff);
        }
    }
}