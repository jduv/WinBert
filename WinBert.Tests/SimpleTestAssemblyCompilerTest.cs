namespace WinBert.RandoopIntegration.Tests
{
    using System.IO;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WinBert.Testing;

    /// <summary>
    /// The randoop test compiler test.
    /// </summary>
    [TestClass]
    public class RandoopTestCompilerTest
    {
        #region Fields and Constants

        /// <summary>
        ///   Where the source files will be housed and the test assemblies generated
        /// </summary>
        private static readonly string workingDirectory = @".\";

        /// <summary>
        ///   Static path to the test assembly we will be testing.
        /// </summary>
        private static readonly string dependentSourceDirectory = @".\dependent-src-test";

        /// <summary>
        ///   Name of a dependent source file for the dependency test.
        /// </summary>
        private static readonly string dependentSourceFile = @"Dependent.cs";

        /// <summary>
        ///   Static path to the test assembly we will be testing.
        /// </summary>
        private static readonly string referencedLibraryPath = @"Dependency.dll";

        /// <summary>
        ///   Randoop test compiler.
        /// </summary>
        private BertAssemblyCompiler compilerUnderTest;
        
        #endregion

        #region Test Plumbing

        /// <summary>
        /// Performs some one-time setup operations for the test.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Directory.CreateDirectory(dependentSourceDirectory);
            File.Move(dependentSourceFile, Path.Combine(dependentSourceDirectory, dependentSourceFile));
        }

        /// <summary>
        /// Cleans up after this test fixture.
        /// </summary>
        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (Directory.Exists(dependentSourceDirectory))
            {
                Directory.Delete(dependentSourceDirectory, true);
            }
        }

        /// <summary>
        /// Initializes properties before each test
        /// </summary>
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

        /// <summary>
        /// Tests to see if the compiler will actually compile some tests.
        /// </summary>
        [TestMethod]
        public void TestCompileTests()
        {
            Assembly assembly = this.compilerUnderTest.CompileTests(workingDirectory);
            Assert.IsNotNull(assembly);
        }

        /// <summary>
        /// Tests to see if we can compile source code with properly referenced satellite assemblies.
        /// </summary>
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
