namespace WinBert.RandoopIntegration
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;
    using Common;
    using Randoop;
    using WinBert.Differencing;
    using WinBert.Exceptions;
    using WinBert.RandoopIntegration.Xml;
    using WinBert.Testing;
    using WinBert.Xml;

    /// <summary>
    /// Uses the Randoop framework to generate a set of tests for the target assembly under test.
    /// </summary>
    public class RandoopTestGenerator : ITestSuiteGenerator
    {
        #region Fields and Constants

        /// <summary>
        ///   Hard coded name for the statistics file. This should eventually be moved out into configuration.
        /// </summary>
        private const string StatsFile = "statistics.txt";

        /// <summary>
        ///   Hard coded name for the execution log file.
        /// </summary>
        private const string ExecutionLog = "log.txt";

        /// <summary>
        ///   Hard coded name for the assembly.
        /// </summary>
        private const string AssemblyName = "tests.dll";

        /// <summary>
        ///   The path to where the tests will be generated.
        /// </summary>
        private string workingDirectory;

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
        public RandoopTestGenerator(string workingDirectory, RandoopPluginConfig config)
        {
            this.workingDirectory = workingDirectory;
            this.config = config;

            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Extracts the Randoop configuration information from a list of embedded configurations--if it exists. 
        ///   If such configuration doesn't exist, then this method will throw a InvalidConfiguration exception.
        /// </summary>
        /// <param name="configs">
        /// A list of configurations to test.
        /// </param>
        /// <returns>
        /// Returns a configuration object.
        /// </returns>
        public static RandoopPluginConfig GetRandoopConfiguration(EmbeddedConfiguration[] configs)
        {
            foreach (EmbeddedConfiguration embeddedConfig in configs)
            {
                if (embeddedConfig.Type.Equals(typeof(RandoopPluginConfig).FullName))
                {
                    try
                    {
                        using (XmlReader reader = new XmlNodeReader(embeddedConfig.Any))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(RandoopPluginConfig));
                            return (RandoopPluginConfig)serializer.Deserialize(reader);                            
                        }
                    }
                    catch (Exception exception)
                    {
                        var message = string.Format(
                            "Could not deserialize RandoopConfig! {0} stack => {1}", 
                            exception.Message, 
                            exception.StackTrace);

                        Trace.WriteLine(message);
                    }
                }
            }

            throw new InvalidConfigurationException("No valid Randoop configuration was found!");
        }

        /// <summary>
        /// Gets a TestAssembly containing a set of compiled tests.
        /// </summary>
        /// <param name="diff">
        /// The differences result context.
        /// </param>
        /// <returns>
        /// A compiled test assembly with executable  tests inside it.
        /// </returns>
        public ITestSuite GetCompiledTests(AssemblyDifferenceResult diff)
        {
            if (this.GenerateTests(diff.NewObject, diff))
            {
                try
                {
                    // output dir is the same dir the tests are generated in
                    IAssemblyCompiler compiler = new BertAssemblyCompiler(this.workingDirectory);
                    compiler.AddReference(diff.NewObject.Location);

                    // run the tests on the new assembly
                    var newTests = compiler.CompileTests(this.workingDirectory);

                    // kill the references, set up to run the tests on the old assembly
                    compiler.ClearReferences();
                    compiler.AddReference(diff.OldObject.Location);

                    // run the tests on the old assembly
                    var oldTests = compiler.CompileTests(this.workingDirectory);

                    // return the properly built test suite.
                    return new TestSuite(newTests, oldTests, diff);

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
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
        private bool GenerateTests(Assembly target, AssemblyDifferenceResult diff)
        {
            /* Much of this method is copied directly from the RandoopBare.exe's main method with some minor 
             * modifications. */

            // first grab the config file
            RandoopConfiguration config = this.GenerateRandoopInput(target.Location);

            // Now we need a list of the assemblies to test
            Collection<Assembly> assemblies = Misc.LoadAssemblies(config.assemblies);
            Collection<Type> typesToExplore = this.GetExplorableTypes(target, diff);

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
            planManager.builderPlans.PrintPrimitives(Console.Out);

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
        /// Gets a list of types that can be explored for the target difference result.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="diff">
        /// The difference to explore.
        /// </param>
        /// <returns>
        /// A collection of types.
        /// </returns>
        private Collection<Type> GetExplorableTypes(Assembly target, AssemblyDifferenceResult diff)
        {
            Collection<Type> collection = new Collection<Type>();

            if (diff.DifferenceResult)
            {
                foreach (TypeDifferenceResult typeDiff in diff.TypeDifferences)
                {
                    if (typeDiff.DifferenceResult)
                    {
                        collection.Add(typeDiff.NewObject);
                    }
                }
            }

            // should have elements added or be empty.
            return collection;
        }

        /// <summary>
        /// This method generates a RandoopConfiguration file for use with each test run. Ideally, this should
        ///   be pulled out and stored in the WinBert configuration file, after some parameters are gathered from
        ///   the user of course.
        /// </summary>
        /// <param name="assemblyPath">
        /// The path to the test assembly that Randoop will use for test generation.
        /// </param>
        /// <returns>
        /// A validated RandoopConfiguration file.
        /// </returns>
        private RandoopConfiguration GenerateRandoopInput(string assemblyPath)
        {
            Random rand = new Random();

            RandoopConfiguration config = new RandoopConfiguration();
            config.assemblies.Add(new FileName(assemblyPath));
            config.outputdir = this.workingDirectory;
            config.statsFile = new FileName(StatsFile);
            config.executionLog = Path.Combine(this.workingDirectory, ExecutionLog);
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
