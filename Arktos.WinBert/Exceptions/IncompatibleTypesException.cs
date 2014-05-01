namespace Arktos.WinBert.Exceptions
{
    using System;

    public class IncompatibleTypesException<T> : Exception
    {
        #region Fields & Constants

        private static readonly string IncompatibleTypesMessage = "Possible analysis file corruption: objects are incompatible for comparison!";

        #endregion

        #region Constructors & Destructors

        public IncompatibleTypesException(T oldValue, T newValue)
            : base(IncompatibleTypesMessage)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the old value.
        /// </summary>
        public T OldValue { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public T NewValue { get; private set; }

        #endregion
    }
}
