﻿namespace Arktos.WinBert.Instrumentation
{
    using Arktos.WinBert.Xml;
    using System;
    using System.IO;
    using System.Xml;

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
        private static ITestStateRecorder recorder;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Static constructor--assigns the test dumper implementation a default value.
        /// </summary>
        static TestUtil()
        {
            // create a default method call dumper to assign to the default state
            // recorder. This won't include any ignore targets.
            var methodDumper = new MethodCallDumper();
            recorder = new TestStateRecorder(methodDumper);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the dumper implementation. It is not recommended to reset this in the middle
        /// of a test session. That will result in unknown behavior.
        /// </summary>
        public static ITestStateRecorder StateRecorder
        {
            get
            {
                return recorder;
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
                        recorder = value;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Static implementation of <see cref="ITestStateRecorder.StartTest"/>.
        /// </summary>
        public static void StartTest(string testName)
        {
            StateRecorder.StartTest(testName);
        }

        /// <summary>
        /// Static implementation of <see cref="ITestStateRecorder.EndTest"/>.
        /// </summary>
        public static void EndTest()
        {
            // End state in the dumper then save the file.
            StateRecorder.EndTest();
        }

        /// <summary>
        /// Static implementation of <see cref="ITestStateRecorder.RecordVoidInstanceMethodCall"/>.
        /// </summary>
        public static void RecordVoidInstanceMethodCall(object target, string signature)
        {
            StateRecorder.RecordVoidInstanceMethodCall(target, signature);
        }

        /// <summary>
        /// Static implementation of <see cref="ITestStateRecorder.RecordInstanceMethodCall"/>.
        /// </summary>
        public static void RecordInstanceMethodCall(object target, object returnValue, string signature)
        {
            StateRecorder.RecordInstanceMethodCall(target, returnValue, signature);
        }

        /// <summary>
        /// Static implementation of <see cref="ITestStateRecorder.AddMethodToDynamicCallGraph"/>.
        /// </summary>
        public static void AddMethodToDynamicCallGraph(string signature)
        {
            StateRecorder.AddMethodToDynamicCallGraph(signature);
        }

        /// <summary>
        /// Saves the results of the state recorder to the target path.
        /// </summary>
        /// <param name="path"></param>
        public static void SaveResults(string path)
        {
            // Do some simple verification.
            if (!VerifyFilePath(path))
            {
                throw new ArgumentException("Cannot save analysis due to an invalid path! Did not pass verification: " + path);
            }

            // Deserialize dumper.
            Serializer.XmlSerialize(recorder.AnalysisLog, path, new XmlWriterSettings() { Indent = true });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Detects if the target path is valid or not. A valid path here means that it's a file that
        /// may or may not exists but certainly has an XML extension. Directories will fail verification
        /// without a file name.
        /// </summary>
        /// <param name="toVerify">
        /// The path to verify.
        /// </param>
        /// <returns>
        /// True if the path is valid, false otherwise.
        /// </returns>
        private static bool VerifyFilePath(string toVerify)
        {
            bool verified = false;
            if (!string.IsNullOrWhiteSpace(toVerify))
            {
                verified = (File.Exists(toVerify) || Directory.Exists(toVerify)) ?
                    !File.GetAttributes(toVerify).HasFlag(FileAttributes.Directory) && VerifyFileExtension(toVerify) :
                    VerifyFileExtension(toVerify);
            }

            return verified;
        }

        /// <summary>
        /// Quick and dirty. Ensures the passed in path points to a valid XML file. Doesn't matter if it
        /// exists or not.
        /// </summary>
        /// <param name="toVerify">
        /// The path to verify.
        /// </param>
        /// <returns>
        /// True if the target path isn't null or empty and contains an XML extension.
        /// </returns>
        private static bool VerifyFileExtension(string toVerify)
        {
            String extension = Path.GetExtension(toVerify);
            return !string.IsNullOrEmpty(extension) && extension.Equals(".xml", StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
