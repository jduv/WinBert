namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using Arktos.WinBert.Environment;
    using Arktos.WinBert.Exceptions;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Xml;

    /// <summary>
    /// Uses the Randoop framework to generate a set of tests for the target assembly under test.
    /// </summary>
    public class RandoopTestGenerator : ITestGenerator
    {
        #region Fields and Constants

        private RandoopPluginConfig config;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RandoopTestGenerator class. 
        /// </summary>
        /// <param name="config">
        /// The Randoop configuration file for the test generator to pull any needed information from.
        /// </param>
        public RandoopTestGenerator(WinBertConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("Config cannot be null!");
            }

            var randoopConfig = GetRandoopConfiguration(config);
            if (randoopConfig != null)
            {
                this.config = randoopConfig;
            }
            else
            {
                throw new InvalidConfigurationException("No valid configuration exists!");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Extracts the Randoop configuration information from a list of embedded configurations--if it exists. 
        ///  If such configuration doesn't exist, then this method will throw a InvalidConfiguration exception.
        /// </summary>
        /// <param name="config">
        /// The configuration object.
        /// </param>
        /// <returns>
        /// Returns a configuration object.
        /// </returns>
        public static RandoopPluginConfig GetRandoopConfiguration(WinBertConfig config)
        {
            if (config != null && config.EmbeddedConfigurations != null)
            {
                foreach (var embeddedConfig in config.EmbeddedConfigurations)
                {
                    if (embeddedConfig.Type.Equals(typeof(RandoopPluginConfig).FullName))
                    {
                        using (var reader = new XmlNodeReader(embeddedConfig.Any))
                        {
                            var serializer = new XmlSerializer(typeof(RandoopPluginConfig));
                            return (RandoopPluginConfig)serializer.Deserialize(reader);
                        }
                    }
                }
            }

            throw new InvalidConfigurationException("No valid Randoop configuration was found!");
        }

        /// <inheritdoc />
        public AssemblyTarget GetTestsFor(AssemblyTarget target, IEnumerable<string> validTypeNames)
        {
            if (target == null)
            {
                throw new ArgumentNullException("assembly");
            }

            if (string.IsNullOrEmpty(target.Location))
            {
                throw new ArgumentException("Assembly must have a valid location.");
            }

            if (validTypeNames == null)
            {
                throw new ArgumentNullException("Types list cannot be null!");
            }

            if (string.IsNullOrEmpty(this.config.GeneratedTestsSubDirName))
            {
                throw new InvalidConfigurationException("Specifying a randoop test sub-directory name is required!");
            }

            // Build the needed paths.
            var workingDirPath = Path.GetDirectoryName(target.Location);
            var testDirPath = Path.Combine(workingDirPath, this.config.GeneratedTestsSubDirName);
            if (Directory.Exists(testDirPath))
            {
                // Clean it
                Directory.Delete(testDirPath, true);
            }

            // Create the new directory
            Directory.CreateDirectory(testDirPath);

            if (this.GenerateTestsInNewAppDomain(target, validTypeNames))
            {
                var compiler = new TestCompiler();
                compiler.AddReference(target.Location);
                return compiler.CompileTests(testDirPath, GetTestAssemblyName(target.Location));
            }
            else
            {
                // BMK Throw an exception here instead.
                return null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates all the tests for the target assembly by spinning up a new application domain and
        /// executing a remote test generator implementation. This will prevent DLL hell.
        /// </summary>
        /// <param name="target">
        /// The assembly to load.
        /// </param>
        /// <param name="validTypes">
        /// A list of valid types to test.
        /// </param>
        /// <returns>
        /// True if the tests were successful, false otherwise.
        /// </returns>
        private bool GenerateTestsInNewAppDomain(AssemblyTarget target, IEnumerable<string> validTypeNames)
        {
            bool success = false;

            using(var environment = new AssemblyContext())
            using (var remote = Remote<RemotableTestGenerator>.CreateProxy(environment.Domain, this.config))
            {
                success = remote.RemoteObject.GenerateTests(target.Location, validTypeNames);
            }

            return success;
        }

        /// <summary>
        /// Gets a name for a new test assembly.
        /// </summary>
        /// <param name="targetAssemblyPath">
        /// The path to the target test assembly. Assumed to end with a file name with the .exe
        /// or .dll extension. Anything else will be met with an ArgumentException.
        /// </param>
        /// <returns>
        /// A name for a test assembly.
        /// </returns>
        private static string GetTestAssemblyName(string targetAssemblyPath)
        {
            var extension = Path.GetExtension(targetAssemblyPath);
            if (string.IsNullOrEmpty(extension) || extension.IndexOf("exe")  + extension.IndexOf("dll") < 0)
            {
                throw new ArgumentException("Target assembly path is invalid!");
            }

            // Tests are always placed in a library.
            var replacement = string.Format(".tests.{0}.dll", Guid.NewGuid().ToString().Substring(0, 7));
            return targetAssemblyPath.Replace(extension, replacement);
        }

        #endregion
    }
}
