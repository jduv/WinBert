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
        public IAssemblyTarget TargetNewAssembly { get; private set; }

        /// <inheritdoc/>
        public IAssemblyTarget TargetOldAssembly { get; private set; }

        /// <inheritdoc/>
        public IAssemblyTarget TestAssembly { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new TestTarget.
        /// </summary>
        /// <param name="oldTestTarget">
        /// The old assembly target to be tested.
        /// </param>
        /// <param name="newTestTarget">
        /// The new assembly target to be tested.
        /// </param>
        /// <param name="testAssembly">
        /// The assembly containing tests for the target assembly.
        /// </param>
        /// <returns>
        /// A new TestTarget.
        /// </returns>
        public static ITestTarget Create(IAssemblyTarget oldTestTarget, IAssemblyTarget newTestTarget, IAssemblyTarget testAssembly)
        {
            if (newTestTarget == null)
            {
                throw new ArgumentNullException("newTestTarget");
            }

            if (oldTestTarget == null)
            {
                throw new ArgumentNullException("testTarget");
            }

            if (testAssembly == null)
            {
                throw new ArgumentNullException("testAssembly");
            }

            return new TestTarget()
            {
                TargetNewAssembly = newTestTarget,
                TargetOldAssembly = oldTestTarget,
                TestAssembly = testAssembly
            };
        }

        /// <summary>
        /// Creates a new TestTarget.
        /// </summary>
        /// <param name="oldTestTarget">
        /// The old assembly to be tested.
        /// </param>
        /// <param name="newTestTarget">
        /// The new assembly to be tested.
        /// </param>
        /// <param name="testAssembly">
        /// The assembly containing tests for the target assembly.
        /// </param>
        /// <returns>
        /// A new TestTarget.
        /// </returns>
        public static ITestTarget Create(Assembly oldTestTarget, Assembly newTestTarget, Assembly testAssembly)
        {
            if (newTestTarget == null)
            {
                throw new ArgumentNullException("newTestTarget");
            }

            if (oldTestTarget == null)
            {
                throw new ArgumentNullException("testTarget");
            }

            if (testAssembly == null)
            {
                throw new ArgumentNullException("testAssembly");
            }

            return new TestTarget()
            {
                TargetNewAssembly = AssemblyTarget.FromAssembly(newTestTarget),
                TargetOldAssembly = AssemblyTarget.FromAssembly(oldTestTarget),
                TestAssembly = AssemblyTarget.FromAssembly(testAssembly)
            };
        }

        #endregion
    }
}
