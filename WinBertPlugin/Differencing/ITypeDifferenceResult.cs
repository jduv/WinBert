namespace WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines a contract for implementations that calculate differences between types.
    /// </summary>
    public interface ITypeDifferenceResult : IDifferenceResult<Type>
    {
        #region Properties

        /// <summary>
        ///   Gets a list of fields that were added to the new version of the class and didn't exist in the old
        ///   version.
        /// </summary>
        IList<FieldInfo> AddedFields { get; }

        /// <summary>
        ///   Gets a list of names of methods that have changed between the two target types. This should not include 
        ///   methods that have the same name but different contracts i.e. foo(x, y) vs foo(x).
        /// </summary>
        IList<MethodInfo> MethodNames { get; }

        /// <summary>
        ///   Gets a list of fields that were removed in the new version of the class.
        /// </summary>
        IList<FieldInfo> RemovedFields { get; }

        #endregion
    }
}