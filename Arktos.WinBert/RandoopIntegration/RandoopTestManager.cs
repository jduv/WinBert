namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AppDomainToolkit;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Xml;
    using System.Threading.Tasks;

    /// <summary>
    /// The class that ties everything together. An implementation of this should be able to manage
    /// pulling together all the miscellaneous pieces required to build out a regression test suite and execute
    /// it, returning an analysis result.
    /// </summary>
    public class RandoopTestManager : TestManager
    {
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
        public override IAssemblyTarget GenerateTests(IAssemblyTarget target, IEnumerable<string> validTypeNames)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            using (var testEnv = AppDomainContext.Create())
            {
                testEnv.RemoteResolver.AddProbePath(Path.GetDirectoryName(target.Location));
                return RemoteFunc.Invoke(
                    testEnv.Domain,
                    this.Config,
                    target,
                    validTypeNames.ToList(),
                    (config, testTarget, types) =>
                    {
                        var tester = new RandoopTestGenerator(config);
                        return tester.GenerateTests(testTarget, types);
                    });
            }
        }

        /// <inheritdoc />
        public override ITestTarget InstrumentTests(ITestTarget toInstrument)
        {
            if (toInstrument == null)
            {
                throw new ArgumentNullException("toInstrument");
            }

            var instrumenter = new RandoopTestInstrumenter();
            return instrumenter.InstrumentTests(toInstrument);
        }

        /// <inheritdoc />
        public override ITestRunResult RunTests(IAssemblyTarget target, IAssemblyTarget tests)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (tests == null)
            {
                throw new ArgumentNullException("tests");
            }

            // First, execute the tests against the new assembly.
            using (var runEnv = AppDomainContext.Create())
            {
                runEnv.RemoteResolver.AddProbePath(Path.GetDirectoryName(target.Location));
                runEnv.RemoteResolver.AddProbePath(Path.GetDirectoryName(tests.Location));

                return RemoteFunc.Invoke(
                    runEnv.Domain,
                    target,
                    tests,
                    (targetArg, testsArg) =>
                    {
                        var runner = new RandoopTestRunner();
                        return runner.RunTests(targetArg, testsArg);
                    });
            }
        }

        #endregion
    }
}
