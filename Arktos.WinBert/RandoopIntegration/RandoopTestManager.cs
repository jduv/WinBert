namespace Arktos.WinBert.RandoopIntegration
{
    using System.Collections.Generic;
    using AppDomainToolkit;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Xml;
    using System;

    /// <summary>
    /// The class that ties everything together. An implementation of this should be able to manage
    /// pulling together all the miscellaneous pieces required to build out a regression test suite and execute
    /// it, returning an analysis result.
    /// </summary>
    public class RandoopTestManager : TestManager
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
        public RandoopTestManager(WinBertConfig config)
            : base(config)
        {
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        /// <remarks>
        /// This method will generate tests using the Randoop fuzzing framework. Generating these tests must happen in a separate application
        /// domain to prevent assembly pollution in the current application domain.
        /// </remarks>
        public override ITestTarget GenerateTests(IAssemblyTarget target, IEnumerable<string> validTypeNames)
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
                        return tester.GenerateTests(target, types);
                    });
            }
        }

        /// <inheritdoc />
        public override ITestRunResult RunTests(ITestTarget toRun)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
