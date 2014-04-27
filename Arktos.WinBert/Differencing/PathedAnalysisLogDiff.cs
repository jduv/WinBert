namespace Arktos.WinBert.Differencing
{
    using System;

    /// <summary>
    /// Implementation of a simple analysis log diff. Conveys some simple information about a difference between
    /// two objects in the object graph built by the Winbert analysis engine.
    /// </summary>
    public class PathedAnalysisLogDiff : AnalysisLogDiff
    {
        #region Constructors & Destructors

        public PathedAnalysisLogDiff(IMemberPath memberPath, string oldValue, string newValue, string typeFullName)
            : base(oldValue, newValue, typeFullName)
        {
            if (memberPath == null)
            {
                throw new ArgumentNullException("memberPath");
            }

            this.Path = memberPath;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IMemberPath Path { get; private set; }

        #endregion
    }
}
