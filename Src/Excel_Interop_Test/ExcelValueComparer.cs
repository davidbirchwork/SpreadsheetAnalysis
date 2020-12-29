using System;

namespace Excel_Interop_Test {
    public static class ExcelValueComparer {
        public static bool CompareExcelValues(string left, string right)
        {
            if (left == right) return true;

            if (double.TryParse(left, out var leftd) && double.TryParse(right, out var rightd)) {
                if (Math.Abs(leftd - rightd) < 0.001) {
                    return true;
                }
            }

            return false;
        }
    }
}