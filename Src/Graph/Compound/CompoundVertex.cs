using System.Diagnostics;
using System.Linq;

namespace Graph.Compound {

	/// <summary>
	/// A simple identifiable vertex.
	/// </summary>
    [DebuggerDisplay("{ID}")]
    public class CompoundVertex : ExcelVertex {
	    public string RealID { get; set; }

	    private AGraph _innerGraph;
        public AGraph InnerGraph {
            get { return _innerGraph; }
            set {
                _innerGraph = value;
                if (_innerGraph != null && _innerGraph.Vertices.Count() == 0) {
                    _innerGraph = null;
                }
                NotifyPropertyChanged("InnerGraph");
            }
        }

        public CompoundVertex(string id, string realID)
            : base(id) {
            this.RealID = realID;
        }

	    public override string ToString() {
            return ID;
        }

    }
}