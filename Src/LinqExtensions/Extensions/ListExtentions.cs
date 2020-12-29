using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqExtensions.Extensions {
   public static class ListExtentions {

       /// <summary>
       /// Extension method to test whether or not two lists contain identical elements
       /// </summary>
       /// <typeparam name="T">type of the two lists</typeparam>
       /// <param name="list1">List1.</param>
       /// <param name="list2">List2.</param>
       /// <returns>true IFF lists contain identical elements</returns>
       public static bool ContainIdenticalElements<T>(this List<T> list1, List<T> list2) {
           if (list1.Count != list2.Count) {
               return false;
           }

           return list1.Count(elem => !list2.Contains(elem)) == 0;
       }
       
       public static void AddElementsNotInList<T>(this List<T> list, IEnumerable<T> range) {           
            list.AddRange(range.Where(t => !list.Contains(t)));           
       }

       /// <summary>
       /// Adds if not existant.
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="list">The list.</param>
       /// <param name="value">The value.</param>
       /// <returns>true if added</returns>
       public static bool AddIfNotExistant<T>(this List<T> list, T value) {
           if (list.Contains(value)) {
               return false;
           }
           list.Add(value);
           return true;
       }
       

       public static List<T> ValueClone<T>(this List<T> list) {
           return list.Select(elem => {
                               ICloneable v = elem as ICloneable;
                               if (v != null) {
                                   return (T)v.Clone();
                               } else {
                                   return elem; // its hopefully a value type
                               }
                       }).ToList();
       }
   }
}
