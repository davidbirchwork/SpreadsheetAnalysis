using System.Xml.Linq;
using LinqExtensions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Command;

namespace Utilities_Test.Command {
    /// <summary>
    /// To test <see cref="CommandHistory"/>
    /// an infinite variety of cases to test...    
    /// </summary>
    [TestClass]
    public class CommandHistoryTest {

        [TestInitialize]
        public void InitializeTest() {
            this.History = new CommandHistory();
            this.root = new XElement("root");
            this.FirstDo  = new AddAttribute(this.root, new XAttribute("a", "1"));
            this.SecondDo = new AddAttribute(this.root, new XAttribute("b", "2"));
            this.ThirdDo  = new AddAttribute(this.root, new XAttribute("c", "3"));
        }

// ReSharper disable InconsistentNaming
        private CommandHistory History;
        private XElement root;
        private ICommand FirstDo;
        private ICommand SecondDo;
        private ICommand ThirdDo;
// ReSharper restore InconsistentNaming

        [TestMethod]
        public void TestSingleExecute() {
            if (!History.Execute(this.FirstDo)) {
                Assert.Fail("Task Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            Assert.IsTrue(History.PrintHistory(false).Contains("attribute"));
            Assert.IsTrue(History.PrintHistory(true).Contains("attribute"));
            Assert.IsFalse(History.PrintUndoStack().Contains("attribute"));

            string fullHistory = History.PrintHistory(true);
            Assert.AreEqual(1, fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(0, fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(0, fullHistory.Occurrences("[REDO]"));
        }

        [TestMethod]
        public void TestDoubleExecute() {
            if (!History.Execute(this.FirstDo)) {
                Assert.Fail("Task1 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Execute(this.SecondDo)) {
                Assert.Fail("Task2 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));

            Assert.AreEqual(2,History.PrintHistory(false).Occurrences("attribute"));
            Assert.AreEqual(2,History.PrintHistory(true).Occurrences("attribute"));
            Assert.IsFalse(History.PrintUndoStack().Contains("attribute"));

            string fullHistory = History.PrintHistory(true);
            Assert.AreEqual(2, fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(0, fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(0, fullHistory.Occurrences("[REDO]"));
        }

        [TestMethod]
        public void TestSingleUndo() {
            if (!History.Execute(this.FirstDo)) {
                Assert.Fail("Task1 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Undo()) {
                Assert.Fail("Task1 Failed Undo");
            }
            Assert.IsNull(this.root.Attribute("a"));
            
            Assert.IsFalse(History.PrintHistory(false).Contains("attribute"));
            Assert.AreEqual(2,History.PrintHistory(true).Occurrences("attribute"));
            Assert.IsTrue(History.PrintUndoStack().Contains("attribute"));

            string fullHistory = History.PrintHistory(true);
            Assert.AreEqual(1, fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(1, fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(0, fullHistory.Occurrences("[REDO]"));
        }

        [TestMethod]
        public void TestDoubleUndo() {
            if (!History.Execute(this.FirstDo)) {
                Assert.Fail("Task1 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Execute(this.SecondDo)) {
                Assert.Fail("Task2 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));

            if (!History.Undo()) {
                Assert.Fail("Task2 Failed Undo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNull(this.root.Attribute("b"));

            if (!History.Undo()) {
                Assert.Fail("Task1 Failed Undo");
            }
            Assert.IsNull(this.root.Attribute("a"));
            Assert.IsNull(this.root.Attribute("b"));

            Assert.AreEqual(0,History.PrintHistory(false).Occurrences("attribute"));
            Assert.AreEqual(4,History.PrintHistory(true).Occurrences("attribute"));
            Assert.AreEqual(2,History.PrintUndoStack().Occurrences("attribute"));

            string fullHistory = History.PrintHistory(true);
            Assert.AreEqual(2, fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(2, fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(0, fullHistory.Occurrences("[REDO]"));
        }

        [TestMethod]
        public void TestDoAfterUndo() {
            if (!History.Execute(this.FirstDo)) {
                Assert.Fail("Task1 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Undo()) {
                Assert.Fail("Task1 Failed Undo");
            }
            Assert.IsNull(this.root.Attribute("a"));

            if (!History.Execute(this.SecondDo)) {
                Assert.Fail("Task2 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("b"));

            Assert.AreEqual(1,History.PrintHistory(false).Occurrences("attribute"));
            Assert.AreEqual(3,History.PrintHistory(true).Occurrences("attribute"));
            Assert.AreEqual(0,History.PrintUndoStack().Occurrences("attribute"));

            string fullHistory = History.PrintHistory(true);
            Assert.AreEqual(2, fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(1, fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(0, fullHistory.Occurrences("[REDO]"));
        }

        [TestMethod]
        public void TestSingleRedo() {
            if (!History.Execute(this.FirstDo)) {
                Assert.Fail("Task1 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Undo()) {
                Assert.Fail("Task1 Failed Undo");
            }
            Assert.IsNull(this.root.Attribute("a"));

            if (!History.Redo()) {
                Assert.Fail("Task1 Failed Redo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            Assert.AreEqual(1,History.PrintHistory(false).Occurrences("attribute"));
            Assert.AreEqual(3,History.PrintHistory(true).Occurrences("attribute"));
            Assert.AreEqual(0,History.PrintUndoStack().Occurrences("attribute"));

            string fullHistory = History.PrintHistory(true);
            Assert.AreEqual(1, fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(1, fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(1, fullHistory.Occurrences("[REDO]"));

        }

        [TestMethod]
        public void TestDoubleRedo() {
            if (!History.Execute(this.FirstDo)) {
                Assert.Fail("Task1 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Execute(this.SecondDo)) {
                Assert.Fail("Task2 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));

            if (!History.Undo()) {
                Assert.Fail("Task2 Failed Undo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNull(this.root.Attribute("b"));

            if (!History.Undo()) {
                Assert.Fail("Task1 Failed Undo");
            }
            Assert.IsNull(this.root.Attribute("a"));
            Assert.IsNull(this.root.Attribute("b"));

            if (!History.Redo()) {
                Assert.Fail("Task1 Failed Redo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNull(this.root.Attribute("b"));

            if (!History.Redo()) {
                Assert.Fail("Task2 Failed Redo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));

            string fullHistory = History.PrintHistory(true);

            Assert.AreEqual(2,History.PrintHistory(false).Occurrences("attribute"));
            Assert.AreEqual(6, fullHistory.Occurrences("attribute"));
            Assert.AreEqual(0,History.PrintUndoStack().Occurrences("attribute"));
            
            Assert.AreEqual(2,fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(2,fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(2,fullHistory.Occurrences("[REDO]"));
        }

        [TestMethod]
        public void TestDoAfterRedo() {
            if (!History.Execute(this.FirstDo)) {
                Assert.Fail("Task1 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Undo()) {
                Assert.Fail("Task1 Failed Undo");
            }
            Assert.IsNull(this.root.Attribute("a"));

            if (!History.Redo()) {
                Assert.Fail("Task1 Failed Redo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Execute(this.SecondDo)) {
                Assert.Fail("Task2 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));

            string fullHistory = History.PrintHistory(true);

            Assert.AreEqual(2, History.PrintHistory(false).Occurrences("attribute"));
            Assert.AreEqual(4, fullHistory.Occurrences("attribute"));
            Assert.AreEqual(0,History.PrintUndoStack().Occurrences("attribute"));

            Assert.AreEqual(2, fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(1, fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(1, fullHistory.Occurrences("[REDO]"));
        }

        [TestMethod]
        public void TestSingleUndoAfterRedo() {
            if (!History.Execute(this.FirstDo)) {
                Assert.Fail("Task1 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Undo()) {
                Assert.Fail("Task1 Failed Undo");
            }
            Assert.IsNull(this.root.Attribute("a"));

            if (!History.Redo()) {
                Assert.Fail("Task1 Failed Redo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Undo()) {
                Assert.Fail("Task1 Failed Undo");
            }
            Assert.IsNull(this.root.Attribute("a"));

            string shortHistory = History.PrintHistory(false);

            Assert.AreEqual(0, shortHistory.Occurrences("attribute"));
            Assert.AreEqual(4, History.PrintHistory(true).Occurrences("attribute"));
            Assert.AreEqual(1, History.PrintUndoStack().Occurrences("attribute"));

            Assert.AreEqual(1, History.PrintHistory(true).Occurrences("[DO]"));
            Assert.AreEqual(2, History.PrintHistory(true).Occurrences("[UNDO]"));
            Assert.AreEqual(1, History.PrintHistory(true).Occurrences("[REDO]"));
        }

        [TestMethod]
        public void TestDoubleUndoAfterSingleRedo() {
            // ie skipping back past an undone redo to find the next node to undo
            if (!History.Execute(this.FirstDo)) {
                Assert.Fail("Task1 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Execute(this.SecondDo)) {
                Assert.Fail("Task2 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));

            if (!History.Undo()) {
                Assert.Fail("Task2 Failed Undo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNull(this.root.Attribute("b"));

            if (!History.Redo()) {
                Assert.Fail("Task2 Failed Redo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));

            if (!History.Undo()) {
                Assert.Fail("Task2 Failed Undo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNull(this.root.Attribute("b"));

            if (!History.Undo()) {
                Assert.Fail("Task1 Failed Undo");
            }
            Assert.IsNull(this.root.Attribute("a"));
            Assert.IsNull(this.root.Attribute("b"));

            string fullHistory = History.PrintHistory(true);

            Assert.AreEqual(0, History.PrintHistory(false).Occurrences("attribute"));
            Assert.AreEqual(6, fullHistory.Occurrences("attribute"));
            Assert.AreEqual(2, History.PrintUndoStack().Occurrences("attribute"));

            Assert.AreEqual(2, fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(3, fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(1, fullHistory.Occurrences("[REDO]"));
        }

        [TestMethod]
        public void TestTripleUndoAfterDoubleRedo() {
            // ie skipping back past two undone redo's to find the next node to undo
            if (!History.Execute(this.FirstDo)) {
                Assert.Fail("Task1 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));

            if (!History.Execute(this.SecondDo)) {
                Assert.Fail("Task2 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));

            if (!History.Execute(this.ThirdDo)) {
                Assert.Fail("Task3 Failed");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));
            Assert.IsNotNull(this.root.Attribute("c"));

            if (!History.Undo()) {
                Assert.Fail("Task3 Failed Undo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));
            Assert.IsNull(this.root.Attribute("c"));

            if (!History.Undo()) {
                Assert.Fail("Task2 Failed Undo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNull(this.root.Attribute("b"));
            Assert.IsNull(this.root.Attribute("c"));

            if (!History.Redo()) {
                Assert.Fail("Task2 Failed Redo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));
            Assert.IsNull(this.root.Attribute("c"));

            if (!History.Redo()) {
                Assert.Fail("Task3 Failed Redo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));
            Assert.IsNotNull(this.root.Attribute("c"));

            if (!History.Undo()) {
                Assert.Fail("Task3 Failed Undo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNotNull(this.root.Attribute("b"));
            Assert.IsNull(this.root.Attribute("c"));

            if (!History.Undo()) {
                Assert.Fail("Task2 Failed Undo");
            }
            Assert.IsNotNull(this.root.Attribute("a"));
            Assert.IsNull(this.root.Attribute("b"));
            Assert.IsNull(this.root.Attribute("c"));

            if (!History.Undo()) {
                Assert.Fail("Task1 Failed Undo");
            }
            Assert.IsNull(this.root.Attribute("a"));
            Assert.IsNull(this.root.Attribute("b"));
            Assert.IsNull(this.root.Attribute("c"));

            string fullHistory = History.PrintHistory(true);

            Assert.AreEqual(0, History.PrintHistory(false).Occurrences("attribute"));
            Assert.AreEqual(10, fullHistory.Occurrences("attribute"));
            Assert.AreEqual(3, History.PrintUndoStack().Occurrences("attribute"));

            Assert.AreEqual(3, fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(5, fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(2, fullHistory.Occurrences("[REDO]"));
        }        

        [TestMethod]
        public void TestDeleteHistory() {
            TestTripleUndoAfterDoubleRedo();
            History.DeleteHistory();

            string fullHistory = History.PrintHistory(true);
            Assert.AreEqual(0, History.PrintHistory(false).Occurrences("attribute"));
            Assert.AreEqual(0, fullHistory.Occurrences("attribute"));
            Assert.AreEqual(0, History.PrintUndoStack().Occurrences("attribute"));

            Assert.AreEqual(0, fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(0, fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(0, fullHistory.Occurrences("[REDO]"));

        }

        [TestMethod]
        public void TestCleanHistory() {
            TestDoAfterRedo();
            History.Clean();

            string fullHistory = History.PrintHistory(true);
            Assert.AreEqual(2, History.PrintHistory(false).Occurrences("attribute"));
            Assert.AreEqual(2, fullHistory.Occurrences("attribute"));
            Assert.AreEqual(0, History.PrintUndoStack().Occurrences("attribute"));

            Assert.AreEqual(2, fullHistory.Occurrences("[DO]"));
            Assert.AreEqual(0, fullHistory.Occurrences("[UNDO]"));
            Assert.AreEqual(0, fullHistory.Occurrences("[REDO]"));

        }

    }
}
