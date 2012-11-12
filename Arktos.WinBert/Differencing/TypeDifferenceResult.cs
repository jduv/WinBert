namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// This class represents a difference result between two types. The differences between the
    ///   two types is represented by a list of method names that have changed. Note that this does not
    ///   truly represent a 'diff' between the two types, as we are only interested in the methods inside
    ///   the two types that 1) have the same signature and 2) have a different body.
    /// </summary>
    public sealed class TypeDifferenceResult : ITypeDifferenceResult
    {
        #region Constants and Fields

        /// <summary>
        ///   The new type (parameter 2 of the constructor).
        /// </summary>
        private readonly Type newType = null;

        /// <summary>
        ///   The old type (parameter 1 of the constructor).
        /// </summary>
        private readonly Type oldType = null;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the TypeDifferenceResult class.
        /// </summary>
        /// <param name="oldType">
        /// The old type.
        /// </param>
        /// <param name="newType">
        /// The new type.
        /// </param>
        public TypeDifferenceResult(Type oldType, Type newType)
        {
            this.oldType = oldType;
            this.newType = newType;
            this.MethodNames = new List<MethodInfo>();
            this.RemovedFields = new List<FieldInfo>();
            this.AddedFields = new List<FieldInfo>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a list of fields that were added to the new version of the class and didn't exist in the old
        ///   version.
        /// </summary>
        public IList<FieldInfo> AddedFields { get; private set; }

        /// <summary>
        ///   Gets a list of fields that were removed in the new version of the class.
        /// </summary>
        public IList<FieldInfo> RemovedFields { get; private set; }

        /// <summary>
        ///   Gets a list of names of methods that have changed between the two target types. This should not include 
        ///   methods that have the same name but different contracts i.e. foo(x, y) vs foo(x).
        /// </summary>
        public IList<MethodInfo> MethodNames { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether there is a difference or not.
        /// </summary>
        public bool IsDifferent
        {
            get
            {
                return this.MethodNames.Count > 0 || this.oldType == null || this.newType == null;
            }
        }

        /// <summary>
        ///   Gets the new type.
        /// </summary>
        public Type NewObject
        {
            get
            {
                return this.newType;
            }
        }

        /// <summary>
        ///   Gets the old type.
        /// </summary>
        public Type OldObject
        {
            get
            {
                return this.oldType;
            }
        }

        #endregion
    }
}