namespace Arktos.WinBert.Analysis
{
    using Arktos.WinBert.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Implementation that performs differences between analysis log elements. There is an important underlying assumption
    /// to make here, and that is that the analysis log will never provide a null value on any element. We enforce that here.
    /// </summary>
    public class ObjectDiffer
    {
        #region Public Methods

        /// <summary>
        /// Performs a diff on the two target return values. If both are null, this method will return null because in the analysis schema
        /// it is legal to have a "null" return value, i.e. void methods. This method will throw an IncompatibleTypesException in the event
        /// that the two underlying types behind the Xml.Value instances are not comparable.  This indicates an analysis log file corruption.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>Null if both old and new values are also null, null if there are no differences between the two target values, or
        /// a ReturnValueDifference instance containing information about the differences between the two values.</returns>
        public ReturnValueDifference DiffReturnValues(Xml.Value oldValue, Xml.Value newValue)
        {
            if (oldValue == null && newValue != null)
            {
                throw new ArgumentException("Old value cannot be null if new value is not null.");
            }

            if (oldValue != null && newValue == null)
            {
                throw new ArgumentException("New value cannot be null if old value is not null.");
            }

            ReturnValueDifference returnValueDiff = ReturnValueDifference.NoDifferences();
            if (oldValue != null && newValue != null)
            {
                if (this.CanDiffAsPrimitives(oldValue, newValue))
                {
                    if (!oldValue.AsPrimitive.Value.Equals(newValue.AsPrimitive.Value))
                    {
                        returnValueDiff = ReturnValueDifference.FromPrimitiveDiff(
                            new AnalysisLogDifference(oldValue.AsPrimitive.Value, newValue.AsPrimitive.Value, newValue.AsPrimitive.FullName));
                    }
                }
                else if (this.CanDiffAsObjects(oldValue, newValue))
                {
                    var objectDiff = this.DiffObjects(oldValue.AsObject, newValue.AsObject);
                    returnValueDiff = ReturnValueDifference.FromObjectDiff(objectDiff);
                }
                else
                {
                    throw new IncompatibleTypesException<Xml.Value>(oldValue, newValue);
                }
            }

            return returnValueDiff;
        }

        /// <summary>
        /// Entry point for differencing two object values.  This method will throw an IncompatibleTypesException in the event
        /// that the two underlying types behind the Xml.Value instances are not comparable.  This indicates an analysis log file corruption.
        /// Also, this method will not tolerate null values for either object value because it is not legal for object nodes to be null in the analysis
        /// log schema. A null object should be referenced by a "null" type.
        /// </summary>
        /// <param name="oldObject">The old object.</param>
        /// <param name="newObject">The new object.</param>
        /// <returns></returns>
        public ObjectDifference DiffObjects(Xml.Object oldObject, Xml.Object newObject)
        {
            // both objects cannot be null in the analysis log, else we have a corruption.
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newObject == null)
            {
                throw new ArgumentNullException("newObject");
            }

            var pathStack = new MemberPathStack(newObject.FullName);
            return this.DiffObjectsWithPathStack(oldObject, newObject, pathStack);
        }

        #endregion

        #region Private Methods

        private ObjectDifference DiffObjectsWithPathStack(Xml.Object oldObject, Xml.Object newObject, MemberPathStack pathStack)
        {
            var fieldDiffs = this.GetDifferences(oldObject.Fields, newObject.Fields, pathStack);
            var propertyDiffs = this.GetDifferences(oldObject.AutoProperties, newObject.AutoProperties, pathStack);

            return new ObjectDifference(
                fieldDiffs.PrimitiveDiffs,
                fieldDiffs.ObjectDiffs,
                fieldDiffs.SentinalDiffs,
                propertyDiffs.PrimitiveDiffs,
                propertyDiffs.ObjectDiffs,
                propertyDiffs.SentinalDiffs);
        }

        private PrimitiveAndObjectDiffsList GetDifferences(IEnumerable<dynamic> oldObjectValues, IEnumerable<dynamic> newObjectValues, MemberPathStack pathStack)
        {
            if (oldObjectValues == null)
            {
                throw new ArgumentNullException("oldObjectFields");
            }

            if (newObjectValues == null)
            {
                throw new ArgumentNullException("newObjectFields");
            }

            PrimitiveAndObjectDiffsList wrapper = new PrimitiveAndObjectDiffsList();
            if (newObjectValues.Any() && oldObjectValues.Any())
            {
                var newObjectValueMap = newObjectValues.ToDictionary(x => x.Name);

                // "named" values are simply fields or properties.
                foreach (var oldNamedValue in oldObjectValues)
                {
                    var newNamedValue = newObjectValueMap[oldNamedValue.Name];
                    pathStack.Push(newNamedValue.Name);

                    ObjectDifference objectDiff = null;
                    PathedAnalysisLogDifference pathedFieldDiff = null;
                    var newValue = newNamedValue.Value;
                    var oldValue = oldNamedValue.Value;

                    // Spot check for incompatible types
                    if (!this.CanDiffValues(oldValue, newValue))
                    {
                        throw new IncompatibleTypesException<Xml.Value>(oldValue, newValue);
                    }

                    // Are primitives?
                    if (this.CanDiffAsPrimitives(oldValue, newValue) &&
                        this.TryDiffPrimitives(oldValue.AsPrimitive, newValue.AsPrimitive, out pathedFieldDiff, pathStack))
                    {
                        // TryDiff found a difference (returned true)
                        wrapper.PrimitiveDiffs.Add(pathedFieldDiff);
                    }
                    // Are sentinals?
                    else if (this.CanDiffAsSentinals(oldValue, newValue) &&
                        this.TryDiffSentinals(oldValue.AsObject, newValue.AsObject, out pathedFieldDiff, pathStack))
                    {
                        // TryDiff found a difference (returned true)
                        wrapper.SentinalDiffs.Add(pathedFieldDiff);
                    }
                    // Are objects?
                    else if (this.CanDiffAsObjects(oldValue, newValue) &&
                        this.TryDiffXmlObjects(oldValue.AsObject, newValue.AsObject, out objectDiff, pathStack))
                    {
                        // TryDiff found a difference (returned true)
                        wrapper.ObjectDiffs.Add(objectDiff);
                    }

                    // Done, pop stack and process next field.
                    pathStack.Pop();
                }
            }

            return wrapper;
        }

        private bool TryDiffPrimitives(Xml.Primitive oldPrimitive, Xml.Primitive newPrimitive, out PathedAnalysisLogDifference diff, MemberPathStack pathStack)
        {
            if (oldPrimitive == null)
            {
                throw new ArgumentNullException("oldPrimitive");
            }

            if (newPrimitive == null)
            {
                throw new ArgumentNullException("newPrimitive");
            }

            if (!oldPrimitive.FullName.Equals(newPrimitive.FullName, StringComparison.OrdinalIgnoreCase))
            {
                throw new IncompatibleTypesException<Xml.Primitive>(oldPrimitive, newPrimitive);
            }

            diff = null;
            bool success = false;
            if (!oldPrimitive.Value.Equals(newPrimitive.Value))
            {
                diff = new PathedAnalysisLogDifference(pathStack.CurrentPath, oldPrimitive.Value, newPrimitive.Value, newPrimitive.FullName);
                success = true;
            }

            return success;
        }

        private bool TryDiffXmlObjects(Xml.Object oldObject, Xml.Object newObject, out ObjectDifference diff, MemberPathStack pathStack = null)
        {
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (oldObject.IsSentinalType)
            {
                throw new ArgumentException("Unexpected sentinal type: " + oldObject.ToString());
            }

            if (newObject.IsSentinalType)
            {
                throw new ArgumentException("Unexpected sentinal type: " + newObject.ToString());
            }

            if (!oldObject.FullName.Equals(newObject.FullName, StringComparison.OrdinalIgnoreCase))
            {
                throw new IncompatibleTypesException<Xml.Object>(oldObject, newObject);
            }

            diff = null;
            bool success = false;
            var objectDifference = pathStack != null ?
                   this.DiffObjectsWithPathStack(oldObject, newObject, pathStack) :
                   this.DiffObjects(oldObject, newObject);
            if (objectDifference.AreDifferences)
            {
                diff = objectDifference;
                success = true;
            }

            return success;
        }

        private bool TryDiffSentinals(Xml.Object oldObject, Xml.Object newObject, out PathedAnalysisLogDifference diff, MemberPathStack pathStack)
        {
            if (oldObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (newObject == null)
            {
                throw new ArgumentNullException("oldObject");
            }

            if (!oldObject.IsSentinalType)
            {
                throw new ArgumentException("Unexpected non-sentinal type: " + oldObject.ToString());
            }

            if (!newObject.IsSentinalType)
            {
                throw new ArgumentException("Unexpected non-sentinal type: " + newObject.ToString());
            }

            diff = null;
            bool success = false;
            if (!oldObject.GetType().Equals(newObject.GetType()))
            {
                diff = new PathedAnalysisLogDifference(pathStack.CurrentPath, oldObject.ToString(), newObject.ToString(), newObject.FullName);
                success = true;
            }

            return success;
        }

        private bool CanDiffValues(Xml.Value oldValue, Xml.Value newValue)
        {
            return this.CanDiffPrimitives(oldValue.AsPrimitive, newValue.AsPrimitive) ||
                this.CanDiffObjects(oldValue.AsObject, oldValue.AsObject) ||
                this.CanDiffSentinals(oldValue.AsObject, newValue.AsObject);
        }

        private bool CanDiffAsPrimitives(Xml.Value oldValue, Xml.Value newValue)
        {
            return this.CanDiffPrimitives(oldValue.AsPrimitive, newValue.AsPrimitive);
        }

        private bool CanDiffPrimitives(Xml.Primitive oldPrimitive, Xml.Primitive newPrimitive)
        {
            return oldPrimitive != null && newPrimitive != null &&
                oldPrimitive.FullName.Equals(newPrimitive.FullName, StringComparison.OrdinalIgnoreCase);
        }

        private bool CanDiffObjects(Xml.Object oldObject, Xml.Object newObject)
        {
            // Both cannot be null, must be objects, and cannot be sentinals
            return oldObject != null && newObject != null &&
               !oldObject.IsSentinalType && !newObject.IsSentinalType &&
                oldObject.FullName.Equals(newObject.FullName, StringComparison.OrdinalIgnoreCase);
        }

        private bool CanDiffAsObjects(Xml.Value oldValue, Xml.Value newValue)
        {
            // Both cannot be null, must be objects, and cannot be sentinals
            return this.CanDiffObjects(oldValue.AsObject, newValue.AsObject);
        }

        private bool CanDiffSentinals(Xml.Object oldObject, Xml.Object newObject)
        {
            return oldObject != null && newObject != null &&
                oldObject.IsSentinalType && newObject.IsSentinalType;
        }

        private bool CanDiffAsSentinals(Xml.Value oldValue, Xml.Value newValue)
        {
            return this.CanDiffSentinals(oldValue.AsObject, newValue.AsObject);
        }

        #endregion
    }

    /// <summary>
    /// Simple small utility class for collecting the primitive and object lists of an object diff and
    /// passing about from methods. this prevents idiotic things like passing lists into methods
    /// and side affecting them.
    /// </summary>
    class PrimitiveAndObjectDiffsList
    {
        #region Constructors & Destructors

        public PrimitiveAndObjectDiffsList()
        {
            this.PrimitiveDiffs = new List<PathedAnalysisLogDifference>();
            this.ObjectDiffs = new List<ObjectDifference>();
            this.SentinalDiffs = new List<PathedAnalysisLogDifference>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of primitive diffs.
        /// </summary>
        public IList<PathedAnalysisLogDifference> PrimitiveDiffs { get; private set; }

        /// <summary>
        /// Gets the list of object diffs.
        /// </summary>
        public IList<ObjectDifference> ObjectDiffs { get; private set; }

        /// <summary>
        /// Gets the list of sentinal diffs.
        /// </summary>
        public IList<PathedAnalysisLogDifference> SentinalDiffs { get; private set; }

        public bool IsEmpty
        {
            get
            {
                return !(this.PrimitiveDiffs.Any() && this.ObjectDiffs.Any() && this.SentinalDiffs.Any());
            }
        }

        #endregion
    }
}
