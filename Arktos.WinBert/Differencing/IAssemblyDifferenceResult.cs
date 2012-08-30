namespace Arktos.WinBert.Differencing
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines the contract for a difference result for an assembly.
    /// </summary>
    public interface IAssemblyDifferenceResult : IDifferenceResult<Assembly>
    {
        #region Properties

        /// <summary>
        ///   Gets a list of type differences for the assembly.
        /// </summary>
        IList<ITypeDifferenceResult> TypeDifferences { get; }

        #endregion
    }
}