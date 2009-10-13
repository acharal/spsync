using Sp.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Collections.Generic;
using System.Data;
using System;

namespace Sp.Data.Tests
{
    
    
    /// <summary>
    ///This is a test class for SpDataReaderTest and is intended
    ///to contain all SpDataReaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpDataReaderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for SpDataReader Constructor
        ///</summary>
        [TestMethod()]
        public void SpDataReaderConstructorTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Close
        ///</summary>
        [TestMethod()]
        public void CloseTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            target.Close();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetBoolean
        ///</summary>
        [TestMethod()]
        public void GetBooleanTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.GetBoolean(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetByte
        ///</summary>
        [TestMethod()]
        public void GetByteTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            byte expected = 0; // TODO: Initialize to an appropriate value
            byte actual;
            actual = target.GetByte(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBytes
        ///</summary>
        [TestMethod()]
        public void GetBytesTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            long fieldOffset = 0; // TODO: Initialize to an appropriate value
            byte[] buffer = null; // TODO: Initialize to an appropriate value
            int bufferoffset = 0; // TODO: Initialize to an appropriate value
            int length = 0; // TODO: Initialize to an appropriate value
            long expected = 0; // TODO: Initialize to an appropriate value
            long actual;
            actual = target.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetChar
        ///</summary>
        [TestMethod()]
        public void GetCharTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            char expected = '\0'; // TODO: Initialize to an appropriate value
            char actual;
            actual = target.GetChar(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetChars
        ///</summary>
        [TestMethod()]
        public void GetCharsTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            long fieldoffset = 0; // TODO: Initialize to an appropriate value
            char[] buffer = null; // TODO: Initialize to an appropriate value
            int bufferoffset = 0; // TODO: Initialize to an appropriate value
            int length = 0; // TODO: Initialize to an appropriate value
            long expected = 0; // TODO: Initialize to an appropriate value
            long actual;
            actual = target.GetChars(i, fieldoffset, buffer, bufferoffset, length);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetData
        ///</summary>
        [TestMethod()]
        public void GetDataTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            IDataReader expected = null; // TODO: Initialize to an appropriate value
            IDataReader actual;
            actual = target.GetData(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDataTypeName
        ///</summary>
        [TestMethod()]
        public void GetDataTypeNameTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetDataTypeName(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDateTime
        ///</summary>
        [TestMethod()]
        public void GetDateTimeTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            actual = target.GetDateTime(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDecimal
        ///</summary>
        [TestMethod()]
        public void GetDecimalTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            Decimal expected = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal actual;
            actual = target.GetDecimal(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDouble
        ///</summary>
        [TestMethod()]
        public void GetDoubleTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            actual = target.GetDouble(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetFieldType
        ///</summary>
        [TestMethod()]
        public void GetFieldTypeTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            Type expected = null; // TODO: Initialize to an appropriate value
            Type actual;
            actual = target.GetFieldType(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetFloat
        ///</summary>
        [TestMethod()]
        public void GetFloatTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = target.GetFloat(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetGuid
        ///</summary>
        [TestMethod()]
        public void GetGuidTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            Guid expected = new Guid(); // TODO: Initialize to an appropriate value
            Guid actual;
            actual = target.GetGuid(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInt16
        ///</summary>
        [TestMethod()]
        public void GetInt16Test()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            short expected = 0; // TODO: Initialize to an appropriate value
            short actual;
            actual = target.GetInt16(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInt32
        ///</summary>
        [TestMethod()]
        public void GetInt32Test()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetInt32(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInt64
        ///</summary>
        [TestMethod()]
        public void GetInt64Test()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            long expected = 0; // TODO: Initialize to an appropriate value
            long actual;
            actual = target.GetInt64(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetName
        ///</summary>
        [TestMethod()]
        public void GetNameTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetName(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetOrdinal
        ///</summary>
        [TestMethod()]
        public void GetOrdinalTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetOrdinal(name);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSchemaTable
        ///</summary>
        [TestMethod()]
        public void GetSchemaTableTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            DataTable expected = null; // TODO: Initialize to an appropriate value
            DataTable actual;
            actual = target.GetSchemaTable();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetString
        ///</summary>
        [TestMethod()]
        public void GetStringTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetString(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetValue
        ///</summary>
        [TestMethod()]
        public void GetValueTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.GetValue(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetValues
        ///</summary>
        [TestMethod()]
        public void GetValuesTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            object[] values = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetValues(values);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsDBNull
        ///</summary>
        [TestMethod()]
        public void IsDBNullTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsDBNull(i);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NextResult
        ///</summary>
        [TestMethod()]
        public void NextResultTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.NextResult();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseRecord
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Sp.Data.dll")]
        public void ParseRecordTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            SpDataReader_Accessor target = new SpDataReader_Accessor(param0); // TODO: Initialize to an appropriate value
            IDictionary<string, string> expected = null; // TODO: Initialize to an appropriate value
            IDictionary<string, string> actual;
            actual = target.ParseRecord();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Read
        ///</summary>
        [TestMethod()]
        public void ReadTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Read();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Depth
        ///</summary>
        [TestMethod()]
        public void DepthTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.Depth;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FieldCount
        ///</summary>
        [TestMethod()]
        public void FieldCountTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.FieldCount;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsClosed
        ///</summary>
        [TestMethod()]
        public void IsClosedTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsClosed;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        [TestMethod()]
        public void ItemTest1()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            object actual;
            actual = target[name];
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        [TestMethod()]
        public void ItemTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int i = 0; // TODO: Initialize to an appropriate value
            object actual;
            actual = target[i];
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RecordsAffected
        ///</summary>
        [TestMethod()]
        public void RecordsAffectedTest()
        {
            XmlReader xmlReader = null; // TODO: Initialize to an appropriate value
            SpDataReader target = new SpDataReader(xmlReader); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.RecordsAffected;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
