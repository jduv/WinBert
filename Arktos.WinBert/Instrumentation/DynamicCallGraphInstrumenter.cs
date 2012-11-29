namespace Arktos.WinBert.Instrumentation
{
    using System;
    using AppDomainToolkit;

    /// <summary>
    /// This class will instrument the target assembly placing probes at the start of all methods. During execution
    /// of the target assembly, the probes will print information about the currently executing method.
    /// </summary>
    public sealed class DynamicCallGraphInstrumenter
    {
        #region Public Methods

        /// <summary>
        /// Instruments the target assembly by spinning through all methods and adding probes used to log
        /// the dynamic call graph of a program in situ.
        /// </summary>
        /// <param name="toInstrument">
        /// The assembly target to instrument.
        /// </param>
        /// <returns>
        /// The instrumented assembly.
        /// </returns>
        public IAssemblyTarget Instrument(IAssemblyTarget toInstrument)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
