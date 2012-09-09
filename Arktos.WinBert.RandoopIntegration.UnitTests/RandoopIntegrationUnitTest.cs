﻿namespace Arktos.WinBert.RandoopIntegration.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [DeploymentItem(@"test-src\", @"test-src\")]
    [DeploymentItem(@"dependent-src\", @"dependent-src\")]
    [DeploymentItem(@"test-configuration\", @"test-configuration\")]
    public abstract class RandoopIntegrationUnitTest
    {
        #region Fields and Constants

        protected static readonly string WorkingDir = @".\";

        protected static readonly string TestSrcDir = @"test-src\";

        protected static readonly string DependentSrcDir = @"dependent-src\";

        protected static readonly string ConfigDir = @"test-configuration\";

        protected static readonly string DependentSrcPath = DependentSrcDir + @"Dependent.cs";

        protected static readonly string RefLibPath = DependentSrcDir + @"Dependency.dll";

        protected static readonly string SecondaryRefLibPath = DependentSrcDir + @"CopyOfDependency.dll";

        protected static readonly string BadExtensionRef = DependentSrcDir + @"Dependency.txt";

        #endregion
    }
}
