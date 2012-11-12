namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;
    using Arktos.WinBert.Exceptions;
    using Arktos.WinBert.RandoopIntegration.Xml;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Xml;
    using Common;
    using Randoop;

    /// <summary>
    /// Uses the Randoop framework to generate a set of tests for the target assembly under test.
    /// </summary>
    public class RandoopTestGenerator : ITestGenerator
    {
        #region Fields and Constants

        /// <summary>
        ///   Hard coded name for the statistics file. This should eventually be moved out into configuration.
        /// </summary>
        private const string StatsFileName = "statistics.txt";

        /// <summary>
        ///   Hard coded name for the execution log file.
        /// </summary>
        private const string ExecutionLogName = "log.txt";

        /// <summary>
        /// Compiles tests.
        /// </summary>
        private readonly ITestCompiler compiler;

        /// <summary>
        ///   The configuration information for Randoop. This is different than the configuration that is built
        ///   and used to provide data to the native Randoop framework, but should replace the need for a system of
        ///   files for holding configuration information as is used presently in the Randoop framework.
        /// </summary>
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
        public Assembly GetTestAssembly(Assembly target, IList<Type> validTypes)
        {
            return this.GetTestAssembly(target, validTypes, target.Location);
        }

        /// <inheritdoc />
        public Assembly GetTestAssembly(Assembly target, IList<Type> validTypes, string assemblyPath)
        {
            if (target == null)
            {
                throw new ArgumentNullException("Difference result cannot be null!");
            }

            if (validTypes == null)
            {
                throw new ArgumentNullException("Types list cannot be null!");
            }

            if (validTypes.Count > 0)
            {
                // First, generate files for the first build.
                if (this.GenerateTests(target, validTypes, assemblyPath))
                {
                    try
                    {
                        var srcDir = Path.GetDirectoryName(assemblyPath);
                        this.compiler.AddReference(assemblyPath);
                        return this.compiler.CompileTests(srcDir);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
            }

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates a suite of tests using Randoop API's.
        /// </summary>
        /// <param name="target">
        /// The assembly to generate tests for.
        /// </param>
        /// <param name="diff">
        /// The difference result that will drive test generation.
        /// </param>
        /// <returns>
        /// True if generation was successful, false otherwise.
        /// </returns>
        private bool GenerateTests(Assembly target, IList<Type> types, string assemblyPath)
        {
            /* Much of this method is copied directly from the RandoopBare.exe's main method with some minor 
             * modifications. */

            // first grab the config file
            RandoopConfiguration config = this.GenerateRandoopInput(assemblyPath);

            // Now we need a list of the assemblies to test
            Collection<Assembly> assemblies = Misc.LoadAssemblies(config.assemblies);
            Collection<Type> typesToExplore = new Collection<Type>(types);

            // no need to continue
            if (typesToExplore.Count <= 0)
            {
                return false;
            }

            // Handle Crypto
            SystemRandom rand = new SystemRandom();
            rand.Init(config.randomseed);

            // Set up the plan manager
            PlanManager planManager = new PlanManager(config);
            planManager.builderPlans.AddEnumConstantsToPlanDB(typesToExplore);
            this.InjectConstants(planManager);

            // Stats manager
            StatsManager stats = new StatsManager(config);

            // Grab the reflection filter
            IReflectionFilter filter = this.GenerateRandoopReflectionFilters(config);

            // Default action set
            ActionSet actions = null;
            try
            {
                actions = new ActionSet(typesToExplore, filter);
            }
            catch (EmpytActionSetException)
            {
                string message = "After filtering based on configuration files, no remaining methods or constructors to explore.";
                Console.WriteLine(message);
                return false;
            }

            // The real deal. 
            RandomExplorer explorer = new RandomExplorer(typesToExplore, filter, true, config.randomseed, config.arraymaxsize, stats, actions);

            // Time it out, and we're done.
            ITimer timer = new Timer(config.timelimit);
            try
            {
                explorer.Explore(timer, planManager, config.methodweighing, config.forbidnull, true, config.fairOpt);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Randoop Explorer raised exception {0}", exception.ToString());
                return false;
            }

            // we made it!
            return true;
        }

        /// <summary>
        /// This method generates a RandoopConfiguration file for use with each test run. Ideally, this should
        ///   be pulled out and stored in the WinBert configuration file, after some parameters are gathered from
        ///   the user of course.
        /// </summary>
        /// <param name="assemblyPath">
        /// The path to the test assembly that Randoop will use for test generation. All test files will be generated into this
        /// directory as well.
        /// </param>
        /// <returns>
        /// A validated RandoopConfiguration file.
        /// </returns>
        private RandoopConfiguration GenerateRandoopInput(string assemblyPath)
        {
            string workingDirectory = Path.GetDirectoryName(assemblyPath);

            Random rand = new Random();
            RandoopConfiguration config = new RandoopConfiguration();
            config.assemblies.Add(new FileName(assemblyPath));
            config.outputdir = workingDirectory;
            config.statsFile = new FileName(Path.Combine(workingDirectory, StatsFileName));
            config.executionLog = Path.Combine(workingDirectory, ExecutionLogName);
            config.singledir = true; // always write to a single directory.
            config.timelimit = 1;    // go with 1 second for now.
            config.singledir = true;
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
        private IReflectionFilter GenerateRandoopReflectionFilters(RandoopConfiguration randoopConfig)
        {
            VisibilityFilter visibilityFilter = new VisibilityFilter(randoopConfig);
            PermissiveReflectionFilter permissiveFilter = new PermissiveReflectionFilter();
            permissiveFilter.ForbiddenFields = this.config.ForbiddenFields;
            permissiveFilter.ForbiddenMembers = this.config.ForbiddenMembers;
            permissiveFilter.ForbiddenTypes = this.config.ForbiddenTypes;

            IReflectionFilter filter = new ComposableFilter(visibilityFilter, permissiveFilter);

            return filter;
        }

        /// <summary>
        /// Injecting our own constants into the plan manager. Using hard coded text files is extremely cumbersome,
        ///   and I refuse :P. Values are pulled from the embedded Randoop configuration file that lives inside our own
        ///   WinBert configuration.
        /// </summary>
        /// <param name="planManager">
        /// The plan manager.
        /// </param>
        private void InjectConstants(PlanManager planManager)
        {
            // signed byte
            foreach (sbyte sb in this.config.SeedValues.ByteSeedValues.Values)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(sbyte), sb));
            }

            // byte
            foreach (byte b in this.config.SeedValues.ByteSeedValues.Values)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(byte), b));
            }

            // short
            foreach (short s in this.config.SeedValues.ShortSeedValues.Values)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(short), s));
            }

            // unsigned short
            foreach (ushort us in this.config.SeedValues.ShortSeedValues.Values)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(ushort), us));
            }

            // int
            foreach (int i in this.config.SeedValues.IntSeedValues.Values)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(int), i));
            }

            // unsigned int
            foreach (uint ui in this.config.SeedValues.UIntSeedValues.Values)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(uint), ui));
            }

            // float
            foreach (float f in this.config.SeedValues.FloatSeedValues.Values)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(float), f));
            }

            // double
            foreach (double d in this.config.SeedValues.DoubleSeedValues.Values)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(double), d));
            }

            // decimal
            foreach (decimal dec in this.config.SeedValues.DecimalSeedValues.Values)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(decimal), dec));
            }

            // string
            foreach (string str in this.config.SeedValues.StringSeedValues.Values)
            {
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(string), str));
            }

            // char
            foreach (string charStr in this.config.SeedValues.CharSeedValues.Values)
            {
                char c = charStr[0];
                planManager.builderPlans.AddPlan(Plan.Constant(typeof(char), c));
            }
        }

        #endregion
    }
}
