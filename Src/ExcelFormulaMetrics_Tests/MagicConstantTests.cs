using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ExcelFormulaMetrics.Metrics;
using Graph;
using GraphMetrics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExcelFormulaTests {
    /// <summary>
    /// Summary description for MagicConstantTests
    /// </summary>
    [TestClass]
    public class MagicConstantTests {
        readonly MagicConstants _metric = new MagicConstants();        

        [TestMethod]
        public void TestIgnoreEndValue() {

            EvaluteMetric(new List<string> {"5.3", "\"hello\""});

            AssertFoundExactly(new List<string>());            

        }        

        [TestMethod]
        public void TestFindsConstants() {
            EvaluteMetric(new List<string> {"1.3+5.3"});

            AssertFoundExactly(new List<string> { "1.3" ,"5.3"});
            
        }

        [TestMethod]
        public void TestFindsOddConstants() {
            EvaluteMetric(new List<string> { "5.1+-5.1"});

            AssertFoundExactly(new List<string> { "5.1", "5.1" });

        }

        [TestMethod]
        public void TestFindsWithinFunctions() {
            EvaluteMetric(new List<string> { "Cos(5.1+-5.1)" });

            AssertFoundExactly(new List<string> { "5.1", "5.1" });

        }

        [TestMethod]
        public void TestTernary() {
            EvaluteMetric(new List<string> { "5=3 ? 5.3 : 6.6" });

            AssertFoundExactly(new List<string> { "5", "3", "5.3", "6.6" });            

        }

        [TestMethod]
        public void TestStrings() {
            EvaluteMetric(new List<string> { "5=3 ? 'Hello' : 'Goodbye'" });

            AssertFoundExactly(new List<string> { "5", "3", "'Hello'", "'Goodbye'" });            

        }

        [TestMethod]
        public void TestMix() {
            EvaluteMetric(new List<string> { "5+Cos(2^2)" });

            AssertFoundExactly(new List<string> { "5", "2", "2"});

        }
        [TestMethod]
        public void TestMix2() {
            EvaluteMetric(new List<string> { "5+Cos(2^3)-Sin(x+6)" });

            AssertFoundExactly(new List<string> { "5", "2", "3", "6" });

        }

        private void EvaluteMetric(IEnumerable<string> formulas) {
            IEnumerable<ExcelVertex> vertixes = formulas.Select(f => new ExcelVertex("test formula") {Formula = f});
            string result = _metric.Compute(new ConcurrentDictionary<string, IMetric>(), vertixes, new List<AEdge>(), new List<string>(),
                                                  new Dictionary<ExcelVertex, Tuple<List<ExcelVertex>, List<ExcelVertex>>>());
            Assert.IsNull(result);// not crashed            
        }

        private void AssertFoundExactly(ICollection<string> expected) {
            // doesn't test multiplicites... 
            Assert.AreEqual(_metric.NewConstantsFound.Count,expected.Count);
            foreach (string expectedValue in expected) {
                Assert.IsTrue(_metric.NewConstantsFound.Contains(expectedValue));
            }
        }
    }
}
