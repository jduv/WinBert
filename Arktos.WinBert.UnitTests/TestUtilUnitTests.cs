namespace Arktos.WinBert.UnitTests
{
    using System;
    using Arktos.WinBert.Instrumentation;
    using Arktos.WinBert.Util;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class TestUtilUnitTests
    {
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
            mock.Verify(x => x.StartTest(), Times.AtMostOnce());
            TestUtil.StateRecorder = mock.Object;
            TestUtil.StartTest();
        }

        #endregion

        #region EndTest

        [TestMethod]
        public void EndTest_HappyPath()
        {
            var targetPath = @"./out.xml";

            // State recorder mock.
            var stateRecorderMock = new Mock<ITestStateRecorder>();
            stateRecorderMock.Verify(x => x.EndTest(), Times.AtMostOnce());
            stateRecorderMock.Setup(x => x.AnalysisLog).Returns(new WinBertAnalysisLog());

            // File system mock.
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>(
                (path, xml) => 
                {
                    // Verify the deserialization at least looks OK in a basic sense.
                    Assert.IsFalse(string.IsNullOrWhiteSpace(xml));

                    // Verify path. Not really needed, but just in case.
                    Assert.AreEqual(targetPath, path);
                });

            // Assign mocks
            TestUtil.StateRecorder = stateRecorderMock.Object;
            TestUtil.FileSystem = fsMock.Object;

            // Go!
            TestUtil.StartTest();
            TestUtil.EndTest(targetPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EndTest_NullArgument_ExceptionThrown()
        {
            TestUtil.StartTest();
            TestUtil.EndTest(null);
        }

        #endregion

        #region RecordVoidInstanceMethodCall

        [TestMethod]
        public void RecordVoidInstanceMethodCall_StateRecorderCalledOnce()
        {
            // These values are meaningless--we're testing that the method gets called, that's all.
            int target = 0;
            var signature = "TestMethod";

            var mock = new Mock<ITestStateRecorder>();
            mock.Verify(x => x.RecordVoidInstanceMethodCall(target, signature), Times.AtMostOnce());
            TestUtil.StateRecorder = mock.Object;
            TestUtil.RecordVoidInstanceMethodCall(target, signature);
        }

        #endregion

        #region RecordInstanceMethodCall

        [TestMethod]
        public void RecordInstanceMethodCall_StateRecorderCalledOnce()
        {
            // These values are meaningless--we're testing that the method gets called, that's all.
            int target = 0;
            int returnValue = 1;
            var signature = "TestMethod";

            var mock = new Mock<ITestStateRecorder>();
            mock.Verify(x => x.RecordInstanceMethodCall(target, returnValue, signature), Times.AtMostOnce());
            TestUtil.StateRecorder = mock.Object;
            TestUtil.RecordInstanceMethodCall(target, returnValue, signature);
        }

        #endregion

        #region AddMethodToDynamicCallGraph

        [TestMethod]
        public void AddMethodToDynamicCallGraph_StateRecorderCalledOnce()
        {
            // These values are meaningless--we're testing that the method gets called, that's all.
            var signature = "TestMethod";

            var mock = new Mock<ITestStateRecorder>();
            mock.Verify(x => x.AddMethodToDynamicCallGraph(signature), Times.AtMostOnce());
            TestUtil.StateRecorder = mock.Object;
            TestUtil.AddMethodToDynamicCallGraph(signature);
        }

        #endregion

        #endregion
    }
}
