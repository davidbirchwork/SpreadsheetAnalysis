namespace ExcelInterop.Domain {
    public static class ExcelErrors {
        //TODO deal with the rest of these errors and make sure the reader can read them accurately
        // ReSharper disable InconsistentNaming
        public const string NA = "-2146826246";
        public const string VALUE = "-2146826273";
        public const string NAN = "-2146826281";
        public const string DIV0 = "-2146826281";
        // ReSharper restore InconsistentNaming

        /*
         * <Cell><Data ss:Type="Error">#DIV/0!</Data></Cell>
<Cell><Data ss:Type="Error">#NUM!</Data></Cell>
<Cell><Data ss:Type="Error">#VALUE!</Data></Cell>
<Cell><Data ss:Type="Error">#N/A</Data></Cell>
<Cell><Data ss:Type="Error">#NAME?</Data></Cell>
<Cell><Data ss:Type="Error">#REF!</Data></Cell>
<Cell><Data ss:Type="Error">#NULL!</Data></Cell>

         */

        public static bool IsError(string val) {
            if (val.Equals(NA) || val.Equals(VALUE) || val.Equals(NAN) || val.Equals(DIV0)) {
                return true;
            }
            return false;
        }
    }
}
