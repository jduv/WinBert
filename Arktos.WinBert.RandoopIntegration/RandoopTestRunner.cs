namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using System.Reflection;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Testing;

    public class RandoopTestRunner : ITestRunner
    {
        #region Fields & Constants

        private static readonly string MethodName = "Main";

        #endregion

        #region Public Methods

        public AnalysisResult RunTests(IRegressionTestSuite testSuite)
        {
            // Execute tests in the old assembly
            this.RunTestsInAssembly(testSuite.OldTargetTestAssembly);

            // Execute tests in the new assembly
            this.RunTestsInAssembly(testSuite.NewTargetTestAssembly);

            return null;
        }

        #endregion

        #region Private Methods

        private static bool TypeContainsTestMethod(Type type)
        {
            foreach (MethodInfo method in type.GetMethods())
            {
                if (method.Name.Equals(MethodName))
                {
                    return true;
                }
            }

            return false;
        }

        private void RunTestsInAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (TypeContainsTestMethod(type))
                {
                    object instance = Activator.CreateInstance(type);
                    type.InvokeMember(MethodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, instance, null);
                }
            }
        }

        #endregion
    }
}
