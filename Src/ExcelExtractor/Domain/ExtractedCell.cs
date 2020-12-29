using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using ExcelInterop.Domain;
using LinqExtensions.Extensions;
using NCalcExcel.Domain;
using ExcelErrors = ExcelInterop.Domain.ExcelErrors;

namespace ExcelExtractor.Domain {



    [DebuggerDisplay("{this.SheetRC+'='+this.Formula}")]
    public class ExtractedCell : IComparable, IEquatable<ExtractedCell> {

        public bool IsRange { get; set; }

        public string Sheet { get; set; }
        public string RcCell { get; set; }

        private ExcelAddress Address => new ExcelAddress(this.Sheet, this.RcCell);

        public string SheetRC => Sheet + "!" + RcCell;

        /// <summary>
        /// Gets or sets the excel names - ie named cells in excel - one cell can have more than 1 name :-/
        /// </summary>
        /// <value>The excel names.</value>
        public List<string> ExcelNames { get; set; }

        public string Formula { get; set; }
        public bool IsFormula { get; set; }

        private string _value;

        public string Value {
            get {
                if (Double.TryParse(_value, out var vald)) {
                    return vald.ToString();
                }
                else {
                    return _value;
                }
            }
            set => _value = value;
        }

        public  List<CellReference> References { get; set; }

        public bool IsBlank
        {
            get { return Formula?.Equals("'BLANK'") ?? true; }
        }

        #region ctors        

        public ExtractedCell() {
            
        }

        public ExtractedCell(string sheet, string rcCell, string value,string formula,bool isFormula,bool isRange) {
            this.ExcelNames = new List<string>();
            this.References = new List<CellReference>();
            this.IsRange = isRange;
            this.Sheet = sheet;            
            this.RcCell = rcCell;
            this.Formula = formula;
            this.IsFormula = isFormula;
            this.Value = value;
            this._hashCode = (this.Sheet + "!" + this.RcCell).GetHashCode();
        }

        #endregion

        #region comparable methods and overrides

        private readonly int _hashCode;
        
        public bool RangeIsFullyEvaluated { get; set; }
        public bool IsEvaluated { get; set; }
        public object EvaluatedValue { get; set; }

        public override int GetHashCode() {
            return this._hashCode;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>. 
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj) {
            ExtractedCell other = obj as ExtractedCell;
            if (other == null) {
                throw new ArgumentException("Could not compare to a non excel cell");
            }
            return this.ToString().CompareTo(other.ToString());
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ExtractedCell other) {
            return this.Sheet.Equals(other.Sheet) && this.RcCell.Equals(other.RcCell);
        }

        public override string ToString() {
            return this.SheetRC;
        }

        public override bool Equals(object obj) {
            if (!(obj is ExtractedCell other)) {
                return false;
            }
            return this.Sheet.Equals(other.Sheet) && this.RcCell.Equals(other.RcCell);
        }

        #endregion

        public XElement ToXml(bool recursive = true) {
            XElement elem = new XElement("Cell",
                new XAttribute("Name",this.ToString())           
                );

            if (!this.IsRange) {
                elem.Add(new XAttribute("Formula", this.Formula),
                         new XAttribute("Value", this.Value));
            } else {
                elem.Add(new XAttribute("Formula", "RANGE"));
            }

            if (this.ExcelNames.Count>0) {
                elem.Add(new XAttribute("KnownAs", this.ExcelNames.Aggregate((acc,next) => acc+"|"+next)));
            }

            foreach (var reference in this.References) {
                if (recursive) {
                    XElement child = reference.Cell.ToXml();
                    child.Add(new XAttribute("NameGivenByParent", reference.KnownbyParentAs));
                    elem.Add(child);
                } else {
                    XElement child = new XElement("Cell",
                                                  new XAttribute("Name", reference.Cell.ToString()),
                                                  new XAttribute("NameGivenByParent", reference.KnownbyParentAs)
                        );
                    elem.Add(child);
                }
            }

            return elem;
        }

        public string GenerateFormula(Dictionary<string, Tuple<ExtractedCell, object>> parameters) {
            string name = this.ToString(); // sheet!RC            

            if (this.IsRange) {
                // recurse for children then concat their returned formulas together...
                //string result = this.References.Aggregate("", (res, reference) => res + ",(" +  reference.Cell.GenerateFormula(parameters,level+1,expressionFactory) + ")"); // todo here we must concat ranges with a {}
                string result = this.References.Aggregate("", (res, reference) => res + ",([" + reference.Cell.ToString()+ "])"); // todo here we must concat ranges with a {}
                if (result[0] == ',') {
                    result = result.Substring(1); // remove first , ...
                }
                return "\"{|{"+ result +"}|}\"";
                //return "\"{|{" + this.ToString() + "}|}\"";
            }

            if (this.IsBlank) {
                parameters.AddIfNotExistant(this.ToString(), new Tuple<ExtractedCell, object>(this,"\"\""));

                foreach (var excelName in ExcelNames) {
                    parameters.Add(excelName, new Tuple<ExtractedCell, object>(this,"\"\""));
                }
                return "[" + name + "]";// wrap the name
            }

            if (!this.IsFormula) { // is value
                parameters.AddIfNotExistant(name, new Tuple<ExtractedCell, object>(this,this.Formula));
                foreach (var excelName in ExcelNames) {
                    parameters.Add(excelName,new Tuple<ExtractedCell, object>(this, "\"\""));
                }
                return "([" + name + "])";// wrap the name 
            }

            string theresult = this.Formula;

            return theresult;
        }
        public void SetEvaluatedValue(object value) {
            double doublevalue;
            if (double.TryParse(value.ToString(), out doublevalue)) {
                if (Double.IsNaN(doublevalue)) {
                    value = ExcelErrors.NAN;
                }
            }

            decimal decimalvalue;
            // we'll reduce the precision to matchExcel PRECISION
            if (decimal.TryParse(value.ToString(), out decimalvalue)) {                
                value = RoundTo15SigFigs(decimalvalue);
            }
            this.EvaluatedValue = value;
            this.IsEvaluated = true;
        }

        public static decimal RoundTo15SigFigs(decimal value) {
            int sign = value < 0 ? -1 : 1;

            //0.0 81662 59168 70416
            //49861 . 03045 06247 05
            string s = $"{value:0.00000000000000000000}".Replace("-", "");
           // return RoundDoubleUp(value, (15 - s.IndexOf(".") ));
            return Math.Round(value, (15 - s.IndexOf(".")));
        /*    decimal scalefactor = (decimal)Math.Pow(10, s.IndexOf(".") - 1);
            if (scalefactor == 1 && s.StartsWith("0.")) {
                s = s.Replace("0.", "");
                scalefactor = scalefactor / 10;
                while (s.StartsWith("0")) {
                    s = s.Substring(1);
                    scalefactor = scalefactor / 10;
                }
            }
            scalefactor = scalefactor == 0 ? 1 : scalefactor;
            decimal atzero = value / scalefactor;
            decimal res = Math.Round(atzero, 14);
            decimal difference = Math.Abs(atzero - res);
            decimal roundedup = res;
            if (difference > 0) {
                decimal padding = (1 / (decimal)Math.Pow(10, 14)) * sign;

                roundedup = roundedup+padding;
            }

       /*     decimal roundTo15SigFigs = scalefactor * res;
            if (roundTo15SigFigs == Decimal.Parse("243995.389649018")) {
                scalefactor = scalefactor*2;
            }
            return roundTo15SigFigs;*/

            //return roundedup * scalefactor;
            //return roundedup * scalefactor;

        }

        public static Dictionary<string, string> GetDataDictionary() {
            return new Dictionary<string, string> {
                {"Sheet", "string"},
                {"Cell", "string"},
                {"Row","int" },
                {"Col","int" },
                {"Formula", "string" },
                {"Value", "string"},
                {"ValueEval", "string"},
                {"isBlank","boolean" },
                {"isRange","boolean" },
                {"isFormula","boolean" },
                {"RangeSize","int" },
                { "Type","string"}
            };
        }

        public Dictionary<string, string> GetData() {
            return new Dictionary<string, string>() {
                {"Sheet", this.Sheet},
                {"Cell", this.RcCell},
                {"Row", this.IsRange ? int.Parse(this.Address.RangeTopLeft().Row).ToString() : this.Address.IntRow.ToString()},
                {"Col", this.IsRange ? this.Address.RangeTopLeft().ColumnNumber().ToString() : this.Address.IntCol.ToString() },
                {"Formula", this.Formula },
                {"Value", this.Value},
                {"ValueEval", this.EvaluatedValue.ToString()},
                {"isBlank",this.IsBlank.ToString().ToLower() },
                {"isRange",this.IsRange.ToString().ToLower()},
                {"isFormula",this.IsFormula.ToString().ToLower()},
                {"RangeSize",this.Address.CellsInRange().ToString() },
                { "Type",this.GetCellType().ToString()}
                
            };
        }

        private ExtractedCellType GetCellType() {
            if (this.IsRange) return ExtractedCellType.Range;
            if (this.IsFormula) return ExtractedCellType.Formula;
            if (this.IsBlank) return ExtractedCellType.Blank;
            if (double.TryParse(this.EvaluatedValue.ToString(), out double v)) return ExtractedCellType.Numeric;
            if (!string.IsNullOrEmpty(this.EvaluatedValue.ToString())) return ExtractedCellType.Text;
            throw new ArgumentOutOfRangeException("bad cell "+this.EvaluatedValue);
        }

        public List<ExcelAddress> ExpandRange() {
            if (!this.IsRange) return null;
            var address = new ExcelAddress(this.Sheet, this.RcCell);
            var range = ExcelAddress.ExpandRangeToExcelAddresses(address);
            return range;
        }
    }

    public enum ExtractedCellType {
        Blank,
        Range,
        Numeric,
        Text,
        Formula
    }
}
