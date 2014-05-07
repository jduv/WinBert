namespace Arktos.WinBert.VsPackage.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Generic boolean converter.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values to return.
    /// </typeparam>
    public abstract class BooleanConverter<T> : IValueConverter
    {
        #region Constructors & Destructors

        public BooleanConverter(T trueValue, T falseValue)
        {
            this.True = trueValue;
            this.False = falseValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the true value.
        /// </summary>
        public T True { get; set; }

        /// <summary>
        /// Gets the false value.
        /// </summary>
        public T False { get; set; }

        /// <summary>
        /// Gets the predicate used for determining which value to return (True or False).
        /// </summary>
        protected abstract Predicate<object> ConverterPredicate { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ConverterPredicate(value) ? True : False;
        }

        /// <inheritdoc />
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }

        #endregion
    }
}