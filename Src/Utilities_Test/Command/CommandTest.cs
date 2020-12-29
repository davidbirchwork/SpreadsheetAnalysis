using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Command;

namespace Utilities_Test.Command {
    /// <summary>
    /// Summary description for Command_Test
    /// </summary>
    [TestClass]
    public class CommandTest {       

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestAddAttribute() {
            XElement elem = new XElement("Elem");
            XAttribute attrib = new XAttribute("Prop1", "value1");
            ICommand addAttribute = new AddAttribute(elem, attrib);
            addAttribute.Execute();
            Assert.IsTrue(elem.FirstAttribute.Equals(attrib));
        }

        [TestMethod]
        public void TestUndoAddAttribute() {
            XElement elem = new XElement("Elem");
            XAttribute attrib = new XAttribute("Prop1", "value1");
            ICommand addAttribute = new AddAttribute(elem, attrib);
            addAttribute.Execute();
            addAttribute.UndoCommand.Execute();
            Assert.IsTrue(elem.FirstAttribute == null);
        }

        [TestMethod]
        public void TestRedoAddAttribute() {
            XElement elem = new XElement("Elem");
            XAttribute attrib = new XAttribute("Prop1", "value1");
            ICommand addAttribute = new AddAttribute(elem, attrib);
            addAttribute.Execute();
            ICommand undocommand = addAttribute.UndoCommand;
            undocommand.Execute();
            undocommand.UndoCommand.Execute();
            Assert.IsTrue(elem.FirstAttribute.Equals(attrib));
        }
    }
}
