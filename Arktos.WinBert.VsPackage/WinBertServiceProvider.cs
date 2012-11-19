namespace Arktos.WinBert.VsPackage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Serialization;
    using Arktos.WinBert.Analysis;
    using Arktos.WinBert.RandoopIntegration;
    using Arktos.WinBert.Testing;
    using Arktos.WinBert.Util;
    using Arktos.WinBert.Xml;
    using EnvDTE;
    using EnvDTE80;

    /// <summary>
    /// This class houses most of the logic for the Bert plug-in. This includes saving and loading solution
    /// independent build archives, handling the differencing of target assemblies, kicking off test generation,
    /// and handing information off for analysis.
    /// </summary>
    public sealed class WinBertServiceProvider
    {
        #region Fields and Constants

        /// <summary>
        /// Static content for a test project GUID
        /// </summary>
        private static readonly string TestProjGuid = @"{3AC096D0-A1C2-E12C-1390-A8335801FDAB}";

        /// <summary>
        /// Static name of the archive directory
        /// </summary>
        private static readonly string ArchiveDir = @".winbert";

        /// <summary>
        /// Static path of the configuration file relative to the archive directory.
        /// </summary>
        private static readonly string ConfigFilePath = Path.Combine(ArchiveDir, @"winbertconfig.xml");

        /// <summary>
        /// A dictionary that holds the build version manager objects that the currently opened solution
        /// uses. The key is the project's unique name while the object is an instance of the BuildVersionManager
        /// class. This enables multiple projects in a single solution.
        /// </summary>
        private readonly Dictionary<string, BuildVersionManager> buildDictionary = null;

        /// <summary>
        /// Build events object. Need a strong reference for delegates and events to fire properly
        /// </summary>
        private readonly BuildEvents buildEvents = null;

        /// <summary>
        /// Solution events object. Need a strong reference for delegates and events to fire properly
        /// </summary>
        private readonly SolutionEvents solutionEvents = null;

        /// <summary>
        /// The reference to the VS DTE object.
        /// </summary>
        private readonly DTE2 dte = null;

        /// <summary>
        /// Flags if a build has failed during this build iteration
        /// </summary>
        private bool buildFailed = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WinBertServiceProvider"/> class. 
        /// Initializes a new instance of the WinBert class.
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

        #region Properties

        /// <summary>
        /// Gets the configuration for this WinBert service provider.
        /// </summary>
        public WinBertConfig Config { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attempts to load a configuration file from the target path.
        /// </summary>
        /// <param name="path">The path to load.</param>
        /// <returns>A configuration object, or null on failure.</returns>
        private static WinBertConfig LoadState(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    return WinBertConfig.Deserialize(File.OpenRead(path));
                }
                catch (Exception exc)
                {
                    var errorMessage = "Unable to deserialize the configuration file!" + Environment.NewLine;
                    errorMessage += "Exception: " + exc;
                    Debug.WriteLine(errorMessage);
                }
            }

            return null;
        }

        /// <summary>
        /// Reads in a default configuration file stored inside the package DLL.
        /// </summary>
        /// <returns></returns>
        private static WinBertConfig GetDefaultConfig()
        {
            try
            {
                var data = Encoding.ASCII.GetBytes(Resources.winbertconfig);
                return WinBertConfig.Deserialize(new MemoryStream(data));
            }
            catch (Exception exc)
            {
                var errorMessage = "Unable to deserialize in-memory configuration file!" + Environment.NewLine;
                errorMessage += "Exception: " + exc;
                Debug.WriteLine(errorMessage);
            }

            return null;
        }

        /// <summary>
        /// Wires up all the required events.
        /// </summary>
        private void InitializeEvents()
        {
            // Hook up build events.
            this.buildEvents.OnBuildBegin += this.OnBuildBegin;
            //this.buildEvents.OnBuildProjConfigBegin += this.OnBuildProjConfigBegin;
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
            if (!this.buildFailed)
            {
                foreach (var manager in this.buildDictionary.Values)
                {
                    // Grab a pointer to the most recent build
                    var currentBuild = manager.GetMostRecentBuild();

                    // Get the last build where tests were executed.
                    var previousBuild = manager.GetBuildRevisionPreceding(currentBuild);

                    if (currentBuild != null && previousBuild != null)
                    {
                        var generator = new RandoopTestGenerator(this.Config, new TestCompiler());
                        var instrumenter = new RandoopTestInstrumenter();
                        var runner = new RandoopTestRunner();
                        var analyzer = new BertBehavioralAnalyzer();

                        var tester = new RegressionTestManager(this.Config, generator, instrumenter, runner, analyzer);
                        tester.BuildAndExecuteTestSuite(previousBuild, currentBuild);
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
                if (this.IsEligibleProject(visualStudioProject, projectFileName))
                {
                    BuildVersionManager manager;
                    if (!this.buildDictionary.TryGetValue(projectName, out manager))
                    {
                        // make a new build manager
                        var fullArchivePath = Path.Combine(this.GetSolutionWorkingDirectory(), ArchiveDir);
                        manager = new BuildVersionManager(archivePath: fullArchivePath, name: projectName);
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
                            var errorMessage = string.Format(
                                "Unable to add the build at path {0} to the archive for project {1}. The file doesn't exist.",
                                buildPath, project);

                            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK);
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
                string path = Path.Combine(this.GetSolutionWorkingDirectory(), ConfigFilePath);

                var config = LoadState(path) ?? GetDefaultConfig();

                if (config == null)
                {
                    var errorMessage = "Unable to load configuration at path " + path + " or create a new one. ";
                    errorMessage += "The state of your build archive will not be maintained! ";
                    MessageBox.Show(errorMessage, "Error!", MessageBoxButtons.OK);
                }
                else
                {
                    this.Config = config;
                    this.LoadConfiguration();
                }
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
        /// Loads up the configuration nodes into real data for the plugin.
        /// </summary>
        private void LoadConfiguration()
        {
            // ick, double for loop
            foreach (WinBertProject project in this.Config.Projects)
            {
                BuildVersionManager manager = new BuildVersionManager(
                    archivePath: this.Config.MasterArchivePath,
                    name: project.Name);

                foreach (Build build in project.BuildsList)
                {
                    if (File.Exists(build.AssemblyPath))
                    {
                        manager.LoadBuild(build.SequenceNumber, build.AssemblyPath);
                    }
                    else
                    {
                        string errorMessage = string.Format(
                            "Error loading file at path {0} into the build manager for project {1}", build.AssemblyPath, project.Name);
                        Debug.WriteLine(errorMessage);
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
            if (this.Config != null)
            {
                string path = Path.Combine(this.GetSolutionWorkingDirectory(), ConfigFilePath);

                var config = new WinBertConfig();
                var projects = this.GetWinBertProjects();

                config.EmbeddedConfigurations = this.Config.EmbeddedConfigurations;
                config.Projects = projects;
                config.MasterArchivePath = Path.Combine(this.GetSolutionWorkingDirectory(), ArchiveDir);
                config.IgnoreList = this.Config.IgnoreList ?? new IgnoreTarget[0];

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
                    var errorMessage = "Unable to serialize the configuration file!" + Environment.NewLine;
                    errorMessage += "Exception: " + Environment.NewLine + exception;
                    MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK);
                }
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
        /// include: unit test projects and any other automatically generated projects.
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
        private bool IsEligibleProject(Project project, string projectFileName)
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
                Debug.WriteLine(errorMessage);
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

        #endregion
    }
}