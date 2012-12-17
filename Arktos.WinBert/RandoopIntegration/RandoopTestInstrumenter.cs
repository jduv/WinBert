﻿namespace Arktos.WinBert.RandoopIntegration
{
    using AppDomainToolkit;
    using Arktos.WinBert.Instrumentation;
    using Arktos.WinBert.Testing;
    using System.Threading.Tasks;

    /// <summary>
    /// Instruments a test assembly that was generated by Randoop.
    /// </summary>
    public class RandoopTestInstrumenter : ITestInstrumenter
    {
        #region Fields & Constants

        private static readonly string testMethodName = "Main";

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public ITestTarget InstrumentTests(ITestTarget toInstrument)
        {
            IAssemblyTarget instrumentedTests = null, instrOldAssembly = null, instrNewAssembly = null;

            // Do all the instrumenting in parallel.
            Parallel.Invoke(
                () =>
                {
                    instrumentedTests = this.InstrumentTestAssembly(toInstrument.TestAssembly);
                },
                () =>
                {
                    instrOldAssembly = this.InstrumentTarget(toInstrument.TargetOldAssembly);
                },
                () =>
                {
                    instrNewAssembly = this.InstrumentTarget(toInstrument.TargetNewAssembly);
                });

            return TestTarget.Create(instrOldAssembly, instrNewAssembly, instrumentedTests);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Instruments the test assembly.
        /// </summary>
        /// <param name="toInstrument">
        /// The assembly to instrument.
        /// </param>
        /// <returns>
        /// A target to the new tests.
        /// </returns>
        private IAssemblyTarget InstrumentTestAssembly(IAssemblyTarget toInstrument)
        {
            using (var testTarget = InstrumentationTarget.Create(toInstrument))
            {
                var rewriter = RandoopTestRewriter.Create(testMethodName, testTarget.Host);
                return rewriter.Rewrite(testTarget);
            }
        }

        /// <summary>
        /// Instyruments the target assembly. Simply adds probes to infer a dynamic call graph during
        /// execution.
        /// </summary>
        /// <param name="toInstrument">
        /// The assembly to instrument.
        /// </param>
        /// <returns>
        /// A target to the new assembly.
        /// </returns>
        private IAssemblyTarget InstrumentTarget(IAssemblyTarget toInstrument)
        {
            using (var target = InstrumentationTarget.Create(toInstrument))
            {
                // Do instrumentation here.
                return target.Save();
            }
        }

        #endregion
    }
}
