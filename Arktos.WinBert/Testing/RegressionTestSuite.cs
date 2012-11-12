namespace Arktos.WinBert.Testing
{
    using System.Reflection;
    using Arktos.WinBert.Differencing;
    using System;

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
        public RegressionTestSuite(Assembly newTargetTests, Assembly oldTargetTests, IAssemblyDifferenceResult diff)
        {
            this.NewTargetTestAssembly = newTargetTests;
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
        public Assembly NewTargetAssembly
        {
            get
            {
                return this.Diff.NewObject;
            }
        }

        /// <summary>
        ///   Gets the TestAssembly for the new assembly target.
        /// </summary>
        public Assembly NewTargetTestAssembly { get; private set; }

        /// <summary>
        ///   Gets the old assembly.
        /// </summary>
        public Assembly OldTargetAssembly
        {
            get
            {
                return this.Diff.OldObject;
            }
        }

        /// <summary>
        ///   Gets the TestAssembly for the old assembly target.
        /// </summary>
        public Assembly OldTargetTestAssembly { get; private set; }

        #endregion
    }
}