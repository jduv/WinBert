﻿namespace Arktos.WinBert.RandoopIntegration.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using Arktos.WinBert.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Arktos.WinBert.Exceptions;

    [TestClass]
    public class RandoopTestCompilerTests : RandoopIntegrationUnitTest
    {
        #region Fields & Constants

        private RandoopTestCompiler compilerUnderTest;

        #endregion

        #region Test Plumbing

        [TestInitialize]
        public void TestInit()
        {
            this.compilerUnderTest = new RandoopTestCompiler(WorkingDir);
        }

        #endregion

        #region Test Methods

        #region Constructors

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_NullPath_ThrowsException()
        {
            var target = new RandoopTestCompiler(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_EmptyPath_ThrowsException()
        {
            var target = new RandoopTestCompiler(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void Ctor_NonExistantPath_ThrowsException()
        {
            var path = @"C:\" + Guid.NewGuid().ToString();
            var target = new RandoopTestCompiler(path);
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
        public void AddReference_ExistingPathIncorrectExtension_ThrowsException()
        {
            this.compilerUnderTest.AddReference(BadExtensionRef);
        }

        [TestMethod]        
        public void AddReference_ExistingPathCorrectExtension_IsSuccessful()
        {
            this.compilerUnderTest.AddReference(RefLibPath);
            Assert.AreEqual(1, this.compilerUnderTest.References.Count());
            Assert.AreEqual(
                Path.GetFullPath(RefLibPath), 
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
        public void AddReferences_ValidListNoDupes_IsSuccessful()
        {
            this.compilerUnderTest.AddReferences(new string[] 
            { 
                RefLibPath, 
                SecondaryRefLibPath
            });

            var references = this.compilerUnderTest.References.ToList();

            Assert.AreEqual(2, references.Count);
            Assert.IsTrue(references.Contains(Path.GetFullPath(RefLibPath)));
            Assert.IsTrue(references.Contains(Path.GetFullPath(SecondaryRefLibPath)));
        }

        #endregion

        #region ClearReferences

        [TestMethod]        
        public void ClearReferences_IsSuccessful()
        {
            this.compilerUnderTest.AddReference(RefLibPath);
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
        public void CompileTests_FilePathInsteadOfDirectory_ThrowsException()
        {
            var assembly = this.compilerUnderTest.CompileTests(RefLibPath);
        }

        [TestMethod]        
        public void CompileTests_MultipleFiles_ValidAssembly()
        {
            var assembly = this.compilerUnderTest.CompileTests(TestSrcDir);
            Assert.IsNotNull(assembly);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilationException))]       
        public void CompileTests_MissingReference_ThrowsException()
        {
            var assembly = this.compilerUnderTest.CompileTests(DependentSrcDir);
            Assert.IsNull(assembly);
        }

        [TestMethod]        
        public void CompileTests_WithReference_ValidAssembly()
        {
            this.compilerUnderTest.AddReference(RefLibPath);
            var assembly = this.compilerUnderTest.CompileTests(DependentSrcDir);
            Assert.IsNotNull(assembly);
        }

        [TestMethod]
        public void CompileTests_EmptyDirectory_ReturnsNull()
        {
            var emptyDir = Path.Combine(WorkingDir, "empty-dir");
            Directory.CreateDirectory(emptyDir);
            
            var assembly = this.compilerUnderTest.CompileTests(emptyDir);
            Assert.IsNull(assembly);
            
            Directory.Delete(emptyDir);
        }

        #endregion

        #endregion
    }
}
