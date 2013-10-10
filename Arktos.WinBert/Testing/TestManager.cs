namespace Arktos.WinBert.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AppDomainToolkit;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.Xml;
    using System.IO;

    /// <summary>
    /// This base class contains some basic implementations that TestManager derived classes will find useful along with
    /// the WinBert pipeline stack.
    /// </summary>
    /// <remarks>
    /// To implement a functional test manager, you must override two specific methods: GenerateTests and RunTests. You may choose
    /// to implement InstrumentTests if you wish, but an implementation is provided that performs no instrumentation by default. The
    /// default implementation of the ExecuteStack method will run the pipeline in paralell for both the current and previous builds that
    /// are supplied to the Run method.
    /// </remarks>
    public abstract class TestManager : ITestManager
    {
        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the TestManager class.
        /// </summary>
        /// <param name="config">
        /// The configuration.
        /// </param>
        public TestManager(WinBertConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            this.Config = config;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the currently loaded configuration object.
        /// </summary>
        public WinBertConfig Config { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method should generate tests for the target assembly and return a target to the assembly containing said tests.
        /// </summary>
        /// <remarks>
        /// No default implementation can exists for this method, so sub-classes must override it and create the test assemblies.
        /// </remarks>
        /// <param name="target">
        /// The target to generate tests for.
        /// </param>
        /// <param name="validTypeNames">
        /// A list of valid type names. If applicable, tests should only be generated for these types.
        /// </param>
        /// <returns>
        /// An ITestTarget implementation pointing to the assembly containing the generated tests along with the original target assembly.
        /// </returns>
        public abstract IAssemblyTarget GenerateTests(IAssemblyTarget target, IEnumerable<string> validTypeNames);

        /// <summary>
        /// Instruments the target assembly and it's tests. Overrides may choose to instrument the tests, the original assembly or
        /// both if desired.
        /// </summary>
        /// <remarks>
        ///  The default implementation of this method performs no instrumentation, that is it simply returns the passed in test target.
        /// </remarks>
        /// <param name="target">
        /// The target assembly.
        /// </param>
        /// <param name="tests">
        /// The tests to instrument.
        /// </param>
        /// <returns>
        /// A new test target replacing the original assemblies with instrumented ones.
        /// </returns>
        public abstract ITestTarget InstrumentTests(ITestTarget toInstrument);

        /// <summary>
        /// Executes tests.
        /// </summary>
        /// <remarks>
        /// No default implementation can exist for this method as this class will have no idea as to what the generated
        /// tests will look like.
        /// </remarks>
        /// <param name="target">
        /// The target assembly to execute the tests against.
        /// </param>
        /// <param name="tests">
        /// The assembly target containing the tests to execute and the corresponding assembly under test.
        /// </param>
        /// <returns>
        /// A test run result implementation.
        /// </returns>
        public abstract ITestRunResult RunTests(IAssemblyTarget target, IAssemblyTarget tests);

        /// <inheritdoc />
        /// <remarks>
        /// This default implementation will perform a diff against the two builds using default differencing mechanisms
        /// and then execute the WinBert stack on the difference result if a different is detected. You may override this method
        /// if needed, but only in rare occasions when you need a hook into the differencing pipeline should this be the case.
        /// </remarks>
        public AnalysisResult Run(Build previous, Build current)
        {
            AnalysisResult result = null;
            var diff = this.Diff(previous, current);
            if (diff != null && diff.IsDifferent)
            {
                result = this.ExecuteStack(diff);
            }

            return result;
        }

        /// <summary>
        /// Executes the entire WinBert stack.
        /// </summary>
        /// <remarks>
        /// Override this mehod for complete control over the way the TestManager executes each phase of the WinBert pipeline.
        /// Be aware that this method invokes test generation, instrumentation, and execution in a paralell fashion.
        /// </remarks>
        /// <param name="diff">
        /// The difference result. 
        /// </param>
        /// <returns>
        /// An AnalysisResult.
        /// </returns>
        protected virtual AnalysisResult ExecuteStack(IAssemblyDifferenceResult diff)
        {
            if (diff == null)
            {
                throw new ArgumentNullException("diff");
            }

            // Invoke two versions of the stack simultaneously
            ITestRunResult oldAssemblyResults = null, newAssemblyResults = null;
            var typeNames = diff.TypeDifferences.Select(x => x.Name);
            var tests = this.GenerateTests(diff.NewAssemblyTarget, typeNames);
            var instrumented = this.InstrumentTests(TestTarget.Create(diff.OldAssemblyTarget, diff.NewAssemblyTarget, tests));

            // Execute tests in parallel.
            Parallel.Invoke(
                () =>
                {
                    newAssemblyResults = this.RunTests(instrumented.TargetNewAssembly, instrumented.TestAssembly);
                },
                () =>
                {
                    oldAssemblyResults = this.RunTests(instrumented.TargetOldAssembly, instrumented.TestAssembly);
                });

            // Perform analysis and we're done.
            return this.Analyze(oldAssemblyResults, newAssemblyResults);
        }

        /// <summary>
        /// Performs analysis on the target test run results. Override this  if you need to do anything special while generating
        /// the analysis restuls or if you use a specialized representation during instrumentation else just use the default 
        /// mplementation. It will use a standard BERT analyzer to load up the analysis results log files and perform a regression
        /// test analysis.
        /// </summary>
        /// <param name="oldAssemblyResults">
        /// Results from running the tests on the old assembly.
        /// </param>
        /// <param name="newAssemblyResults">
        /// Results from running the tests on the new assembly.
        /// </param>
        /// <returns>
        /// An AnalysisResult containing the results of an analysis pass on the target two test run results.
        /// </returns>
        protected virtual AnalysisResult Analyze(ITestRunResult oldAssemblyResults, ITestRunResult newAssemblyResults)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs a diff in another application domain. The method by which this diff occurs should likely
        /// never change, so the method is considered sealed. Derived classes should always call this to get
        /// the differences between the two assemblies in question.
        /// </summary>
        /// <param name="previous">
        /// The previous build.
        /// </param>
        /// <param name="current">
        /// The current build.
        /// </param>
        /// <returns>
        /// The assembly difference result.
        /// </returns>
        protected IAssemblyDifferenceResult Diff(Build previous, Build current)
        {
            if (previous == null)
            {
                throw new ArgumentNullException("previous");
            }

            if (current == null)
            {
                throw new ArgumentNullException("current");
            }

            // Boot up another application domain.
            using (var diffEnv = AppDomainContext.Create())
            {
                // Pull in paths of the targets in case of dependencies.
                diffEnv.RemoteResolver.AddProbePaths(
                    Path.GetDirectoryName(previous.AssemblyPath), 
                    Path.GetDirectoryName(current.AssemblyPath));

                // Execute the diff in another application domain.
                return RemoteFunc.Invoke(
                    diffEnv.Domain,
                    this.Config.IgnoreList,
                    previous.AssemblyPath,
                    current.AssemblyPath,
                    (ignoreTargets, previousTargetPath, currentTargetPath) =>
                    {
                        // Fire up an assembly loader.
                        var loader = new AssemblyLoader();
                        var oldAssembly = loader.LoadAssembly(LoadMethod.LoadFile, previousTargetPath);
                        var newAssembly = loader.LoadAssembly(LoadMethod.LoadFile, currentTargetPath);

                        var differ = new AssemblyDifferenceEngine(ignoreTargets);
                        return differ.Diff(oldAssembly, newAssembly);
                    });
            }
        }

        #endregion 
    }
}
