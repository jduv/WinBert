namespace Arktos.WinBert.Testing
{
    using System;
    using Arktos.WinBert.Differencing;
    using Microsoft.Cci;

    /// <summary>
    /// Represents a pattern of tests that can be executed on new and old versions
    /// of a specific assembly as represented by a difference result.
    /// </summary>
    public class RegressionTestSuite : IRegressionTestSuite
    {
        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the TestSuite class.
        /// </summary>
        /// <param name="newTargetTests">
        /// An Assembly containing tests for the new target.
        /// </param>
        /// <param name="oldTargetTests">
        /// An Assembly containing tests for the old target.
        /// </param>
        /// <param name="diff">
        /// The original difference context describing the differences between both
        /// assemblies that tests were generated for.
        /// </param>
        public RegressionTestSuite(
            IAssembly newTarget,
            IAssembly newTargetTests,
            IAssembly oldTarget,
            IAssembly oldTargetTests,
            IAssemblyDifferenceResult diff)
        {
            if (newTarget == null)
            {
                throw new ArgumentNullException("newTarget");
            }

            if (newTargetTests == null)
            {
                throw new ArgumentNullException("newTargetTests");
            }

            if (oldTarget == null)
            {
                throw new ArgumentNullException("oldTarget");
            }

            if (oldTargetTests == null)
            {
                throw new ArgumentNullException("oldTargetTests");
            }

            if (diff == null)
            {
                throw new ArgumentNullException("diff");
            }

            this.NewTargetAssembly = newTarget;
            this.NewTargetTestAssembly = newTargetTests;
            this.OldTargetAssembly = oldTarget;
            this.OldTargetTestAssembly = oldTargetTests;
            this.Diff = diff;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the difference result.
        /// </summary>
        public IAssemblyDifferenceResult Diff { get; private set; }

        /// <summary>
        ///   Gets the new assembly.
        /// </summary>
        public IAssembly NewTargetAssembly { get; private set; }

        /// <summary>
        ///   Gets the TestAssembly for the new assembly target.
        /// </summary>
        public IAssembly NewTargetTestAssembly { get; private set; }

        /// <summary>
        ///   Gets the old assembly.
        /// </summary>
        public IAssembly OldTargetAssembly { get; private set; }

        /// <summary>
        ///   Gets the TestAssembly for the old assembly target.
        /// </summary>
        public IAssembly OldTargetTestAssembly { get; private set; }

        #endregion
    }
}