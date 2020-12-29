namespace Utilities.Designs {
    /// <summary>
    /// Interface for a design 
    /// </summary>
    public interface IDesign {

        /// <summary>
        /// Can this design system generate a design of this size?
        /// </summary>
        /// <param name="factors">number of parameters to evaluate</param>
        /// <returns>bool</returns>
        bool QueryAcceptableSize(int factors);

        /// <summary>
        /// How many runs will this design require?
        /// </summary>
        /// <param name="factors">number of factors</param>
        /// <param name="folded">is the design to be foldedover to improve resolution</param>
        /// <returns>number of runs  - if negative then there is no design</returns>
        int QueryNumberofRuns(int factors, bool folded);

        /// <summary>
        /// Return a matrix of the design with low/high values denoted by false = low true = high
        /// </summary>
        /// <param name="factors">number of factors</param>
        /// <param name="folded">is the design folded over?</param>
        /// <returns>boolean areay with low/high values denoted by false = low true = high</returns>
        bool[,] GetDesignMatrix(int factors, bool folded);

        /// <summary>
        /// Analyise a design [some how]
        /// </summary>
        /// <param name="design">desisng [rus, factors]</param>
        /// <param name="results">results -- [run,results] </param>
        /// <returns>a level of sensitivity for each variable [factors, sensitivity] </returns>
        double[,] AnalyiseDesign(bool[,] design, double[,] results);

    }
}
