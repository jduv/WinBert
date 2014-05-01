namespace Arktos.WinBert.Analysis
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a difference between two instances.
    /// </summary>
    public class ObjectDifference
    {
        #region Constructors & Destructors

        public ObjectDifference(
            IEnumerable<PathedAnalysisLogDifference> primitiveFieldDiffs,
            IEnumerable<ObjectDifference> fieldObjectDiffs,
            IEnumerable<PathedAnalysisLogDifference> fieldSentinalDiffs,
            IEnumerable<PathedAnalysisLogDifference> primitivePropertyDiffs,
            IEnumerable<ObjectDifference> propertyObjectDiffs,
            IEnumerable<PathedAnalysisLogDifference> propertySentinalDiffs)
        {
            this.PrimitiveFieldDiffs = primitiveFieldDiffs;
            this.PrimitivePropertyDiffs = primitiveFieldDiffs;
            this.FieldObjectDiffs = fieldObjectDiffs.Where(x => x.AreDifferences);
            this.PropertyObjectDiffs = propertyObjectDiffs.Where(x => x.AreDifferences);
            this.FieldSentinalDiffs = fieldSentinalDiffs;
            this.PropertySentinalDiffs = propertySentinalDiffs;
            this.AreDifferences = this.PrimitiveFieldDiffs.Any() || this.PrimitivePropertyDiffs.Any() || this.FieldObjectDiffs.Any() ||
                this.PropertyObjectDiffs.Any() || this.FieldSentinalDiffs.Any() || this.PropertySentinalDiffs.Any();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of field differences.
        /// </summary>
        public IEnumerable<PathedAnalysisLogDifference> PrimitiveFieldDiffs { get; private set; }

        /// <summary>
        /// Gets a list of property differences.
        /// </summary>
        public IEnumerable<PathedAnalysisLogDifference> PrimitivePropertyDiffs { get; private set; }

        /// <summary>
        /// Gets a list of child object field differences.
        /// </summary>
        public IEnumerable<ObjectDifference> FieldObjectDiffs { get; private set; }

        /// <summary>
        /// Gets a list of child object proeprty differences.
        /// </summary>
        public IEnumerable<ObjectDifference> PropertyObjectDiffs { get; private set; }

        /// <summary>
        /// Gets a list of sentinal field differences.
        /// </summary>
        public IEnumerable<PathedAnalysisLogDifference> FieldSentinalDiffs { get; private set; }

        /// <summary>
        /// Gets a list of sentinal property differences.
        /// </summary>
        public IEnumerable<PathedAnalysisLogDifference> PropertySentinalDiffs { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there are differences.
        /// </summary>
        public bool AreDifferences { get; private set; }

        #endregion
    }
}
