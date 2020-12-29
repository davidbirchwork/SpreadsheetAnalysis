using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Algorithms {
    public static class WorkList {

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Recursively evaluates the set of items, to be used if the evaluation of one item depends upon another. 
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="evaluator">The evaluator - item to be eval'd, already eval'd items, return true/false and optionalyl add your item to eval'd items</param>        
        /// <returns>list of evaluated variables</returns>
        public static Dictionary<TKey, TResult> RecursiveEval<TKey, TItem, TResult>(
            Dictionary<TKey, TItem> items,
            Func<KeyValuePair<TKey, TItem>, Dictionary<TKey, TResult>, bool> evaluator) {
            return RecursiveEval(items, evaluator, new Dictionary<TKey, TResult>());
        }

        /// <summary>
        /// Recursively evaluates the set of items, to be used if the evaluation of one item depends upon another. 
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="evaluator">The evaluator - item to be eval'd, already eval'd items, return true/false and optionalyl add your item to eval'd items</param>
        /// <param name="evaluatedItems">a dictionary of evaluated items</param>
        /// <returns>list of evaluated variables</returns>
        public static Dictionary<TKey, TResult> RecursiveEval<TKey, TItem, TResult>(
            Dictionary<TKey, TItem> items,
            Func<KeyValuePair<TKey, TItem>, Dictionary<TKey, TResult>, bool> evaluator,
            Dictionary<TKey, TResult> evaluatedItems) {

            int initCount = evaluatedItems.Count;
            Dictionary<TKey, TItem> remainingItems = items; //.ToDictionary(kpv => kpv.Key, kpv => kpv.Value)
            int oldCount = -1;
            int count = 0;

            while (count != items.Count && count != oldCount) {
                // not finished and we've made some progress

                /*   if (evaluatedItems.Count == 0) { // optimisation - could be made parallel
                       foreach (KeyValuePair<TKey, TItem> item in
                           items.Where(item => !evaluator.Invoke(item, evaluatedItems))) {
                           remainingItems.Add(item.Key, item.Value);
                       }
                   } else { // optimisation - could be made parallel*/
                //List<TKey> completedItems = (from item in remainingItems where evaluator.Invoke(item, evaluatedItems) select item.Key).ToList();
                //remainingItems.Remove(completedItems);

                remainingItems =
                    (from item in remainingItems where !evaluator.Invoke(item, evaluatedItems) select item)
                    .ToDictionary(item => item.Key, item => item.Value);

                //}

                oldCount = count;
                count = evaluatedItems.Count - initCount;
            }
            //test if we have processed all of the elements 
            if (remainingItems.Count>0 || evaluatedItems.Count - initCount < items.Count) {
                Log.Error(remainingItems.Keys.Aggregate("Likely encountered a circular reference :( \n",
                    (acc, key) => acc + "\n" + key.ToString()));
            }

            return evaluatedItems;
        }

        /// <summary>
        /// Recursively evaluates the set of items, to be used if the evaluation of one item depends upon another. 
        /// note that the items dictionary must not be modified by the item
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="evaluator">The evaluator - item to be eval'd, already eval'd items, return true/false and optionalyl add your item to eval'd items</param>        
        /// <returns>list of evaluated variables</returns>
        public static ConcurrentDictionary<TKey, TResult> ParallelRecursiveEval<TKey, TItem, TResult>(
            IDictionary<TKey, TItem> items,
            Func<KeyValuePair<TKey, TItem>, ConcurrentDictionary<TKey, TResult>, bool> evaluator) {

            ConcurrentDictionary<TKey, TResult> evaluatedItems = new ConcurrentDictionary<TKey, TResult>();
            ConcurrentDictionary<TKey, TItem> remainingItems = new ConcurrentDictionary<TKey, TItem>();
            int oldCount = -1;
            int count = 0;

            if (items.Count == 0) {
                return evaluatedItems;
            }

            while (count != items.Count && count != oldCount) {
                // not finished and we've made some progress

                if (evaluatedItems.Count == 0) {
                    Parallel.ForEach(items.Where(item => !evaluator.Invoke(item, evaluatedItems)),
                        item => remainingItems.AddOrUpdate(item.Key, item.Value, (k, v) => v)
                    );
                }
                else {
                    List<TKey> completedItems = (from item in remainingItems.AsParallel()
                        where evaluator.Invoke(item, evaluatedItems)
                        select item.Key).ToList();
                    foreach (TKey completedItem in completedItems) {
                        TItem dummy;
                        remainingItems.TryRemove(completedItem, out dummy);
                    }
                }

                oldCount = count;
                count = evaluatedItems.Count;
            }

            if (evaluatedItems.Count != items.Count) {
                Log.Error(remainingItems.Aggregate("Likely encountered a circular reference :( \n",
                    (acc, next) => acc + "\n" + next.Key.ToString() + " = " + next.Value.ToString()));
                // now one last go! (for debugging)
                Parallel.ForEach(items.Where(item => !evaluator.Invoke(item, evaluatedItems)),
                    item => remainingItems.AddOrUpdate(item.Key, item.Value, (k, v) => v)
                );
            }

            return evaluatedItems;
        }

        /// <summary>
        /// Recursively evaluates the set of items, to be used if the evaluation of one item depends upon another. 
        /// note that the items dictionary must not be modified by the item
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="evaluator">The evaluator - item to be eval'd, already eval'd items, return why you can't execute or null that you have and optionalyl add your item to eval'd items</param>
        /// <param name="debugFunc"> called only if printing debug info</param>
        /// <returns>list of evaluated variables</returns>
        public static ConcurrentDictionary<TKey, TResult> ParallelRecursiveEvalDebug<TKey, TItem, TResult>(
            IDictionary<TKey, TItem> items,
            Func<KeyValuePair<TKey, TItem>, ConcurrentDictionary<TKey, TResult>, string> evaluator,
            Func<TItem, string> debugFunc = null) {

            ConcurrentDictionary<TKey, TResult> evaluatedItems = new ConcurrentDictionary<TKey, TResult>();
            ConcurrentDictionary<TKey, TItem> remainingItems = new ConcurrentDictionary<TKey, TItem>();
            int oldCount = -1;
            int count = 0;

            if (items.Count == 0) {
                return evaluatedItems;
            }

            while (count != items.Count && count != oldCount) {
                // not finished and we've made some progress

                if (evaluatedItems.Count == 0) {
                    Parallel.ForEach(items.Where(item => evaluator.Invoke(item, evaluatedItems) != null),
                        item => remainingItems.AddOrUpdate(item.Key, item.Value, (k, v) => v)
                    );
                }
                else {
                    List<TKey> completedItems = (from item in remainingItems.AsParallel()
                        where evaluator.Invoke(item, evaluatedItems) == null
                        select item.Key).ToList();
                    foreach (TKey completedItem in completedItems) {
                        TItem dummy;
                        remainingItems.TryRemove(completedItem, out dummy);
                    }
                }

                oldCount = count;
                count = evaluatedItems.Count;
            }

            if (evaluatedItems.Count != items.Count) {
                if (debugFunc == null) debugFunc = i => i.ToString();
                Log.Error(remainingItems.Aggregate(
                    "Likely encountered a circular reference :( \n Progress=" + evaluatedItems.Count + "/" +
                    items.Count,
                    (acc, next) => acc + "\n" + next.Key.ToString() + " = " + debugFunc(next.Value) + " why= " +
                                   evaluator.Invoke(next, evaluatedItems)));
            }

            return evaluatedItems;
        }
    }
}