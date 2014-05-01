namespace Arktos.WinBert.Differencing
{
    using System;

    /// <summary>
    /// Abstract class for basic analysis log difference functionality. More specific instances should be built to
    /// handle the nuances of each situation.
    /// </summary>
    public abstract class AnalysisLogDiff : IAnalysisLogDiff
    {
        #region Constructors & Destructors

        public AnalysisLogDiff(string oldValue, string newValue, string typeFullName)
        {
            if (oldValue == null)
            {
                throw new ArgumentNullException("oldValue");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }
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
