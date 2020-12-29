using System;
using System.Collections.Generic;
using System.Linq;
using ExcelInterop.Domain;

namespace GraphAnalysis.InputPartitioner {
    public class IsoMorphResults {
        public List<Tuple<ExcelAddress, List<string>>> FoundVectors = new List<Tuple<ExcelAddress, List<string>>>();

        public Dictionary<string,List<ExcelAddress>> GetColourings() {
            return this.FoundVectors.ToDictionary(k => k.Item1.ToString(),
                v => v.Item2.Select(a => new ExcelAddress(a)).ToList());
        }
    }
}