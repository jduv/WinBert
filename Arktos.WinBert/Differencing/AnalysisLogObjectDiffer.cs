namespace Arktos.WinBert.Differencing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Implementation that performs differences between analysis log elements. There is an important underlying assumption
    /// to make here, and that is that the analysis log will never provide a null value on any element. We enforce that here.
    /// </summary>
    public class AnalysisLogObjectDiffer : IAnalysisLogObjectDiffer
    {
        #region Public Methods

        /// <inheritdoc />
        public IEnumerable<IAnalysisLogDiff> DiffReturnValues(Xml.Value oldValue, Xml.Value newValue)
        {
            // If both values are not null...
            IEnumerable<IAnalysisLogDiff> list;
            if (oldValue != null && newValue != null)
            {
                if (!newValue.IsComparableTo(oldValue))
                {
                    var msg = string.Format(
                        "Items are incompatible for comparison! oldObject.Item.Type => {0}, newObject.Item.Type => {1}",
                        oldValue.UnderlyingType,
                        newValue.UnderlyingType);
                    throw new ArgumentException(msg);
                }

                if (newValue.IsPrimtive && oldValue.IsPrimtive && !newValue.AsPrimitive.Equals(oldValue.AsPrimitive))
                {
                    list = new IAnalysisLogDiff[] 
                    { 
                        new ReturnValueAnalysisLogDiff(oldValue.AsPrimitive.Value, newValue.AsPrimitive.Value, newValue.AsPrimitive.FullName) 
                    };
                }
                else if (newValue.IsObject && oldValue.IsObject)
                {
                    list = this.DiffObjects(oldValue.AsObject, newValue.AsObject);
                }
                else
                {
                    // No diff.
                    list = Enumerable.Empty<IAnalysisLogDiff>();
                }
            }
            else if (oldValue == null && newValue == null)
            {
                // No diff.
                list = Enumerable.Empty<IAnalysisLogDiff>();
            }
            else
            {
                // Either the old or new value is null. Not sure what happened here--should be impossible.
                var msg = string.Format(
                    "The {0} return value was null! This indicates an analysis log file corruption. Terminating analysis!",
                    oldValue == null ? "old" : "current");
                throw new ArgumentException(msg);
            }

            return list;
        }

        /// <inheritdoc />
        public IEnumerable<IAnalysisLogDiff> DiffObjects(Xml.Object oldObject, Xml.Object newObject)
        {
            yield break;
        }

        #endregion

        #region Private Methods

        private IEnumerable<IAnalysisLogDiff> DiffObjectsWithPathStack(Xml.Object oldObject, Xml.Object newObject, MemberPathStack pathStack)
        {
            yield break;
        }

        #endregion
    }
}
