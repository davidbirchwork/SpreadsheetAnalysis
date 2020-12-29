using System.Collections.Generic;

namespace ParseTreeExtractor.Domain {

    public class NamedRange : Node {// todo this should become a subclass of Range

        public NamedRange() : base() {

        }

        private NamedRange(NamedRange other) : base(other) {
            this.Size = other.Size;
        }

        public int Size { get; set; }

        public override string Label => Id;
        public override string NodeType { get; set; } = "NamedRange";
        public string Type => "Range";

        public override string ToString() {
            return $"{{ Name = {Id}, Size = {Size} }}";
        }

        public override object Clone() {
            return  new NamedRange(this);
        }

        public override bool Equals(object value) {
            var type = value as NamedRange;
            return (type != null) && EqualityComparer<string>.Default.Equals(type.Id, Id) &&
                   EqualityComparer<int>.Default.Equals(type.Size, Size);
        }

        public override int GetHashCode() {
            int num = 0x7a2f0b42;
            num = (-1521134295 * num) + EqualityComparer<string>.Default.GetHashCode(Id);
            return (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(Size);
        }

        public override Dictionary<string, string> GetData() {
            var d = base.GetData();
            d.Add(nameof(Size), Size.ToString());
            d.Add(nameof(Label), Label);
            d.Add(nameof(Type), Type);
            return d;
        }

        public override Dictionary<string, string> GetDataTypes() {
            var d = base.GetDataTypes();
            d.Add(nameof(Size), "int");
            d.Add(nameof(Label), "string");
            d.Add(nameof(Type),"string");
            return d;
        }
        
    }

}