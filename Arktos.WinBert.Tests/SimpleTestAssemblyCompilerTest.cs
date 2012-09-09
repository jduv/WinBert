namespace Arktos.WinBert.UnitTests
{
    using System.IO;
    using System.Reflection;
    using Arktos.WinBert.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class RandoopTestCompilerTest
    {
        #region Fields and Constants

        private const string WorkingDirectory = @".\";

        private const string DependentSourceDirectory = @"dependent-src-test\";

        private const string DependentSourcePath = DependentSourceDirectory + @"Dependent.cs";

        private const string ReferencedLibraryPath = DependentSourceDirectory + @"Dependency.dll";

        private BertAssemblyCompiler compilerUnderTest;
        
        #endregion

        #region Test Plumbing

        [TestInitialize]
        public void PreTestInit()
        {
            if (!Directory.Exists(WorkingDirectory))
            {
                Directory.CreateDirectory(WorkingDirectory);
            }

            this.compilerUnderTest = new BertAssemblyCompiler(WorkingDirectory);
        }

        #endregion

        #region Test Methods

        [TestMethod]        
        [ExpectedException(typeof(ArgumentException))]
        public void CompileTests_EmptyPath_ExceptionThrown()
        {
            Assembly assembly = this.compilerUnderTest.CompileTests(string.Empty);
            Assert.IsNull(assembly);
        }

        [TestMethod]
        [DeploymentItem(DependentSourcePath, DependentSourceDirectory)]
        public void CompileTests_MissingReference_Null()
        {
            Assembly assembly = this.compilerUnderTest.CompileTests(DependentSourceDirectory);
            Assert.IsNull(assembly);
        }

        [TestMethod]
        [DeploymentItem(DependentSourcePath, DependentSourceDirectory)]
        [DeploymentItem(ReferencedLibraryPath, DependentSourceDirectory)]
        public void CompileTests_WithReference_ValidAssembly()
        {
            this.compilerUnderTest.AddReference(ReferencedLibraryPath);
            Assert.IsNull(assembly);
        }

        #endregion
    }
}
