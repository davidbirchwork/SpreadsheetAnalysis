using System;
using System.Collections.Generic;
using System.IO;
using NCalcExcel;

namespace ExcelExtractor {     
    public class CellMap :IEqualityComparer<CellMap>, IComparable<CellMap>,IEquatable<CellMap> {
        public string Sheet;
        public string Cell;
        public string FriendlyName;        
        public string LowFormula; // -20 *0.90
        public string HighFormula; // +20 *1.10

        public string GetName() {
            return this.Sheet + "!" + this.Cell;
        }
        
        private string _defaultvalue;
        public void PutDefaultValue(string value) {
            this._defaultvalue = value;
        }
        public string AccessDefaultValue() {
            return this._defaultvalue;
        }

        public string EvalLow(ExpressionFactory factory) {
            return (factory.Evaluate(this.LowFormula, new Dictionary<string, object>() {{"value", decimal.Parse(this.AccessDefaultValue())}})).ToString();
        }

        public string EvalHigh(ExpressionFactory factory) {
            return (factory.Evaluate(this.HighFormula, new Dictionary<string, object>() { { "value", decimal.Parse(this.AccessDefaultValue()) } })).ToString();
        }

        public CellMap() {
            
        }

        public CellMap(string[] values) {
            this.Sheet = values[0];
            this.Cell = values[1];
            this.FriendlyName = values[2];            
            this.LowFormula = values[3];
            this.HighFormula = values[4];
            this._defaultvalue = values[5];
        }

        public string ToTSV() {
            return this.Sheet + "\t" + this.Cell + "\t" + this.FriendlyName + "\t" + this.HighFormula + "\t" +
                   this.LowFormula + "\t" + this._defaultvalue;
        }

        public static CellMap[] ReadTSV(string fileName) {
            List<CellMap> cells = new List<CellMap>();
            string[] file = File.ReadAllLines(fileName);
            for (int i = 1; i < file.Length; i++) {
                cells.Add(new CellMap(file[i].Split('\t')));
            }
            return cells.ToArray();
        }

        #region Implementation of IEqualityComparer<in CellMap>

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(CellMap x, CellMap y) {
            if (x == null || y == null) return x == y;
            return (x.Sheet + "!" + x.Cell).Equals(y.Sheet + "!" + y.Cell);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="x">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="x"/> is a reference type and <paramref name="x"/> is null.</exception>
        public int GetHashCode(CellMap x) {
            return (x.Sheet + "!" + x.Cell).GetHashCode();
        }

        #endregion

        #region Implementation of IComparable<in CellMap>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(CellMap other) {
            return this.GetHashCode().CompareTo(other.GetHashCode());
        }

        #endregion

        #region Implementation of IEquatable<CellMap>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(CellMap other) {
            return other != null && this.GetHashCode().Equals(other.GetHashCode());
        }

        #endregion
    }
}
