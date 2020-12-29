using System;
using System.Collections.Generic;

namespace Algorithms {
    public static class GraphAlgorithms {
        /// <summary>
        /// Gather a list of nodes by walking from a node - only stop if a condition is true
        /// </summary>
        /// <param name="root"></param>
        /// <param name="getChildren"></param>
        /// <param name="follow"></param>
        /// <param name="include">include the node in what is returned</param>
        /// <param name="includeRoot"></param>
        /// <param name="testRoot"></param>
        /// <returns></returns>
        public static List<TN> WalkUntilTrue<TN>(TN root, Func<TN, IEnumerable<TN>> getChildren, Func<TN, bool> follow,
            Func<TN, bool> include, bool includeRoot = false, bool testRoot = false) {
            var nodes = new List<TN>();
            var toTest = new Stack<TN>();
            if (includeRoot) {
                if (!testRoot || include(root)) {
                    nodes.Add(root);
                }
            }

            if (testRoot && !follow(root)) {// no where to go 
                return nodes;
            }

            foreach (var child in getChildren(root)) {
                toTest.Push(child);
            }

            while (toTest.Count > 0) {
                var next = toTest.Pop();
                if (include(next)) {
                    nodes.Add(next);
                }

                if (follow(next)) {
                    foreach (var child in getChildren(next)) {
                        toTest.Push(child);
                    }
                }
            }

            return nodes;
        }

        public static bool PathTo<TN>(TN root, Func<TN, IEnumerable<TN>> getChildren, Func<TN, bool> follow,
            Func<TN, bool> isMatch, bool testRoot = false) {
            if (testRoot && isMatch(root)) return true;

            var toTest = new Stack<TN>();//todo a queue may be better here so that one does breadth first not depth first

            foreach (var child in getChildren(root)) {
                toTest.Push(child);
            }

            while (toTest.Count > 0) {
                var next = toTest.Pop();
                if (isMatch(next)) {
                    return true;
                }

                if (!follow(next)) continue;
                foreach (var child in getChildren(next)) {
                    toTest.Push(child);
                }
            }

            return false;

        }
    }
}