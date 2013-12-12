namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class represents a difference result between two types. The differences between the
    /// two types is represented by a list of method names that have changed. Note that this does not
    /// truly represent a 'diff' between the two types, as we are only interested in the methods inside
    /// the two types that 1) have the same signature and 2) have a different body. This class should always 
    /// be marked serializable to avoid issues with app domain lifecycles. Never pass the raw types, 
    /// however back and forth across application domains--you'll pollute the current app domain.
    /// </summary>
    [Serializable]
    public sealed class TypeDifference : ITypeDifference
    {
        #region Fields & Constants

        private readonly HashSet<string> methods;
        private readonly IEnumerable<string> removedFields;
        private readonly IEnumerable<string> addedFields;

        #endregion

        #region Constructors & Destructors

        public TypeDifference(
            Type type,
            IEnumerable<string> methods = null,
            IEnumerable<string> removedFields = null,
            IEnumerable<string> addedFields = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            this.Name = type.Name;
            this.FullName = type.FullName;
            this.methods = methods == null ? new HashSet<string>() : new HashSet<string>(methods);
            this.removedFields = removedFields ?? Enumerable.Empty<string>();
            this.addedFields = addedFields ?? Enumerable.Empty<string>();
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool AreDifferences
        {
            get
            {
                return this.Methods.Any() || this.AddedFields.Any() || this.RemovedFields.Any();
            }
        }

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public string FullName { get; private set; }

        /// <inheritdoc />
        public IEnumerable<string> AddedFields
        {
            get
            {
                return this.addedFields;
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> Methods
        {
            get
            {
                return this.methods;
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> RemovedFields
        {
            get
            {
                return this.removedFields;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public bool Contains(string methodSignature)
        {
            return this.methods.Contains(methodSignature);
        }

        #endregion
    }
}