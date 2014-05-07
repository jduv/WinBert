namespace Arktos.WinBert.Differencing
{
    using System;

    /// <summary>
    /// Implementation of a simple analysis log diff. Conveys some simple information about a difference between
    /// two objects in the object graph built by the Winbert analysis engine.
    /// </summary>
    public class PathedAnalysisLogDifference : AnalysisLogDifference
    {
        #region Constructors & Destructors

        public PathedAnalysisLogDifference(string memberPath, string oldValue, string newValue, string typeFullName)
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

        /// <summary>
        /// Gets the path to the difference with respect to the object it's contained within.
        /// </summary>
        public string Path { get; private set; }

        #endregion
    }
}
