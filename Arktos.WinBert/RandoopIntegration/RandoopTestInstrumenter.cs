﻿namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using Arktos.WinBert.Instrumentation;
    using Arktos.WinBert.Testing;

    /// <summary>
    /// Instruments a test assembly that was generated by Randoop.
    /// </summary>
    public class RandoopTestInstrumenter : ITestInstrumenter
    {
        #region Public Methods

        /// <inheritdoc/>
        public ITestTarget InstrumentTests(ITestTarget toInstrument)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
