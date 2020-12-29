using System.Diagnostics;
using QuickGraph;

namespace Graph.Compound
{
	/// <summary>
	/// A simple identifiable edge.
	/// </summary>
	[DebuggerDisplay( "{Source.ID} -> {Target.ID}" )]
	public class CompoundEdge :  AEdge, IEdge<CompoundVertex> {		
		public CompoundEdge( string id, CompoundVertex source, CompoundVertex target )
			: base(id, source, target ) {
			
		}

	    #region Implementation of IEdge<CompoundVertex>

	    public new CompoundVertex Source {
            get { return base.Source as CompoundVertex; }            
	    }

	    public new CompoundVertex Target {
            get { return base.Target as CompoundVertex; }
	    }

	    #endregion
	}
}