using System.Collections.Generic;
using ParseTreeExtractor.Graph;

namespace ParseTreeExtractor.Domain {
    public class Edge : IExportGraphData{
        public string Id;
        public Node Source;
        public Node Target;

        public override string ToString() {
            return $"{nameof(Id)}: {Id}, {nameof(Source)}: {Source.Id}, {nameof(Target)}: {Target.Id}";
        }

        public virtual Dictionary<string, string> GetData() {
            return new Dictionary<string, string>();
        }

        public virtual Dictionary<string, string> GetDataTypes() {
            return new Dictionary<string, string>();
        }
    }
}