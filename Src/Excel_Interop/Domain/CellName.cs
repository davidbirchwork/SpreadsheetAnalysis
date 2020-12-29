using System;
using System.Linq;
using System.Reflection;

namespace ExcelInterop.Domain {
    [Serializable]
    public class CellName {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Column { get; private set; }
        public string Row { get; private set; }

        public int RowNum { get; private set; }

        public CellName() : this("A1") {
            
        }

        public CellName(string str) {
            string origstr = str;
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str)) {
                Log.Error("null or empty cell address " + origstr);
                //throw new ArgumentNullException(nameof(str),@"null or empty cell address " + origstr);
            }
            str = str.ToUpper();
            str = str.Replace("$", "");
            this.Column = "";
            while (str.Length > 0 && !"0123456789".Contains(str[0])) {
                if ("ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(str[0])) {
                    this.Column += str[0];
                } else {
                    Log.Error("Found a " + str[0] + " in a cellname! " + origstr);
                    throw new ArgumentException("Found a "+str[0]+" in a cellname! " + origstr);
                }
                str = str.Substring(1);
            }            
            this.Row = str;
            RowNum = int.Parse(this.Row);             // should be a valid number

            if (string.IsNullOrEmpty(this.Row) || string.IsNullOrWhiteSpace(this.Row)) {
                throw new ArgumentNullException(nameof(str), @"null or empty cell row address " + origstr);
            }

            if (string.IsNullOrEmpty(this.Column) || string.IsNullOrWhiteSpace(this.Column)) {
                if (str.Contains(":")) {
                    Log.Error("null or empty cell col address - please remove entire row references such as sheet!6:100 " + origstr);
                    //throw new ArgumentNullException(nameof(str), @"null or empty cell col address - please remove entire row references such as sheet!6:100 "+ origstr);
                } else {
                    Log.Error("Bad Cell address "+origstr);
                    //throw new ArgumentNullException(nameof(str), @"null or empty cell col address " + origstr);
                }
            }
        }

        public CellName OneRight() {
            return new CellName(IncColumn(this.Column) + this.Row);
        }

        public CellName OneLeft() {
            return new CellName(DecColumn(this.Column) + this.Row);
        }
        private string DecColumn(string column) {
            return column.ToUpperInvariant() == "A" ? "A" 
                : IntToColumn(this.ColumnNumber() - 1);
        }

        private string IncColumn(string column) {
            if (string.IsNullOrWhiteSpace(column)) {
                return "A";
            }
            if (!column.EndsWith("Z", StringComparison.CurrentCultureIgnoreCase)) {
                char lastLetter = column.Last();
                return column.Substring(0, column.Length - 1) + IncLetter(lastLetter);
            }
            return IncColumn(column.Substring(0, column.Length - 1)) + "A";
        }

        private static char IncLetter(char letter) {
            return (char)(((int)letter) + 1);
        }

        public CellName OneDown() {
            return new CellName(this.Column + (int.Parse(this.Row) + 1));
        }
        public CellName OneUp() {
            var r = int.Parse(this.Row);
            return r == 1 ? this : new CellName(this.Column + (r -1));
        }

        public override string ToString() {
            return this.Column + this.Row;
        }

        public static string IntToColumn(int n) {
            if (n <= 0) throw new ArgumentException(nameof(n));
            string res = "";

            while (n != 0) {
                int rem = n % 26;
                res = (char) (byte) (64 + (rem == 0 ? 26 : rem)) + res;
                n = n == 26 ? 0 : n / 26;
            }

            return res;
        }

        public int ColumnNumber() {
            return (int) Column
                .ToUpper()
                .Reverse()
                .Select(Convert.ToByte)
                .Select(t => t - 64)
                .Select((no, i) => 
                    Math.Pow(26, i) * no
                    )
                .Sum();
        }
        
    }
}
