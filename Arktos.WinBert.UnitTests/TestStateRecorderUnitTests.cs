namespace Arktos.WinBert.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Arktos.WinBert.Instrumentation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class TestStateRecorderUnitTests
    {
        #region Fields & Constants

        private static IMethodCallDumper mockMethodDumper;

        #endregion

        #region Test Plumbing

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // get a new build manager after each test
            var dumperMock = new Mock<IMethodCallDumper>();

            // Set up instance method dumps
            dumperMock.Setup(x => x.DumpInstanceMethod(It.IsAny<uint>(), It.IsAny<object>(), It.IsAny<object>(), It.IsAny<string>()))
                .Returns<uint, object, object, string>(
                    (id, obj, retVal, sig) =>
                    {
                        return new Xml.MethodCall()
                        {
                            Id = id,
                            ReturnValue = new Xml.Value(),
                            Signature = sig,
                            PostCallInstance = new Xml.NotNull(),
                            Type = Xml.MethodCallType.Instance,
                            DynamicCallGraph = new List<Xml.CallGraphNode>()
                        };
                    });

            // Set up void instance method dumps
            dumperMock.Setup(x => x.DumpVoidInstanceMethod(It.IsAny<uint>(), It.IsAny<object>(), It.IsAny<string>()))
                .Returns<uint, object, string>(
                    (id, obj, sig) =>
                    {
                        return new Xml.MethodCall()
                        {
                            Id = id,
                            Signature = sig,
                            PostCallInstance = new Xml.NotNull(),
                            Type = Xml.MethodCallType.Instance,
                            DynamicCallGraph = new List<Xml.CallGraphNode>()
                        };
                    });

            mockMethodDumper = dumperMock.Object;
        }

        [ClassCleanup]
        public static void TestCleanup()
        {
            mockMethodDumper = null;
        }

        #endregion

        #region Test Methods

        #region StartTest

        [TestMethod]
        public void StartTest_CorrectState()
        {
            var target = new TestStateRecorder();
            target.StartTest();

            Assert.IsNotNull(target.CurrentTest);
            Assert.IsNull(target.CurrentMethodCall);
            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 0U,
                expectedTestCounter: 0U,
                numberOfTestExecutions: 0,
                recorder: target);
        }

        #endregion

        #region EndTest

        [TestMethod]
        public void EndTest_CorrectState()
        {
            var target = new TestStateRecorder();
            target.StartTest();
            target.EndTest();

            Assert.IsNull(target.CurrentMethodCall);
            Assert.IsNull(target.CurrentTest);
            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 0U,
                expectedTestCounter: 1U,
                numberOfTestExecutions: 1,
                recorder: target);

            // Check Id is set to correct counter value.
            Assert.AreEqual(0U, target.AnalysisLog.TestExecutions.First().Id);
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
            var methodSig = "Foo";
            var target = new TestStateRecorder(mockMethodDumper);
            target.StartTest();
            target.RecordVoidInstanceMethodCall(new TestClass(), methodSig);

            // Before: method counter incremented, test counter hasn't yet. Methods has one
            // element, tests has none until EndTest is called.
            Assert.IsNotNull(target.CurrentTest);
            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 1U,
                expectedTestCounter: 0U,
                numberOfTestExecutions: 0,
                recorder: target);

            target.EndTest();

            // After: Method counter and test counter at one now. Both lists have one element.
            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 0U,
                expectedTestCounter: 1U,
                numberOfTestExecutions: 1,
                recorder: target);

            // The point here isn't to re-test the method call dumper, but just the pieces we interact
            // with in the state recorder.
            Assert.IsNotNull(target.AnalysisLog);
            Assert.IsNotNull(target.AnalysisLog.TestExecutions);

            // Test executions
            var testExecution = target.AnalysisLog.TestExecutions.FirstOrDefault();
            Assert.IsNotNull(testExecution);
            Assert.IsNotNull(testExecution.MethodCalls);
            Assert.AreEqual(1, testExecution.MethodCalls.Count);
            Assert.AreEqual(0U, testExecution.Id);

            // Method calls
            var dumpedCall = testExecution.MethodCalls.FirstOrDefault();
            Assert.IsNotNull(dumpedCall);
            Assert.AreEqual(methodSig, dumpedCall.Signature); // Make sure we recorded the right sig
            Assert.AreEqual(0U, dumpedCall.Id); // Make sure we recorded the right method call sequence number
        }

        [TestMethod]
        public void RecordVoidInstanceMethodCall_EnsureMethodCounterIncrements()
        {
            var methodSig1 = "Foo";
            var methodSig2 = "Bar";
            var target = new TestStateRecorder(mockMethodDumper);
            target.StartTest();
            target.RecordVoidInstanceMethodCall(new TestClass(), methodSig1);

            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 1U,
                expectedTestCounter: 0U,
                numberOfTestExecutions: 0,
                recorder: target);

            target.RecordVoidInstanceMethodCall(new TestClass(), methodSig2);

            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 2U,
                expectedTestCounter: 0U,
                numberOfTestExecutions: 0,
                recorder: target);

            target.EndTest();

            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 0U,
                expectedTestCounter: 1U,
                numberOfTestExecutions: 1,
                recorder: target);
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
            var methodSig = "Foo";
            var retVal = 3.0F;
            var target = new TestStateRecorder(mockMethodDumper);
            target.StartTest();
            target.RecordInstanceMethodCall(new TestClass(), retVal, methodSig);

            // Before: method counter incremented, test counter hasn't yet. Methods has one
            // element, tests has none until EndTest is called.
            Assert.IsNotNull(target.CurrentTest);
            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 1U,
                expectedTestCounter: 0U,
                numberOfTestExecutions: 0,
                recorder: target);

            target.EndTest();

            // After: Method counter at zero and test counter at one now. Both lists have one element.
            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 0U,
                expectedTestCounter: 1U,
                numberOfTestExecutions: 1,
                recorder: target);

            // The point here isn't to re-test the method call dumper, but just the pieces we interact
            // with in the state recorder.
            Assert.IsNotNull(target.AnalysisLog);
            Assert.IsNotNull(target.AnalysisLog.TestExecutions);

            // Test executions
            var testExecution = target.AnalysisLog.TestExecutions.FirstOrDefault();
            Assert.IsNotNull(testExecution);
            Assert.IsNotNull(testExecution.MethodCalls);
            Assert.AreEqual(1, testExecution.MethodCalls.Count);
            Assert.AreEqual(0U, testExecution.Id);

            // Method calls
            var dumpedCall = testExecution.MethodCalls.FirstOrDefault();
            Assert.IsNotNull(dumpedCall);
            Assert.AreEqual(methodSig, dumpedCall.Signature); // Make sure we recorded the right sig
            Assert.AreEqual(0U, dumpedCall.Id); // Make sure we recorded the right method call sequence number
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
            var target = new TestStateRecorder(mockMethodDumper);
            target.StartTest();
            target.RecordVoidInstanceMethodCall("Hello World", "Foo");
            target.AddMethodToDynamicCallGraph("Bar");
            target.AddMethodToDynamicCallGraph("Baz");
            target.AddMethodToDynamicCallGraph("Biff");

            // Assert counter stats
            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 1U,
                expectedTestCounter: 0U,
                numberOfTestExecutions: 0,
                recorder: target);

            // Should have three calls.
            Assert.AreEqual(3, target.CurrentMethodCall.DynamicCallGraph.Count);

            AssertCallGraphIsCorrect(target);

            target.EndTest();

            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 0U,
                expectedTestCounter: 1U,
                numberOfTestExecutions: 1,
                recorder: target);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddMethodToDynamicCallGraph_NoTestMethodCalledYet()
        {
            var target = new TestStateRecorder(mockMethodDumper);
            target.StartTest();
            target.AddMethodToDynamicCallGraph("Foo");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddMethodToDynamicCallGraph_CalledWithoutStartTest()
        {
            var target = new TestStateRecorder();
            target.RecordVoidInstanceMethodCall(new TestClass(), "SayHello");
        }

        #endregion

        #region End To End Tests

        [TestMethod]
        public void EndToEndTest_PassTestBoundary_CorrectState()
        {
            var testClass = new TestClass();
            var target = new TestStateRecorder(mockMethodDumper);

            // Begin.
            target.StartTest();
            target.RecordVoidInstanceMethodCall(testClass, "Foo");
            target.AddMethodToDynamicCallGraph("Baz");
            target.AddMethodToDynamicCallGraph("Biff");
            target.RecordInstanceMethodCall(testClass, 5, "Bar");
            target.AddMethodToDynamicCallGraph("Zoo");
            target.EndTest();

            // Test counter state.
            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 0U,
                expectedTestCounter: 1U,
                numberOfTestExecutions: 1,
                recorder: target);

            // Do it all again.
            target.StartTest();
            target.RecordVoidInstanceMethodCall(testClass, "Foo");
            target.AddMethodToDynamicCallGraph("Baz");
            target.RecordInstanceMethodCall(testClass, 5, "Bar");
            target.AddMethodToDynamicCallGraph("Zoo");
            target.AddMethodToDynamicCallGraph("Boo");
            target.EndTest();

            // Test counter state
            AssertRecorderListAndCounterStates(
                expectedMethodCounter: 0U,
                expectedTestCounter: 2U,
                numberOfTestExecutions: 2,
                recorder: target);

            AssertCallGraphIsCorrect(target);
        }

        #endregion

        #endregion

        #region Private Methods

        private static void AssertRecorderListAndCounterStates(
            uint expectedMethodCounter,
            uint expectedTestCounter,
            int numberOfTestExecutions,
            TestStateRecorder recorder)
        {
            Assert.IsNotNull(recorder.AnalysisLog);
            Assert.AreEqual(expectedMethodCounter, recorder.MethodCounter);
            Assert.AreEqual(expectedTestCounter, recorder.TestCounter);
            Assert.IsNotNull(recorder.AnalysisLog.TestExecutions);
            Assert.AreEqual(numberOfTestExecutions, recorder.AnalysisLog.TestExecutions.Count());
        }

        private static void AssertCallGraphIsCorrect(TestStateRecorder target)
        {
            Assert.IsNotNull(target.AnalysisLog);
            Assert.IsNotNull(target.AnalysisLog.TestExecutions);
            foreach (var testExecution in target.AnalysisLog.TestExecutions)
            {
                Assert.IsNotNull(testExecution.MethodCalls);
                foreach (var methodCall in testExecution.MethodCalls)
                {
                    Assert.IsNotNull(methodCall.DynamicCallGraph);
                    for (int i = 0; i < methodCall.DynamicCallGraph.Count; i++)
                    {
                        // Ensure sequence number generated is correct and signature is a non-empty string
                        var currentNode = methodCall.DynamicCallGraph[i];
                        Assert.AreEqual((uint)i, currentNode.SequenceNumber);
                        Assert.IsFalse(string.IsNullOrEmpty(currentNode.Signature));
                    }
                }
            }
        }

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
