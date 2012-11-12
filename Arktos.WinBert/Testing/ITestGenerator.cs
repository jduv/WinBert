namespace Arktos.WinBert.Testing
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

        /// <summary>
        /// Retrieves an assembly containing generated tests. This overload allows tests to be generated
        /// for anonymous and dynamic assemblies. Typically these types of assemblies won't have a 
        /// location attribute, and the normal method will fail.
        /// </summary>
        /// <param name="target">
        /// The target assembly.
        /// </param>
        /// <param name="validTypes">
        /// A list of valid types to generate tests for.
        /// </param>
        /// <param name="path">
        /// The path to the assembly passed in as the <paramref name="target"/> parameter.
        /// </param>
        /// <returns>
        /// A compiled assembly containint the generated tests.
        /// </returns>
        Assembly GetTestAssembly(Assembly target, IList<Type> validTypes, string path);
    }
}
