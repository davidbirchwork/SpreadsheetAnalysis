using System.Collections.Generic;

namespace ParseTreeExtractor.Domain {
    public class CellNode : Node {
        public CellNode() : base() {

        }
        private CellNode(CellNode cellNode) : base(cellNode){
            this.Sheet = cellNode.Sheet;// strings passed by value
            this.ColNo = cellNode.ColNo;
            this.RowNo = cellNode.RowNo;
        }

        public string Sheet { get; set; }
        public int ColNo { get; set; }
        public int RowNo { get; set; }    
        
        public override string Label => Id;
        public override string NodeType { get; set; } = "Cell";

        public override string ToString() {
            return $"{{ Sheet = {Sheet}, ColNo = {ColNo}, RowNo = {RowNo}, Name = {Id} }}";
        }

        public override object Clone() {
            return new CellNode(this);
        }

        public override bool Equals(object value) {
            var type = value as CellNode;
            return (type != null) && EqualityComparer<string>.Default.Equals(type.Sheet, Sheet) &&
                   EqualityComparer<int>.Default.Equals(type.ColNo, ColNo) &&
                   EqualityComparer<int>.Default.Equals(type.RowNo, RowNo) &&
                   EqualityComparer<string>.Default.Equals(type.Id, Id);
        }

        public override int GetHashCode() {
            int num = 0x7a2f0b42;
            num = (-1521134295 * num) + EqualityComparer<string>.Default.GetHashCode(Sheet);
            num = (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(ColNo);
            num = (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(RowNo);
            return (-1521134295 * num) + EqualityComparer<string>.Default.GetHashCode(Id);
        }

        public override Dictionary<string, string> GetData() {
            var d = base.GetData();
            d.Add(nameof(Sheet),Sheet);
            d.Add(nameof(ColNo),ColNo.ToString());
            d.Add(nameof(RowNo), RowNo.ToString());
            d.Add(nameof(Label), Label);
            d.Add("Type", "Cell");
            return d;
        }

        public override Dictionary<string, string> GetDataTypes() {
            var d = base.GetDataTypes();
            d.Add(nameof(Sheet), "string");
            d.Add(nameof(ColNo), "int");
            d.Add(nameof(RowNo), "int");
            d.Add(nameof(Label), "string");
            d.Add("Type", "string");

            return d;
        }
    }
}

