namespace Arktos.WinBert.Instrumentation
{
    using System;
    using Microsoft.Cci;

    /// <summary>
    /// Contains information for injecting calls to the static TestUtil class.
    /// </summary>
    public class TestUtilMethodInjector : ILRewriter
    {
        #region Fields & Constants

        private static readonly string StartTestName = "StartTest";
        private static readonly string RecordVoidInstanceMethodCallName = "RecordVoidInstanceMethodCall";
        private static readonly string RecordInstanceMethodCallName = "RecordInstanceMethodCall";
        private static readonly string AddMethodToDynamicCallGraphName = "AddMethodToDynamicCallGraph";
        private static readonly string EndTestName = "EndTest";

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

            // StartTest()
            this.StartTestDefinition =  TypeHelper.GetMethod(
                testUtilDefinition,
                host.NameTable.GetNameFor(StartTestName));

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

            // EndTest(string)
            this.EndTestDefinition = TypeHelper.GetMethod(
                testUtilDefinition,
                host.NameTable.GetNameFor(EndTestName), 
                host.PlatformType.SystemString);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the method definition for <see cref="TestUtil.StartTest"/>
        /// </summary>
        public IMethodDefinition StartTestDefinition { get; private set; }

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

        #endregion
    }
}
