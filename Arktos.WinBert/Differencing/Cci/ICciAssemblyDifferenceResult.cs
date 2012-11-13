namespace Arktos.WinBert.Differencing.Cci
{
    using System.Collections.Generic;
    using Microsoft.Cci;

    /// <summary>
    /// Defines behavior for a difference result between two assemblies in the CCI metadata
    /// context.
    /// </summary>
    public interface ICciAssemblyDifferenceResult : IDifferenceResult<IAssembly>
    {
        #region Properties

        /// <summary>
        /// Gets a list of type differences.
        /// </summary>
        IList<ICciTypeDifferenceResult> TypeDifferences { get; }

        #endregion
    }
}
