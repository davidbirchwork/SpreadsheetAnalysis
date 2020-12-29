using System;

namespace NCalcExcel
{
    // Summary:
    //     Provides enumerated values to use to set evaluation options.
    [Flags]
    public enum EvaluateOptions
    {
        // Summary:
        //     Specifies that no options are set.
        None = 1,
        //
        // Summary:
        //     Specifies case-insensitive matching.
        IgnoreCase = 2,
        //
        // Summary:
        //     No-cache mode. Ingores any pre-compiled expression in the cache.
        NoCache = 4,
        //
        // Summary:
        //     Treats parameters as arrays and result a set of results.
        IterateParameters = 8,
        //
        // Summary:
        //     When using Round(), if a number is halfway between two others, it is rounded toward the nearest number that is away from zero. 
        RoundAwayFromZero = 16,
        //
        // Summary:  
        //     Use for speed if you do not need to override the built in or MEF'd functions
        BuiltInFunctionsFirst = 32,
        //
        // Summary:  
        //     use to ensure that all parameters in a function get evaluated - so as to ensure that all parameters get evaluated
        DebugMode = 64,
        /// <summary>
        /// dont use string contact - treast as numbers instead
        /// </summary>
        DontUseStringConcat = 128,
        /// <summary>
        /// Reduces precision to 15 sig fig. 
        /// </summary>
        ReduceTo15Sigfig = 256,
        /// <summary>
        /// Stop hydrating blank cells to zero
        /// </summary>
        DonotHydrateBlanksToZero = 512,
    }
}
