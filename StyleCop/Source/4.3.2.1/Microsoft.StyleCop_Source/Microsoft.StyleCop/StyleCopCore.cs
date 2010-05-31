namespace Microsoft.StyleCop
{
    using Microsoft.Build.Framework;
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    public sealed class StyleCopCore : IPropertyContainer, IDisposable
    {
        private bool addinsDisabledByDefault;
        private Dictionary<string, SourceAnalyzer> analyzers;
        private bool analyzing;
        private bool cancel;
        private CoreParser coreParser;
        private bool displayUI;
        private StyleCopEnvironment environment;
        private object hostTag;
        private Log log;
        private Dictionary<string, SourceParser> parsers;
        internal const string ProjectSettingsPropertyPageIdProperty = "StyleCopLocalProperties";
        private RegistryUtils registry;
        private bool writeResultsCache;

        public event EventHandler<AddSettingsPagesEventArgs> AddSettingsPages;

        public event EventHandler<OutputEventArgs> OutputGenerated;

        public event EventHandler ProjectSettingsChanged;

        public event EventHandler<ViolationEventArgs> ViolationEncountered;

        public StyleCopCore() : this(null)
        {
        }

        public StyleCopCore(StyleCopEnvironment environment) : this(environment, null)
        {
        }

        public StyleCopCore(StyleCopEnvironment environment, object hostTag)
        {
            this.writeResultsCache = true;
            this.displayUI = true;
            this.parsers = new Dictionary<string, SourceParser>();
            this.analyzers = new Dictionary<string, SourceAnalyzer>();
            this.registry = new RegistryUtils();
            this.coreParser = new CoreParser();
            this.environment = environment;
            this.hostTag = hostTag;
            if (this.environment == null)
            {
                this.environment = new FileBasedEnvironment();
            }
            this.environment.Core = this;
            this.log = new Log(this);
            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.StyleCop.CoreParser.xml"))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string xml = reader.ReadToEnd();
                        XmlDocument initializationXml = new XmlDocument();
                        initializationXml.LoadXml(xml);
                        this.coreParser.Initialize(this, initializationXml, true, true);
                    }
                }
            }
            catch (XmlException)
            {
                AlertDialog.Show(this, null, Strings.StyleCopUnableToLoad, Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            catch (ArgumentException exception)
            {
                AlertDialog.Show(this, null, string.Format(CultureInfo.CurrentUICulture, Strings.StyleCopUnableToLoad, new object[] { exception.Message }), Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        internal void AddViolation(CodeElement element, Violation violation)
        {
            if ((element != null) && element.AddViolation(violation))
            {
                this.OnViolationEncountered(new ViolationEventArgs(violation));
            }
        }

        internal void AddViolation(SourceCode sourceCode, Violation violation)
        {
            bool flag = true;
            if ((sourceCode != null) && !sourceCode.AddViolation(violation))
            {
                flag = false;
            }
            if (flag)
            {
                this.OnViolationEncountered(new ViolationEventArgs(violation));
            }
        }

        internal void AddViolation(CodeElement element, Rule type, int line, params object[] values)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.CurrentUICulture, type.Context, values);
            Violation violation = new Violation(type, element, line, builder.ToString());
            this.AddViolation(element, violation);
        }

        internal void AddViolation(SourceCode sourceCode, Rule type, int line, params object[] values)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.CurrentUICulture, type.Context, values);
            Violation violation = new Violation(type, sourceCode, line, builder.ToString());
            this.AddViolation(sourceCode, violation);
        }

        public void Analyze(IList<CodeProject> projects)
        {
            Param.RequireNotNull(projects, "projects");
            this.Analyze(projects, false, null);
        }

        public void Analyze(IList<CodeProject> projects, string settingsFilePath)
        {
            Param.RequireNotNull(projects, "projects");
            Param.RequireValidString(settingsFilePath, "settingsFilePath");
            this.Analyze(projects, false, settingsFilePath);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification="Cannot allow exception from plug-in to kill VS or build")]
        private void Analyze(IList<CodeProject> projects, bool ignoreCache, string settingsPath)
        {
            lock (this)
            {
                this.analyzing = true;
                this.cancel = false;
            }
            int count = Math.Max(GetCpuCount(), 2);
            try
            {
                foreach (SourceParser parser in this.parsers.Values)
                {
                    parser.PreParse();
                    foreach (SourceAnalyzer analyzer in parser.Analyzers)
                    {
                        analyzer.EnabledRules = new Dictionary<CodeProject, Dictionary<string, Rule>>();
                        analyzer.PreAnalyze();
                    }
                }
                ResultsCache resultsCache = null;
                if (this.writeResultsCache)
                {
                    resultsCache = new ResultsCache(this);
                }
                StyleCopThread.Data data = new StyleCopThread.Data(this, projects, resultsCache, ignoreCache, settingsPath);
                foreach (CodeProject project in projects)
                {
                    InitializeProjectForAnalysis(project, data, resultsCache);
                }
                while (!this.Cancel)
                {
                    data.ResetEmumerator();
                    if (this.RunWorkerThreads(data, count))
                    {
                        break;
                    }
                    data.PassNumber++;
                }
                if (resultsCache != null)
                {
                    resultsCache.Flush();
                }
                foreach (SourceParser parser2 in this.parsers.Values)
                {
                    parser2.PostParse();
                }
                foreach (SourceParser parser3 in this.Parsers)
                {
                    foreach (SourceAnalyzer analyzer2 in parser3.Analyzers)
                    {
                        analyzer2.EnabledRules = null;
                        analyzer2.PostAnalyze();
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception exception)
            {
                this.coreParser.AddViolation(null, 1, Rules.ExceptionOccurred, new object[] { exception.GetType(), exception.Message });
            }
            finally
            {
                lock (this)
                {
                    this.analyzing = false;
                }
            }
        }

        internal static string CleanPath(string path)
        {
            string str = path;
            if (str == null)
            {
                return str;
            }
            while ((str.Length > 0) && (str[str.Length - 1] == '\\'))
            {
                str = str.Substring(0, str.Length - 1);
            }
            return str.ToUpperInvariant();
        }

        private static bool CompareCachedConfiguration(Configuration configuration, string flagList)
        {
            string[] strArray = new string[0];
            if (flagList.Trim().Length > 0)
            {
                strArray = flagList.Split(new char[] { ';' });
            }
            if (strArray.Length != configuration.Flags.Count)
            {
                return false;
            }
            foreach (string str2 in strArray)
            {
                if (!configuration.Contains(str2))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ComparePublicKeys(byte[] key1, byte[] key2)
        {
            if (key1.Length != key2.Length)
            {
                return false;
            }
            for (int i = 0; i < key1.Length; i++)
            {
                if (key1[i] != key2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void Dispose()
        {
            if (this.log != null)
            {
                this.log.Dispose();
            }
        }

        public void FullAnalyze(IList<CodeProject> projects)
        {
            Param.RequireNotNull(projects, "projects");
            this.Analyze(projects, true, null);
        }

        public void FullAnalyze(IList<CodeProject> projects, string settingsFilePath)
        {
            Param.RequireNotNull(projects, "projects");
            Param.RequireValidString(settingsFilePath, "settingsFilePath");
            this.Analyze(projects, true, settingsFilePath);
        }

        public StyleCopAddIn GetAddIn(string addInId)
        {
            SourceParser parser;
            SourceAnalyzer analyzer;
            Param.RequireValidString(addInId, "addInId");
            if (this.parsers.TryGetValue(addInId, out parser))
            {
                return parser;
            }
            if (this.analyzers.TryGetValue(addInId, out analyzer))
            {
                return analyzer;
            }
            return null;
        }

        public SourceAnalyzer GetAnalyzer(string analyzerId)
        {
            SourceAnalyzer analyzer;
            Param.RequireValidString(analyzerId, "analyzerId");
            if (this.analyzers.TryGetValue(analyzerId, out analyzer))
            {
                return analyzer;
            }
            return null;
        }

        private static int GetCpuCount()
        {
            int subKeyCount = 1;
            RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor", false);
            if (key != null)
            {
                if (key.SubKeyCount >= 1)
                {
                    subKeyCount = key.SubKeyCount;
                }
                key.Close();
            }
            return subKeyCount;
        }

        private static System.Type GetNextAddInAttributeType(System.Type addInType, System.Type attributeType, out object attribute)
        {
            attribute = null;
            while (addInType != null)
            {
                object[] customAttributes = addInType.GetCustomAttributes(attributeType, false);
                if ((customAttributes != null) && (customAttributes.Length > 0))
                {
                    attribute = customAttributes[0];
                    return addInType;
                }
                addInType = addInType.BaseType;
            }
            return addInType;
        }

        public SourceParser GetParser(string parserId)
        {
            SourceParser parser;
            Param.RequireValidString(parserId, "parserId");
            if (this.parsers.TryGetValue(parserId, out parser))
            {
                return parser;
            }
            return null;
        }

        internal static List<IPropertyControlPage> GetSettingsPages(StyleCopCore core)
        {
            List<IPropertyControlPage> list = new List<IPropertyControlPage>();
            foreach (SourceParser parser in core.Parsers)
            {
                ICollection<IPropertyControlPage> settingsPages = parser.SettingsPages;
                if ((settingsPages != null) && (settingsPages.Count > 0))
                {
                    list.AddRange(settingsPages);
                }
                foreach (SourceAnalyzer analyzer in parser.Analyzers)
                {
                    ICollection<IPropertyControlPage> collection = analyzer.SettingsPages;
                    if ((collection != null) && (collection.Count > 0))
                    {
                        list.AddRange(collection);
                    }
                }
            }
            return list;
        }

        public void Initialize(ICollection<string> addInPaths, bool loadFromDefaultPath)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            byte[] publicKeyToken = executingAssembly.GetName().GetPublicKeyToken();
            if (loadFromDefaultPath)
            {
                this.LoadAddins(Path.GetDirectoryName(executingAssembly.Location), publicKeyToken);
            }
            if ((addInPaths != null) && (addInPaths.Count > 0))
            {
                foreach (string str in addInPaths)
                {
                    try
                    {
                        if (Directory.Exists(str))
                        {
                            goto Label_005D;
                        }
                    }
                    catch (IOException)
                    {
                    }
                    catch (SecurityException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                    catch (ArgumentException)
                    {
                    }
                    continue;
                Label_005D:
                    this.LoadAddins(str, publicKeyToken);
                }
            }
            foreach (SourceAnalyzer analyzer in this.analyzers.Values)
            {
                SourceParser item = this.parsers[analyzer.ParserId];
                if (item != null)
                {
                    item.Analyzers.Add(analyzer);
                    analyzer.SetParser(item);
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification="This is a false violation.")]
        private StyleCopAddIn InitializeAddIn(System.Type addInType, bool isKnownAssembly)
        {
            object obj2;
            System.Type type = GetNextAddInAttributeType(addInType, typeof(StyleCopAddInAttribute), out obj2);
            if (type == null)
            {
                Log.Write("The type {0} does not contain the StyleCopAddInAttribute", new string[] { addInType.Name });
                return null;
            }
            Log.Write("Creating an instance of type {0}", new string[] { addInType.Name });
            StyleCopAddIn @in = (StyleCopAddIn) Activator.CreateInstance(addInType);
            StyleCopAddInAttribute attribute = (StyleCopAddInAttribute) obj2;
            XmlDocument initializationXml = LoadAddInResourceXml(addInType, attribute.AddInXmlId);
            if (initializationXml == null)
            {
                Log.Write("Failed to load addin initialization Xml", new string[0]);
                return null;
            }
            @in.Initialize(this, initializationXml, true, isKnownAssembly);
            while (true)
            {
                do
                {
                    type = GetNextAddInAttributeType(type.BaseType, typeof(StyleCopAddInAttribute), out obj2);
                    if (type == null)
                    {
                        goto Label_00D7;
                    }
                    attribute = (StyleCopAddInAttribute) obj2;
                    initializationXml = LoadAddInResourceXml(type, attribute.AddInXmlId);
                }
                while (initializationXml == null);
                @in.Initialize(this, initializationXml, false, isKnownAssembly);
            }
        Label_00D7:
            @in.InitializeAddIn();
            foreach (Rule rule in @in.AddInRules)
            {
                PropertyDescriptor<bool> descriptor = new PropertyDescriptor<bool>(rule.Name + "#Enabled", PropertyType.Boolean, string.Empty, string.Empty, true, false, rule.EnabledByDefault);
                @in.PropertyDescriptors.AddPropertyDescriptor(descriptor);
            }
            return @in;
        }

        private static void InitializeProjectForAnalysis(CodeProject project, StyleCopThread.Data data, ResultsCache cache)
        {
            ProjectStatus projectStatus = data.GetProjectStatus(project);
            if (!project.SettingsLoaded)
            {
                project.Settings = data.GetSettings(project);
                project.SettingsLoaded = true;
            }
            string flagList = (cache == null) ? null : cache.LoadProject(project);
            if (flagList == null)
            {
                projectStatus.IgnoreResultsCache = true;
            }
            else
            {
                projectStatus.IgnoreResultsCache = !CompareCachedConfiguration(project.Configuration, flagList);
            }
            if ((cache != null) && project.WriteCache)
            {
                cache.SaveProject(project);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId="System.Xml.XmlNode", Justification="Compliance would break well-defined API.")]
        public static XmlDocument LoadAddInResourceXml(System.Type addInType, string resourceId)
        {
            XmlDocument document2;
            Param.RequireNotNull(addInType, "addInType");
            if (resourceId == null)
            {
                resourceId = StyleCopAddIn.GetIdFromAddInType(addInType) + ".xml";
            }
            Log.Write("Attempting to load addin initialization Xml from path {0}", new string[] { resourceId });
            if (addInType.Assembly.GetManifestResourceInfo(resourceId) == null)
            {
                Log.Write("Could not find addin initialization Xml", new string[0]);
                return null;
            }
            using (Stream stream = addInType.Assembly.GetManifestResourceStream(resourceId))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string xml = reader.ReadToEnd();
                    if ((xml == null) || (xml.Length == 0))
                    {
                        throw new ArgumentException(Strings.InvalidAddInXmlDocument);
                    }
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(xml);
                    document2 = document;
                }
            }
            return document2;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId="System.Reflection.Assembly.LoadFrom", Justification="No alternative is provided.")]
        private void LoadAddins(string path, byte[] publicKey)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Log.Write("Loading addins from path {0}", new string[] { path });
                    foreach (string str in Directory.GetFiles(path, "*.dll"))
                    {
                        if (!str.EndsWith(@"\microsoft.stylecop.dll", StringComparison.OrdinalIgnoreCase) && !str.EndsWith(@"\microsoft.stylecop.vspackage.dll", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                Log.Write("Loading assembly {0}", new string[] { str });
                                Assembly assembly = Assembly.LoadFrom(str);
                                assembly.GetCustomAttributes(true);
                                System.Type[] exportedTypes = assembly.GetExportedTypes();
                                byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
                                bool isKnownAssembly = ComparePublicKeys(publicKey, publicKeyToken);
                                foreach (System.Type type in exportedTypes)
                                {
                                    if (type.IsSubclassOf(typeof(SourceAnalyzer)))
                                    {
                                        Log.Write("Discovered SourceAnalyzer type {0}", new string[] { type.Name });
                                        SourceAnalyzer analyzer = this.InitializeAddIn(type, isKnownAssembly) as SourceAnalyzer;
                                        if ((analyzer != null) && !this.analyzers.ContainsKey(analyzer.Id))
                                        {
                                            this.analyzers.Add(analyzer.Id, analyzer);
                                        }
                                    }
                                    else if (type.IsSubclassOf(typeof(SourceParser)))
                                    {
                                        Log.Write("Discovered SourceParser type {0}", new string[] { type.Name });
                                        SourceParser parser = this.InitializeAddIn(type, isKnownAssembly) as SourceParser;
                                        if ((parser != null) && !this.parsers.ContainsKey(parser.Id))
                                        {
                                            this.parsers.Add(parser.Id, parser);
                                            this.environment.AddParser(parser);
                                        }
                                    }
                                }
                            }
                            catch (BadImageFormatException)
                            {
                            }
                            catch (ThreadAbortException)
                            {
                            }
                            catch (OutOfMemoryException)
                            {
                                throw;
                            }
                            catch (Exception exception)
                            {
                                AlertDialog.Show(this, null, string.Format(CultureInfo.CurrentUICulture, Strings.ExceptionWhileLoadingAddins, new object[] { exception.GetType(), exception.Message }), Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                throw;
                            }
                        }
                    }
                    foreach (string str2 in Directory.GetDirectories(path, "*.*"))
                    {
                        this.LoadAddins(str2, publicKey);
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        internal static string MakeAbsolutePath(string rootFolder, string relativePath)
        {
            string str = rootFolder.Substring(0, rootFolder.Length);
            int startIndex = 0;
        Label_0010:
            if ((relativePath.Length - startIndex) >= 3)
            {
                if ((relativePath[startIndex] == '.') && (relativePath[startIndex + 1] == '\\'))
                {
                    startIndex += 2;
                    goto Label_0010;
                }
                if (relativePath[startIndex] == '\\')
                {
                    startIndex++;
                    goto Label_0010;
                }
                if (((relativePath[startIndex] == '.') && (relativePath[startIndex + 1] == '.')) && (relativePath[startIndex + 2] == '\\'))
                {
                    startIndex += 3;
                    while ((str.Length > 0) && (str[str.Length - 1] == '\\'))
                    {
                        str = str.Substring(0, str.Length - 1);
                    }
                    int num2 = str.LastIndexOf(@"\", StringComparison.Ordinal);
                    if (num2 == -1)
                    {
                        return relativePath;
                    }
                    str = str.Substring(0, num2 + 1);
                    goto Label_0010;
                }
            }
            return Path.Combine(str, relativePath.Substring(startIndex, relativePath.Length - startIndex));
        }

        private void OnAddSettingsPages(AddSettingsPagesEventArgs e)
        {
            if (this.AddSettingsPages != null)
            {
                this.AddSettingsPages(this, e);
            }
        }

        private void OnOutputGenerated(OutputEventArgs e)
        {
            if (this.OutputGenerated != null)
            {
                this.OutputGenerated(this, e);
            }
        }

        private void OnProjectSettingsChanged(EventArgs e)
        {
            if (this.ProjectSettingsChanged != null)
            {
                this.ProjectSettingsChanged(this, e);
            }
        }

        private void OnViolationEncountered(ViolationEventArgs e)
        {
            if (this.ViolationEncountered != null)
            {
                this.ViolationEncountered(this, e);
            }
        }

        private bool RunWorkerThreads(StyleCopThread.Data data, int count)
        {
            bool flag = true;
            BackgroundWorker[] workerArray = new BackgroundWorker[count];
            StyleCopThread[] threadArray = new StyleCopThread[count];
            for (int i = 0; i < count; i++)
            {
                workerArray[i] = new BackgroundWorker();
                threadArray[i] = new StyleCopThread(data);
                workerArray[i].DoWork += new DoWorkEventHandler(threadArray[i].DoWork);
                threadArray[i].ThreadCompleted += new EventHandler<StyleCopThreadCompletedEventArgs>(this.StyleCopThreadCompleted);
                data.IncrementThreadCount();
            }
            lock (this)
            {
                for (int k = 0; k < count; k++)
                {
                    workerArray[k].RunWorkerAsync();
                }
                Monitor.Wait(this);
            }
            for (int j = 0; j < count; j++)
            {
                workerArray[j].Dispose();
                if (!threadArray[j].Complete)
                {
                    flag = false;
                }
            }
            return flag;
        }

        internal bool ShowProjectSettings(WritableSettings settings, IList<IPropertyControlPage> pages, string caption, string id)
        {
            PropertyDialog dialog = new PropertyDialog(pages, settings, id, this, null, new object[0]);
            dialog.Text = caption;
            if (!this.displayUI)
            {
                throw new InvalidOperationException(Strings.CannotDisplaySettingsInNonUIMode);
            }
            dialog.ShowDialog();
            this.OnProjectSettingsChanged(new EventArgs());
            return dialog.SettingsChanged;
        }

        internal bool ShowProjectSettings(string settingsPath, IList<IPropertyControlPage> pages, string caption, string id, bool defaultSettings)
        {
            Exception exception = null;
            WritableSettings writableSettings = this.environment.GetWritableSettings(settingsPath, out exception);
            if (exception != null)
            {
                AlertDialog.Show(this, null, string.Format(CultureInfo.CurrentUICulture, Strings.ProjectSettingsFileNotLoadedOrCreated, new object[] { exception.Message }), Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (writableSettings != null)
            {
                writableSettings.DefaultSettings = defaultSettings;
                return this.ShowProjectSettings(writableSettings, pages, caption, id);
            }
            return false;
        }

        public bool ShowSettings(string settingsFilePath)
        {
            return this.ShowSettings(settingsFilePath, "StyleCopLocalProperties");
        }

        public bool ShowSettings(string settingsPath, string id)
        {
            Param.RequireValidString(settingsPath, "settingsPath");
            Param.RequireValidString(id, "id");
            return this.ShowSettings(settingsPath, id, false);
        }

        internal bool ShowSettings(string settingsPath, string id, bool defaultSettings)
        {
            List<IPropertyControlPage> settingsPages = GetSettingsPages(this);
            settingsPages.Insert(0, new AnalyzersOptions());
            settingsPages.Insert(1, new GlobalSettingsFileOptions());
            settingsPages.Insert(2, new CacheOptions());
            AddSettingsPagesEventArgs e = new AddSettingsPagesEventArgs(settingsPath);
            this.OnAddSettingsPages(e);
            foreach (IPropertyControlPage page in e.Pages)
            {
                settingsPages.Add(page);
            }
            string caption = defaultSettings ? Strings.DefaultSettingsDialogTitle : Strings.LocalSettingsDialogTitle;
            return this.ShowProjectSettings(settingsPath, settingsPages.AsReadOnly(), caption, id, defaultSettings);
        }

        internal void SignalOutput(string output)
        {
            this.SignalOutput(MessageImportance.Normal, output);
        }

        internal void SignalOutput(MessageImportance importance, string output)
        {
            this.OnOutputGenerated(new OutputEventArgs(output, importance));
        }

        private void StyleCopThreadCompleted(object sender, StyleCopThreadCompletedEventArgs e)
        {
            lock (this)
            {
                if (e.Data.DecrementThreadCount() == 0)
                {
                    Monitor.Pulse(this);
                }
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Addins", Justification="API has already been published and should not be changed.")]
        public bool AddinsDisabledByDefault
        {
            get
            {
                return this.addinsDisabledByDefault;
            }
            set
            {
                this.addinsDisabledByDefault = value;
            }
        }

        public bool Analyzing
        {
            get
            {
                return this.analyzing;
            }
        }

        public bool Cancel
        {
            get
            {
                lock (this)
                {
                    return this.cancel;
                }
            }
            set
            {
                lock (this)
                {
                    this.cancel = value;
                }
            }
        }

        internal CoreParser CoreViolations
        {
            get
            {
                return this.coreParser;
            }
        }

        public bool DisplayUI
        {
            get
            {
                return this.displayUI;
            }
            set
            {
                this.displayUI = value;
            }
        }

        public StyleCopEnvironment Environment
        {
            get
            {
                return this.environment;
            }
        }

        public object HostTag
        {
            get
            {
                return this.hostTag;
            }
        }

        public ICollection<SourceParser> Parsers
        {
            get
            {
                return this.parsers.Values;
            }
        }

        public Microsoft.StyleCop.PropertyDescriptorCollection PropertyDescriptors
        {
            get
            {
                return this.coreParser.PropertyDescriptors;
            }
        }

        public RegistryUtils Registry
        {
            get
            {
                return this.registry;
            }
        }

        public bool WriteResultsCache
        {
            get
            {
                return this.writeResultsCache;
            }
            set
            {
                this.writeResultsCache = value;
            }
        }
    }
}

