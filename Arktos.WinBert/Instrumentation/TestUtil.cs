using System;
namespace Arktos.WinBert.Instrumentation
{
    /// <summary>
    /// The main class that's injected into every test assembly and target assembly that handles
    /// building the logging object model and writing it out to file. This class may also be used to
    /// handle building the dynamic call graphs needed for the WinBert testing procedure. More or less
    /// it's simply a static wrapper around an ITestDumper.
    /// </summary>
    public static class TestUtil
    {
        #region Fields & Constants

        private static readonly object lockObj = new object();
        private static ITestDumper dumper;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Static constructor--assigns the test dumper implementation a default value.
        /// </summary>
        static TestUtil()
        {
            Dumper = new TestDumper();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the dumper implementation. Primarily exists for a test seam--it is not recommended
        /// to re-set this in the middle of a testing session. That will likely result in unknown behavior.
        /// </summary>
        public static ITestDumper Dumper
        {
            get
            {
                return dumper;
            }

            set
            {
                // Since this is a static property, we need to make it thread safe.
                lock (lockObj)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("Dumper");
                    }
                    else
                    {
                        dumper = value;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Static implementation of <see cref="ITestDumper.StartTest"/>.
        /// </summary>
        public static void StartTest()
        {
            Dumper.StartTest();
        }

        /// <summary>
        /// Static implementation of <see cref="ITestDumper.EndTest"/>.
        /// </summary>
        public static void EndTest(string path)
        {
            Dumper.EndTest(path);
        }

        /// <summary>
        /// Static implementation of <see cref="ITestDumper.DumpVoidInstanceMethodCall"/>.
        /// </summary>
        public static void DumpVoidInstanceMethodCall(object target, string signature)
        {
            Dumper.DumpVoidInstanceMethodCall(target, signature);
        }

        /// <summary>
        /// Static implementation of <see cref="ITestDumper.DumpInstanceMethodCall"/>.
        /// </summary>
        public static void DumpInstanceMethodCall(object target, object returnValue, string signature)
        {
            Dumper.DumpInstanceMethodCall(target, returnValue, signature);
        }

        /// <summary>
        /// Static implementation of <see cref="ITestDumper.AddMethodToDynamicCallGraph"/>.
        /// </summary>
        public static void AddMethodToDynamicCallGraph(string signature)
        {
            Dumper.AddMethodToDynamicCallGraph(signature);
        }

        #endregion
    }
}
