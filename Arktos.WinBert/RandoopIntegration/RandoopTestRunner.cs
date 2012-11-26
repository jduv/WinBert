﻿namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using System.Linq;
    using AppDomainToolkit;
    using Arktos.WinBert.Testing;

    /// <summary>
    /// Executes a suite of tests generated by Randoop.
    /// </summary>
    public class RandoopTestRunner : ITestRunner
    {
        #region Fields & Constants

        private static readonly string TestMethodName = "Main";

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public ITestRunResult RunTests(ITestTarget target)
        {
            var loader = new AssemblyLoader();
            loader.LoadAssemblyWithReferences(LoadMethod.LoadFrom, target.TargetAssembly.Location);
            loader.LoadAssemblyWithReferences(LoadMethod.LoadFrom, target.TestAssembly.Location);

            var assembly = loader.LoadAssembly(LoadMethod.LoadFrom, target.TestAssembly.Location);
            foreach (var type in assembly.GetTypes())
            {
                var testObj = Activator.CreateInstance(type);
                var method = type.GetMethods().First(x => x.Name.Equals(TestMethodName));
                method.Invoke(testObj, null);
            }

            return TestRunResult.Successful(@"C:\out.log");
        }

        #endregion
    }
}