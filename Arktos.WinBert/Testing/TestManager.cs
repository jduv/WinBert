namespace Arktos.WinBert.Testing
{
    using System;
    using AppDomainToolkit;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// This base class contains some basic implementations that TestManager derived classes will 
    /// find useful.
    /// </summary>
    public abstract class TestManager : ITestManager
    {
        #region Fields & Constants

        private readonly WinBertConfig config;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the TestManager class.
        /// </summary>
        /// <param name="config">
        /// The configuration.
        /// </param>
        public TestManager(WinBertConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            this.config = config;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public abstract AnalysisResult BuildAndExecuteTests(Build previous, Build current);

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs a diff in another application domain. The method by which this diff occurs should likely
        /// never change, so the method is considered sealed. Derived classes should always call this to get
        /// the differences between the two assemblies in question.
        /// </summary>
        /// <param name="previous">
        /// The previous build.
        /// </param>
        /// <param name="current">
        /// The current build.
        /// </param>
        /// <returns>
        /// The assembly difference result.
        /// </returns>
        protected IAssemblyDifferenceResult DoDiff(Build previous, Build current)
        {
            using (var diffEnv = AppDomainContext.Create())
            {
                // Execute the diff in another application domain.
                return RemoteFunc.Invoke(
                    diffEnv.Domain,
                    this.config.IgnoreList,
                    previous.AssemblyPath,
                    current.AssemblyPath,
                    (ignoreTargets, previousTargetPath, currentTargetPath) =>
                    {
                        // Fire up an assembly loader.
                        var loader = new AssemblyLoader();
                        var oldAssembly = loader.LoadAssembly(LoadMethod.LoadFile, previousTargetPath);
                        var newAssembly = loader.LoadAssembly(LoadMethod.LoadFile, currentTargetPath);

                        var differ = new AssemblyDifferenceEngine(ignoreTargets);
                        return differ.Diff(oldAssembly, newAssembly);
                    });
            }
        }

        #endregion
    }
}
