namespace Arktos.WinBert.UnitTests
{
    using System;
    using Arktos.WinBert.Instrumentation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class TestUtilUnitTests
    {
        #region Test Methods

        #region Properties

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DumperProperty_NullArgument_ThrowsException()
        {
            TestUtil.Dumper = null;
        }

        [TestMethod]
        public void DumperProperty_Assigns()
        {
            var mock = new Mock<ITestDumper>();
            TestUtil.Dumper = mock.Object;

            Assert.AreSame(mock.Object, TestUtil.Dumper);
        }

        #endregion

        #region StartTest

        [TestMethod]
        public void StartTest_DumperCalledOnce()
        {
            var mock = new Mock<ITestDumper>();
            mock.Verify(x => x.StartTest(), Times.AtMostOnce());
            TestUtil.Dumper = mock.Object;
            TestUtil.StartTest();
        }

        #endregion

        #region EndTest

        [TestMethod]
        public void EndTest_DumperCalledOnce()
        {
            // Use current dir for giggles.
            var pathToSaveTo = @"./";
            var mock = new Mock<ITestDumper>();
            mock.Verify(x => x.EndTest(pathToSaveTo), Times.AtMostOnce());
            TestUtil.Dumper = mock.Object;
            TestUtil.EndTest(pathToSaveTo);
        }

        #endregion

        #region DumpVoidInstanceMethodCall

        [TestMethod]
        public void DumpVoidInstanceMethodCall_DumperCalledOnce()
        {
            // These values are meaningless--we're testing that the method gets called, that's all.
            int target = 0;
            var signature = "TestMethod";

            var mock = new Mock<ITestDumper>();
            mock.Verify(x => x.DumpVoidInstanceMethodCall(target, signature), Times.AtMostOnce());
            TestUtil.Dumper = mock.Object;
            TestUtil.DumpVoidInstanceMethodCall(target, signature);
        }

        #endregion

        #region DumpInstanceMethodCall

        [TestMethod]
        public void DumpInstanceMethodCall_DumperCalledOnce()
        {
            // These values are meaningless--we're testing that the method gets called, that's all.
            int target = 0;
            int returnValue = 1;
            var signature = "TestMethod";

            var mock = new Mock<ITestDumper>();
            mock.Verify(x => x.DumpInstanceMethodCall(target, returnValue, signature), Times.AtMostOnce());
            TestUtil.Dumper = mock.Object;
            TestUtil.DumpInstanceMethodCall(target, returnValue, signature);
        }

        #endregion

        #region AddMethodToDynamicCallGraph

        [TestMethod]
        public void AddMethodToDynamicCallGraph_DumperCalledOnce()
        {
            // These values are meaningless--we're testing that the method gets called, that's all.
            var signature = "TestMethod";

            var mock = new Mock<ITestDumper>();
            mock.Verify(x => x.AddMethodToDynamicCallGraph(signature), Times.AtMostOnce());
            TestUtil.Dumper = mock.Object;
            TestUtil.AddMethodToDynamicCallGraph(signature);
        }

        #endregion

        #endregion
    }
}
