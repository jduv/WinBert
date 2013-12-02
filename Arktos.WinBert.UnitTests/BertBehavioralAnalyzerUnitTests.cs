namespace Arktos.WinBert.UnitTests
{
    using AppDomainToolkit;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Util;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;

    [TestClass]
    public class BertBehavioralAnalyzerUnitTests
    {
        #region Fields & Constants

        private Mock<IFileSystem> fileSystemMock;
        private Mock<IAssemblyTarget> targetMock;

        #endregion

        #region Test Plumbing

        [TestInitialize]
        public void TestInit()
        {
            this.fileSystemMock = new Mock<IFileSystem>();
            this.targetMock = new Mock<IAssemblyTarget>();
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

        [ExpectedException(typeof(ArgumentNullException))]
        public void Analyze_AllNullArguments()
        {
            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            target.Analyze(null, null, null);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        public void Analyze_NullDiff()
        {
            var logPath = "./out.xml"; // fake log path
            var assemblyTarget = this.targetMock.Object;
            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            target.Analyze(null, TestRunResult.Successful(logPath, assemblyTarget), TestRunResult.Successful(logPath, assemblyTarget));
        }

        [ExpectedException(typeof(ArgumentNullException))]
        public void Analyze_TestRunResult1()
        {
            var logPath = "./out.xml"; // fake log path\
            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            target.Analyze(null, null, TestRunResult.Successful(logPath, this.targetMock.Object));
        }

        [ExpectedException(typeof(ArgumentNullException))]
        public void Analyze_TestRunResult2()
        {
            var logPath = "./out.xml"; // fake log path
            var target = new BertBehavioralAnalyzer(this.fileSystemMock.Object);
            target.Analyze(null, TestRunResult.Successful(logPath, this.targetMock.Object), null);
        }

        #endregion

        #endregion
    }
}
