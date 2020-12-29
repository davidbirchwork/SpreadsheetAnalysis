namespace ExcelFunctions.ExcelFunctions {
    public static class ExcelErrors {
        // ReSharper disable InconsistentNaming
        public const string NA = "-2146826246";
        public const string VALUE = "-2146826273";
        public const string NAN = "-2146826281";
        // ReSharper restore InconsistentNaming
        public static bool IsError(string val) {
            if (val.Equals(NA) || val.Equals(VALUE) || val.Equals(NAN)) {
                return true;
            }
            return false;
        }
    }
}