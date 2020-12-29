using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace LinqExtensions.Extensions {
    public static class DictionaryExtensions {

        /// <summary>
        /// Adds a key value pair if not the key is not existant in the dictionary.
        /// </summary>
        /// <typeparam name="TK">key type</typeparam>
        /// <typeparam name="TV">value type</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>true on successful insert</returns>
        public static bool AddIfNotExistant<TK,TV>(this Dictionary<TK,TV> dictionary, TK key, TV value) {
            if (dictionary.ContainsKey(key)) {
                return false;
            }
            dictionary.Add(key, value);
            return true;
        }

        public static bool AddIfNotExistant<TK, TV>(this ConcurrentDictionary<TK, TV> dictionary, TK key, TV value) {
            if (dictionary.ContainsKey(key)) {
                return false;
            }
            dictionary.AddOrUpdate(key, value,(k,v) => v);
            return true;
        }

        public static bool AddOrThrow<TK, TV>(this ConcurrentDictionary<TK, TV> dictionary, TK key, TV value) {
            if (dictionary.ContainsKey(key)) {
                return false;
            }
            dictionary.AddOrUpdate(key, value, (k, v) => throw new Exception("An element already Exists!"));
            return true;
        }

        public static void AddToDictionaryList<TK, TV>(this Dictionary<TK, List<TV>> dict, TK k, TV v) {
            if (dict.ContainsKey(k)) {
                dict[k].Add(v);
            } else {
                dict.Add(k, new List<TV>() { v });
            }
        }

        public static void AddToDictionaryList<TK, TV>(this Dictionary<TK, List<TV>> dict, TK k, IEnumerable<TV> v) {
            if (dict.ContainsKey(k)) {
                dict[k].AddRange(v);
            } else {
                dict.Add(k, new List<TV>(v));
            }
        }

        public static void AddRange<TK,TV>(this Dictionary<TK,TV> dict, IEnumerable<Tuple<TK,TV>> range) {
            foreach (Tuple<TK, TV> tuple in range) {
                dict.Add(tuple.Item1,tuple.Item2);
            }
        }

        public static void Remove<TK,TV>(this Dictionary<TK,TV> dict,IEnumerable<TK> keys) {
            foreach (TK key in keys) {
                dict.Remove(key);
            }
        }

        /// <summary>
        /// Removes items from the dictionary where they meet the filter
        /// </summary>
        /// <typeparam name="TK">The type of the Key.</typeparam>
        /// <typeparam name="TV">The type of the Value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="removeFilter">The predicate to remove item on</param>
        /// <returns></returns>
        public static IEnumerable<TK> RemoveWhere<TK, TV>(this Dictionary<TK, TV> dictionary, Func<KeyValuePair<TK, TV>,bool> removeFilter) {
            IEnumerable<TK> keysToRemove = dictionary.Where(removeFilter).Select(pair => pair.Key).ToArray();
            foreach (TK k in keysToRemove.Where(k => !dictionary.Remove(k))) {
                throw new Exception("Could not remove key " + k);
            }

            return keysToRemove;
        }

        /// <summary>
        /// Clones a dictionary trying to use IClonable upon values if availible
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static Dictionary<TK, TV> ValueClone<TK, TV>(this Dictionary<TK, TV> dict) // resharper            
            where TK : ICloneable {
            return dict.ToDictionary(pair => (TK) pair.Key.Clone(), pair => {
                                                                        ICloneable v = pair.Value as ICloneable;
                                                                        if (v != null) {
                                                                            return (TV) v.Clone();
                                                                        } else {
                                                                            return pair.Value; // its hopefully a value type
                                                                        }
                                                                    });
        }

        public static Dictionary<TK,Dictionary<TK,TV>> CloneVarMapDictionary<TK,TV>(this Dictionary<TK,Dictionary<TK,TV>> dict) 
        where TK : ICloneable {
            return dict.ToDictionary(p => (TK) p.Key.Clone(), p => p.Value.ValueClone());
        }

        /// <summary>
        /// Merges the specified dictionary returning a merged dict
        /// </summary>
        /// <typeparam name="TK">The type of the K.</typeparam>
        /// <typeparam name="TV">The type of the V.</typeparam>
        /// <param name="dict1">The dict1.</param>
        /// <param name="dict2">The dict2.</param>
        /// <returns></returns>
        public static Dictionary<TK,TV> Merge<TK,TV>(this Dictionary<TK,TV> dict1, Dictionary<TK,TV> dict2) {
            return dict1.Concat(dict2).ToDictionary(x => x.Key, x => x.Value);
        }

        public static void AddorUpdate<TK, TV>(this Dictionary<TK, TV> dict, TK key, TV value) {
            if (dict.ContainsKey(key)) {
                dict[key] = value;
            } else {
                dict.Add(key,value);
            }
       }
    }
}
