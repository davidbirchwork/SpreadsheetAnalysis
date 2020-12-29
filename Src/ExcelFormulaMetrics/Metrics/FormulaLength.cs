using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Graph;
using GraphMetrics;

namespace ExcelFormulaMetrics.Metrics {
    [Metric("Formula Length Metrics","Provides metrics on large formulas")]
    public class FormulaLength : IMetric {
        
        private double _averageLength;
        private int _fifthpercentile;
        private int _ninetyfifthpercentile;
        private List<ExcelVertex> _largestFormulas;
        private int _count;
        private Dictionary<int, int> Distribution;
        private const int LargestN = 15;

        #region Implementation of IMetric

        public string Compute(ConcurrentDictionary<string, IMetric> computedMetrics, IEnumerable<ExcelVertex> vertices, IEnumerable<AEdge> edges, List<string> colours, Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>> vertexdict) {
            if (vertices == null) {
                return "no vertices";
            }

            var vertexesWithFormulas = vertices.Where(v=> v.isFormula).ToList();
            if (!vertexesWithFormulas.Any()) {
                return "no meta :-/ ";
            }

            SortedDictionary<int, List<ExcelVertex>> vertexbyFormulaLength = new SortedDictionary<int, List<ExcelVertex>>(new Intcomparer());
            this._averageLength = 0;
            this._count = 0;
            foreach (var vertex in vertexesWithFormulas) {
                if (!vertex.Formula.Contains("{|{")) {// ie its not a range
                    int length = vertex.Formula.Length;
                    if (!vertexbyFormulaLength.ContainsKey(length)) {
                        vertexbyFormulaLength.Add(length, new List<ExcelVertex>());
                    }
                    vertexbyFormulaLength[length].Add(vertex);
                    this._averageLength += length;
                    this._count++;
                }
            }

            this._averageLength = this._averageLength / this._count;

            var values = vertexbyFormulaLength.Keys.ToArray();

            int fifth = (int)Math.Floor(values.Length * 0.05);
            int ninetyfifth = (int)Math.Floor(values.Length * 0.95);
            this._fifthpercentile = values[fifth];
            this._ninetyfifthpercentile = values[ninetyfifth];


            this._largestFormulas = vertexbyFormulaLength.Values.Take(LargestN).Aggregate(new List<ExcelVertex>(),
                                                                                          (acc, next) => {
                                                                                              acc.AddRange(next);
                                                                                              return acc;
                                                                                          });
            this._largestFormulas = this._largestFormulas.ToList();

            this.Distribution = vertexbyFormulaLength.ToDictionary(kpv => kpv.Key, kpv => kpv.Value.Count);            

            return null;
        }

        public string Print() {
            string text = $"Average Formula Length = {this._averageLength:00} \n";
            text += $"Number of Formulas = {this._count}  \n";
            text += $"Fifth Percentile = {this._fifthpercentile}  \n";
            text += $"Ninety-fifth Percentile = {this._ninetyfifthpercentile} \n";
            text += "Largest Formulas:";
            int printed = 0;
            int lastSizePrinted = int.MaxValue;
            for (int f = 0; printed<LargestN &&  f < this._largestFormulas.Count(); f++) {
                ExcelVertex vert = this._largestFormulas[f];
                int length = vert.Formula.Length;
                if (length < lastSizePrinted) {
                    printed++;                    
                    text += "\n" + length + " - " + vert.ID + "\n" + vert.Formula;
                    lastSizePrinted = length;
                }
            }            
            text += this.Distribution.Aggregate("\n\n Distribution: \nLength,Frequency\n", (acc, next) => acc +next.Key+","+next.Value+"\n");
            return text;
        }

        public List<string> PreRequisiteMetrics() {
            return  new List<string>();
        }

        #endregion

        private class Intcomparer : IComparer<int> {
            #region Implementation of IComparer<in int>

            public int Compare(int x, int y) {
                if (x < y) {
                    return 1;
                } else if (x == y) {
                    return 0;
                }

                return -1;
            }

            #endregion
        }
    }
}