using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using LinqExtensions.Extensions;
using Utilities.XmlSerialisation;

namespace Utilities_Test.XmlSerialisation {
    [TestClass]
    public class XmlSerialisationTest {

        public class TestClass : IEquatable<TestClass> {
            // ReSharper disable MemberCanBePrivate.Global
            // ReSharper disable FieldCanBeMadeReadOnly.Global
            // ReSharper disable ConvertToConstant.Global
            public string Name = "Fred";
            public int Age = 20;
            public XElement Element = new XElement("Child", "Fredia");
            // ReSharper restore ConvertToConstant.Global
            // ReSharper restore FieldCanBeMadeReadOnly.Global
            // ReSharper restore MemberCanBePrivate.Global

            public bool Equals(TestClass other) {
                return this.Name.Equals(other.Name) &&
                       this.Age.Equals(other.Age) &&
                       this.Element.DeepEquals(other.Element);
            }
        }        

        [TestMethod]
        public void TestXmlSerialisation() {
            string xml = SerialisationController.Serialize(new TestClass());

            TestClass testClass = SerialisationController.Deserialize<TestClass>(xml);
            Assert.IsTrue(testClass.Equals(new TestClass()));
        }

        public class TestClassTwo : IEquatable<TestClassTwo> {
            // ReSharper disable MemberCanBePrivate.Global
            // ReSharper disable FieldCanBeMadeReadOnly.Global
            // ReSharper disable ConvertToConstant.Global
            public string Name = "Fred";
            [XmlElement("Level1___Level2")]
            public int Age = 20;
            public XElement Element = new XElement("Child", "Fredia");
            // ReSharper restore ConvertToConstant.Global
            // ReSharper restore FieldCanBeMadeReadOnly.Global
            // ReSharper restore MemberCanBePrivate.Global

            public bool Equals(TestClassTwo other) {
                return this.Name.Equals(other.Name) &&
                       this.Age.Equals(other.Age) &&
                       this.Element.DeepEquals(other.Element);
            }
        }

        [TestMethod]
        public void TestNodeSplitting() {
            string xml = SerialisationController.Serialize(new TestClassTwo());
            Assert.IsTrue(xml.Contains("PropertyGroup"));
            TestClassTwo testClass = SerialisationController.Deserialize<TestClassTwo>(xml);
            Assert.IsTrue(testClass.Equals(new TestClassTwo()));
        }

        public class TestClassThree : IEquatable<TestClassThree> {
            // ReSharper disable MemberCanBePrivate.Global
            // ReSharper disable FieldCanBeMadeReadOnly.Global
            // ReSharper disable ConvertToConstant.Global
            public string Name = "Fred";
            [XmlElement("Level1___Level2___Level3")]
            public int Age = 20;
            public XElement Element = new XElement("Child", "Fredia");
            // ReSharper restore ConvertToConstant.Global
            // ReSharper restore FieldCanBeMadeReadOnly.Global
            // ReSharper restore MemberCanBePrivate.Global

            public bool Equals(TestClassThree other) {
                return this.Name.Equals(other.Name) &&
                       this.Age.Equals(other.Age) &&
                       this.Element.DeepEquals(other.Element);
            }
        }

        [TestMethod]
        public void TestNodeSplittingTwoLevel() {
            string xml = SerialisationController.Serialize(new TestClassThree());
            Assert.IsTrue(xml.Occurrences("PropertyGroup") ==2);
            TestClassThree testClass = SerialisationController.Deserialize<TestClassThree>(xml);
            Assert.IsTrue(testClass.Equals(new TestClassThree()));
        }

        public class TestClassFour : IEquatable<TestClassFour> {
            // ReSharper disable MemberCanBePrivate.Global
            // ReSharper disable FieldCanBeMadeReadOnly.Global
            // ReSharper disable ConvertToConstant.Global
            [XmlElement("Level1___Name")]
            public string Name = "Fred";
            [XmlElement("Level1___Level2___Level3___Age")]
            public int Age = 20;
            [XmlElement("Level1___Level2___Level3___elem")]
            public XElement Element = new XElement("Child", "Fredia");
            // ReSharper restore ConvertToConstant.Global
            // ReSharper restore FieldCanBeMadeReadOnly.Global
            // ReSharper restore MemberCanBePrivate.Global

            public bool Equals(TestClassFour other) {
                return this.Name.Equals(other.Name) &&
                       this.Age.Equals(other.Age) &&
                       this.Element.DeepEquals(other.Element);
            }
        }

        [TestMethod]
        public void TestNodeSplittingTwoLevelTwoItems() {
            string xml = SerialisationController.Serialize(new TestClassFour());
            Assert.AreEqual(3,xml.Occurrences("PropertyGroup"));
            Assert.AreEqual(2,xml.Occurrences("Level1")); // close tags
            Assert.AreEqual(2,xml.Occurrences("Level2"));
            Assert.AreEqual(2,xml.Occurrences("Level3"));
            TestClassFour testClass = SerialisationController.Deserialize<TestClassFour>(xml);
            Assert.IsTrue(testClass.Equals(new TestClassFour()));
        }
    }
}
