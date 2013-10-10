namespace Arktos.WinBert.Instrumentation
{
    using Arktos.WinBert.Testing;

    /// <summary>
    /// Defines behavior for a class that instruments tests.
    /// </summary>
    public interface ITestInstrumentor
    {
        #region Methods

        /// <summary>
        /// Instruments the target assembly and returns a handle to the instrumented tests.
        /// </summary>
        /// <param name="toInstrument">
        /// The assembly pair to instrument.
        /// </param>
        /// <returns>
        /// The instrumented targets.
        /// </returns>
        ITestTarget InstrumentTests(ITestTarget toInstrument);

        #endregion
    }
}
