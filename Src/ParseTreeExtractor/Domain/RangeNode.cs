using System.Collections.Generic;

namespace ParseTreeExtractor.Domain {

    public class RangeNode : Node {
        public RangeNode() : base() {

        }

        private RangeNode(RangeNode other) : base(other) {
            this.Sheet = other.Sheet;
            this.Size = other.Size;
        }

        public string Sheet { get; set; }
        public int Size { get; set; }

        public override string Label => Id;
        public string Type => "Range";
        public override string NodeType { get; set; } = "Range";

        public override string ToString() {
            return $"{{ Name = {Id}, Sheet = {Sheet}, Size = {Size} }}";
        }

        public override object Clone() {
            return new RangeNode(this);
        }

        public override bool Equals(object value) {
            return (value is RangeNode type) && EqualityComparer<string>.Default.Equals(type.Id, Id) &&
                   EqualityComparer<string>.Default.Equals(type.Sheet, Sheet) &&
                   EqualityComparer<int>.Default.Equals(type.Size, Size);
        }

        public override int GetHashCode() {
            int num = 0x7a2f0b42;
            num = (-1521134295 * num) + EqualityComparer<string>.Default.GetHashCode(Id);
            num = (-1521134295 * num) + EqualityComparer<string>.Default.GetHashCode(Sheet);
            return (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(Size);
        }

        public override Dictionary<string, string> GetData()
        {
            var d = base.GetData();
            d.Add(nameof(Size), Size.ToString());
            d.Add(nameof(Label), Label);
            d.Add(nameof(Sheet),Sheet);
            d.Add(nameof(Type), Type);
            return d;
        }

        public override Dictionary<string, string> GetDataTypes()
        {
            var d = base.GetDataTypes();
            d.Add(nameof(Size), "int");
            d.Add(nameof(Label), "string");
            d.Add(nameof(Sheet), "string");
            d.Add(nameof(Type), "string");
            return d;
        }

        public RangeNode(Node other) : base(other) {
        }

        public RangeNode(string id, string workSheet, int size) {
            this.Id = id;
            this.Sheet = workSheet;
            this.Size = size;
        }
    }

}