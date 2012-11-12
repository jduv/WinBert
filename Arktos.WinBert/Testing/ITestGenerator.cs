namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Represents an implementation that generates tests.
    /// </summary>
    public interface ITestGenerator
    {
        /// <summary>
        /// Retrieves an assembly containing generated tests.
        /// </summary>
        /// <param name="target">
        /// The target assembly.
        /// </param>
        /// <param name="validTypes">
        /// A list of valid types to generate tests for.
        /// </param>
        /// <returns>
        /// A compiled assembly containing the tests.
        /// </returns>
        Assembly GetTestAssembly(Assembly target, IList<Type> validTypes);
    }
}
