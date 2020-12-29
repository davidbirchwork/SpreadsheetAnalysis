using System;

namespace LinqExtensions.Extensions {
    public static class ArrayExtensions {

        /// <summary>
        /// Finds the first index that matches the predicate or returns -1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static int FirstIndexOf<T>(this T[] array, Predicate<T> predicate) {
            if (array.Length == 0) {
                return -1;
            }

            int i = 0;
            while (i<array.Length && !predicate(array[i])) {
                i++;
            }

            return i == array.Length ? -1 : i;
        }
    }
}
