using System;
using System.IO;
using System.Linq;
using Utilities.Config;
using Utilities.Loggers;

namespace Utilities.Designs.PB {
    public class PBDesign : IDesign {

        private readonly string _designDirectory = AppConfig.GetExeDirectory() + @"\Designs\PB\DesignFiles\";
        private readonly string[] _designs;
        private readonly int[] _designSizes;

        /// <summary>
        ///  Create a design manager class
        /// This class will load a set of design files in @"Designs\PB\DesignFiles\" upon instantiation
        /// </summary>
        public PBDesign() {
            // get designs
            this._designs = Directory.GetFiles(_designDirectory,"*.PB");
            this._designSizes = new int[this._designs.Length];
            for (int i = 0; i < _designs.Length; i++) {
                this._designSizes[i] = int.Parse(Path.GetFileNameWithoutExtension(_designs[i]));
                _designs[i] = Path.GetFileName(_designs[i]);
            }
        }

        #region Implementation of IDesign

        /// <summary>
        /// Can this design system generate a design of this size?
        /// </summary>
        /// <param name="factors">number of parameters to evaluate</param>
        /// <returns>bool</returns>
        public bool QueryAcceptableSize(int factors) {
            return factors < this._designSizes.Max();
        }

        /// <summary>
        /// How many runs will this design require?
        /// </summary>
        /// <param name="factors">number of factors</param>
        /// <param name="folded">is the design to be foldedover to improve resolution</param>
        /// <returns>number of runs - if negative then there is no design</returns>
        public int QueryNumberofRuns(int factors, bool folded) {
            int bestdesign = GetBestDesign(factors);
            return folded ? 2 * this._designSizes[bestdesign] : this._designSizes[bestdesign];
        }

        private int GetBestDesign(int factors) {
            int bestdesign = -1;
            int designSize = int.MaxValue;
            for (int i = 0; i < this._designSizes.Length; i++) {
                
                if (factors < (this._designSizes[i] - 1) && this._designSizes[i] < designSize) {                   
                    bestdesign = i;
                    designSize = this._designSizes[bestdesign];
                }
            }
            return bestdesign;
        }

        /// <summary>
        /// Return a matrix of the design with low/high values denoted by false = low true = high
        /// </summary>
        /// <param name="factors">number of factors</param>
        /// <param name="folded">is the design folded over?</param>
        /// <returns>boolean array [run, factor] with low/high values denoted by false = low true = high , Runs X Factors</returns>
        public bool[,] GetDesignMatrix(int factors, bool folded) {
            int bestdesign = GetBestDesign(factors);
            if (bestdesign < 0) {
                Logger.ERROR("Tried to get design matrix for invalid design");
            }
            string[] csvfile = File.ReadAllLines(_designDirectory + this._designs[bestdesign]);

            bool[,] design = new bool[folded ? this._designSizes[bestdesign] * 2 : this._designSizes[bestdesign], factors];

            for (int run = 0; run < this._designSizes[bestdesign]; run++) { // rows
            //    string[] cols = csvfile[run].Split(new[] {','});
                for (int factor = 0; factor < factors; factor++) { // cols
                    design[run, factor] = csvfile[run][factor].Equals('1'); // read from file
                }
            }

            if (folded) { // fold over the design by folding it over by inverting the previous design
                for (int run = this._designSizes[bestdesign]; run < this._designSizes[bestdesign] * 2; run++) {
                    for (int factor = 0; factor < factors; factor++) {
                        design[run, factor] = !design[run - this._designSizes[bestdesign], factor];
                    }
                }
            }

            return design;
        }

        /// <summary>
        /// Analyise a design [Analysis is as per paper #116 for the moment]
        /// </summary>
        /// <param name="design">design [result, factors]</param>
        /// <param name="results">results -- [run,results] </param>
        /// <returns>a level of sensitivity for each variable [factors, sensitivitys] </returns>
        public double[,] AnalyiseDesign(bool [,] design, double[,] results) {
            int numRuns = design.GetLength(0);
            int numFactors = design.GetLength(1);
            int numResults = results.GetLength(1);
            if (numRuns != results.GetLength(0)) {
                Logger.ERROR("mis matched design matrix and result matrix - " + numRuns + " vs " + results.GetLength(0));
            }

            // init results grid
            double[,] analysis = new double[numFactors, numResults];
            for (int factor = 0; factor < numFactors; factor++) {
                for (int result = 0; result < numResults; result++) {
                    analysis[factor,result] = 0;
                }
            }

            // now do the summing up

            for (int run = 0; run < numRuns; run++) {
                for (int factor = 0; factor < numFactors; factor++) {
                    for (int result = 0; result < numResults; result++) {
                        if (design[run,factor]) {
                            analysis[factor, result] += results[run, result];
                        } else {
                            analysis[factor, result] -= results[run, result];
                        }
                    }
                }
            }

            // now normalise the results since "the sign of the effect is meaningless"
            for (int factor = 0; factor < numFactors; factor++) {
                for (int result = 0; result < numResults; result++) {
                    analysis[factor, result] = Math.Abs(analysis[factor, result]);
                }
            }

            return analysis;
        }

        #endregion
    }
}
