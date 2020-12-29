using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ExcelInterop.Domain;
using Irony.Parsing;
using Newtonsoft.Json;
using XLParser;

namespace ParseTreeExtractor.Domain {
    /// <summary>
    /// Holds the results of extracting a spreadsheet in various forms
    /// </summary>
    public class Extraction {
        private Dictionary<ExcelAddress, CellNode> _cellNodes;
        private List<NamedRange> _namedRanges;

        [JsonIgnore]
        public ConcurrentDictionary<ExcelAddress, ParseTreeNode> ParseTrees { get; set; }
        [JsonIgnore]
        public ExcelFormulaGrammar Grammar { get; set; }
        public Dictionary<string, List<ExcelAddress>> NamedRangeMap { get; set; }
        
        public Dictionary<ExcelAddress, CellNode> CellNodes {
            get => _cellNodes ?? (_cellNodes = ParseTrees.Keys.ToDictionary(key => key, address =>
                       new CellNode {
                           Sheet = address.WorkSheet,
                           ColNo = address.IntCol,
                           RowNo = address.IntRow,
                           Id = address.FullName
                       }));
            set => _cellNodes = value;
        }

        public List<RangeNode> Ranges { get; set; }

        public List<NamedRange> NamedRanges {
            get => _namedRanges ?? NamedRangeMap.Select(r => new NamedRange { Id = r.Key, Size = r.Value.Count }).ToList();
            set => _namedRanges = value;
        }

        public List<Tuple<string, string>> References { get; set; }
        public List<ExcelAddress> Cells { get; set; }
        public ConcurrentDictionary<ExcelAddress, string> CellFormulas { get; set; }
        public Dictionary<string, ExcelAddress> SheetRanges { get; set; }
    }
}