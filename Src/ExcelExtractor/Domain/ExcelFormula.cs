using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelExtractor.Domain {
    public static class ExcelFormula {

        public static string SanitiseExcelFormula(string formula, IEnumerable<Tuple<string, string>> shims) {
            if (formula.Equals("-")) {
                return "'-'";
            }
            if (formula.Contains("%")) {// % is used to mark percentages - so divide by 100
                formula = DealwithPercentages(formula);
            }

            if (formula[0] == '=') {
                formula = formula.Substring(1); // remove the = to avoid NCalc playing up      
                foreach (var shim in shims) {
                    if (shim.Item1.Contains("XXXVALUEXXX")) {
                        var shimkey   = shim.Item1.Split(new [] {"XXXVALUEXXX"}, StringSplitOptions.None);
                        var shimvalue = shim.Item2.Split(new [] { "XXXVALUEXXX" }, StringSplitOptions.None);
                        string formula1 = formula;
                        if (shimkey.All(formula1.Contains)) {
                            for (int i = 0; i < shimkey.Length; i++) {
                                formula = formula.Replace(shimkey[i], shimvalue[i]);
                            }
                        }
                    } else {
                        formula = formula.Replace(shim.Item1, shim.Item2);
                    }                    
                }
                while (formula.Contains(",,")) { // apparently empty arguments are ok in excel
                    formula = formula.Replace(",,", ",");
                }
                while (formula.Contains("++")) { // apparently its fine to have A1++A2 in excel - its treated like a single +
                    formula = formula.Replace("++", "+");
                }
                while (formula.Contains("!!")) { // TODO check what sheet1!!a1 actually does
                    formula = formula.Replace("!!", "!");
                }
                formula = formula.Replace("TRUE", "true");
                formula = formula.Replace("FALSE", "false");                
                return formula;
            }

            // its a literal .. if its not a number then its a string we need to wrap
            if (!decimal.TryParse(formula, out _)) {
                if (double.TryParse(formula, out var adouble))
                { // this should handle scientific numbers
                    return adouble.ToString("F17");// output all digits
                } else
                {
                    return "'" + formula + "'";
                }
            }
            return formula;
        }

        public static string DealwithPercentages(string source) {            

            string[] sourcesplits = source.Split('\'');
            string destination = string.Empty;
            int i = 0;
            foreach (string sbs in sourcesplits) {
                destination += i % 2 == 0 ? sbs.Replace("%", "/100") : "'" + sbs + "'";//for the every odd value of i "%" is within single quotes 
                i++;
            } 

            return destination;
            //stackoverflow.com/questions/7529827/c-regex-replace-strings-only-outside-of-quotes
        }
    }
}
