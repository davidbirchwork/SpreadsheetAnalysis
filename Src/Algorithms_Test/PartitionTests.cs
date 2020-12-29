using System;
using System.Linq;
using Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algorithms_Test {
    [TestClass]
    public class PartitionTests {
        [TestMethod]
        public void TestPartitionWithTest() {
            var partitions = PartitionAlgorithms.PartitionWithTest(Enumerable.Range(1, 1000), i => i.ToString(),
                (dict,setA, setB) => setA.Any(i => int.Parse(i) % 2 == 0) == setB.Any(i => int.Parse(i) % 2 == 0));
            Assert.AreEqual(partitions.Count, 2, "failed to partition odd and even numbers");
        }

        [TestMethod]
        public void TestPartitionWithTestComplex() {
            for (int s = 5; s < 16; s++) {
                var s1 = s;
                var partitions = PartitionAlgorithms.PartitionWithTest(Enumerable.Range(1, 1000), i => i.ToString(),
                    (dict,setA, setB) => int.Parse(setA.First()) % s1 == int.Parse(setB.First()) % s1);
                Assert.AreEqual(partitions.Count, s, "failed to partition");
            }


        }
    }
}
