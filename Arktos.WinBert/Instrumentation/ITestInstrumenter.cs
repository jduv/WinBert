namespace Arktos.WinBert.Instrumentation
{
    using Arktos.WinBert.Environment;

    public interface ITestInstrumenter
    {
        #region Methods

        AssemblyTarget InstrumentTests(AssemblyTarget toInstrument);

        #endregion
    }
}
