using SpServerSync.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace SpServerSync.Data.Tests
{
    
    
    /// <summary>
    ///This is a test class for SpSyncAnchorTest and is intended
    ///to contain all SpSyncAnchorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpSyncAnchorTest
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
        ///A test for PagingToken
        ///</summary>
        [TestMethod()]
        public void PagingTokenTest()
        {
            SpSyncAnchor target = new SpSyncAnchor(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.PagingToken = expected;
            actual = target.PagingToken;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PageNumber
        ///</summary>
        [TestMethod()]
        public void PageNumberTest()
        {
            SpSyncAnchor target = new SpSyncAnchor(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.PageNumber = expected;
            actual = target.PageNumber;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NextChangesToken
        ///</summary>
        [TestMethod()]
        public void NextChangesTokenTest()
        {
            SpSyncAnchor target = new SpSyncAnchor(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.NextChangesToken = expected;
            actual = target.NextChangesToken;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NextChangesAnchor
        ///</summary>
        [TestMethod()]
        public void NextChangesAnchorTest()
        {
            SpSyncAnchor target = new SpSyncAnchor(); // TODO: Initialize to an appropriate value
            SpSyncAnchor expected = null; // TODO: Initialize to an appropriate value
            SpSyncAnchor actual;
            target.NextChangesAnchor = expected;
            actual = target.NextChangesAnchor;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBytes
        ///</summary>
        [TestMethod()]
        public void GetBytesTest()
        {
            SpSyncAnchor anchor = null; // TODO: Initialize to an appropriate value
            byte[] expected = null; // TODO: Initialize to an appropriate value
            byte[] actual;
            actual = SpSyncAnchor.GetBytes(anchor);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAnchor
        ///</summary>
        [TestMethod()]
        public void GetAnchorTest()
        {
            byte[] buffer = null; // TODO: Initialize to an appropriate value
            SpSyncAnchor expected = null; // TODO: Initialize to an appropriate value
            SpSyncAnchor actual;
            actual = SpSyncAnchor.GetAnchor(buffer);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SpSyncAnchor Constructor
        ///</summary>
        [TestMethod()]
        public void SpSyncAnchorConstructorTest3()
        {
            string changeToken = string.Empty; // TODO: Initialize to an appropriate value
            SpSyncAnchor target = new SpSyncAnchor(changeToken);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for SpSyncAnchor Constructor
        ///</summary>
        [TestMethod()]
        public void SpSyncAnchorConstructorTest2()
        {
            SpSyncAnchor target = new SpSyncAnchor();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for SpSyncAnchor Constructor
        ///</summary>
        [TestMethod()]
        public void SpSyncAnchorConstructorTest1()
        {
            string changeToken = string.Empty; // TODO: Initialize to an appropriate value
            string pageToken = string.Empty; // TODO: Initialize to an appropriate value
            int num = 0; // TODO: Initialize to an appropriate value
            SpSyncAnchor target = new SpSyncAnchor(changeToken, pageToken, num);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for SpSyncAnchor Constructor
        ///</summary>
        [TestMethod()]
        public void SpSyncAnchorConstructorTest()
        {
            string changeToken = string.Empty; // TODO: Initialize to an appropriate value
            string pageToken = string.Empty; // TODO: Initialize to an appropriate value
            SpSyncAnchor target = new SpSyncAnchor(changeToken, pageToken);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
