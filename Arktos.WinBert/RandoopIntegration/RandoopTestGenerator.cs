namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;
    using AppDomainToolkit;
    using Arktos.WinBert.Exceptions;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Xml;
    using Common;
    using Randoop;

    /// <summary>
    /// Uses the Randoop framework to generate a set of tests for the target assembly under test.
    /// </summary>
    public class RandoopTestGenerator : ITestGenerator
    {
        #region Fields & Constants

        private static readonly string StatsFileName = "statistics.txt";
        private static readonly string ExecutionLogName = "log.txt";
        private readonly RandoopPluginConfig config;

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

            var randoopConfig = GetRandoopPluginConfig(config);
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
        public static RandoopPluginConfig GetRandoopPluginConfig(WinBertConfig config)
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
        public IAssemblyTarget GenerateTests(IAssemblyTarget target, IEnumerable<string> validTypeNames)
        {
            if (target == null)
            {
                throw new ArgumentNullException("assembly");
            }

            if (validTypeNames == null)
            {
                throw new ArgumentNullException("validTypeNames");
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

            // Execute Randoop
            RunRandoop(target, validTypeNames, testDirPath, this.config);

            // Compile the generated tests.
            var compiler = new TestCompiler();
            compiler.AddReference(target.Location);
            return compiler.CompileTests(testDirPath, GetTestAssemblyName(target.Location));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Executes randoop.
        /// </summary>
        /// <param name="target">
        /// The assembly under test.
        /// </param>
        /// <param name="validTypeNames">
        /// A list of valid type names to test.
        /// </param>
        /// <param name="workingDir">
        /// The working directory.
        /// </param>
        /// <param name="config">
        /// The randoop plugin configuration.
        /// </param>
        private static void RunRandoop(IAssemblyTarget target, IEnumerable<string> validTypeNames, string workingDir, RandoopPluginConfig config)
        {
            // first grab the config file
            RandoopConfiguration randoopConfig = GetRandoopConfig(workingDir);

            // Load up the target along with it's references.
            var loader = new AssemblyLoader();

            var test = AppDomain.CurrentDomain.GetAssemblies();

            var assemblies = loader.LoadAssemblyWithReferences(LoadMethod.LoadFrom, target.Location);
            var targetAssembly = assemblies.First(x => x.FullName.Equals(target.FullName));

            // Filter types.
            var typesToExplore = new Collection<Type>(targetAssembly.GetTypes()
                .Where(x => validTypeNames.Any(y => y.Equals(x.Name))).ToList());

            // no need to continue
            if (typesToExplore.Count <= 0)
            {
                throw new TestGenerationException("No type to explore! No tests will be generated.");
            }

            // Handle Crypto
            SystemRandom rand = new SystemRandom();
            rand.Init(randoopConfig.randomseed);

            // Set up the plan manager
            PlanManager planManager = new PlanManager(randoopConfig);
            planManager.builderPlans.AddEnumConstantsToPlanDB(new Collection<Type>(typesToExplore.ToList()));
            SeedPlanManager(planManager, config);

            // Stats manager
            StatsManager statsManager = new StatsManager(randoopConfig);

            // Grab the reflection filter
            IReflectionFilter filter = GenerateRandoopReflectionFilters(randoopConfig, config);

            // Default action set
            ActionSet actions = null;
            try
            {
                actions = new ActionSet(typesToExplore, filter);
            }
            catch (EmpytActionSetException exc)
            {
                throw new TestGenerationException(exc);
            }

            // The real deal. 
            RandomExplorer explorer = new RandomExplorer(
                typesToExplore,
                filter,
                true,
                randoopConfig.randomseed,
                randoopConfig.arraymaxsize,
                statsManager,
                actions);

            // Time it out, and we're done.
            ITimer timer = new Timer(0.5);
            try
            {
                explorer.Explore(timer, planManager, randoopConfig.methodweighing, randoopConfig.forbidnull, true, randoopConfig.fairOpt);
            }
            catch (Exception exc)
            {
                throw new TestGenerationException(exc);
            }
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
            if (string.IsNullOrEmpty(extension) || extension.IndexOf("exe") + extension.IndexOf("dll") < 0)
            {
                throw new ArgumentException("Target assembly path is invalid!");
            }

            // Tests are always placed in a library.
            var replacement = string.Format(".tests.{0}.dll", Guid.NewGuid().ToString().Substring(0, 7));
            return targetAssemblyPath.Replace(extension, replacement);
        }

        /// <summary>
        /// Injecting our own constants into the plan manager. Using hard coded text files is extremely cumbersome,
        /// and I refuse :P. Values are pulled from the embedded Randoop configuration file that lives inside our own
        /// WinBert configuration.
        /// </summary>
        /// <param name="planManager">
        /// The plan manager.
        /// </param>
        /// <param name="config">
        /// The plugin config holding all the constants to inject.
        /// </param>
        private static void SeedPlanManager(PlanManager planManager, RandoopPluginConfig config)
        {
            // signed byte
            foreach (sbyte sb in config.SeedValues.ByteSeedValues)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(sbyte), sb));
            }

            // byte
            foreach (byte b in config.SeedValues.ByteSeedValues)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(byte), b));
            }

            // short
            foreach (short s in config.SeedValues.ShortSeedValues)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(short), s));
            }

            // unsigned short
            foreach (ushort us in config.SeedValues.ShortSeedValues)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(ushort), us));
            }

            // int
            foreach (int i in config.SeedValues.IntSeedValues)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(int), i));
            }

            // unsigned int
            foreach (uint ui in config.SeedValues.UIntSeedValues)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(uint), ui));
            }

            // float
            foreach (float f in config.SeedValues.FloatSeedValues.Value)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(float), f));
            }

            // decimal
            foreach (decimal dec in config.SeedValues.DecimalSeedValues)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(decimal), dec));
            }

            // double
            foreach (double d in config.SeedValues.DoubleSeedValues.Value)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(double), d));
            }

            // string
            foreach (string str in config.SeedValues.StringSeedValues.Value)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(string), str));
            }

            // char
            foreach (string charStr in config.SeedValues.CharSeedValues)
            {
                char c = charStr[0];
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(char), c));
            }
        }

        /// <summary>
        /// Generates a RandoopConfiguration file for use with each test run.
        /// </summary>
        /// <param name="workingDir">
        /// The working directory for the test generation pass.
        /// </param>
        /// <returns>
        /// A validated RandoopConfiguration file.
        /// </returns>
        private static RandoopConfiguration GetRandoopConfig(string workingDir)
        {
            Random rand = new Random();
            RandoopConfiguration config = new RandoopConfiguration();
            config.outputdir = workingDir;
            config.statsFile = new FileName(Path.Combine(workingDir, StatsFileName));
            config.executionLog = Path.Combine(workingDir, ExecutionLogName);
            config.singledir = true; // always write to a single directory.
            config.randomseed = rand.Next();
            config.planstartid = 0;
            config.useinternal = false;
            config.usestatic = false;
            config.outputnormalinputs = true;
            config.methodweighing = MethodWeighing.Uniform;
            config.fairOpt = false;

            return config;
        }

        /// <summary>
        /// Generates a set of reflection filters for the Randoop test run.
        /// </summary>
        /// <param name="randoopConfig">
        /// The configuration file to generate the filter from.
        /// </param>
        /// <param name="pluginConfig">
        /// The randoop plugin config to generate the filter from.
        /// </param>
        private static IReflectionFilter GenerateRandoopReflectionFilters(RandoopConfiguration randoopConfig, RandoopPluginConfig pluginConfig)
        {
            VisibilityFilter visibilityFilter = new VisibilityFilter(randoopConfig);
            PermissiveReflectionFilter permissiveFilter = new PermissiveReflectionFilter();
            permissiveFilter.ForbiddenFields = pluginConfig.ForbiddenFields;
            permissiveFilter.ForbiddenMembers = pluginConfig.ForbiddenMembers;
            permissiveFilter.ForbiddenTypes = pluginConfig.ForbiddenTypes;
            IReflectionFilter filter = new ComposableFilter(visibilityFilter, permissiveFilter);

            return filter;
        }

        #endregion
    }
}
