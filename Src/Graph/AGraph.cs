using QuickGraph;

namespace Graph
{
	public class AGraph : BidirectionalGraph<ExcelVertex, AEdge>
	{
		public AGraph() { }

		public AGraph(bool allowParallelEdges)
			: base(allowParallelEdges) { }

		public AGraph(bool allowParallelEdges, int vertexCapacity)
			: base(allowParallelEdges, vertexCapacity) { }
	}
}