using System.Collections.Generic;

namespace ParseTreeExtractor.Graph {
    public interface IExportGraphData {
        Dictionary<string, string> GetData();
        Dictionary<string, string> GetDataTypes();
    }
}