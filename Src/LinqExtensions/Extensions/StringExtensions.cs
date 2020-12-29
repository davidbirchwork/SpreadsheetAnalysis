using System;

namespace LinqExtensions.Extensions {
    public static class StringExtensions {

        /// <summary>
        /// return the number of occurrences of the substring in the string
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="substring">The substring.</param>
        /// <returns>number of occurrences of substring in string</returns>
        public static int Occurrences(this string str, string substring) {
            string[] array = str.Split(new[] { substring }, StringSplitOptions.None);
            return array.Length-1;
        }
    }
}
