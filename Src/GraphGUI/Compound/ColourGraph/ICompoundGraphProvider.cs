using System;
using System.Collections.Generic;
using Graph;
using Graph.Compound;

namespace GraphGUI.Compound.ColourGraph {
    /// <summary>
    /// Provides a compound coloured graph with a graph on each inner node
    /// </summary>
    public interface ICompoundGraphProvider {

        /// <summary>
        /// Gets the mappings which the user can choose - these cause a relayout of the entire graph
        /// </summary>
        /// <value>The mappings.</value>
        IEnumerable<string> Mappings { get; }

        /// <summary>
        /// Gets the graph options which dictate how the inner graphs are displayed
        /// </summary>
        /// <value>The graph options.</value>
        IEnumerable<string> GraphOptions { get; }

        /// <summary>
        /// Gets the maximum number of graph options to provide.
        /// </summary>
        /// <value>The maximum number of graph options to provide.</value>
        int MaxGraphOptions { get; }

        /// <summary>
        /// Changes the mapping - should cause generation of an entirely new graph
        /// </summary>
        /// <param name="mapping">The mapping for which to generate.</param>
        /// <returns>an entirely new graph</returns>
        CompoundGraph CreateGraphforMapping(string mapping);

        /// <summary>
        /// Recreates the inner graphs
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="graphSelection">The graph selection.</param>
        /// <param name="showReferencedOutputs">showReferencedOutputs</param>
        AGraph GetInnerGraph(CompoundVertex vertex, string mapping, IEnumerable<string> graphSelection, bool showReferencedOutputs);

        /// <summary>
        /// Refreshes the inner graph colours.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="graphSelection">The graph selection.</param>
        void RefreshInnerGraphColours(CompoundGraph graph, string mapping, IEnumerable<string> graphSelection);

        /// <summary>
        /// Prints the vertexes.
        /// </summary>
        /// <param name="rootPath">The root path to save an xml file to</param>
        /// <param name="vertexandFiles">The vertex and image file names.</param>
        void PrintVertexes(string rootPath, IEnumerable<Tuple<object, string, string>> vertexandFiles);

        /// <summary>
        /// Translates a Vertex to it's name.
        /// </summary>
        /// <param name="vertex">The vertex</param>
        /// <returns>vertex's name</returns>
        string VertexToName(object vertex);

        /// <summary>
        /// Visualises the flags.
        /// </summary>
        /// <param name="flagsIndexXMLfile">The flags index XMl file.</param>
        /// <param name="variableMapping">The variable mapping.</param>
        /// <returns>a visualisation context</returns>
        object VisualiseFlags(string flagsIndexXMLfile, string variableMapping);

        /// <summary>
        /// Updates the visualisation for inner vertex.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vertex">The vertex.</param>
        /// <param name="visualisationContext">The visualisation context.</param>
        /// <param name="variableMapping"></param>
        void UpdateVisualisationForInnerVertex(CompoundGraph graph, ExcelVertex vertex, object visualisationContext, string variableMapping);

        /// <summary>
        /// Notifies the graph provider that an inner vertex had been clicked. 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vertex">The vertex.</param>
        void InnerFocusChanged(CompoundGraph graph, ExcelVertex vertex);
    }
}
