using LinqExtensions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilities_Test.Extensions {
    [TestClass]
    public class StringExtensionTests {
        [TestMethod]
        public void TestOccurrences() {
            Assert.AreEqual("a".Occurrences("b"), 0);
            Assert.AreEqual("aba".Occurrences("b"), 1);
            Assert.AreEqual("bbb".Occurrences("b"), 3);
            Assert.AreEqual("ababab".Occurrences("b"), 3);
            Assert.AreEqual("abab".Occurrences("ab"), 2);
            Assert.AreEqual("abab".Occurrences(""), 0);
        }
    }
}
