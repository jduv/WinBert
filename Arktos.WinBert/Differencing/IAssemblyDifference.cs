﻿namespace Arktos.WinBert.Differencing
{
    using System.Collections.Generic;
    using AppDomainToolkit;

    /// <summary>
    /// Defines the contract for a difference result for an assembly.
    /// </summary>
    public interface IAssemblyDifference : IDifferenceResult
    {
        #region Properties

        /// <summary>
        ///   Gets a list of type differences for the assembly.
        /// </summary>
        IEnumerable<ITypeDifference> TypeDifferences { get; }

        /// <summary>
        /// Gets the new assembly target.
        /// </summary>
        IAssemblyTarget NewAssemblyTarget { get; }

        /// <summary>
        /// Gets the old assembly target.
        /// </summary>
        IAssemblyTarget OldAssemblyTarget { get; }

        #endregion
    }
}