using System;

namespace ExcelExtractor.Domain
{
   public static class ExcelEqualityComparison
    {
        public static bool ResultsAreEqual(string result, string expected) {
            const int sigfigs = 8;

            if (result == null ) {
                return expected == null;
            }

            if (result.Equals(expected)) return true;
            // first filter nclac wrappers
            result = result.Replace("\"", "").Replace("'", "");
            if (result.Equals(expected)) return true;
            result = result.Trim();// ncalc can add a space at the end of serializing an expression
            if (result.Equals(expected)) return true;
            // then see if its a boolean
            if (bool.TryParse(result, out var resultBool)) {
                return resultBool == bool.Parse(expected);
            }
            // then try decimal parsing

            if (decimal.TryParse(result, out var resultDecimal) && decimal.TryParse(expected, out var expectedDecimal)) {
                return resultDecimal.Equals(expectedDecimal) || EqualityToXsigfig(resultDecimal, expectedDecimal, sigfigs);
            }

            if (ExcelFormula.DealwithPercentages(result).Equals(ExcelFormula.DealwithPercentages(expected))) {
                return true;
            }

            if (decimal.TryParse(result, out resultDecimal) && double.TryParse(expected, out var expectDouble)) {
                decimal anexpectedDecimal = Convert.ToDecimal(expectDouble);

                return anexpectedDecimal == resultDecimal || EqualityToXsigfig(resultDecimal, anexpectedDecimal, sigfigs);
            }

            return false;
        }

        private static bool EqualityToXsigfig(decimal resultDecimal, decimal expectedDecimal, int sigfigs) {

            decimal resultSigfig = Math.Round(resultDecimal, sigfigs);
            decimal expectedSigfig = Math.Round(expectedDecimal, sigfigs);
            if (resultSigfig.Equals(expectedSigfig)) {
                return true;
            }

            // round to the min number of decimal places                                                
            string sres = resultDecimal.ToString("G17");
            string sexp = resultDecimal.ToString("G17");
            if (sres == sexp) return true;
            int extraplaces = 0;
            if (sres.Contains(".") || sexp.Contains(".")) {
                extraplaces++;
            }
            if (sres.Contains("-") || sexp.Contains("-")) {
                extraplaces++;
            }
            sexp = sexp.Replace("-", "");
            if (sexp.StartsWith("0.")) {
                extraplaces++;
                sexp = sexp.Replace("0.", "");
            }
            while (sexp.StartsWith("0")) {
                extraplaces++;
                sexp = sexp.Substring(1);
            }

            string rtrunc =  sres.Length> sigfigs + extraplaces ?   sres.Substring(0, sigfigs+extraplaces) :sres;
            string etrunc = sexp.Length > sigfigs + extraplaces ? sexp.Substring(0, sigfigs+extraplaces): sexp;

            return rtrunc.Equals(etrunc);
        }
    }
}
