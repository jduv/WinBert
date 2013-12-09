namespace Arktos.WinBert.UnitTests
{
    using AppDomainToolkit;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Util;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.IO;
    using System.Reflection;

    [TestClass]
    [DeploymentItem("test-analysis", "test-analysis")]
    public class BertBehavioralAnalyzerUnitTests
    {
        #region Fields & Constants

        private static readonly string AnalysisDir = @"test-analysis\";
        private static readonly string NewAnalysisPath = AnalysisDir + "analysisNew.xml";
        private static readonly string OldAnalysisPath = AnalysisDir + "analysisOld.xml";

        private Mock<IFileSystem> fileSystemMock;
        private Mock<ITestRunResult> simpleSuccessfulTestResultMock;
        private Mock<ITestRunResult> simpleUnsuccessfulTestResultMock;
        private Mock<IAssemblyDifferenceResult> differenceMock;
        private Mock<IAssemblyDifferenceResult> noDifferenceMock;

        #endregion

        #region Test Plumbing

        [TestInitialize]
        public void TestInit()
        {
            this.fileSystemMock = new Mock<IFileSystem>();
            this.fileSystemMock.Setup(x => x.OpenRead(It.IsAny<string>())).Returns<string>(path => File.OpenRead(path));

            this.differenceMock = new Mock<IAssemblyDifferenceResult>();
            this.differenceMock.Setup(x => x.IsDifferent).Returns(true);

            this.noDifferenceMock = new Mock<IAssemblyDifferenceResult>();
            this.noDifferenceMock.Setup(x => x.IsDifferent).Returns(false);

            var simpleTargetMock = new Mock<IAssemblyTarget>();
            simpleTargetMock.Setup(x => x.Location).Returns(Assembly.GetExecutingAssembly().Location);
            simpleTargetMock.Setup(x => x.FullName).Returns(Assembly.GetExecutingAssembly().FullName);
            simpleTargetMock.Setup(x => x.CodeBase).Returns(new Uri(Assembly.GetExecutingAssembly().CodeBase));

            this.simpleSuccessfulTestResultMock = new Mock<ITestRunResult>();
            simpleSuccessfulTestResultMock.Setup(x => x.Success).Returns(true);
            simpleSuccessfulTestResultMock.Setup(x => x.PathToAnalysisLog).Returns(string.Empty);
            simpleSuccessfulTestResultMock.Setup(x => x.Target).Returns(simpleTargetMock.Object);

            this.simpleUnsuccessfulTestResultMock = new Mock<ITestRunResult>();
            simpleUnsuccessfulTestResultMock.Setup(x => x.Success).Returns(false);
            simpleUnsuccessfulTestResultMock.Setup(x => x.PathToAnalysisLog).Returns(string.Empty);
            simpleUnsuccessfulTestResultMock.Setup(x => x.Target).Returns(simpleTargetMock.Object);
        }

        #endregion

        #region Test Methods

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullFileSystem()
        {
            var target = new BertBehavioralAnalyzer(null);
        }

        #endregion

        #region Analyze

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Analyze_AllNullArguments()
        {
            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            target.Analyze(null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Analyze_NullDiff()
        {
            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            target.Analyze(null, this.simpleSuccessfulTestResultMock.Object, this.simpleSuccessfulTestResultMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Analyze_NullNewTestRunResult()
        {
            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            target.Analyze(this.differenceMock.Object, null, this.simpleSuccessfulTestResultMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Analyze_NullOldTestRunResult()
        {
            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            var type = target.Analyze(this.differenceMock.Object, this.simpleSuccessfulTestResultMock.Object, null);
        }

        [TestMethod]
        public void Analyze_UnuccessfulTestRunResult()
        {
            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            var result = target.Analyze(
                this.differenceMock.Object,
                this.simpleUnsuccessfulTestResultMock.Object,
                this.simpleSuccessfulTestResultMock.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InconclusiveAnalysisResult));
        }

        [TestMethod]
        public void Analyze_BothUnsuccessfulTestRunResults()
        {
            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            var result = target.Analyze(
                this.differenceMock.Object,
                this.simpleUnsuccessfulTestResultMock.Object,
                this.simpleUnsuccessfulTestResultMock.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InconclusiveAnalysisResult));
        }

        [TestMethod]
        public void Analyze_NoDifferences()
        {
            var noDiffMock = new Mock<IAssemblyDifferenceResult>();
            noDiffMock.Setup(x => x.IsDifferent).Returns(false);

            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            var result = target.Analyze(
                noDiffMock.Object,
                this.simpleUnsuccessfulTestResultMock.Object,
                this.simpleUnsuccessfulTestResultMock.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InconclusiveAnalysisResult));
        }

        [TestMethod]
        public void Analyze_SuccessfulRuns()
        {
            var newAssemblyResult = new Mock<ITestRunResult>();
            newAssemblyResult.Setup(x => x.Success).Returns(true);
            newAssemblyResult.Setup(x => x.PathToAnalysisLog).Returns(NewAnalysisPath);

            var oldAssemblyResult = new Mock<ITestRunResult>();
            oldAssemblyResult.Setup(x => x.Success).Returns(true);
            oldAssemblyResult.Setup(x => x.PathToAnalysisLog).Returns(OldAnalysisPath);

            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            var result = target.Analyze(this.differenceMock.Object, oldAssemblyResult.Object, newAssemblyResult.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(SuccessfulAnalysisResult));
        }

        #endregion

        #endregion
    }
}
