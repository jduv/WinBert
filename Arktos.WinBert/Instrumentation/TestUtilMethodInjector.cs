namespace Arktos.WinBert.Instrumentation
{
    using System;
    using Microsoft.Cci;

    /// <summary>
    /// Contains information for injecting calls to the static TestUtil class. Extend this class with more
    /// specific functionality.
    /// </summary>
    public abstract class TestUtilMethodInjector : ILRewriter
    {
        #region Fields & Constants

        public static readonly string StartTestName = "StartTest";
        public static readonly string RecordVoidInstanceMethodCallName = "RecordVoidInstanceMethodCall";
        public static readonly string RecordInstanceMethodCallName = "RecordInstanceMethodCall";
        public static readonly string AddMethodToDynamicCallGraphName = "AddMethodToDynamicCallGraph";
        public static readonly string EndTestName = "EndTest";
        public static readonly string SaveResultsName = "SaveResults";

        #endregion

        #region Constructors & Destructors

        public TestUtilMethodInjector(
            IMetadataHost host,
            ILocalScopeProvider localScopeProvider,
            ISourceLocationProvider sourceLocationProvider,
            IAssembly winbertCore)
            : base(host, localScopeProvider, sourceLocationProvider)
        {
            if (winbertCore == null)
            {
                throw new ArgumentNullException("winbertCore");
            }

            var testUtilDefinition = (INamespaceTypeDefinition)UnitHelper.FindType(host.NameTable, winbertCore, typeof(TestUtil).FullName);

            // AddMethodToDynamicCallGraph(string)
            this.AddMethodToDynamicCallGraphDefinition = TypeHelper.GetMethod(
                testUtilDefinition,
                host.NameTable.GetNameFor(AddMethodToDynamicCallGraphName), 
                host.PlatformType.SystemString);

            // StartTest(string)
            this.StartTestWithNameDefinition = TypeHelper.GetMethod(
                testUtilDefinition,
                host.NameTable.GetNameFor(StartTestName),
                host.PlatformType.SystemString);

            // RecordVoidInstanceMethod(object, string)
             this.RecordVoidInstanceMethodDefinition = TypeHelper.GetMethod(
                testUtilDefinition,
                host.NameTable.GetNameFor(RecordVoidInstanceMethodCallName),
                host.PlatformType.SystemObject,
                host.PlatformType.SystemString);

            // RecordInstanceMethod(object, object, string)
            this.RecordInstanceMethodDefinition = TypeHelper.GetMethod(
                testUtilDefinition,
                host.NameTable.GetNameFor(RecordInstanceMethodCallName), 
                host.PlatformType.SystemObject,
                host.PlatformType.SystemObject,
                host.PlatformType.SystemString);

            // EndTest()
            this.EndTestDefinition = TypeHelper.GetMethod(
                testUtilDefinition,
                host.NameTable.GetNameFor(EndTestName));

            // SaveResults(string)
            this.SaveResultsDefinition = TypeHelper.GetMethod(
                testUtilDefinition,
                host.NameTable.GetNameFor(SaveResultsName),
                host.PlatformType.SystemString);
        }

        #endregion

        #region Properties

        public IMethodDefinition TestDefinition { get; private set; }

        /// <summary>
        /// Gets the method definition for <see cref="TestUtil.StartTest"/>
        /// </summary>
        public IMethodDefinition StartTestDefinition { get; private set; }

        /// <summary>
        /// Gets the method definition for <see cref="TestUtil.StartTest"/> with a test name parameter.
        /// </summary>
        public IMethodDefinition StartTestWithNameDefinition { get; private set; }

        /// <summary>
        /// Gets the method definition for <see cref="TestUtil.RecordVoidInstanceMethod"/>
        /// </summary>
        public IMethodDefinition RecordVoidInstanceMethodDefinition  { get; private set; }

        /// <summary>
        /// Gets the method definition for <see cref="TestUtil.RecordInstanceMethod"/>
        /// </summary>
        public IMethodDefinition RecordInstanceMethodDefinition  { get; private set; }

        /// <summary>
        /// Gets the method definition for <see cref="TestUtil.EndTest"/>
        /// </summary>
        public IMethodDefinition EndTestDefinition  { get; private set; }

        /// <summary>
        /// Gets the method definition for <see cref="TestUtil.AddMethodToDynamicCallGraph"/>
        /// </summary>
        public IMethodDefinition AddMethodToDynamicCallGraphDefinition  { get; private set; }

        /// <summary>
        /// Gets the method definition for <see cref="TestUtil.SaveResults"/>
        /// </summary>
        public IMethodDefinition SaveResultsDefinition { get; private set; }

        #endregion
    }
}
