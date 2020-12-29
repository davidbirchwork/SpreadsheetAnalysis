using System.Collections.Generic;

namespace Graph {
    /// <summary>
    /// shows how to build a DAG.
    /// </summary>    
    public interface IDAG<out TVertex>  {

        TVertex ParentVertex { get; }
        IEnumerable<TVertex> ChildVertexes { get; }        

        object CreateGraph();
    }
}
