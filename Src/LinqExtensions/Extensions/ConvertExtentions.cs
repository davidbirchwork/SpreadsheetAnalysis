using System;

namespace LinqExtensions.Extensions {
    public static class ConvertExtentions {

        public static double? ToNullableDouble(this object value) {
            if (value == null) {
                return null;
            }

            try {
                double doubleValue = Convert.ToDouble(value);
                return doubleValue;
            } catch (Exception) {
                return null;
            }
        }
    }
}
