using System;
using System.Xml.Serialization;
using ExcelInterop.Domain;

namespace ExcelExtractor.Domain {
    public class CellReference : IEquatable<CellReference> {
        
        public enum CellReferenceType {
            Formula,
            Lookup,
            RangeExpansion
        }

        public string KnownbyParentAs { get; set; }
        public ExcelAddress Tagname { get; set; }
        [XmlIgnore]
        public ExtractedCell Cell { get; set; }

        private string _referenceName;
        public string ReferenceName {
            get { return Cell!= null ? Cell.ToString() : _referenceName; }
            set { _referenceName = value; }
        }

        public CellReferenceType ReferenceType { get; set; }

        public CellReference() {
            
        }

        public CellReference(ExtractedCell cell, CellReferenceType referenceType, string knownbyParentAs, ExcelAddress tagname) {
            this.Cell = cell;
            this.KnownbyParentAs = knownbyParentAs;
            this.Tagname = tagname;
            this.ReferenceType = referenceType;
        }

        #region equality members        

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (CellReference)) return false;
            return Equals((CellReference) obj);
        }


        public bool Equals(CellReference other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.KnownbyParentAs, KnownbyParentAs) && Equals(other.Cell, Cell) && Equals(other.ReferenceType, ReferenceType);
        }

        public override int GetHashCode() {
            unchecked {
                int result = (KnownbyParentAs != null ? KnownbyParentAs.GetHashCode() : 0);
                result = (result*397) ^ (Cell != null ? Cell.GetHashCode() : 0);
                result = (result*397) ^ ReferenceType.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(CellReference left, CellReference right) {
            return Equals(left, right);
        }

        public static bool operator !=(CellReference left, CellReference right) {
            return !Equals(left, right);
        }

        #endregion
    }
}