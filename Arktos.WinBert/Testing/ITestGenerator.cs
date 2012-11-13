namespace Arktos.WinBert.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Cci;

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
        /// A reference to the metadata for a compiled assembly containing the tests.
        /// </returns>
        IAssembly GetTestsFor(IAssembly target, IList<INamedTypeDefinition> validTypes);
    }
}
