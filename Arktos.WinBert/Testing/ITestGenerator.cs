namespace Arktos.WinBert.Testing
{
    using System.Collections.Generic;
    using AppDomainToolkit;

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
        /// A reference to the compiled assembly containing the tests.
        /// </returns>
        IAssemblyTarget GenerateTests(IAssemblyTarget target, IEnumerable<string> validTypeNames);
    }
}
