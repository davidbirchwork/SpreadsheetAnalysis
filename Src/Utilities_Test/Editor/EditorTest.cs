using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Editor;

namespace Utilities_Test.Editor {
    /// <summary>
    /// Summary description for EditorTest
    /// </summary>
    [TestClass]
    public class EditorTest {

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException),"expected it to signal the gui")]
        public void TestMethod1() {
            object testobj = (new TestClass() {Name = "TestingClass"});
            Assert.IsTrue(User.Edit("TestingClass", testobj, null, elem => true, new Utilities.Command.CommandHistory(), false));
        }
    }
}
