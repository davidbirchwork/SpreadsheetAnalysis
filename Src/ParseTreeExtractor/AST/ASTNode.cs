using System.Collections.Generic;
using ParseTreeExtractor.Domain;

namespace ParseTreeExtractor.AST {

    public class ASTNode : Node {
        public string Type { get; set; }
        public List<string> Children { get; set; }//this is used within the parse tree construction

        private string _label;

        public ASTNode() : base() {

        }

        private ASTNode(ASTNode other) :base(other) {
            this.Type = other.Type;
            this.ParentCell = other.ParentCell;
            this.IsOperator = other.IsOperator;
            this._label = other.Label;
        }

        public override string Label {
            get => _label;
            // clean up the irony pretty printing
            set => _label = value.Replace(" (NumberToken)", "")
                                 .Replace(" (Key symbol)", "")
                                 .Replace(" (TextToken)", "")
                                 .Replace("(ExcelFunction)", "")
                                 .Replace("(", "")
                                 .Replace(" ", "");
        }

        public override string NodeType => Type.Contains("ExcelFunction") ? Label : Type;

        public string ParentCell { get; set; }
        public bool IsOperator { get; set; }

        public override string ToString() {
            return $"{{ Type = {Type}, Children = {Children}, Label = {Label}, ParentCell = {ParentCell} }}";
        }

        public override object Clone() {
            return new ASTNode(this);
        }

        public override bool Equals(object value) {
            var type = value as ASTNode;
            return (type != null) && EqualityComparer<string>.Default.Equals(type.Type, Type) &&
                   EqualityComparer<List<string>>.Default.Equals(type.Children, Children) &&
                   EqualityComparer<string>.Default.Equals(type.Label, Label) &&
                   EqualityComparer<string>.Default.Equals(type.ParentCell, ParentCell);
        }

        public override int GetHashCode() {
            int num = 0x7a2f0b42;
            num = (-1521134295 * num) + EqualityComparer<string>.Default.GetHashCode(Type);
            num = (-1521134295 * num) + EqualityComparer<List<string>>.Default.GetHashCode(Children);
            num = (-1521134295 * num) + EqualityComparer<string>.Default.GetHashCode(Label);
            return (-1521134295 * num) + EqualityComparer<string>.Default.GetHashCode(ParentCell);
        }

        public override Dictionary<string, string> GetData() {
            var d = base.GetData();
            d.Add(nameof(Type), Type);
            d.Add(nameof(Label), Label);
            d.Add(nameof(IsOperator), IsOperator.ToString());
            return d;
        }

        public override Dictionary<string, string> GetDataTypes() {
            var d = base.GetDataTypes();
            d.Add(nameof(Type), "string");
            d.Add(nameof(Label), "string");
            d.Add(nameof(IsOperator), "bool");
            return d;
        }
    }
}
