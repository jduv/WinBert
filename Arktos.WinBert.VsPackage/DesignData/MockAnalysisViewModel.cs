namespace Arktos.WinBert.VsPackage.DesignData
{
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.VsPackage.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Mock view model for displaying design time data. Enables testing without running the solution.
    /// </summary>
    public class MockAnalysisViewModel : AnalysisToolWindowVm
    {
        #region Fields & Constants

        private static Random random = new Random();
        private static readonly PathedAnalysisLogDifference[] pathedDiffs = new PathedAnalysisLogDifference[] {
            new PathedAnalysisLogDifference("foo.bar.baz", "1", "2", "System.Int16"),
            new PathedAnalysisLogDifference("foo.far.biff", "hello", "world", "System.string"),
            new PathedAnalysisLogDifference("foo.lar.boof", "245", "210", "System.byte"),
            new PathedAnalysisLogDifference("foo.far.boo", "120.3496", "1239.46247", "System.double"),
            new PathedAnalysisLogDifference("foo.bum.bam", "boring", "coding", "System.string")
        };
        private static readonly AnalysisLogDifference[] primitiveDiffs = new AnalysisLogDifference[] {
            new AnalysisLogDifference("1", "2", "System.Int16"),
            new AnalysisLogDifference("hello", "world", "System.string"),
            new AnalysisLogDifference("245", "210", "System.byte"),
            new AnalysisLogDifference("120.3496", "1239.46247", "System.double"),
            new AnalysisLogDifference("boring", "coding", "System.string")
        };

        #endregion

        #region Constructors & Destructors

        public MockAnalysisViewModel()
        {
            this.AnalysisResults.Add(new InconclusiveAnalysisVm(new InconclusiveAnalysisResult("Inconclusive"), "Inconclusive Project"));
            this.AnalysisResults.Add(new AnalysisErrorInfoVm("An error occurred!", "My Bad Project"));
            this.AnalysisResults.Add(new AnalysisErrorInfoVm(new ArgumentNullException("myParam"), "Exception Thrown"));
            this.AnalysisResults.Add(new SuccessfulAnalysisVm(CreateSuccessfulAnalysisResult(), "Successful Project"));
            this.AnalysisResults.Add(new SuccessfulAnalysisVm(new SuccessfulAnalysisResult(Enumerable.Empty<TestExecutionDifference>()), "No differences"));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Factory method that creates a successful analysis result for consumption of the UI design
        /// time view.
        /// </summary>
        /// <returns>
        /// A mock successful analysis result. Used for design time tweaks of the UI.
        /// </returns>
        private static SuccessfulAnalysisResult CreateSuccessfulAnalysisResult()
        {
            var testExecutions = CreateTestExecutionDiffs();
            var result = new SuccessfulAnalysisResult(testExecutions);
            return result;
        }

        /// <summary>
        /// Creates a list of fake test executions.
        /// </summary>
        /// <returns>A list of fake test executions.</returns>
        private static IEnumerable<TestExecutionDifference> CreateTestExecutionDiffs()
        {
            return new List<TestExecutionDifference>() {
                GenerateTestExecutionDiff("Test1"),
                GenerateTestExecutionDiff("Test2"),
                GenerateTestExecutionDiff("Test3")
            };
        }

        /// <summary>
        /// Generates a test execution diff.
        /// </summary>
        /// <returns>A test execution difference.</returns>
        private static TestExecutionDifference GenerateTestExecutionDiff(string testName)
        {
            var methodDiffs = GenerateMethodDiffs(testName);
            return new TestExecutionDifference(
                new Xml.TestExecution() { Name = testName },
                new Xml.TestExecution() { Name = testName },
                methodDiffs);
        }

        /// <summary>
        /// Generates a list of method diffs.
        /// </summary>
        /// <param name="pair">The test execution pair to generate from.</param>
        /// <returns>A list of method diffs.</returns>
        private static IEnumerable<MethodCallDifference> GenerateMethodDiffs(string testName)
        {
            // Only give current call a signature.
            return new List<MethodCallDifference>() { 
                new MethodCallDifference(
                    new Xml.MethodCall() { Signature = testName + ".Foo" }, 
                    new Xml.MethodCall() { Signature = testName + ".Foo" }, 
                    null, 
                    GenerateObjectDiff(), 
                    ReturnValueDifference.NoDifferences()),
                new MethodCallDifference(
                    new Xml.MethodCall() { Signature =  testName + ".Bar" }, 
                    new Xml.MethodCall() { Signature =  testName + ".Bar" }, 
                    4, 
                    GenerateObjectDiff(), 
                    ReturnValueDifference.FromObjectDiff(GenerateObjectDiff())),
                new MethodCallDifference(
                    new Xml.MethodCall() { Signature =  testName + ".Baz" }, 
                    new Xml.MethodCall() { Signature =  testName + ".Baz" }, 
                    2,
                    GenerateObjectDiff(),
                    ReturnValueDifference.FromPrimitiveDiff(GeneratePrimitiveDiff()))
            };
        }

        /// <summary>
        /// Generates an object diff.
        /// </summary>
        /// <returns>An object diff.</returns>
        private static ObjectDifference GenerateObjectDiff()
        {
            return new ObjectDifference(
                new List<PathedAnalysisLogDifference>() { GeneratePathedPrimitiveDiff(), GeneratePathedPrimitiveDiff(), GeneratePathedPrimitiveDiff() },
                new List<ObjectDifference>() { GeneratePrimitiveOnlyObjectDiff() },
                new List<PathedAnalysisLogDifference>() { GeneratePathedPrimitiveDiff(), GeneratePathedPrimitiveDiff() },
                new List<PathedAnalysisLogDifference>() { GeneratePathedPrimitiveDiff() },
                new List<ObjectDifference>() { GeneratePrimitiveOnlyObjectDiff() },
                new List<PathedAnalysisLogDifference>() { GeneratePathedPrimitiveDiff(), GeneratePathedPrimitiveDiff() });
        }

        /// <summary>
        /// Generates an object diff with only primitives inside.
        /// </summary>
        /// <returns>An object diff with only primitives.</returns>
        private static ObjectDifference GeneratePrimitiveOnlyObjectDiff()
        {
            return new ObjectDifference(
                new List<PathedAnalysisLogDifference>() { GeneratePathedPrimitiveDiff(), GeneratePathedPrimitiveDiff() },
                new List<ObjectDifference>() { },
                new List<PathedAnalysisLogDifference>() { GeneratePathedPrimitiveDiff(), GeneratePathedPrimitiveDiff() },
                new List<PathedAnalysisLogDifference>() { GeneratePathedPrimitiveDiff(), GeneratePathedPrimitiveDiff() },
                new List<ObjectDifference>() { },
                new List<PathedAnalysisLogDifference>() { GeneratePathedPrimitiveDiff(), GeneratePathedPrimitiveDiff() });
        }

        /// <summary>
        /// Generates a primitive diff.
        /// </summary>
        /// <returns>A primitive diff.</returns>
        private static AnalysisLogDifference GeneratePrimitiveDiff()
        {
            return primitiveDiffs[random.Next(0, primitiveDiffs.Length)];
        }

        /// <summary>
        /// Generates a pathed primitive diff.
        /// </summary>
        /// <returns>A pathed primitive diff.</returns>
        private static PathedAnalysisLogDifference GeneratePathedPrimitiveDiff()
        {
            return pathedDiffs[random.Next(0, pathedDiffs.Length)];
        }

        #endregion
    }
}
