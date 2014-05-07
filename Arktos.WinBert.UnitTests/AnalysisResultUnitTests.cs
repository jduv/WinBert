namespace Arktos.WinBert.UnitTests
{
    using AppDomainToolkit;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Linq;

    [TestClass]
    public class AnalysisResultUnitTests
    {
        #region Fields & Constants

        private Mock<IAssemblyTarget> targetOneMock;
        private Mock<IAssemblyTarget> targetTwoMock;
        private Mock<ITestRunResult> simpleSuccessfulTestResultMock;
        private Mock<ITestRunResult> simpleUnsuccessfulTestResultMock;

        #endregion

        #region Test Plumbing

        [TestInitialize]
        public void TestInit()
        {
            this.targetOneMock = new Mock<IAssemblyTarget>();
            this.targetOneMock.Setup(x => x.Location).Returns(@"C:\MyLocation\MyAssembly1.dll");
            this.targetOneMock.Setup(x => x.FullName).Returns("MyAssembly1.dll");

            this.targetTwoMock = new Mock<IAssemblyTarget>();
            this.targetTwoMock.Setup(x => x.Location).Returns(@"C:\MyLocation\MyAssembly2.dll");
            this.targetTwoMock.Setup(x => x.FullName).Returns("MyAssembly2.dll");

            this.simpleSuccessfulTestResultMock = new Mock<ITestRunResult>();
            simpleSuccessfulTestResultMock.Setup(x => x.Success).Returns(true);
            simpleSuccessfulTestResultMock.Setup(x => x.PathToAnalysisLog).Returns(string.Empty);
            simpleSuccessfulTestResultMock.Setup(x => x.Target).Returns(this.targetOneMock.Object);

            this.simpleUnsuccessfulTestResultMock = new Mock<ITestRunResult>();
            simpleUnsuccessfulTestResultMock.Setup(x => x.Success).Returns(false);
            simpleUnsuccessfulTestResultMock.Setup(x => x.PathToAnalysisLog).Returns(string.Empty);
            simpleUnsuccessfulTestResultMock.Setup(x => x.Target).Returns(this.targetOneMock.Object);
        }

        #endregion

        #region Test Methods

        #region UnsuccessfulTestRun

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnsuccessfulTestRun_NullArguments()
        {
            AnalysisResult.UnsuccessfulTestRun(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnsuccessfulTestRun_NullPreviousTestRun()
        {
            AnalysisResult.UnsuccessfulTestRun(null, this.simpleSuccessfulTestResultMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnsuccessfulTestRun_NullCurrentTestRun()
        {
            AnalysisResult.UnsuccessfulTestRun(this.simpleSuccessfulTestResultMock.Object, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UnsuccessfulTestRun_SuccessfulRuns()
        {
            AnalysisResult.UnsuccessfulTestRun(
                this.simpleSuccessfulTestResultMock.Object,
                this.simpleSuccessfulTestResultMock.Object);
        }

        [TestMethod]
        public void UnsuccessfulTestRun_UnsuccessfulPreviousTestRun()
        {
            var unsuccessfulMock = new Mock<ITestRunResult>();
            unsuccessfulMock.Setup(x => x.Success).Returns(false);
            unsuccessfulMock.Setup(x => x.PathToAnalysisLog).Returns(string.Empty);
            unsuccessfulMock.Setup(x => x.Target).Returns(this.targetOneMock.Object);

            var successfulMock = new Mock<ITestRunResult>();
            successfulMock.Setup(x => x.Success).Returns(true);
            successfulMock.Setup(x => x.PathToAnalysisLog).Returns(string.Empty);
            successfulMock.Setup(x => x.Target).Returns(this.targetTwoMock.Object);

            var result = AnalysisResult.UnsuccessfulTestRun(unsuccessfulMock.Object, successfulMock.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InconclusiveAnalysisResult));

            var typedResult = result as InconclusiveAnalysisResult;
            Assert.IsNotNull(typedResult.Reason);
            Assert.IsTrue(typedResult.Reason.Contains(AnalysisResult.SingleRunUnsuccessfulMessage));
            Assert.IsTrue(typedResult.Reason.Contains(this.targetOneMock.Object.Location));
            Assert.IsFalse(typedResult.Reason.Contains(this.targetTwoMock.Object.Location));
        }

        [TestMethod]
        public void UnsuccessfulTestRun_UnsuccessfulCurrentTestRun()
        {
            var unsuccessfulMock = new Mock<ITestRunResult>();
            unsuccessfulMock.Setup(x => x.Success).Returns(false);
            unsuccessfulMock.Setup(x => x.PathToAnalysisLog).Returns(string.Empty);
            unsuccessfulMock.Setup(x => x.Target).Returns(this.targetOneMock.Object);

            var successfulMock = new Mock<ITestRunResult>();
            successfulMock.Setup(x => x.Success).Returns(true);
            successfulMock.Setup(x => x.PathToAnalysisLog).Returns(string.Empty);
            successfulMock.Setup(x => x.Target).Returns(this.targetTwoMock.Object);

            var result = AnalysisResult.UnsuccessfulTestRun(successfulMock.Object, unsuccessfulMock.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InconclusiveAnalysisResult));

            var typedResult = result as InconclusiveAnalysisResult;
            Assert.IsNotNull(typedResult.Reason);
            Assert.IsTrue(typedResult.Reason.Contains(AnalysisResult.SingleRunUnsuccessfulMessage));
            Assert.IsTrue(typedResult.Reason.Contains(this.targetOneMock.Object.Location));
            Assert.IsFalse(typedResult.Reason.Contains(this.targetTwoMock.Object.Location));
        }

        [TestMethod]
        public void UnsuccessfulTestRun_BothRunsUnsuccessful()
        {
            var unsuccessfulMock1 = new Mock<ITestRunResult>();
            unsuccessfulMock1.Setup(x => x.Success).Returns(false);
            unsuccessfulMock1.Setup(x => x.PathToAnalysisLog).Returns(string.Empty);
            unsuccessfulMock1.Setup(x => x.Target).Returns(this.targetOneMock.Object);

            var unsuccessfulMock2 = new Mock<ITestRunResult>();
            unsuccessfulMock2.Setup(x => x.Success).Returns(false);
            unsuccessfulMock2.Setup(x => x.PathToAnalysisLog).Returns(string.Empty);
            unsuccessfulMock2.Setup(x => x.Target).Returns(this.targetTwoMock.Object);

            var result = AnalysisResult.UnsuccessfulTestRun(unsuccessfulMock1.Object, unsuccessfulMock2.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InconclusiveAnalysisResult));

            var typedResult = result as InconclusiveAnalysisResult;
            Assert.IsNotNull(typedResult.Reason);
            Assert.IsTrue(typedResult.Reason.Contains(AnalysisResult.BothRunsUnsuccessfulMessage));
            Assert.IsTrue(typedResult.Reason.Contains(this.targetOneMock.Object.Location));
            Assert.IsTrue(typedResult.Reason.Contains(this.targetTwoMock.Object.Location));
        }

        #endregion

        #region NoDifference

        [TestMethod]
        public void NoDifference()
        {
            var result = AnalysisResult.NoDifference();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(SuccessfulAnalysisResult));

            var typedResult = result as SuccessfulAnalysisResult;
            Assert.IsTrue(typedResult.Success);
            Assert.IsFalse(typedResult.Differences.Any());
        }

        #endregion

        #region UnknownError

        [TestMethod]
        public void UnknownError()
        {
            var result = AnalysisResult.UnknownError();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InconclusiveAnalysisResult));

            var typedResult = result as InconclusiveAnalysisResult;
            Assert.IsNotNull(typedResult.Reason);
            Assert.AreEqual(typedResult.Reason, AnalysisResult.UnknownErrorMessage, true);
        }

        #endregion

        #region FromException

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromException_NullArgument()
        {
            AnalysisResult.FromException(null);
        }

        [TestMethod]
        public void FromException()
        {
            var message = "My test message";
            var exception = new ArgumentException(message);
            var result = AnalysisResult.FromException(exception);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InconclusiveAnalysisResult));

            var typedResult = result as InconclusiveAnalysisResult;
            Assert.IsNotNull(typedResult.Reason);
            Assert.IsTrue(typedResult.Reason.Contains(AnalysisResult.ExceptionMessage));
            Assert.IsTrue(typedResult.Reason.Contains(message));
        }

        #endregion

        #endregion
    }
}
