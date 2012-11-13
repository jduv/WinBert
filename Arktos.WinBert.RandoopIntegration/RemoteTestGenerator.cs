namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Common;
    using Randoop;
using Arktos.WinBert.Xml;
    using Arktos.WinBert.RandoopIntegration.Xml;

    public class RemoteTestGenerator : MarshalByRefObject
    {
        #region Fields & Constants

        private const string StatsFileName = "statistics.txt";
        private const string ExecutionLogName = "log.txt";
        private readonly RandoopPluginConfig config;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the RandoopTestGeneratorProxy class.
        /// </summary>
        /// <param name="config">
        /// The configuration to initialize with.
        /// </param>
        public RemoteTestGenerator(RandoopPluginConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            this.config = config;
        }

        #endregion

        #region Public Methods

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
        public bool GenerateTests(IList<string> includedTypeNames, string assemblyPath)
        {
            // first grab the config file
            RandoopConfiguration config = this.GenerateRandoopInput(assemblyPath);

            // Now we need a list of the assemblies to test
            Collection<Assembly> assemblies = Misc.LoadAssemblies(config.assemblies);

            // Retrieve the target assembly from the loaded ones.
            var targetAssembly = assemblies.FirstOrDefault(x => x.Location.Equals(assemblyPath));

            // Filter types.
            var typesToExplore = new Collection<Type>(targetAssembly.GetTypes()
                .Where(x => includedTypeNames.Any(y => y.Equals(x.Name))).ToList());

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
            planManager.builderPlans.AddEnumConstantsToPlanDB(new Collection<Type>(typesToExplore.ToList()));
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
            catch (Exception)
            {
                return false;
            }

            // we made it!
            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method generates a RandoopConfiguration file for use with each test run. Ideally, this should
        /// be pulled out and stored in the WinBert configuration file, after some parameters are gathered from
        /// the user of course.
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
