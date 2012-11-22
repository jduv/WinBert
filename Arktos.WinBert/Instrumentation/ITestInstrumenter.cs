namespace Arktos.WinBert.Instrumentation
{
    using AppDomainToolkit;

    /// <summary>
    /// Defines behavior for a class that instruments tests.
    /// </summary>
    public interface ITestInstrumenter
    {
        #region Methods

        /// <summary>
        /// Instruments the target assembly and returns a handle to the instrumented tests.
        /// </summary>
        /// <param name="toInstrument">
        /// The assembly to instrument.
        /// </param>
        /// <returns>
        /// The instrumented target.
        /// </returns>
        IAssemblyTarget InstrumentTests(IAssemblyTarget toInstrument);

        #endregion
    }
}
