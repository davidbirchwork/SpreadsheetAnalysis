using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Graph;

namespace GraphMetrics.Metrics {
    [Metric("Colour Coupling","Computes instability metrics via efferent and afferent coupling based on the number of other colours referenced")]
    public class ColourCoupling : IMetric {

        private Dictionary<string, int> AfferentCouplings = new Dictionary<string, int>();
        private Dictionary<string, int> EfferentCouplings = new Dictionary<string, int>();
        private Dictionary<string, double> Instability = new Dictionary<string, double>();
        private List<string> Colours;

        #region Implementation of IMetric

        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics, IEnumerable<ExcelVertex> vertices, IEnumerable<AEdge> edges, List<string> colours, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            this.Colours = colours;

            var directSheetLinkMatrix = ((ColourMatrixes)computedMetrics[PreRequisiteMetrics()[0]]).DirectSheetLinkMatrix;

            #region  EfferentCouplings

            foreach (string sheet in directSheetLinkMatrix.Keys.ToArray()) {
                this.EfferentCouplings.Add(sheet, 0);
                foreach (KeyValuePair<string, int> sheetsreferingtosheet in directSheetLinkMatrix[sheet]) {
                    if (sheetsreferingtosheet.Key != sheet && sheetsreferingtosheet.Value > 0) {
                        this.EfferentCouplings[sheet] = this.EfferentCouplings[sheet] + 1;
                    }
                }
            }

            #endregion

            #region  AfferentCouplings

            foreach (string sheet in directSheetLinkMatrix.Keys.ToArray()) {
                this.AfferentCouplings.Add(sheet, 0);
                foreach (string sheetreferedto in directSheetLinkMatrix.Keys.ToArray()) {
                    if (sheet != sheetreferedto && directSheetLinkMatrix.ContainsKey(sheetreferedto) && directSheetLinkMatrix[sheetreferedto].ContainsKey(sheet)
                        && directSheetLinkMatrix[sheetreferedto][sheet] > 0) {
                            this.AfferentCouplings[sheet] = this.AfferentCouplings[sheet] + 1;
                    }
                }
            }

            #endregion

            #region instability

            foreach (string sheet in directSheetLinkMatrix.Keys.ToArray()) {
                double instability = this.EfferentCouplings[sheet] + this.AfferentCouplings[sheet];
                instability = this.EfferentCouplings[sheet] / instability;
                this.Instability.Add(sheet, instability);
            }

            #endregion

            return null;
        }

        public string Print() {
            string text = "";
            text += "Afferent Couplings (Ca): The number of other packages that depend upon classes within the package is an indicator of the package's responsibility.\n";
            text += "Efferent Couplings (Ce): The number of other packages that the classes in the package depend upon is an indicator of the package's independence.\n";
            text += "Instability (I): The ratio of efferent coupling (Ce) to total coupling (Ce + Ca) such that I = Ce / (Ce + Ca). This metric is an indicator of the package's resilience to change. The range for this metric is 0 to 1, with I=0 indicating a completely stable package and I=1 indicating a completely instable package.\n";

            foreach (string sheet in this.Colours) {
                string instability = string.Format("{0:00.00′%}", this.Instability[sheet]);
                text += "Sheet " + sheet + " -  Afferent Coupling: " + this.AfferentCouplings[sheet] + " Efferent Coupling: " + this.EfferentCouplings[sheet] + " Instability: " + instability + "\n";
            }
            
            return text;
        }

        public List<string> PreRequisiteMetrics() {
            return new List<string> {"Colour Matrixes"};
        }

        #endregion
    }
}
