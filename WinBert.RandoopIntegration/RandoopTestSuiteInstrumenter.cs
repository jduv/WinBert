﻿namespace WinBert.RandoopIntegration
{
    using System;
    using System.Reflection;
    using WinBert.Instrumentation;
    using WinBert.Testing;

    /// <summary>
    /// Instruments a test assembly that was generated by Randoop.
    /// </summary>
    public class RandoopTestSuiteInstrumenter
    {
        #region Public Methods

        /// <summary>
        /// This method will instrument the target test assembly and return a modified copy.
        /// </summary>
        /// <param name="testsToInstrument">
        /// The tests to instrument.
        /// </param>
        /// <returns>
        /// A test assembly that has been successfully instrumented.
        /// </returns>
        public ITestSuite InstrumentTestSuite(ITestSuite testsToInstrument)
        {
            var instrumentedOldTests = this.InstrumentTestAssembly(testsToInstrument.OldTargetTestAssembly);
            var instrumentedNewTests = this.InstrumentTestAssembly(testsToInstrument.NewTargetTestAssembly);
            var instrumentedOldAssembly = this.InstrumentTargetAssembly(testsToInstrument.OldTargetAssembly);
            var instrumentedNewAssembly = this.InstrumentTargetAssembly(testsToInstrument.NewTargetAssembly);

            return new InstrumentedTestSuite(testsToInstrument)
            {
                InstrumentedOldTargetTestAssembly = instrumentedOldTests, 
                InstrumentedNewTargetTestAssembly = instrumentedNewTests, 
                InstrumentedNewTargetAssembly = instrumentedNewAssembly, 
                InstrumentedOldTargetAssembly = instrumentedOldAssembly
            };
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Returns an instrumented copy of the target TestAssembly.
        /// </summary>
        /// <param name="testAssembly">
        /// The TestAssembly to instrument.
        /// </param>
        /// <returns>
        /// An instrumented copy of the target TestAssembly.
        /// </returns>
        private Assembly InstrumentTestAssembly(Assembly testAssembly)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an instrumented copy of the target Assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly to instrument.
        /// </param>
        /// <returns>
        /// An instrumented copy of the target Assembly.
        /// </returns>
        private Assembly InstrumentTargetAssembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
