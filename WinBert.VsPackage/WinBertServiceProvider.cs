namespace WinBert.VsPackage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Serialization;
    using EnvDTE;
    using EnvDTE80;
    using WinBert.Analysis;
    using WinBert.Differencing;
    using WinBert.Util;
    using WinBert.Xml;

    /// <summary>
    /// This class houses most of the logic for the Bert plug-in. This includes saving and loading solution
    ///   independent build archives, handling the differencing of target assemblies, kicking off test generation,
    ///   and handing information off for analysis.
    /// </summary>
    public sealed class WinBertServiceProvider : IWinBertServiceProvider, INotifyPropertyChanged
    {
        #region Fields and Constants

        /// <summary>
        ///   A list of ignore targets to
        /// </summary>
        private readonly IgnoreTarget[] ignoreTargets = null;

        /// <summary>
        ///   A dictionary that holds the build version manager objects that the currently opened solution
        ///   uses. The key is the project's unique name while the object is an instance of the BuildVersionManager
        ///   class. This enables multiple projects in a single solution.
        /// </summary>
        private readonly Dictionary<string, BuildVersionManager> buildDictionary = null;

        /// <summary>
        ///   Build events object. Need a strong reference for delegates and events to fire properly
        /// </summary>
        private readonly BuildEvents buildEvents = null;

        /// <summary>
        ///   Solution events object. Need a strong reference for delegates and events to fire properly
        /// </summary>
        private readonly SolutionEvents solutionEvents = null;

        /// <summary>
        ///   Static content for a test project GUID
        /// </summary>
        private const string TestProjGuid = @"{3AC096D0-A1C2-E12C-1390-A8335801FDAB}";

        /// <summary>
        ///   Static name of the configuration file
        /// </summary>
        private const string ConfigFileName = @"winbertconfig.xml";

        /// <summary>
        ///   Static name of the archive directory
        /// </summary>
        private const string ArchiveDir = @".winbert";

        /// <summary>
        ///   The reference to the VS DTE object.
        /// </summary>
        private readonly DTE2 dte = null;

        /// <summary>
        ///   Flags if a build has failed during this build iteration
        /// </summary>
        private bool buildFailed = false;

        /// <summary>
        ///   A list of AnalysisResults describing the most recent run of the engine.
        /// </summary>
        private AnalysisResults currentRunResults = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WinBertServiceProvider"/> class. 
        ///   Initializes a new instance of the WinBert class.
        /// </summary>
        /// <param name="dte">
        /// The dte.
        /// </param>
        public WinBertServiceProvider(DTE2 dte)
        {
            this.buildDictionary = new Dictionary<string, BuildVersionManager>();

            this.dte = dte;
            this.solutionEvents = dte.Events.SolutionEvents;
            this.buildEvents = dte.Events.BuildEvents;

            this.InitializeEvents();
        }

        #endregion

        #region Events

        /// <summary>
        ///   Subscribe to this event to receive notifications that a property on this object has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a list of results returned by the analysis engine after a run of the WinBert engine.
        /// </summary>
        public AnalysisResults AnalysisResults
        {
            get
            {
                return this.currentRunResults;
            }

            private set
            {
                this.currentRunResults = value;
                this.RaisePropertyChanged("AnalysisResults");
            }
        }

        /// <summary>
        ///   Gets the configuration for this WinBert service provider.
        /// </summary>
        public WinBertConfig Config { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that has changed.
        /// </param>
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Wires up all the required events.
        /// </summary>
        private void InitializeEvents()
        {
            // Hook up build events.
            this.buildEvents.OnBuildBegin += this.OnBuildBegin;
            this.buildEvents.OnBuildProjConfigBegin += this.OnBuildProjConfigBegin;
            this.buildEvents.OnBuildDone += this.OnBuildDone;
            this.buildEvents.OnBuildProjConfigDone += this.OnBuildProjConfigDone;

            // Hook up solution events
            this.solutionEvents.BeforeClosing += this.BeforeSolutionClosed;
            this.solutionEvents.Opened += this.OnSolutionOpened;
        }

        /// <summary>
        /// Handles the OnBuildBegin event.
        /// </summary>
        /// <param name="scope">
        /// Scope of the event.
        /// </param>
        /// <param name="action">
        /// Action performed.
        /// </param>
        private void OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            this.buildFailed = false;
        }

        /// <summary>
        /// Handles build project events.
        /// </summary>
        /// <param name="project">
        /// The name of the project configuration file that just built.
        /// </param>
        /// <param name="projectConfig">
        /// Project configuration.
        /// </param>
        /// <param name="platform">
        /// Project platform.
        /// </param>
        /// <param name="solutionConfig">
        /// Solution configuration.
        /// </param>
        private void OnBuildProjConfigBegin(string project, string projectConfig, string platform, string solutionConfig)
        {
        }

        /// <summary>
        /// Handles the OnbuildDone event.
        /// </summary>
        /// <param name="scope">
        /// Scope of the event.
        /// </param>
        /// <param name="action">
        /// Action performed.
        /// </param>
        private void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            var differ = new BertAssemblyDifferenceEngine(this.ignoreTargets);

            if (!this.buildFailed)
            {
                foreach (var buildDictionaryEntry in this.buildDictionary)
                {
                    var diff = this.DoDiff(buildDictionaryEntry.Value, differ);

                    if (diff != null && diff.DifferenceResult)
                    {
                        // Fetch configuration from the list of embedded configurations
                        ////ITestSuiteGenerator testGen = new RandoopTestGenerator(
                        ////    buildDictionaryEntry.Value.ArchivePath, 
                        ////    RandoopTestGenerator.GetRandoopConfiguration(this.Config.EmbeddedConfigurations));

                        ////ITestSuiteInstrumenter instrumenter = new RandoopTestSuiteInstrumenter();
                        ////ITestSuiteRunner testRunner = new RandoopTestRunner();
                        ////IBehavioralAnalyzer analyzer = new BertBehavioralAnalyzer();

                        ////ITestSuite generatedTests = testGen.GetCompiledTests(diff);
                        ////ITestSuite instrumentedTests = instrumenter.InstrumentTestSuite(generatedTests);
                        ////TestSuiteRunResult result = testRunner.RunTests(instrumentedTests);
                        ////this.AnalysisResults = analyzer.Analyze(result);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the BuildProjConfigDone event.
        /// </summary>
        /// <param name="project">
        /// The name of the project configuration file that just built.
        /// </param>
        /// <param name="projectConfig">
        /// Project configuration.
        /// </param>
        /// <param name="platform">
        /// Project platform.
        /// </param>
        /// <param name="solutionConfig">
        /// Solution configuration.
        /// </param>
        /// <param name="success">
        /// True if the build was successful, false otherwise.
        /// </param>
        private void OnBuildProjConfigDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            if (success)
            {
                var projectFileName = Path.GetFileName(project);
                var projectName = Path.GetFileNameWithoutExtension(project);
                var visualStudioProject = this.FindVsProject(projectName);

                // Only archive eligible assemblies and PE files
                if (this.IsEligbleProject(visualStudioProject, projectFileName))
                {
                    BuildVersionManager manager;
                    if (!this.buildDictionary.TryGetValue(projectName, out manager))
                    {
                        // make a new build manager
                        var fullArchivePath = Path.Combine(this.GetSolutionWorkingDirectory(), ArchiveDir);
                        manager = new BuildVersionManager(fullArchivePath, projectName);
                        this.buildDictionary.Add(projectName, manager);
                    }

                    // Get the active configuration.
                    var confManager = visualStudioProject.ConfigurationManager;
                    var config = confManager.ActiveConfiguration;

                    // Get the output path for the current configuration.
                    var outputPathProperty = config.Properties.Item("OutputPath");
                    var assemblyNameProperty = visualStudioProject.Properties.Item("OutputFileName");

                    if (outputPathProperty != null && assemblyNameProperty != null)
                    {
                        var outputPath = Path.GetFullPath(outputPathProperty.Value.ToString());
                        var buildPath = Path.Combine(outputPath, assemblyNameProperty.Value.ToString());

                        if (File.Exists(buildPath))
                        {
                            manager.AddNewSuccessfulBuild(buildPath);
                        }
                        else
                        {
                            var error = string.Format(
                                "Unable to add the build at path {0} to the archive for project {1}. The file doesn't exist.", 
                                buildPath, project);

                            MessageBox.Show(error);
                        }
                    }
                }
            }
            else
            {
                this.buildFailed = true;
            }
        }

        /// <summary>
        /// Handles the SolutionOpened event passed from the IDE.
        /// </summary>
        private void OnSolutionOpened()
        {
            if (this.dte.Solution.IsOpen)
            {
                this.LoadState();
            }
        }

        /// <summary>
        /// Handles the BeforeSolutionClosed event passed from the IDE
        /// </summary>
        private void BeforeSolutionClosed()
        {
            if (this.dte.Solution.IsOpen)
            {
                this.SaveState();
            }

            this.buildDictionary.Clear();
        }

        /// <summary>
        /// Loads a configuration file based on the passed in path.
        /// </summary>
        private void LoadState()
        {
            string path = Path.Combine(this.GetSolutionWorkingDirectory(), ConfigFileName);

            try
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(WinBertConfig));
                    this.Config = (WinBertConfig)serializer.Deserialize(reader);
                }
            }
            catch (Exception exception)
            {
                string errorMessage = string.Format(
                    "Unable to deserialize the configuration file! {0} stack => {1}", exception.Message, exception.StackTrace);
                Trace.TraceError(errorMessage);
                return;
            }

            // ick, double for loop
            foreach (WinBertProject project in this.Config.Projects)
            {
                BuildVersionManager manager = new BuildVersionManager(this.Config.MasterArchivePath, project.Name);

                foreach (Build build in project.BuildsList)
                {
                    if (File.Exists(build.Path))
                    {
                        manager.LoadBuild(build.SequenceNumber, build.Path);
                    }
                    else
                    {
                        string errorMessage = string.Format(
                            "Error loading file at path {0} into the build manager for project {1}", build.Path, project.Name);
                        Trace.TraceError(errorMessage);
                    }
                }

                this.buildDictionary.Add(project.Name, manager);
            }
        }

        /// <summary>
        /// Saves a configuration file to the target path.
        /// </summary>
        private void SaveState()
        {
            string path = Path.Combine(this.GetSolutionWorkingDirectory(), ConfigFileName);

            var config = new WinBertConfig();
            var projects = this.GetWinBertProjects();

            config.EmbeddedConfigurations = this.Config.EmbeddedConfigurations;
            config.Projects = projects;
            config.MasterArchivePath = Path.Combine(this.GetSolutionWorkingDirectory(), ArchiveDir);
            config.IgnoreList = this.ignoreTargets ?? new IgnoreTarget[0];

            try
            {
                using (XmlWriter writer = XmlWriter.Create(path))
                {
                    var serializer = new XmlSerializer(typeof(WinBertConfig));
                    serializer.Serialize(writer, config);
                }
            }
            catch (Exception exception)
            {
                var errorMessage = string.Format(
                    "Unable to serialize the configuration file! {0} stack => {1}", exception.Message, exception.StackTrace);
                Trace.TraceError(errorMessage);
            }
        }

        /// <summary>
        /// Returns a list of WinBertProject objects translated directly from this WinBert instance's BuildDictionary.
        /// </summary>
        /// <returns>
        /// A list of WinBertProjects.
        /// </returns>
        private WinBertProject[] GetWinBertProjects()
        {
            int i = 0;
            var projects = new WinBertProject[this.buildDictionary.Count];

            foreach (var buildManagerEntry in this.buildDictionary)
            {
                var project = new WinBertProject();
                var manager = buildManagerEntry.Value;

                project.Name = buildManagerEntry.Key;
                project.BuildsList = new Build[manager.BuildArchive.Count];

                int j = 0;
                foreach (var buildEntry in manager.BuildArchive)
                {
                    project.BuildsList[j] = buildEntry.Value;
                    j++;
                }

                projects[i] = project;
                i++;
            }

            return projects;
        }

        /// <summary>
        /// Finds a VS project given a unique name.
        /// </summary>
        /// <param name="name">
        /// The name to find
        /// </param>
        /// <returns>
        /// A reference to the named project or null if it doesn't exist
        /// </returns>
        private Project FindVsProject(string name)
        {
            foreach (Project project in this.dte.Solution.Projects)
            {
                if (project.Name.Equals(name))
                {
                    return project;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if the target project is an eligible BERT target. Examples of projects that should be ignored
        ///   include: unit test projects and any other automatically generated projects.
        /// </summary>
        /// <param name="project">
        /// The project whose eligibility to determine
        /// </param>
        /// <param name="projectFileName">
        /// The path to the project's configuration file
        /// </param>
        /// <returns>
        /// True if the project can be WinBert enabled.
        /// </returns>
        private bool IsEligbleProject(Project project, string projectFileName)
        {
            var fullPathProperty = project.Properties.Item("FullPath");

            if (fullPathProperty != null)
            {
                var fullPath = Path.Combine(fullPathProperty.Value.ToString(), projectFileName);
                var projectTypeGuids = this.GetProjectTypeGuids(fullPath);

                if (!string.IsNullOrEmpty(projectTypeGuids) && projectTypeGuids.Contains(TestProjGuid))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the project type guids string from the target configuration file.
        /// </summary>
        /// <param name="pathToConfigFile">
        /// The path to the configuration file to look into
        /// </param>
        /// <returns>
        /// A string of GUIDs or null on error
        /// </returns>
        private string GetProjectTypeGuids(string pathToConfigFile)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(pathToConfigFile);

                var nav = doc.CreateNavigator();
                var namespaceManager = new XmlNamespaceManager(doc.NameTable);
                namespaceManager.AddNamespace("ms", "http://schemas.microsoft.com/developer/msbuild/2003");
                var expression = nav.Compile(@"ms:Project/ms:PropertyGroup/ms:ProjectTypeGuids");
                expression.SetContext(namespaceManager);

                var projectTypeGuids = nav.SelectSingleNode(expression);

                if (projectTypeGuids != null)
                {
                    return projectTypeGuids.Value;
                }
            }
            catch (Exception exception)
            {
                string errorMessage = string.Format(
                    "Unable to extract project guids from file {0}! {1} stack => {2}", pathToConfigFile, exception.Message, exception.StackTrace);
                Trace.TraceError(errorMessage);
            }

            return null;
        }

        /// <summary>
        /// Returns the working directory for the solution.
        /// </summary>
        /// <returns>
        /// The solution's working directory
        /// </returns>
        private string GetSolutionWorkingDirectory()
        {
            return Path.GetDirectoryName(this.dte.Solution.FullName);
        }

        /// <summary>
        /// Handles differencing two builds retrieved from the passed in build version manager with the passed in 
        ///   differencing engine.
        /// </summary>
        /// <param name="manager">
        /// The build manager to retrieve builds from
        /// </param>
        /// <param name="differ">
        /// The differencing engine to use
        /// </param>
        /// <returns>
        /// An IDifferenceResult for an Assembly type.
        /// </returns>
        private AssemblyDifferenceResult DoDiff(BuildVersionManager manager, BertAssemblyDifferenceEngine differ)
        {
            if (manager.BuildArchive.Count > 1)
            {
                var newBuild = manager.GetMostRecentBuild();
                var oldBuild = manager.GetBuildRevisionPreceding(newBuild.SequenceNumber);

                try
                {
                    var newAssembly = Assembly.LoadFile(newBuild.Path);
                    var oldAssembly = Assembly.LoadFile(oldBuild.Path);

                    return differ.Diff(oldAssembly, newAssembly);
                }
                catch (Exception exception)
                {
                    Trace.TraceError("Unable to diff assemblies {0} and {1}. Exception: {2}", oldBuild.Path, newBuild.Path, exception);
                }
            }

            return null;
        }

        #endregion
    }
}