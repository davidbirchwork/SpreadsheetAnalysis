using System.Collections.Generic;
using System.Linq;

namespace Algorithms {
    public static class FrequencyTable {

        /// <summary>
        /// Makes the frequency table, sorts it ascending or descending and optionally returns only N classes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="sortAscending">if set to <c>true</c> [sort ascending].</param>
        /// <param name="takeN">The take N.</param>
        /// <returns>sorted dictionary of frequency counts - sorted by number of items in class</returns>
        public static Dictionary<T,int> MakeFrequencyTable<T>(this IEnumerable<T> list,bool sortAscending,int takeN) {
            Dictionary<T, int> dict = new Dictionary<T, int>();

            foreach (T item in list) {
                if (!dict.ContainsKey(item)) {
                    dict.Add(item, 1);
                } else {
                    dict[item] = dict[item] + 1;
                }
            }

            IEnumerable<KeyValuePair<T, int>> res;
            if (sortAscending) {
                res = (from entry in dict orderby entry.Value ascending select entry);

            } else {
                res = (from entry in dict orderby entry.Value descending select entry);

            }
            if (takeN != 0) {
                res = res.Take(takeN);
            }
            return res.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
