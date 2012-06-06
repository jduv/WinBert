namespace WinBertUnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WinBert.Differencing;
    using WinBert.RandoopIntegration;
    using WinBert.Testing;
    using WinBert.Xml;

    /// <summary>
    /// Tests the RandoopTestGenerator class.
    /// </summary>
    [TestClass]
    public class RandoopTestGeneratorTest
    {
        #region Fields and Constants

        /// <summary>
        ///   Static path to the output directory for this test generator.
        /// </summary>
        private const string OutputDirectory = "RandoopOut";

        /// <summary>
        ///   Static path to test configuration.
        /// </summary>
        private const string TestConfigPath = "WinbertTestConfig.xml";

        /// <summary>
        ///   Static path to the test assembly we will be testing.
        /// </summary>
        private const string TestAssembly1Path = @"DiffTestAssembly1.dll";

        /// <summary>
        ///   Static path to another test assembly we will be testing.
        /// </summary>
        private const string TestAssembly2Path = @"DiffTestAssembly2.dll";

        /// <summary>
        ///   Randoop test generator.
        /// </summary>
        private RandoopTestGenerator generatorUnderTest;
        
        #endregion

        #region Test Plumbing

        /// <summary>
        /// Initializes properties before each test
        /// </summary>
        [TestInitialize]
        public void PreTestInit()
        {
            // get a new build manager after each test
            WinBertConfig config = this.LoadTestConfig();
            this.generatorUnderTest = new RandoopTestGenerator(
                OutputDirectory, 
                RandoopTestGenerator.GetRandoopConfiguration(config.EmbeddedConfigurations));
        }

        #endregion

        #region Test Methods

        /// <summary>
        /// Gets all the compiled tests for the target test assembly.
        /// </summary>
        [TestMethod]
        public void TestGetCompiledTests()
        {
            Assembly oldAssembly = this.LoadAssembly(TestAssembly1Path);
            Assembly target = this.LoadAssembly(TestAssembly2Path);
            AssemblyDifferenceResult diffResult = this.GetAssemblyDifferences(target, oldAssembly);
            
            ITestSuite testSuite = this.generatorUnderTest.GetCompiledTests(diffResult);

            Assert.IsNotNull(testSuite);
            Assert.IsNotNull(testSuite.NewTargetAssembly);
            Assert.IsNotNull(testSuite.OldTargetAssembly);
            Assert.IsNotNull(testSuite.NewTargetTestAssembly);
            Assert.IsNotNull(testSuite.OldTargetTestAssembly);
        }
        
        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates an AssemblyDifference result given two passed in assemblies.
        /// </summary>
        /// <param name="target">
        /// The target assembly.
        /// </param>
        /// <param name="oldAssembly">
        /// The old assembly
        /// </param>
        /// <returns>
        /// An assembly difference result.
        /// </returns>
        private AssemblyDifferenceResult GetAssemblyDifferences(Assembly target, Assembly oldAssembly)
        {
            // this should be replaced by a mock!
            var assemblyDiff = new AssemblyDifferenceResult(oldAssembly, target);

            foreach (Type newType in target.GetTypes())
            {
                var oldType = oldAssembly.GetType(newType.FullName, true);
                var typeDiff = new TypeDifferenceResult(oldType, newType);

                foreach (MethodInfo methodInfo in newType.GetMethods().TakeWhile((m) => m.DeclaringType.FullName.Equals(newType.FullName)))
                {
                    typeDiff.MethodNames.Add(methodInfo);
                }

                assemblyDiff.TypeDifferences.Add(typeDiff);
            }

            return assemblyDiff;
        }

        /// <summary>
        /// This small utility method will load an assembly from the given path. In the event an error occurs, it will
        ///   Assert.Fail().
        /// </summary>
        /// <param name="path">
        /// The path of the assembly to load.
        /// </param>
        /// <returns>
        /// A loaded assembly object of null on error.
        /// </returns>
        private Assembly LoadAssembly(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return Assembly.LoadFile(Path.GetFullPath(path));
                }
            }
            catch (Exception exception)
            {
                string errorMsg = string.Format("Error loading assembly with path {0}. {1}", path, exception.ToString());
                Assert.Fail(errorMsg);
            }

            return null;
        }

        /// <summary>
        /// Loads a test configuration object.
        /// </summary>
        /// <returns>
        /// A test WinBertConfig object.
        /// </returns>
        private WinBertConfig LoadTestConfig()
        {
            WinBertConfig config = null;
            try
            {
                using (XmlReader reader = XmlReader.Create(TestConfigPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(WinBertConfig));
                    config = (WinBertConfig)serializer.Deserialize(reader);
                }
            }
            catch (Exception exception)
            {
                var errorMessage = string.Format(
                    "Unable to deserialize the configuration file! {0} stack => {1}", 
                    exception.Message, 
                    exception.StackTrace);

                Assert.Fail(errorMessage);
            }

            return config;
        }

        #endregion
    }
}
