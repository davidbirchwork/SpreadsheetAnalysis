using System;
using System.Collections.Generic;
using ParseTreeExtractor.Graph;

namespace ParseTreeExtractor.Domain {
    public abstract class Node : IExportGraphData , ICloneable{
        public Node() {

        }
        protected Node(Node other) {
            this.Id = other.Id;
        }

        #region equality

        protected bool Equals(Node other) {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        #endregion

        public string Id { get; set; }
        public List<Node> LinksTo { get; } = new List<Node>();
        public List<Node> LinkedFrom { get; } = new List<Node>();
        public virtual string NodeType { get; set; } = "Node";

        public virtual string Label { get; set; }

        public override string ToString() {
            return $"{nameof(Id)}: {Id}";
        }

        public abstract object Clone();

        public virtual Dictionary<string, string> GetData() {
            return new Dictionary<string, string>();
        }

        public virtual Dictionary<string, string> GetDataTypes() {
            return  new Dictionary<string, string>();
        }
    }
}