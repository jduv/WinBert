namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Xml;
    using System.Collections.Generic;

    /// <summary>
    /// A test suite manager aimed and generating and instrumenting Randoop generated tests.
    /// </summary>
    public class RandoopRegressionTestSuiteManager : RegressionTestSuiteManager
    {
        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the RandoopRegressionTestSuiteManager class.
        /// </summary>
        /// <param name="config">
        /// The configuration to use when executing tests. Must contain a valid randoop plugin
        /// configuration.
        /// </param>
        public RandoopRegressionTestSuiteManager(WinBertConfig config)
            : base(config)
        {
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc/>
        protected override IRegressionTestSuite BuildTestSuite(Build current, Build previous, AssemblyDifferenceResult diff)
        {
            IRegressionTestSuite result = null;
            var testGen = new RandoopTestGenerator(this.Config);
            var types = diff.TypeDifferences.Select(x => x.NewObject).ToList();

            // Generate tests for the last tested build if we need to
            Assembly previousBuildTests;
            if (string.IsNullOrEmpty(previous.TestAssemblyPath))
            {
                previousBuildTests = testGen.GetTestAssembly(
                    diff.OldObject,
                    types,
                    previous.AssemblyPath);
            }
            else
            {
                previousBuildTests = LoadAssembly(previous.AssemblyPath, null);
            }

            // Generate tests for the newest build
            var currentBuildTests = testGen.GetTestAssembly(diff.NewObject, types, current.AssemblyPath);

            if (previousBuildTests != null && currentBuildTests != null)
            {
                current.TestAssemblyPath = currentBuildTests.Location;
                previous.TestAssemblyPath = previousBuildTests.Location;
                result = new RegressionTestSuite(currentBuildTests, previousBuildTests, diff);
            }

            return result;
        }

        /// <inheritdoc/>
        protected override AnalysisResult ExecuteTestSuite(IRegressionTestSuite testSuite)
        {
            return null;
        }



        #endregion
    }
}
