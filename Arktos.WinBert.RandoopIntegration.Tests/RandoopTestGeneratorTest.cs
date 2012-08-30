namespace Arktos.WinBertUnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;
    using Arktos.WinBert.Differencing;
    using Arktos.WinBert.RandoopIntegration;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RandoopTestGeneratorTest
    {
        #region Fields and Constants

        private const string OutputDirectory = "RandoopOut";

        private const string TestConfigPath = "WinbertTestConfig.xml";

        private const string TestAssembly1Path = @"DiffTestAssembly1.dll";

        private const string TestAssembly2Path = @"DiffTestAssembly2.dll";

        private RandoopTestGenerator generatorUnderTest;
        
        #endregion

        #region Test Plumbing

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

        [TestMethod]
        public void TestGetCompiledTests()
        {
            Assembly oldAssembly = this.LoadAssembly(TestAssembly1Path);
            Assembly target = this.LoadAssembly(TestAssembly2Path);
            AssemblyDifferenceResult diffResult = this.GetAssemblyDifferences(target, oldAssembly);
            
            IRegressionTestSuite testSuite = this.generatorUnderTest.GetCompiledTests(diffResult);

            Assert.IsNotNull(testSuite);
            Assert.IsNotNull(testSuite.NewTargetAssembly);
            Assert.IsNotNull(testSuite.OldTargetAssembly);
            Assert.IsNotNull(testSuite.NewTargetTestAssembly);
            Assert.IsNotNull(testSuite.OldTargetTestAssembly);
        }
        
        #endregion

        #region Private Methods

        private AssemblyDifferenceResult GetAssemblyDifferences(Assembly target, Assembly oldAssembly)
        {
            // this should be replaced by a mock!
            var assemblyDiff = new AssemblyDifferenceResult(oldAssembly, target);

            foreach (var newType in target.GetTypes())
            {
                var oldType = oldAssembly.GetType(newType.FullName, true);
                var typeDiff = new TypeDifferenceResult(oldType, newType);

                foreach (var methodInfo in newType.GetMethods().TakeWhile((m) => m.DeclaringType.FullName.Equals(newType.FullName)))
                {
                    typeDiff.MethodNames.Add(methodInfo);
                }

                assemblyDiff.TypeDifferences.Add(typeDiff);
            }

            return assemblyDiff;
        }

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
