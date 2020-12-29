using System.Collections.Generic;
using System.Linq;

namespace LinqExtensions
{
    public static class LinqExtensions
    {

        /// <summary>
        /// Extension method to test whether or not two lists contain identical elements
        /// </summary>
        /// <typeparam name="T">type of the two lists</typeparam>
        /// <param name="list1">List1.</param>
        /// <param name="list2">List2.</param>
        /// <returns>true IFF lists contain identical elements</returns>
        public static bool ContainIdenticalElements<T>(this List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            return list1.Count(elem => !list2.Contains(elem)) == 0;
        }

    }
}
