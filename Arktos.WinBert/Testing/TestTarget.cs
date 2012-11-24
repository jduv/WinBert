namespace Arktos.WinBert.Testing
{
    using System;
    using System.Reflection;
    using AppDomainToolkit;

    /// <summary>
    /// Simple serializable implementation of the ITestTarget interface.
    /// </summary>
    [Serializable]
    public sealed class TestTarget : ITestTarget
    {
        #region Constructors & Destructors

        /// <summary>
        /// Prevents a default instance of the TestTarget class from being created.
        /// </summary>
        private TestTarget()
        {
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public IAssemblyTarget TargetAssembly { get; private set; }

        /// <inheritdoc/>
        public IAssemblyTarget TestAssembly { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new TestTarget.
        /// </summary>
        /// <param name="testTarget">
        /// The assembly to be tested.
        /// </param>
        /// <param name="testAssembly">
        /// The assembly containing tests for the target assembly.
        /// </param>
        /// <returns>
        /// A new TestTarget.
        /// </returns>
        public static ITestTarget Create(IAssemblyTarget testTarget, IAssemblyTarget testAssembly)
        {
            if (testTarget == null)
            {
                throw new ArgumentNullException("testTarget");
            }

            if (testAssembly == null)
            {
                throw new ArgumentNullException("testAssembly");
            }

            return new TestTarget()
            {
                TargetAssembly = testTarget,
                TestAssembly = testAssembly
            };
        }

        /// <summary>
        /// Creates a new TestTarget.
        /// </summary>
        /// <param name="testTarget">
        /// The assembly to be tested.
        /// </param>
        /// <param name="testAssembly">
        /// The assembly containing tests for the target assembly.
        /// </param>
        /// <returns>
        /// A new TestTarget.
        /// </returns>
        public static ITestTarget Create(Assembly testTarget, Assembly testAssembly)
        {
            if (testTarget == null)
            {
                throw new ArgumentNullException("testTarget");
            }

            if (testAssembly == null)
            {
                throw new ArgumentNullException("testAssembly");
            }

            return new TestTarget()
            {
                TargetAssembly = AssemblyTarget.FromAssembly(testTarget),
                TestAssembly = AssemblyTarget.FromAssembly(testAssembly)
            };
        }

        #endregion
    }
}
