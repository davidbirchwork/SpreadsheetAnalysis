using System;

namespace NCalcExcel
{
    public class FunctionArgs : EventArgs
    {
        private object result;
        public object Result
        {
            get => result;
            set 
            { 
                result = value;
                HasResult = true;
            }
        }

        public bool HasResult { get; set; }

        private Expression[] parameters = new Expression[0];

        public Expression[] Parameters
        {
            get => parameters;
            set => parameters = value;
        }

        public object[] _EvaluateParameters()
        {
            object[] values = new object[parameters.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = parameters[i].Evaluate();
            }

            return values;
        }
    }
}
