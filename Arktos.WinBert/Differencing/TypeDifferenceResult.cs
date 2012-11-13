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
            this.OldObject = oldType;
            this.NewObject = newType;
            this.Methods = new List<MethodInfo>();
            this.RemovedFields = new List<FieldInfo>();
            this.AddedFields = new List<FieldInfo>();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool IsDifferent
        {
            get
            {
                return this.Methods.Count > 0 || this.OldObject == null || this.NewObject == null;
            }
        }

        /// <inheritdoc />
        public IList<FieldInfo> AddedFields { get; private set; }

        /// <inheritdoc />
        public IList<FieldInfo> RemovedFields { get; private set; }

        /// <inheritdoc />
        public IList<MethodInfo> Methods { get; private set; }

        /// <inheritdoc />
        public Type NewObject { get; private set; }

        /// <inheritdoc />
        public Type OldObject { get; private set; }

        #endregion
    }
}