using System.Collections.Generic;
using ExcelInterop.Domain;
using LinqExtensions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Excel_Interop_Test
{
    [TestClass]
    public class RangeExpansions
    {
        [TestMethod]
        public void Test1CellExpand()
        {
            // 1 cell range
            Assert.IsTrue(ExcelAddress.ExpandRangeToExcelAddresses(new ExcelAddress("Sheet1", "A1:A1")).
                ContainIdenticalElements(new List<ExcelAddress> {
                                                                    new ExcelAddress("Sheet1","A1")
                                                                }));
        }

        [TestMethod]
        public void Test1RowExpand()
        {
            Assert.IsTrue(ExcelAddress.ExpandRangeToExcelAddresses(new ExcelAddress("Sheet1", "A1:C1")).
                ContainIdenticalElements(new List<ExcelAddress> {
                                                                    new ExcelAddress("Sheet1","A1"),
                                                                    new ExcelAddress("Sheet1","B1"),
                                                                    new ExcelAddress("Sheet1","C1")
                                                                }));
        }

        [TestMethod]
        public void Test1ColExpand()
        {
            Assert.IsTrue(ExcelAddress.ExpandRangeToExcelAddresses(new ExcelAddress("Sheet1", "A1:A3")).
                ContainIdenticalElements(new List<ExcelAddress> {
                                                                    new ExcelAddress("Sheet1","A1"),
                                                                    new ExcelAddress("Sheet1","A2"),
                                                                    new ExcelAddress("Sheet1","A3")
                                                                }));
        }

        [TestMethod]
        public void TestSquareExpand()
        {
            List<ExcelAddress> expansion = ExcelAddress.ExpandRangeToExcelAddresses(new ExcelAddress("Sheet1", "A1:B2"));
            Assert.IsTrue(expansion.ContainIdenticalElements(new List<ExcelAddress> {
                                                                    new ExcelAddress("Sheet1","A1"),
                                                                    new ExcelAddress("Sheet1","A2"),
                                                                    new ExcelAddress("Sheet1","B1"),
                                                                    new ExcelAddress("Sheet1","B2")
                                                                }));
        }

        [TestMethod]
        public void TestWideRectangleExpand()
        {
            Assert.IsTrue(ExcelAddress.ExpandRangeToExcelAddresses(new ExcelAddress("Sheet1", "A1:C2")).
                ContainIdenticalElements(new List<ExcelAddress> {
                                                                    new ExcelAddress("Sheet1","A1"),
                                                                    new ExcelAddress("Sheet1","A2"),
                                                                    new ExcelAddress("Sheet1","B1"),
                                                                    new ExcelAddress("Sheet1","B2"),
                                                                    new ExcelAddress("Sheet1","C1"),
                                                                    new ExcelAddress("Sheet1","C2")
                                                                }));
        }

        [TestMethod]
        public void TestTallRectangleExpand()
        {
            Assert.IsTrue(ExcelAddress.ExpandRangeToExcelAddresses(new ExcelAddress("Sheet1", "A1:B3")).
                ContainIdenticalElements(new List<ExcelAddress> {
                                                                    new ExcelAddress("Sheet1","A1"),
                                                                    new ExcelAddress("Sheet1","A2"),
                                                                    new ExcelAddress("Sheet1","A3"),
                                                                    new ExcelAddress("Sheet1","B1"),
                                                                    new ExcelAddress("Sheet1","B2"),
                                                                    new ExcelAddress("Sheet1","B3")
                                                                }));
        }

        [TestMethod]
        public void TestColumnIncrement()
        {
            Assert.AreEqual((new CellName("B1")).ToString(), (new CellName("A1")).OneRight().ToString());
            Assert.AreEqual((new CellName("AB1")).ToString(), (new CellName("AA1")).OneRight().ToString());
            Assert.AreEqual((new CellName("AF1")).ToString(), (new CellName("AE1")).OneRight().ToString());
            Assert.AreEqual((new CellName("AA1")).ToString(), (new CellName("Z1")).OneRight().ToString());
            Assert.AreEqual((new CellName("CA1")).ToString(), (new CellName("BZ1")).OneRight().ToString());
            Assert.AreEqual((new CellName("AAA1")).ToString(), (new CellName("ZZ1")).OneRight().ToString());
            Assert.AreEqual((new CellName("ACA1")).ToString(), (new CellName("ABZ1")).OneRight().ToString());
            Assert.AreEqual((new CellName("AAAA1")).ToString(), (new CellName("ZZZ1")).OneRight().ToString());
            Assert.AreEqual((new CellName("AAAAA1")).ToString(), (new CellName("ZZZZ1")).OneRight().ToString());

            Assert.AreEqual((new CellName("b1")).ToString(), (new CellName("a1")).OneRight().ToString());
            Assert.AreEqual((new CellName("AAA1")).ToString(), (new CellName("zz1")).OneRight().ToString());
        }
    }
}
