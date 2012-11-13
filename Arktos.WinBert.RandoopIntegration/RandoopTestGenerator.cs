namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using Arktos.WinBert.Exceptions;
    using Arktos.WinBert.RandoopIntegration.Xml;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Xml;
    using Microsoft.Cci;
    using Arktos.WinBert.Remoting;

    /// <summary>
    /// Uses the Randoop framework to generate a set of tests for the target assembly under test.
    /// </summary>
    public class RandoopTestGenerator : ITestGenerator
    {
        #region Fields and Constants

        private readonly ITestCompiler compiler;
        private RandoopPluginConfig config;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RandoopTestGenerator"/> class. 
        /// Initializes a new instance of the RandoopTestGeneratorClass.
        /// </summary>
        /// <param name="workingDirectory">
        /// The path to where tests should be generated. If this directory doesn't exist the generator will create it.
        /// </param>
        /// <param name="config">
        /// The Randoop configuration file for the test generator to pull any needed information from.
        /// </param>
        public RandoopTestGenerator(WinBertConfig config, ITestCompiler compiler)
        {
            if (config == null)
            {
                throw new ArgumentNullException("Config cannot be null!");
            }

            if (compiler == null)
            {
                throw new ArgumentNullException("Test compiler cannot be null!");
            }

            this.compiler = compiler;
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
        public IAssembly GetTestsFor(IAssembly target, IList<INamedTypeDefinition> validTypes)
        {
            if (target == null)
            {
                throw new ArgumentNullException("assembly");
            }

            if (string.IsNullOrEmpty(target.Location))
            {
                throw new ArgumentException("Assembly must have a valid location.");
            }

            if (validTypes == null)
            {
                throw new ArgumentNullException("Types list cannot be null!");
            }

            if (validTypes.Count > 0)
            {
                if (this.GenerateTestsInNewAppDomain(target, validTypes))
                {
                    try
                    {
                        var srcDir = Path.GetDirectoryName(target.Location);
                        this.compiler.AddReference(target.Location);
                        return this.compiler.CompileTests(srcDir, GetTestAssemblyName(target.Location));
                    }
                    catch (Exception)
                    {
                        // BMK Handle exception
                    }
                    finally
                    {
                        this.compiler.ClearReferences();
                    }
                }
            }

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates all the tests for the target assembly by spinning up a new application domain and
        /// executing a remote test generator implementation. This will prevent assembly memory hell.
        /// </summary>
        /// <param name="assembly">
        /// The assembly to load.
        /// </param>
        /// <param name="validTypes">
        /// A list of valid types to test.
        /// </param>
        /// <returns>
        /// True if the tests were successful, false otherwise.
        /// </returns>
        private bool GenerateTestsInNewAppDomain(IAssembly assembly, IList<INamedTypeDefinition> validTypes)
        {
            bool success = false;
            var assemblyPath = assembly.Location;
            var typeNames = validTypes.Select(x => x.Name.Value).ToList();
            using (var isolated = new Isolated<RemoteTestGenerator>(this.config))
            {
                success = isolated.Value.GenerateTests(typeNames, assemblyPath);
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
            if (string.IsNullOrEmpty(extension))
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
