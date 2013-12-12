namespace Arktos.WinBert.Differencing
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a contract for implementations that calculate differences between types.
    /// </summary>
    public interface ITypeDifference : IDifferenceResult
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
        string FullName { get; }

        /// <summary>
        ///   Gets a list of field names that were added to the new version of the class and didn't exist in the old
        ///   version.
        /// </summary>
        IEnumerable<string> AddedFields { get; }

        /// <summary>
        ///   Gets a list of method names that have changed between the two target types.
        IEnumerable<string> Methods { get; }

        /// <summary>
        ///   Gets a list of field names that were removed in the new version of the class.
        /// </summary>
        IEnumerable<string> RemovedFields { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Does this type difference contain a method with the target signature flagged as changed?
        /// </summary>
        /// <param name="methodName">
        /// The signature to test.
        /// </param>
        /// <returns>
        /// True if this difference contains a method signature in it's change set that matches the target
        /// method signture, false otherwise.
        /// </returns>
        bool Contains(string methodName);

        #endregion
    }
}