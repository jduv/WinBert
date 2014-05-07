namespace Arktos.WinBert.Differencing
{
    using System;

    /// <summary>
    /// Abstract class for basic analysis log difference functionality. More specific instances should be built to
    /// handle the nuances of each situation.
    /// </summary>
    public class AnalysisLogDifference
    {
        #region Constructors & Destructors

        public AnalysisLogDifference(string oldValue, string newValue, string typeFullName)
        {
            if (oldValue == null)
            {
                throw new ArgumentNullException("oldValue");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.TypeFullName = typeFullName;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string OldValue { get; private set; }

        /// <inheritdoc />
        public string NewValue { get; private set; }

        /// <inheritdoc />
        public string TypeFullName { get; private set; }

        #endregion
    }
}
