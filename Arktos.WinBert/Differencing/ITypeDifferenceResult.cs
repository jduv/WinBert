namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines a contract for implementations that calculate differences between types.
    /// </summary>
    public interface ITypeDifferenceResult : IDifferenceResult
    {
        #region Properties

        /// <summary>
        /// Gets the name of the type. This should be the same in both target assembly 
        /// representations.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the fully qualified name of the type.
        /// </summary>
        string FullName { get;  }

        /// <summary>
        ///   Gets a list of field names that were added to the new version of the class and didn't exist in the old
        ///   version.
        /// </summary>
        IList<string> AddedFields { get; }

        /// <summary>
        ///   Gets a list of method names that have changed between the two target types.
        IList<string> Methods { get; }

        /// <summary>
        ///   Gets a list of field names that were removed in the new version of the class.
        /// </summary>
        IList<string> RemovedFields { get; }

        #endregion
    }
}