using System;

namespace NCalcExcel
{
    public class ParameterArgs : EventArgs
    {
        public EvaluateOptions Options { get; }

        public ParameterArgs(EvaluateOptions options) {
            Options = options;
        }

        private object result;
        public object Result
        {
            get { return result; }
            set
            {
                result = value;
                HasResult = true;
            }
        }

        public bool HasResult { get; set; }
    }
}
