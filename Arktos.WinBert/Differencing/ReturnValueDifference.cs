namespace Arktos.WinBert.Differencing
{

    /// <summary>
    /// Represents a difference between a return value. This can be a tricky object, as it has either an Object difference or
    /// a primitive difference wrapped inside.
    /// </summary>
    public class ReturnValueDifference : IDifferenceResult
    {
        #region Fields & Constants

        private static readonly ReturnValueDifference NoDiff = new ReturnValueDifference() { AreDifferences = false };

        #endregion

        #region Constructors & Destructors

        private ReturnValueDifference() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the primitive difference, if one exists.
        /// </summary>
        public AnalysisLogDifference PrimitiveDifference { get; private set; }

        /// <summary>
        /// Gets a value indicating if the difference is a primitive diff.
        /// </summary>
        public bool IsPrimitive
        {
            get
            {
                return this.PrimitiveDifference != null;
            }
        }

        /// <summary>
        /// Gets the object difference.
        /// </summary>
        public ObjectDifference ObjectDifference { get; private set; }

        /// <summary>
        /// Gets a value indicating if the difference is an object diff.
        /// </summary>
        public bool IsObject
        {
            get
            {
                return this.ObjectDifference != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are differences.
        /// </summary>
        public bool AreDifferences { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a return value difference representing no differences.
        /// </summary>
        /// <returns>A ReturnValueDifference instance.</returns>
        public static ReturnValueDifference NoDifferences()
        {
            return NoDiff;
        }

        /// <summary>
        /// Creates a return value difference from the target object diff.
        /// </summary>
        /// <param name="diff">The object diff to process.</param>
        /// <returns>A ReturnValueDifference instance.</returns>
        public static ReturnValueDifference FromObjectDiff(ObjectDifference diff)
        {
            return new ReturnValueDifference() { ObjectDifference = diff, AreDifferences = diff.AreDifferences };
        }

        /// <summary>
        /// Creates a return value difference from the target primitive diff.
        /// </summary>
        /// <param name="diff">The primitive diff to process.</param>
        /// <returns>A ReturnValueDifference instance.</returns>
        public static ReturnValueDifference FromPrimitiveDiff(AnalysisLogDifference diff)
        {
            return new ReturnValueDifference { PrimitiveDifference = diff, AreDifferences = true };
        }

        #endregion
    }
}
