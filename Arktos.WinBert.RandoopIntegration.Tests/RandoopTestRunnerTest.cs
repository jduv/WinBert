namespace Arktos.WinBert.RandoopIntegration.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RandoopTestRunnerTest
    {
        #region Constants and Fields

        ////private static readonly string TestsPath = @"tests.dll";

        private RandoopTestRunner runnerUnderTest = null;

        #endregion

        #region Test Methods

        [TestMethod]
        public void ExecuteTestsTest()
        {
            ////ITestSuite mockTests = new MockTestSuite("tests.dll", "tests.dll");
            ////var results = runnerUnderTest.RunTests(mockTests);
        }

        [TestInitialize]
        public void PreTestInit()
        {
            this.runnerUnderTest = new RandoopTestRunner();
        }

        #endregion
    }
}