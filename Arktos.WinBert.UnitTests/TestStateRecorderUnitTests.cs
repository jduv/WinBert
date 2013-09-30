namespace Arktos.WinBert.UnitTests
{
    using Arktos.WinBert.Instrumentation;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class TestStateRecorderUnitTests
    {
        #region Test Methods

        #region StartTest

        [TestMethod]
        public void StartTest_HappyPath()
        {
            var target = new TestStateRecorder();
            target.StartTest();

            Assert.AreEqual(0U, target.TestCounter);
            Assert.AreEqual(0U, target.MethodCounter);
            Assert.IsNotNull(target.CurrentTest);

            // Test executions should be added at the EndTest call.
            Assert.AreEqual(0, target.TestExeuctions.Count());
        }

        #endregion

        #region EndTest

        [TestMethod]
        public void EndTest_CorrectState()
        {
            var target = new TestStateRecorder();
            target.StartTest();
            target.EndTest();

            Assert.AreEqual(1U, target.TestCounter);
            Assert.AreEqual(0U, target.MethodCounter);
            Assert.IsNull(target.CurrentTest);

            // Test executions should be added at the EndTest call.
            Assert.AreEqual(1, target.TestExeuctions.Count());

            // Analysis log should be populated
            Assert.IsNotNull(target.AnalysisLog);
            Assert.AreEqual(1, target.AnalysisLog.TestExecutions.Length);
            Assert.AreSame(target.TestExeuctions.First(), target.AnalysisLog.TestExecutions[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EndTest_CalledWithoutStartTest()
        {
            var target = new TestStateRecorder();
            target.EndTest();
        }

        #endregion

        #region RecordVoidInstanceMethodCall

        [TestMethod]
        public void RecordVoidInstanceMethodCall_CorrectState()
        {
            Assert.Fail("Not Implemented.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RecordVoidInstanceMethodCall_CalledWithoutStartTest()
        {
            var target = new TestStateRecorder();
            target.RecordVoidInstanceMethodCall(new TestClass(), "SayHello");
        }

        #endregion

        #region RecordInstanceMethodCall

        [TestMethod]
        public void RecordInstanceMethodCall_CorrectState()
        {
            Assert.Fail("Not Implemented.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RecordInstanceMethodCall_CalledWithoutStartTest()
        {
            var target = new TestStateRecorder();
            target.RecordVoidInstanceMethodCall(new TestClass(), "SayHello");
        }

        #endregion

        #region AddMethodToDynamicCallGraph

        [TestMethod]
        public void AddMethodToDynamicCallGraph_CorrectState()
        {
            Assert.Fail("Not Implemented.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddMethodToDynamicCallGraph_CalledWithoutStartTest()
        {
            var target = new TestStateRecorder();
            target.RecordVoidInstanceMethodCall(new TestClass(), "SayHello");
        }

        #endregion

        #endregion

        #region Test Classes

        private class TestClass
        {

            #region Fields & Constants

            private string name;

            #endregion

            #region Constructors & Destructors

            public TestClass()
            {
                this.name = "Foo";
            }

            #endregion

            #region Properties

            public int Foo { get; set; }

            #endregion

            #region Public Methods

            public string SayHello(string name)
            {
                return "Hi " + name + ", " + "my name is " + this.name;
            }

            #endregion

        }
        #endregion
    }
}
