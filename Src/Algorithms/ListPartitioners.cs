using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms {
    public static class ListPartitioners {
        public static List<List<T>> ContiguousPartitioner<T>(this List<T> list, Func<List<T>, T, bool> adjacencyTest) {
            // i am sure this could be beautified 
            var partitions = new List<List<T>>();
            var partition = new List<T>();
            foreach (var t in list) {
                if (!partition.Any()) {
                    partition.Add(t);
                    continue;
                }

                if (adjacencyTest(partition, t)) {
                    partition.Add(t);
                }
                else {
                    partitions.Add(partition);
                    partition = new List<T> {t};
                }
            }

            if (partition.Any()) {
                partitions.Add(partition);
            }

            return partitions;
        }
    }
}