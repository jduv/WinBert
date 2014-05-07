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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    [TestClass]
    [DeploymentItem("test-analysis", "test-analysis")]
    public class BertBehavioralAnalyzerUnitTests
    {
        #region Fields & Constants

        private static readonly string AnalysisDir = @"test-analysis\";
        private static readonly string NewReturnValueAnalysisPath = AnalysisDir + "analysisNew_ReturnValues.xml";
        private static readonly string OldReturnValueAnalysisPath = AnalysisDir + "analysisOld_ReturnValues.xml";
        private static readonly string NewSentinalAnalysisPath = AnalysisDir + "analysisNew_SentinalValues.xml";
        private static readonly string OldSentinalAnalysisPath = AnalysisDir + "analysisOld_SentinalValues.xml";

        private Mock<IFileSystem> fileSystemMock;
        private Mock<ITestRunResult> simpleSuccessfulTestResultMock;
        private Mock<ITestRunResult> simpleUnsuccessfulTestResultMock;
        private Mock<IAssemblyDifference> differenceMock;
        private Mock<IAssemblyDifference> noDifferenceMock;

        #endregion

        #region Test Plumbing

        [TestInitialize]
        public void TestInit()
        {
            this.fileSystemMock = new Mock<IFileSystem>();
            this.fileSystemMock.Setup(x => x.OpenRead(It.IsAny<string>())).Returns<string>(path => File.OpenRead(path));

            this.differenceMock = new Mock<IAssemblyDifference>();
            this.differenceMock.Setup(x => x.AreDifferences).Returns(true);

            this.noDifferenceMock = new Mock<IAssemblyDifference>();
            this.noDifferenceMock.Setup(x => x.AreDifferences).Returns(false);

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
            var noDiffMock = new Mock<IAssemblyDifference>();
            noDiffMock.Setup(x => x.AreDifferences).Returns(false);

            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            var result = target.Analyze(
                noDiffMock.Object,
                this.simpleUnsuccessfulTestResultMock.Object,
                this.simpleUnsuccessfulTestResultMock.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(SuccessfulAnalysisResult));
        }

        [TestMethod]
        public void Analyze_SuccessfulRun_ReturnValuesDifferent()
        {
            string typeName = "Class1";
            string typeFullName = "InterfaceTestAssembly2.Class1";
            var methodName = "InterfaceTestAssembly2.Class1.I1Bar";

            // Mock out some type diff stuff. Correlates with sample analysis files.
            var typeDiffMock = new Mock<ITypeDifference>();
            typeDiffMock.Setup(x => x.AreDifferences).Returns(true);
            typeDiffMock.Setup(x => x.FullName).Returns(typeFullName);
            typeDiffMock.Setup(x => x.Methods).Returns(new List<string>() { methodName });
            typeDiffMock.Setup(x => x.Name).Returns(typeName);
            typeDiffMock.Setup(x => x.Contains(It.Is<string>(s => s.Equals(methodName)))).Returns(true);

            var diffMock = new Mock<IAssemblyDifference>();
            diffMock.Setup(x => x.AreDifferences).Returns(true);
            diffMock.Setup(x => x.TypeDifferences).Returns(new List<ITypeDifference>() { typeDiffMock.Object });
            diffMock.Setup(x => x[It.Is<string>(s => s.Equals(typeFullName))]).Returns(typeDiffMock.Object);

            var newAssemblyResult = new Mock<ITestRunResult>();
            newAssemblyResult.Setup(x => x.Success).Returns(true);
            newAssemblyResult.Setup(x => x.PathToAnalysisLog).Returns(NewReturnValueAnalysisPath);

            var oldAssemblyResult = new Mock<ITestRunResult>();
            oldAssemblyResult.Setup(x => x.Success).Returns(true);
            oldAssemblyResult.Setup(x => x.PathToAnalysisLog).Returns(OldReturnValueAnalysisPath);

            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            var result = target.Analyze(diffMock.Object, oldAssemblyResult.Object, newAssemblyResult.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(SuccessfulAnalysisResult));

            var typedResult = result as SuccessfulAnalysisResult;
            Assert.IsTrue(typedResult.Differences.Count() > 0);

            foreach (var testDiff in typedResult.Differences)
            {
                Assert.IsTrue(testDiff.AreDifferences);
                Assert.IsNotNull(testDiff.TestName);
                Assert.IsNotNull(testDiff.PreviousExecution);
                Assert.IsNotNull(testDiff.CurrentExecution);
                Assert.IsTrue(testDiff.MethodDifferences.Count() > 0);

                foreach (var methodDiff in testDiff.MethodDifferences)
                {
                    Assert.IsTrue(methodDiff.AreDifferences);
                    Assert.IsNotNull(methodDiff.CurrentCall);
                    Assert.IsNotNull(methodDiff.PreviousCall);
                    Assert.IsNotNull(methodDiff.ReturnValueDifference);
                    Assert.IsNotNull(methodDiff.PostCallObjectDifferences);
                }
            }
        }

        [TestMethod]
        public void Analyze_SuccessfulRun_SentinalValuesDifferent()
        {
            string typeName = "Class1";
            string typeFullName = "TestAssembly.Class1";
            var methodName = "TestAssembly.Class1.TestMe";

            // Mock out some type diff stuff. Correlates with sample analysis files.
            var typeDiffMock = new Mock<ITypeDifference>();
            typeDiffMock.Setup(x => x.AreDifferences).Returns(true);
            typeDiffMock.Setup(x => x.FullName).Returns(typeFullName);
            typeDiffMock.Setup(x => x.Methods).Returns(new List<string>() { methodName });
            typeDiffMock.Setup(x => x.Name).Returns(typeName);
            typeDiffMock.Setup(x => x.Contains(It.Is<string>(s => s.Equals(methodName)))).Returns(true);

            var diffMock = new Mock<IAssemblyDifference>();
            diffMock.Setup(x => x.AreDifferences).Returns(true);
            diffMock.Setup(x => x.TypeDifferences).Returns(new List<ITypeDifference>() { typeDiffMock.Object });
            diffMock.Setup(x => x[It.Is<string>(s => s.Equals(typeFullName))]).Returns(typeDiffMock.Object);

            var newAssemblyResult = new Mock<ITestRunResult>();
            newAssemblyResult.Setup(x => x.Success).Returns(true);
            newAssemblyResult.Setup(x => x.PathToAnalysisLog).Returns(NewSentinalAnalysisPath);

            var oldAssemblyResult = new Mock<ITestRunResult>();
            oldAssemblyResult.Setup(x => x.Success).Returns(true);
            oldAssemblyResult.Setup(x => x.PathToAnalysisLog).Returns(OldSentinalAnalysisPath);

            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            var result = target.Analyze(diffMock.Object, oldAssemblyResult.Object, newAssemblyResult.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(SuccessfulAnalysisResult));

            var typedResult = result as SuccessfulAnalysisResult;
            Assert.IsTrue(typedResult.Differences.Count() > 0);

            foreach (var testDiff in typedResult.Differences)
            {
                Assert.IsTrue(testDiff.AreDifferences);
                Assert.IsNotNull(testDiff.TestName);
                Assert.IsNotNull(testDiff.PreviousExecution);
                Assert.IsNotNull(testDiff.CurrentExecution);
                Assert.IsTrue(testDiff.MethodDifferences.Count() > 0);
                Assert.AreEqual(1, testDiff.MethodDifferences.Count());

                var methodDiff = testDiff.MethodDifferences.ToList()[0];
                Assert.IsTrue(methodDiff.AreDifferences);
                Assert.IsNotNull(methodDiff.CurrentCall);
                Assert.IsNotNull(methodDiff.PreviousCall);
                Assert.IsNotNull(methodDiff.ReturnValueDifference);
                Assert.IsNotNull(methodDiff.PostCallObjectDifferences);

                Assert.IsTrue(methodDiff.PostCallObjectDifferences.AreDifferences);
                Assert.AreEqual(3, methodDiff.PostCallObjectDifferences.FieldSentinalDiffs.Count());
            }
        }

        #endregion

        #endregion
    }
}
