namespace Arktos.WinBert.VsPackage.Converters
{
    using System;
    using System.Collections;
    using System.Windows;

    /// <summary>
    /// Handles visibility for lists.
    /// </summary>
    public class EmptyListVisibilityConverter : BooleanConverter<Visibility>
    {
        #region Fields & Constants

        private static Predicate<object> predicate = new Predicate<object>(
                (list) =>
                {
                    return list != null && ((ICollection)list).Count == 0;
                });

        #endregion

        #region Constructors & Destructors

        public EmptyListVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        {
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        protected override Predicate<object> ConverterPredicate
        {
            get
            {
                return predicate;
            }
        }

        #endregion
    }
}
