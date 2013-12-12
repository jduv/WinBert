namespace Arktos.WinBert.Differencing
{
    using System;

    /// <summary>
    /// Implementation of a simple analysis log diff. Conveys some simple information about a difference between
    /// two objects in the object graph built by the Winbert analysis engine.
    /// </summary>
    public class AnalysisLogDiff : IAnalysisLogDiff
    {
        #region Constructors & Destructors

        public AnalysisLogDiff(IMemberPath memberPath, string oldValue, string newValue)
        {
            if (memberPath == null)
            {
                throw new ArgumentNullException("memberPath");
            }

            if (oldValue == null)
            {
                throw new ArgumentNullException("oldValue");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }
            this.Path = memberPath;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IMemberPath Path { get; private set; }

        /// <inheritdoc />
        public string OldValue { get; private set; }

        /// <inheritdoc />
        public string NewValue { get; private set; }

        #endregion
    }
}
