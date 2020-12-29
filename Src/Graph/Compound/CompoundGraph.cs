using QuickGraph;

namespace Graph.Compound {

	public class CompoundGraph : BidirectionalGraph<CompoundVertex, CompoundEdge>
	{
		public CompoundGraph() { }

		public CompoundGraph(bool allowParallelEdges)
			: base(allowParallelEdges) { }

		public CompoundGraph(bool allowParallelEdges, int vertexCapacity)
			: base(allowParallelEdges, vertexCapacity) { }
	}
}