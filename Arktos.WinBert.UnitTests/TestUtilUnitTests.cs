namespace Arktos.WinBert.UnitTests
{
    using Arktos.WinBert.Instrumentation;
    using Arktos.WinBert.Util;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;

    [TestClass]
    public class TestUtilUnitTests
    {
        #region Fields & Constants

        private Mock<IFileSystem> fileSystemMock;
        private Mock<ITestStateRecorder> stateRecorderMock;

        #endregion

        #region Test Plumbing

        [TestInitialize]
        public void TestInit()
        {
            this.fileSystemMock = new Mock<IFileSystem>();
            this.stateRecorderMock = new Mock<ITestStateRecorder>();

            // Required because we're hitting a static class
            TestUtil.FileSystem = fileSystemMock.Object;
            TestUtil.StateRecorder = stateRecorderMock.Object;
            // If these aren't reset, things'll go weird.
        }

        #endregion

        #region Test Methods

        #region Properties

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StateRecorderProperty_NullArgument_ThrowsException()
        {
            TestUtil.StateRecorder = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileSystemProperty_NullArgument_ThrowsException()
        {
            TestUtil.FileSystem = null;
        }

        [TestMethod]
        public void StateRecorderProperty_Assigns()
        {
            var mock = new Mock<ITestStateRecorder>();
            TestUtil.StateRecorder = mock.Object;

            Assert.AreSame(mock.Object, TestUtil.StateRecorder);
        }

        [TestMethod]
        public void FileSystemProperty_Assigns()
        {
            var mock = new Mock<IFileSystem>();
            TestUtil.FileSystem = mock.Object;

            Assert.AreSame(mock.Object, TestUtil.FileSystem);
        }

        #endregion

        #region StartTest

        [TestMethod]
        public void StartTest_StateRecorderCalledOnce()
        {
            var mock = new Mock<ITestStateRecorder>();
            TestUtil.StateRecorder = mock.Object;
            TestUtil.StartTest();

            mock.Verify(x => x.StartTest(), Times.Once());
        }

        #endregion

        #region EndTest

        [TestMethod]
        public void EndTest_StateRecorderCalledOnce()
        {
            var mock = new Mock<ITestStateRecorder>();
            TestUtil.StateRecorder = mock.Object;
            TestUtil.StartTest();
            TestUtil.EndTest();

            mock.Verify(x => x.EndTest(), Times.Once());
        }

        #endregion

        #region RecordVoidInstanceMethodCall

        [TestMethod]
        public void RecordVoidInstanceMethodCall_StateRecorderMethodCalledOnce()
        {
            // These values are meaningless--we're testing that the method gets called, that's all.
            int target = 0;
            var signature = "TestMethod";

            this.stateRecorderMock.Verify(x => x.RecordVoidInstanceMethodCall(target, signature), Times.AtMostOnce());
            TestUtil.RecordVoidInstanceMethodCall(target, signature);
        }

        #endregion

        #region RecordInstanceMethodCall

        [TestMethod]
        public void RecordInstanceMethodCall_StateRecorderMethodCalledOnce()
        {
            // These values are meaningless--we're testing that the method gets called, that's all.
            int target = 0;
            int returnValue = 1;
            var signature = "TestMethod";

            this.stateRecorderMock.Verify(x => x.RecordInstanceMethodCall(target, returnValue, signature), Times.AtMostOnce());
            TestUtil.RecordInstanceMethodCall(target, returnValue, signature);
        }

        #endregion

        #region AddMethodToDynamicCallGraph

        [TestMethod]
        public void AddMethodToDynamicCallGraph_StateRecorderMethodCalledOnce()
        {
            // These values are meaningless--we're testing that the method gets called, that's all.
            var signature = "TestMethod";

            this.stateRecorderMock.Verify(x => x.AddMethodToDynamicCallGraph(signature), Times.AtMostOnce());
            TestUtil.AddMethodToDynamicCallGraph(signature);
        }

        #endregion

        #region SaveResults

        [TestMethod]
        public void SaveResults_SavesSuccessfully()
        {
            var targetPath = @"./out.xml";

            // Set up the state recorder
            this.stateRecorderMock.Verify(x => x.EndTest(), Times.AtMostOnce());
            this.stateRecorderMock.Setup(x => x.AnalysisLog).Returns(new WinBertAnalysisLog());

            // File system mock.
            this.fileSystemMock.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>(
                (path, xml) =>
                {
                    // Verify the deserialization at least looks OK in a basic sense.
                    Assert.IsFalse(string.IsNullOrWhiteSpace(xml));

                    // Verify path. Not really needed, but just in case.
                    Assert.AreEqual(targetPath, path);
                });

            // Mocks are assigned in test init, Go!
            TestUtil.StartTest();
            TestUtil.EndTest();
            TestUtil.SaveResults(targetPath);
        }

        #endregion

        #endregion
    }
}
