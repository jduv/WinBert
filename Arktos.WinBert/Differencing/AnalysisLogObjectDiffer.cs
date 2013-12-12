namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation that performs differences between analysis log elements. There is an important underlying assumption
    /// to make here, and that is that the analysis log will never provide a null value on any element. We enforce that here.
    /// </summary>
    public class AnalysisLogObjectDiffer : IAnalysisLogObjectDiffer
    {
        #region Public Methods

        /// <inheritdoc />
        public IEnumerable<IAnalysisLogDiff> DiffValues(Xml.Value oldObject, Xml.Value newObject)
        {
            // Neither root may be null.
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            yield break;
        }

        /// <inheritdoc />
        public IEnumerable<IAnalysisLogDiff> DiffObjects(Xml.Object oldObject, Xml.Object newObject)
        {
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            yield break;
        }

        #endregion
    }
}
