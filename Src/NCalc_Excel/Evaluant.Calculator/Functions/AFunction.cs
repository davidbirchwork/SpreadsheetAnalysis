using System.Collections.Generic;
using NCalcExcel.Domain;

namespace NCalcExcel.Functions {
    /// <summary>
    /// Abstract class to implement to add user generated functions
    /// </summary>
    public abstract class AFunction{

        protected AFunction(string name, string usage, string description) {
            this.Name = name;
            this.Usage = usage;
            this.Description = description;
            this.ParameterHelp = new Dictionary<string, string>();
        }

        protected void AddParamterDescription(string paramName, string paramDescription) {
            this.ParameterHelp.Add(paramName, paramDescription);
        }

        /// <summary>
        /// Return the name of this function - this will be used to match function calls within client code
        /// </summary>        
        public string Name { get; protected set; }

        /// <summary>
        /// Evaluate the function - your client code goes here        
        /// </summary>
        /// <param name="evaluator"> Evaluation visitor to use to evaluate parameters </param>
        /// <param name="function">parsed object</param>              
        /// <returns>result of the evaluation</returns>
        public abstract object Evaluate(EvaluationVisitor evaluator, Function function);

        /// <summary>
        /// Get usage for example
        /// Abs(number)
        /// </summary>        
        public string Usage { get; protected set; }

        /// <summary>
        /// Gets description of this function
        /// eg
        /// Computes Absolute magnitude of a given number
        /// </summary>        
        public string Description { get; protected set; }

        /// <summary>
        /// Gets a description of each parameter this function uses
        /// parameter names should match those given in GetUsage
        /// eg
        /// (number,the number to take the absolute value of)
        /// </summary>        
        public Dictionary<string, string> ParameterHelp { get; protected set; }
    }
}
