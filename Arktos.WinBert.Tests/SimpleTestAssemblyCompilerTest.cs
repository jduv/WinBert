namespace Arktos.WinBert.RandoopIntegration.Tests
{
    using System.IO;
    using System.Reflection;
    using Arktos.WinBert.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RandoopTestCompilerTest
    {
        #region Fields and Constants

        private static readonly string workingDirectory = @".\";

        private static readonly string dependentSourceDirectory = @".\dependent-src-test";

        private static readonly string dependentSourceFile = @"Dependent.cs";

        private static readonly string referencedLibraryPath = @"Dependency.dll";

        private BertAssemblyCompiler compilerUnderTest;
        
        #endregion

        #region Test Plumbing

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Directory.CreateDirectory(dependentSourceDirectory);
            File.Move(dependentSourceFile, Path.Combine(dependentSourceDirectory, dependentSourceFile));
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (Directory.Exists(dependentSourceDirectory))
            {
                Directory.Delete(dependentSourceDirectory, true);
            }
        }

        [TestInitialize]
        public void PreTestInit()
        {
            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }

            this.compilerUnderTest = new BertAssemblyCompiler(workingDirectory);
        }

        #endregion

        #region Test Methods

        [TestMethod]
        public void TestCompileTests()
        {
            Assembly assembly = this.compilerUnderTest.CompileTests(workingDirectory);
            Assert.IsNotNull(assembly);
        }

        [TestMethod]
        public void TestAddReferencesAndCompile()
        {
            this.compilerUnderTest.AddReference(referencedLibraryPath);
            Assembly assembly = this.compilerUnderTest.CompileTests(dependentSourceDirectory);
            Assert.IsNotNull(assembly);
        }

        #endregion
    }
}
