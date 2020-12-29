using QuickGraph;
using System.Diagnostics;

namespace Graph
{
	/// <summary>
	/// A simple identifiable edge.
	/// </summary>
	[DebuggerDisplay( "{Source.ID} -> {Target.ID}" )]
	public class AEdge :  Edge<ExcelVertex> 
	{
		public string ID
		{
			get;
			private set;
		}

		public AEdge( string id, ExcelVertex source, ExcelVertex target )
			: base( source, target )
		{
			ID = id;
		}
	}
}