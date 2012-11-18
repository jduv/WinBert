namespace Arktos.WinBert.Instrumentation
{
    using Arktos.WinBert.Environment;

    public interface ITestInstrumenter
    {
        #region Methods

        IAssemblyTarget InstrumentTests(IAssemblyTarget toInstrument);

        #endregion
    }
}
