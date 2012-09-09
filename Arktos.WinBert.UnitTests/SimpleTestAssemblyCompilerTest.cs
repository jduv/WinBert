namespace Arktos.WinBert.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using Arktos.WinBert.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Arktos.WinBert.Exceptions;

    [TestClass]
    public class RandoopTestCompilerTest
    {
        #region Fields and Constants

        private const string WorkingDirectory = @".\";

        private const string DependentSourceDirectory = @"dependent-src-test\";

        private const string DependentSourcePath = DependentSourceDirectory + @"Dependent.cs";

        private const string ReferencedLibraryPath = DependentSourceDirectory + @"Dependency.dll";

        private const string SecondaryReferencedLibraryPath = DependentSourceDirectory + @"CopyOfDependency.dll";

        private const string BadExtensionReference = DependentSourceDirectory + @"Dependency.txt";

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

        #region Constructors

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NullPath_ThrowsException()
        {
            var target = new BertAssemblyCompiler(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_EmptyPath_ThrowsException()
        {
            var target = new BertAssemblyCompiler(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void Ctor_NonExistantPath_ThrowsException()
        {
            var path = @"C:\" + Guid.NewGuid().ToString();
            var target = new BertAssemblyCompiler(path);
        }

        #endregion

        #region AddReference

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddReference_NullString_ThrowsException()
        {
            this.compilerUnderTest.AddReference(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddReference_EmptyString_ThrowsException()
        {
            this.compilerUnderTest.AddReference(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void AddReference_NonExistantPath_ThrowsException()
        {
            var path = @"C:\" + Guid.NewGuid().ToString();
            this.compilerUnderTest.AddReference(path);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem(BadExtensionReference, DependentSourceDirectory)]
        public void AddReference_ExistingPathIncorrectExtension_ThrowsException()
        {
            this.compilerUnderTest.AddReference(BadExtensionReference);
        }

        [TestMethod]
        [DeploymentItem(ReferencedLibraryPath, DependentSourceDirectory)]
        public void AddReference_ExistingPathCorrectExtension_IsSuccessful()
        {
            this.compilerUnderTest.AddReference(ReferencedLibraryPath);
            Assert.AreEqual(1, this.compilerUnderTest.References.Count());
            Assert.AreEqual(
                Path.GetFullPath(ReferencedLibraryPath), 
                this.compilerUnderTest.References.First(), 
                true);
        }

        #endregion

        #region AddReferences

        [TestMethod]
        public void AddReferences_EmptyList_IsSuccessful()
        {
            this.compilerUnderTest.AddReferences(new string[] { });            
            Assert.IsFalse(this.compilerUnderTest.References.Any());
        }

        [TestMethod]
        [DeploymentItem(ReferencedLibraryPath, DependentSourceDirectory)]
        [DeploymentItem(SecondaryReferencedLibraryPath, DependentSourceDirectory)]
        public void AddReferences_ValidListNoDupes_IsSuccessful()
        {
            this.compilerUnderTest.AddReferences(new string[] 
            { 
                ReferencedLibraryPath, 
                SecondaryReferencedLibraryPath
            });
        }

        #endregion

        #region ClearReferences

        [TestMethod]
        [DeploymentItem(ReferencedLibraryPath, DependentSourceDirectory)]
        public void ClearReferences_IsSuccessful()
        {
            this.compilerUnderTest.AddReference(ReferencedLibraryPath);
            Assert.AreEqual(1, this.compilerUnderTest.References.Count(), "A precondition for the test failed!");            

            this.compilerUnderTest.ClearReferences();
            Assert.AreEqual(0, this.compilerUnderTest.References.Count());
        }

        #endregion

        #region CompileTests

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompileTests_NullPath_ThrowsException()
        {
            var assembly = this.compilerUnderTest.CompileTests(null);            
        }

        [TestMethod]        
        [ExpectedException(typeof(ArgumentException))]
        public void CompileTests_EmptyPath_ThrowsException()
        {
            var assembly = this.compilerUnderTest.CompileTests(string.Empty);            
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        [DeploymentItem(ReferencedLibraryPath, DependentSourceDirectory)]
        public void CompileTests_FilePathInsteadOfDirectory_ThrowsException()
        {
            var assembly = this.compilerUnderTest.CompileTests(ReferencedLibraryPath);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilationException))]
        [DeploymentItem(DependentSourcePath, DependentSourceDirectory)]
        public void CompileTests_MissingReference_ThrowsException()
        {
            var assembly = this.compilerUnderTest.CompileTests(DependentSourceDirectory);
            Assert.IsNull(assembly);
        }

        [TestMethod]
        [DeploymentItem(DependentSourcePath, DependentSourceDirectory)]
        [DeploymentItem(ReferencedLibraryPath, DependentSourceDirectory)]
        public void CompileTests_WithReference_ValidAssembly()
        {
            this.compilerUnderTest.AddReference(ReferencedLibraryPath);
            var assembly = this.compilerUnderTest.CompileTests(DependentSourceDirectory);
            Assert.IsNotNull(assembly);
        }

        [TestMethod]
        public void CompileTests_EmptyDirectory_ReturnsNull()
        {
            var emptyDir = Path.Combine(WorkingDirectory, "empty-dir");
            Directory.CreateDirectory(emptyDir);
            
            var assembly = this.compilerUnderTest.CompileTests(WorkingDirectory);
            Assert.IsNull(assembly);
            
            Directory.Delete(emptyDir);
        }

        #endregion

        #endregion
    }
}
