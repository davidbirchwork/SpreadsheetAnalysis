using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExcelInterop.Domain;
using log4net;
using ParseTreeExtractor.Domain;

namespace GraphAnalysis.InputPartitioner {
    /// <summary>
    /// Direction of the label relative to the cells it labels.
    /// numbers indicate how tightly they bind 
    /// </summary>
    public enum Direction {
        Left =1,
        Top =2,
        Right =3,
        Bottom =4
    }

    public class CellLabel {
        public CellLabel(Direction direction, ExcelAddress cells) {
            Direction = direction;
            Cells = cells;
        }

        public Direction Direction { get;}
        public ExcelAddress Cells { get; }
        public List<string> Values { get; set; }
        
    }

    public class LabeledInput {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ExcelAddress Range { get; }
        private List<string> _cells;
        public List<string> Cells => _cells ?? (_cells = ExcelAddress.ExpandRangeToExcelAddresses(Range).Select(a=>a.ToString()).ToList());
        public List<string> RangeValues { get; set; }

        public List<CellLabel> Labels = new List<CellLabel>();

        public ExcelAddress Label { get; set; }
        public string LabelValue { get; set; }

        public string Name { get; set; }

        public LabeledInput(ExcelAddress vector, params CellLabel[] cellLabels) {
            Range = vector;
            Labels.AddRange(cellLabels.Where(l => l != null && l.Cells!=null));
        }

        public override string ToString() {
            return
                $"{nameof(Range)}: {Range}," + 
                $"{nameof(Label)}: {Label?.ToString() ?? "none"},"+
                $"{nameof(LabelValue)}: {LabelValue ?? "none"}," +
                Labels.Aggregate("Labels: ",(acc,next) => acc+","+next.ToString());
        }

        public void AcquireValues(Extraction extraction) {
            if (Label != null && extraction.CellFormulas.TryGetValue(Label, out var val)) {
                LabelValue = val;
            }

            RangeValues = FindRangeValues(Range, extraction);

            foreach (var label in Labels) {
                label.Values = FindRangeValues(label.Cells, extraction);
            }
            // remove any labels that somehow didn't get values
            Labels = Labels.Where(l => l.Values.Any()).ToList();

        }

        private List<string> FindRangeValues(ExcelAddress excelAddress,
            Extraction extraction) {
            if (excelAddress == null) return null;

            List<string> values = new List<string>();
            var cells = ExcelAddress.ExpandRangeToExcelAddresses(excelAddress);
            foreach (var cellAddress in cells) {
                if (extraction.CellFormulas.TryGetValue(cellAddress, out var value)) {
                    if(string.IsNullOrWhiteSpace(value)) { value = "0";}
                    values.Add(value);
                }
                else {
                    Log.Warn("not found range value");
                }
            }

            return cells.Count == values.Count ? values : null;
        }

        /// <summary>
        /// Return a list of the cells used for the labels of the table
        /// </summary>
        /// <returns></returns>
        public readonly List<string> LabelCells = new List<string>();

        public void SeekMetaLabel(Dictionary<ExcelAddress, Node> labelMap, Extraction extraction, Direction hozDirection, Direction vertDirection) {
            if (Label != null) return;

            var hoz = Labels.FirstOrDefault(l => l.Direction == hozDirection);
            var vert = Labels.FirstOrDefault(l => l.Direction == vertDirection);
            if (hoz == null || vert == null) return;

            var label = new ExcelAddress(Range.WorkSheet,
                vert.Cells.RangeTopLeft().Column + hoz.Cells.RangeTopLeft().Row);
            if (labelMap.ContainsKey(label) && !double.TryParse(extraction.CellFormulas[label], out _)) {
                Label = label;
            }
        }
    }
}