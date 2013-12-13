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
        public IEnumerable<IAnalysisLogDiff> DiffValues(Xml.Value oldValue, Xml.Value newValue)
        {
            // Neither root may be null.
            if (oldValue == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newObject");
            }

            // Ensure that both items are comparable.
            if (newValue.IsPrimtive && oldValue.IsPrimtive)
            {
                return DiffPrimitives(oldValue.AsPrimitive, newValue.AsPrimitive);
            }
            else if (newValue.IsObject && oldValue.IsObject)
            {
                return this.DiffObjects(oldValue.AsObject, newValue.AsObject);
            }
            else
            {
                var msg = string.Format(
                    "Items are incompatible for comparison! oldObject.Item.Type => {0}, newObject.Item.Type => {1}",
                    oldValue.UnderlyingType,
                    newValue.UnderlyingType);
                throw new ArgumentException(msg);
            }
        }

        /// <inheritdoc />
        public IEnumerable<IAnalysisLogDiff> DiffPrimitives(Xml.Primitive oldPrimitive, Xml.Primitive newPrimitive)
        {
            if (oldPrimitive == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newPrimitive == null)
            {
                throw new ArgumentNullException("newObject");
            }

            if (oldPrimitive.IsComparableTo(newPrimitive))
            {
                // Return a new diff here.
                yield break;
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
