namespace Arktos.WinBert.RandoopIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using AppDomainToolkit;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// The class that ties everything together. An implementation of this should be able to manage
    /// pulling together all the miscellaneous pieces required to build out a regression test suite and execute
    /// it, returning an analysis result.
    /// </summary>
    public class RandoopRegressionTestManager : TestManager
    {
        #region Fields & Constants

        private readonly WinBertConfig config;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the RegressionTestSuiteManager class.
        /// </summary>
        /// <param name="config">
        /// The configuration to initialize with.
        /// </param>
        public RandoopRegressionTestManager(WinBertConfig config)
            : base(config)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates a test suite.
        /// </summary>
        /// <param name="previous">
        /// The previous build.
        /// </param>
        /// <param name="current">
        /// The current build.
        /// </param>
        /// <returns>A test suite, or null if something went wrong.</returns>
        public override AnalysisResult Run(Build previous, Build current)
        {
            var diff = this.DoDiff(previous, current);

            if (diff != null && diff.IsDifferent)
            {
                var typeNames = diff.TypeDifferences.Select(x => x.Name);
                var previousTestTarget = this.BuildAndExecuteTests(diff.OldAssemblyTarget, typeNames);
            }

            return null;
            ////using (var currentBuildEnv = new AssemblyEnvironment())
            ////using (var previousBuildEnv = new AssemblyEnvironment())
            ////{
            ////    // Add assembly paths for the target assemblies
            ////    currentBuildEnv.Resolver.AddProbePath(Path.GetDirectoryName(current.AssemblyPath));
            ////    currentBuildEnv.Resolver.LoadMethod = LoadMethod.LoadFile;
            ////    previousBuildEnv.Resolver.AddProbePath(Path.GetDirectoryName(previous.AssemblyPath));
            ////    previousBuildEnv.Resolver.LoadMethod = LoadMethod.LoadFile;

            ////    AnalysisResult result = null;
            ////    var currentAssembly = currentBuildEnv.LoadFile(current.AssemblyPath);
            ////    var previousAssembly = previousBuildEnv.LoadFile(previous.AssemblyPath);

            ////    var differ = new AssemblyDifferenceEngine(this.config.IgnoreList);
            ////    var diff = differ.Diff(previousAssembly, currentAssembly);

            ////    if (diff.IsDifferent)
            ////    {
            ////        // First get the tests.
            ////        var typeNames = diff.TypeDifferences.Select(x => x.Name);
            ////        var oldTests = this.generator.GetTestsFor(previousAssembly, typeNames);
            ////        var newTests = this.generator.GetTestsFor(currentAssembly, typeNames);

            ////        // Next instrument the tests
            ////        var instrumentedOldTests = this.instrumenter.InstrumentTests(oldTests);
            ////        var instrumentedNewTests = this.instrumenter.InstrumentTests(newTests);

            ////        // Finally execute the tests
            ////        var oldResult = this.runner.RunTests(previousAssembly);
            ////        var newResult = this.runner.RunTests(currentAssembly);

            ////        result = this.analyzer.Analyze(oldResult, newResult);
            ////    }

            ////    return result;
            ////}
        }

        /// <summary>
        /// Generates tests for the target assembly in another application domain.
        /// </summary>
        /// <param name="target">
        /// The assembly target to generate tests for.
        /// </param>
        /// <param name="validTypeNames">
        /// A list of valid types to generate those tests for.
        /// </param>
        /// <returns>
        /// An assembly target pointing to the assembly containing the generated tests.
        /// </returns>
        public IAssemblyTarget BuildAndExecuteTests(IAssemblyTarget target, IEnumerable<string> validTypeNames)
        {
            using (var testEnv = AppDomainContext.Create())
            {
                return RemoteFunc.Invoke(
                    testEnv.Domain,
                    this.config,
                    target,
                    validTypeNames,
                    (config, testTarget, types) =>
                    {
                        var tester = new RandoopTestGenerator(config);
                        return tester.GetTestsFor(target, types);
                    });
            }
        }

        #endregion
    }
}
