using Sp.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace Sp.Data.Tests
{
    
    
    /// <summary>
    ///This is a test class for SpConnectionTest and is intended
    ///to contain all SpConnectionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpConnectionTest
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
        ///A test for SpConnection Constructor
        ///</summary>
        [TestMethod()]
        public void SpConnectionConstructorTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for BeginTransaction
        ///</summary>
        [TestMethod()]
        public void BeginTransactionTest1()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server); // TODO: Initialize to an appropriate value
            IDbTransaction expected = null; // TODO: Initialize to an appropriate value
            IDbTransaction actual;
            actual = target.BeginTransaction();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for BeginTransaction
        ///</summary>
        [TestMethod()]
        public void BeginTransactionTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server); // TODO: Initialize to an appropriate value
            IsolationLevel il = new IsolationLevel(); // TODO: Initialize to an appropriate value
            IDbTransaction expected = null; // TODO: Initialize to an appropriate value
            IDbTransaction actual;
            actual = target.BeginTransaction(il);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ChangeDatabase
        ///</summary>
        [TestMethod()]
        public void ChangeDatabaseTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server); // TODO: Initialize to an appropriate value
            string databaseName = string.Empty; // TODO: Initialize to an appropriate value
            target.ChangeDatabase(databaseName);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Close
        ///</summary>
        [TestMethod()]
        public void CloseTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server); // TODO: Initialize to an appropriate value
            target.Close();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CreateCommand
        ///</summary>
        [TestMethod()]
        public void CreateCommandTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server); // TODO: Initialize to an appropriate value
            IDbCommand expected = null; // TODO: Initialize to an appropriate value
            IDbCommand actual;
            actual = target.CreateCommand();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Open
        ///</summary>
        [TestMethod()]
        public void OpenTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server); // TODO: Initialize to an appropriate value
            target.Open();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ConnectionString
        ///</summary>
        [TestMethod()]
        public void ConnectionStringTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.ConnectionString = expected;
            actual = target.ConnectionString;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ConnectionTimeout
        ///</summary>
        [TestMethod()]
        public void ConnectionTimeoutTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.ConnectionTimeout;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Database
        ///</summary>
        [TestMethod()]
        public void DatabaseTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Database;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for State
        ///</summary>
        [TestMethod()]
        public void StateTest()
        {
            string server = string.Empty; // TODO: Initialize to an appropriate value
            SpConnection target = new SpConnection(server); // TODO: Initialize to an appropriate value
            ConnectionState actual;
            actual = target.State;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
