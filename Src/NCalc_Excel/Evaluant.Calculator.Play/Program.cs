using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NCalc.Play
{
    /// <summary>
    /// Summary description for Program.
    /// </summary>
    public class Program
    {
        public static void  Main(string[] args)
        {
            Console.WriteLine("Welcome to NCalc \n http://ncalc.codeplex.com/ \n\n");

            ExpressionFactory factory = new ExpressionFactory(); // here we can pass a new MEF catalogue to find more NCalc functions
            Console.WriteLine(factory.PrintLanguage()+"\n\n"); // there is also specific help on each method

            string[] expressions = new string[]
            {
                "2 * (3 + 5)",
                "2 * (2*(2*(2+1)))",
                "10 % 3",
                "false || not (false and true)",
                "3 > 2 and 1 <= (3-2)",
                "3 % 2 != 10 % 3",
                "if( age >= 18, 'majeur', 'mineur')"                
            };

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("age", 30); // custom parameters - can also be further expressions

            foreach (string expression in expressions)
                Console.WriteLine("{0} = {1}", 
                    expression, 
                    (new Expression(expression){Parameters=parameters}).Evaluate());

            Console.WriteLine("\n\nAn example with a custom function");
            Expression expr = new Expression("CalculateNthRoot(3,1000)");
            expr.EvaluateFunction += new EvaluateFunctionHandler(ExprEvaluateFunction); // - not using lambda's for clarity
            Console.WriteLine("CalculateNthRoot(3,1000)"+" := "+expr.Evaluate());

            Console.WriteLine("\n\nAn example iterating parameters:\n");

            Expression e = new Expression("(a * b) ^ c", EvaluateOptions.IterateParameters);
            e.Parameters["a"] = new int[] { 1, 2, 3, 4, 5 };
            e.Parameters["b"] = new int[] { 6, 7, 8, 9, 0 };
            e.Parameters["c"] = 3;

            Console.WriteLine("iterating parameters: " + "(a * b) ^ c");
            Console.WriteLine(((int[]) e.Parameters["a"]).Aggregate<int, string>("a:", (str, num) => str + " " + num));
            Console.WriteLine(((int[]) e.Parameters["b"]).Aggregate<int, string>("b:", (str, num) => str + " " + num));
            Console.WriteLine("c: " + e.Parameters["c"]);
            Console.Write("Results: ");
            foreach (var result in (IList)e.Evaluate()) {
                Console.Write(result+" ");
            }

        /*    Expression exp = new Expression("A:9*2");
            exp.Parameters["A:9"] = 2;
            Console.WriteLine("answer ="+exp.Evaluate());*/

            Console.WriteLine();
            Console.WriteLine("new test:");

            Console.WriteLine(Evaluate("in(1,EU)").ToString());

            Console.ReadLine();

        }

        private static void EvaluateArgument(String name, NCalc.ParameterArgs arg) {
            if (name == "EU")
                arg.Result = "1,2,3,4";
            
        }
        public static object Evaluate(string expression) {
            Expression ex = new Expression(expression, EvaluateOptions.IgnoreCase | EvaluateOptions.NoCache);
            ex.EvaluateParameter += EvaluateArgument;
            return ex.Evaluate();
        }     

        static void ExprEvaluateFunction(string name, FunctionArgs args) {
            if (name.ToLower().Equals("CalculateNthRoot".ToLower())) {
                if (args.Parameters.Length != 2) {
                    throw new ArgumentException("CalculateNthRoot requires two arguments");
                }
                int root = (int) args.Parameters[0].Evaluate();
                double number = Convert.ToDouble(args.Parameters[1].Evaluate());
                if (root <= 0 || number <= 0) {
                    throw new ArgumentException("CalculateNthRoot requires two positive arguments");
                }
                args.HasResult = true;
                args.Result = Math.Pow(number, (1.0/root));
            }
        }
    }
}
