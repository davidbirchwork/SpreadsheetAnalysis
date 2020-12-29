using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace ExcelInterop.Domain {
    [Serializable]
    public class ExcelAddress : ICloneable, IEquatable<ExcelAddress> {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string WorkSheet { get; }
        public string CellReference { get; }

        private CellName _CellName;
        [JsonIgnore]
        public CellName CellName {
            get {
                if (this.IsRange()) return null;
                return _CellName ?? (_CellName = new CellName(this.CellReference));
            }
        }

        public string FullName => this.WorkSheet + "!" + this.CellReference;
        
        public ExcelAddress() :this("sheet1!A1") {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelAddress"/> struct.
        /// from a string "Sheet1!A1"
        /// </summary>
        /// <param name="cellName">Name of the cell.</param>
        public ExcelAddress(string cellName) {
            cellName = cellName.Replace("'", "");// remove quotes from cell names starting with numbers...
            string[] components = cellName.Split(new[] { '!' });
            if (components.Length != 2) {
                Log.Fatal("Cell names must be in the format Sheet!Cell");
            }
            this.WorkSheet = components[0];
            this.CellReference = components[1];
        }

        public ExcelAddress(string workSheet, string cellReference) {
            this.WorkSheet = workSheet;
            this.CellReference = cellReference;
        }

        #region Implementation of ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone() {
            return new ExcelAddress(this.WorkSheet, this.CellReference);
        }

        #endregion

        public override string ToString() {
            return this.FullName;
        }

        public override int GetHashCode() {
            unchecked {
                return ((CellReference != null ? CellReference.GetHashCode() : 0)*397) ^ (WorkSheet != null ? WorkSheet.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// returns the column of this instance of NULL if excel address is a range
        /// </summary>
        /// <returns></returns>
        public string Column() {
            try {
                return this.CellName.Column;
            } catch (Exception) {
                return null;
            }
        }

        /// <summary>
        /// returns the row of this instance of NULL if excel address is a range
        /// </summary>
        /// <returns></returns>
        public string Row() {
            try {
                return this.CellName.Row;
            } catch (Exception) {
                return null;
            }
        }

        public int IntRow {
            get {
                int r = 0;
                var rows = this.Row();
                if (rows != null && int.TryParse(rows, out r)) {
                    return r;
                }

                return -1;
            }
        }

        [JsonIgnore]
        public int IntCol => this.IsRange() ? 
            this.RangeTopLeftCell().IntCol
            : this.CellName.ColumnNumber();

        public int RangeCellCount => !this.IsRange() ? 1 : this.RangeDimensions().Item1 * this.RangeDimensions().Item2;

        #region static expand range methods:

        public static List<String> ExpandRangeToCellList(ExcelAddress address) {
            return ExpandRangeToExcelAddresses(address).Select(addr => addr.CellReference).ToList();
        }

        public static List<CellName> ExpandRangeToCellNames(ExcelAddress address) {
            return ExpandRangeToExcelAddresses(address).Select(addr => new CellName(addr.CellReference)).ToList();
        }

        public static List<ExcelAddress> ExpandRangeToExcelAddresses(ExcelAddress address) {           

            string[] split = address.CellReference.Replace("$", "").Split(':');

            return GetCellsBetween(address.WorkSheet, new CellName(split[0]), new CellName(split[1]));
        }

        public static List<ExcelAddress> GetCellsBetween(string workSheet, CellName topLeft, CellName bottomRight) {
            List<ExcelAddress> cells = new List<ExcelAddress>();
            CellName current = topLeft;

            if (topLeft.Row == bottomRight.Row && topLeft.Column == bottomRight.Column) {
                // only 1 cell
                cells.Add(new ExcelAddress(workSheet, current.ToString()));
            } else if (topLeft.Row == bottomRight.Row) {
                // one row

                while (current.Column != bottomRight.Column) {
                    // a single row selection
                    cells.Add(new ExcelAddress(workSheet, current.ToString()));
                    current = current.OneRight();
                }
                cells.Add(new ExcelAddress(workSheet, current.ToString()));

            } else if (topLeft.Column == bottomRight.Column) {
                // one column

                while (current.Row != bottomRight.Row) {
                    cells.Add(new ExcelAddress(workSheet, current.ToString()));
                    current = current.OneDown();
                }

                cells.Add(new ExcelAddress(workSheet, current.ToString()));

            } else {
                // a rectangle

                while (!(current.Row == bottomRight.Row && current.Column == bottomRight.Column)) {
                    CellName leftmost = current;
                    // a rectangle
                    while (current.Column != bottomRight.Column) {
                        cells.Add(new ExcelAddress(workSheet, current.ToString()));
                        current = current.OneRight();
                    }
                    cells.Add(new ExcelAddress(workSheet, current.ToString()));
                    if (current.Row != bottomRight.Row) {
                        current = leftmost.OneDown();
                    }
                }                
            }
            return cells;
        }

        #endregion

        public bool IsRange() {
            return this.CellReference.Contains(":");
        }

        /// <summary>
        /// Returns Width, Height of range
        /// </summary>
        /// <returns>Width, Height of range</returns>
        public Tuple<int, int> RangeDimensions() {
            if (!IsRange()) {
                return null;
            }

            string[] split = this.CellReference.Replace("$", "").Split(':');
            CellName topLeft = new CellName(split[0]);
            CellName bottomRight = new CellName(split[1]);

            int width = 1;
            int height = 1;

            CellName current = topLeft;            

            while (current.Column != bottomRight.Column) {
                width++;
                current = current.OneRight();
            }
            while (current.Row != bottomRight.Row) {
                height++;
                current = current.OneDown();
            }

            return new Tuple<int, int>(width, height);
        }

        public int CellsInRange() {
            var rangeDimensions = RangeDimensions();
            return IsRange() ? rangeDimensions.Item1 * rangeDimensions.Item2 : 1;
        }

        /// <summary>
        /// Returns the top left cell in a range
        /// </summary>
        /// <returns></returns>
        public CellName RangeTopLeft() {
            if (!IsRange()) {
                throw  new ArgumentOutOfRangeException("ExcelAddress is not a range "+this);
            }

            string[] split = this.CellReference.Replace("$", "").Split(':');
            return new CellName(split[0]);
            
        }

        public ExcelAddress RangeTopLeftCell() {
            return new ExcelAddress(this.WorkSheet,RangeTopLeft().ToString());
        }

        public CellName RangeBottomRight() {
            if (!IsRange()) {
                throw new ArgumentOutOfRangeException("ExcelAddress is not a range " + this);
            }

            string[] split = this.CellReference.Replace("$", "").Split(':');
            return new CellName(split[1]);
        }

        public ExcelAddress RangeBottomRightCell() {
            if (!this.IsRange()) {
                throw new ArgumentOutOfRangeException("ExcelAddress is not a range " + this);
            }

            return new ExcelAddress(this.WorkSheet, this.RangeBottomRight().ToString());
        }

        public static ExcelAddress CreateRange(string workSheet, CellName rangeTopLeft, int width, int height) {
            
            CellName bottomRight = new CellName(rangeTopLeft.ToString());
            for (int i = 0; i < width; i++) {
                bottomRight = bottomRight.OneRight();
            }
            for (int i = 0; i < height; i++) {
                bottomRight = bottomRight.OneDown();
            }

            return new ExcelAddress(workSheet,rangeTopLeft+":"+bottomRight);
        }

        #region equality        

        public bool Equals(ExcelAddress other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.CellReference, CellReference) && Equals(other.WorkSheet, WorkSheet);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ExcelAddress)) return false;
            return Equals((ExcelAddress) obj);
        }

        public static bool operator ==(ExcelAddress left, ExcelAddress right) {
            return Equals(left, right);
        }

        public static bool operator !=(ExcelAddress left, ExcelAddress right) {
            return !Equals(left, right);
        }

        #endregion

        public ExcelAddress OneLeft() {
            return new ExcelAddress(this.WorkSheet,this.CellName.OneLeft().ToString());
        }

        public ExcelAddress OneDown() {
            return new ExcelAddress(this.WorkSheet, this.CellName.OneDown().ToString());
        }

        public ExcelAddress OneUp() {
            return new ExcelAddress(this.WorkSheet, this.CellName.OneUp().ToString());
        }
        public ExcelAddress OneRight() {
            return new ExcelAddress(this.WorkSheet, this.CellName.OneRight().ToString());
        }

       public RelativeReference RelativeFrom(ExcelAddress givenOrigin) {
            var target = this.IsRange()? this.RangeTopLeftCell() :  this;
            var origin = givenOrigin.IsRange() ? givenOrigin.RangeTopLeftCell() : givenOrigin;
            bool differentSheet = target.WorkSheet != origin.WorkSheet;
            if (differentSheet) {
                origin = new ExcelAddress(target.WorkSheet,"A1");
            }

            var rowDiff = target.IntRow - origin.IntRow;
            var colDiff = target.IntCol - origin.IntCol;

            return new RelativeReference(target, differentSheet, rowDiff, colDiff);
        }

       public List<ExcelAddress> RangeRows() {
           if(!this.IsRange()) return  new List<ExcelAddress>();

           var topLeftCell = this.RangeTopLeftCell();
           var bottomRightCell = this.RangeBottomRightCell();
           var left = topLeftCell;
           var right = new ExcelAddress(this.WorkSheet, bottomRightCell.Column() + left.Row());

           
           List<ExcelAddress> rows = new List<ExcelAddress>();
           while (left.IntRow <= bottomRightCell.IntRow) {
               var row = new ExcelAddress(this.WorkSheet, left.CellName + ":" + right.CellName);
                rows.Add(row);
                left = left.OneDown();
                right = right.OneDown();
           }
           return rows;
       }

       public List<ExcelAddress> RangeCols() {
           if (!this.IsRange()) return new List<ExcelAddress>();

           var topLeftCell = this.RangeTopLeftCell();
           var bottomRightCell = this.RangeBottomRightCell();
           var top = topLeftCell;
           var bottom = new ExcelAddress(this.WorkSheet, top.Column() + bottomRightCell.Row());

            List<ExcelAddress> cols = new List<ExcelAddress>();
           while (top.IntCol <= bottomRightCell.IntCol){
               var col = new ExcelAddress(this.WorkSheet, top.CellName + ":" + bottom.CellName);
               cols.Add(col);
               top    = top.OneRight();
               bottom = bottom.OneRight();
           }
           return cols;
       }
    }
}
