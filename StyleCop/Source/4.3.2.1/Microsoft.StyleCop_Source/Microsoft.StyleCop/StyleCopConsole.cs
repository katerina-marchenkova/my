namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Xml;

    public sealed class StyleCopConsole : IDisposable
    {
        private StyleCopCore core;
        private string outputFile;
        private string settingsPath;
        private int violationCount;
        private XmlDocument violations;

        public event EventHandler<OutputEventArgs> OutputGenerated;

        public event EventHandler<ViolationEventArgs> ViolationEncountered;

        public StyleCopConsole(string settings, bool writeResultsCache, string outputFile, ICollection<string> addInPaths, bool loadFromDefaultPath) : this(settings, writeResultsCache, outputFile, addInPaths, loadFromDefaultPath, null)
        {
        }

        public StyleCopConsole(string settings, bool writeResultsCache, string outputFile, ICollection<string> addInPaths, bool loadFromDefaultPath, object hostTag)
        {
            this.violations = new XmlDocument();
            this.settingsPath = settings;
            if (outputFile == null)
            {
                this.outputFile = "StyleCopViolations.xml";
            }
            else
            {
                this.outputFile = outputFile;
            }
            this.core = new StyleCopCore(null, hostTag);
            this.core.Initialize(addInPaths, loadFromDefaultPath);
            this.core.WriteResultsCache = writeResultsCache;
            this.core.DisplayUI = false;
            this.core.ViolationEncountered += new EventHandler<ViolationEventArgs>(this.CoreViolationEncountered);
            this.core.OutputGenerated += new EventHandler<OutputEventArgs>(this.CoreOutputGenerated);
            XmlElement newChild = this.violations.CreateElement("StyleCopViolations");
            this.violations.AppendChild(newChild);
        }

        private void CoreOutputGenerated(object sender, OutputEventArgs e)
        {
            lock (this)
            {
                this.OnOutputGenerated(new OutputEventArgs(e.Output, e.Importance));
            }
        }

        private void CoreViolationEncountered(object sender, ViolationEventArgs e)
        {
            lock (this)
            {
                XmlElement newChild = this.violations.CreateElement("Violation");
                XmlAttribute node = null;
                if (e.Element != null)
                {
                    node = this.violations.CreateAttribute("Section");
                    node.Value = CreateSafeSectionName(e.Element.FullyQualifiedName);
                    newChild.Attributes.Append(node);
                }
                node = this.violations.CreateAttribute("LineNumber");
                node.Value = e.LineNumber.ToString(CultureInfo.InvariantCulture);
                newChild.Attributes.Append(node);
                SourceCode sourceCode = e.SourceCode;
                if (((sourceCode == null) && (e.Element != null)) && (e.Element.Document != null))
                {
                    sourceCode = e.Element.Document.SourceCode;
                }
                if (sourceCode != null)
                {
                    node = this.violations.CreateAttribute("Source");
                    node.Value = sourceCode.Path;
                    newChild.Attributes.Append(node);
                }
                node = this.violations.CreateAttribute("RuleNamespace");
                node.Value = e.Violation.Rule.Namespace;
                newChild.Attributes.Append(node);
                node = this.violations.CreateAttribute("Rule");
                node.Value = e.Violation.Rule.Name;
                newChild.Attributes.Append(node);
                node = this.violations.CreateAttribute("RuleId");
                node.Value = e.Violation.Rule.CheckId;
                newChild.Attributes.Append(node);
                newChild.InnerText = e.Message;
                this.violations.DocumentElement.AppendChild(newChild);
                this.violationCount++;
            }
            this.OnViolationEncountered(new ViolationEventArgs(e.Violation));
        }

        private static string CreateSafeSectionName(string originalName)
        {
            if (originalName == null)
            {
                return null;
            }
            if (originalName.IndexOf('<') == -1)
            {
                return originalName;
            }
            StringBuilder builder = new StringBuilder(originalName.Length * 2);
            int num2 = 0;
            for (int i = 0; i < originalName.Length; i++)
            {
                char c = originalName[i];
                if (c == '<')
                {
                    num2++;
                    builder.Append('`');
                }
                else if (num2 > 0)
                {
                    switch (c)
                    {
                        case '>':
                            num2--;
                            break;

                        case ',':
                            builder.Append('`');
                            break;

                        default:
                            if (!char.IsWhiteSpace(c))
                            {
                                builder.Append(c);
                            }
                            break;
                    }
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (this.core != null)
            {
                this.core.Dispose();
            }
        }

        private void LoadSettingsFiles(IList<CodeProject> projects)
        {
            Settings mergedSettings = null;
            Settings localSettings = null;
            if (this.settingsPath != null)
            {
                localSettings = this.core.Environment.GetSettings(this.settingsPath, false);
                if (localSettings != null)
                {
                    SettingsMerger merger = new SettingsMerger(localSettings, this.core.Environment);
                    mergedSettings = merger.MergedSettings;
                }
            }
            foreach (CodeProject project in projects)
            {
                Settings projectSettings = mergedSettings;
                if (projectSettings == null)
                {
                    projectSettings = this.core.Environment.GetProjectSettings(project, true);
                }
                if (projectSettings != null)
                {
                    project.Settings = projectSettings;
                    project.SettingsLoaded = true;
                }
            }
        }

        private void OnOutputGenerated(OutputEventArgs e)
        {
            if (this.OutputGenerated != null)
            {
                this.OutputGenerated(this, e);
            }
        }

        private void OnViolationEncountered(ViolationEventArgs e)
        {
            if (this.ViolationEncountered != null)
            {
                this.ViolationEncountered(this, e);
            }
        }

        public bool Start(IList<CodeProject> projects, bool fullAnalyze)
        {
            Param.RequireNotNull(projects, "projects");
            bool flag = false;
            try
            {
                Exception exception;
                this.LoadSettingsFiles(projects);
                this.violationCount = 0;
                if (!string.IsNullOrEmpty(this.outputFile))
                {
                    this.core.Environment.RemoveAnalysisResults(this.outputFile);
                }
                if (fullAnalyze)
                {
                    this.core.FullAnalyze(projects);
                }
                else
                {
                    this.core.Analyze(projects);
                }
                if (this.violationCount > 0)
                {
                    this.OnOutputGenerated(new OutputEventArgs(string.Format(CultureInfo.CurrentCulture, Strings.ViolationsEncountered, new object[] { this.violationCount })));
                }
                else
                {
                    this.OnOutputGenerated(new OutputEventArgs(Strings.NoViolationsEncountered));
                }
                if (!this.core.Environment.SaveAnalysisResults(this.outputFile, this.violations, out exception))
                {
                    string text = (exception == null) ? Strings.CouldNotSaveViolationsFile : string.Format(CultureInfo.CurrentCulture, Strings.CouldNotSaveViolationsFileWithException, new object[] { exception.Message });
                    this.OnOutputGenerated(new OutputEventArgs(text));
                    flag = true;
                }
            }
            catch (IOException exception2)
            {
                this.OnOutputGenerated(new OutputEventArgs(string.Format(CultureInfo.CurrentCulture, Strings.AnalysisErrorOccurred, new object[] { exception2.Message })));
                flag = true;
            }
            catch (XmlException exception3)
            {
                this.OnOutputGenerated(new OutputEventArgs(string.Format(CultureInfo.CurrentCulture, Strings.AnalysisErrorOccurred, new object[] { exception3.Message })));
                flag = true;
            }
            catch (SecurityException exception4)
            {
                this.OnOutputGenerated(new OutputEventArgs(string.Format(CultureInfo.CurrentCulture, Strings.AnalysisErrorOccurred, new object[] { exception4.Message })));
                flag = true;
            }
            catch (UnauthorizedAccessException exception5)
            {
                this.OnOutputGenerated(new OutputEventArgs(string.Format(CultureInfo.CurrentCulture, Strings.AnalysisErrorOccurred, new object[] { exception5.Message })));
                flag = true;
            }
            return !flag;
        }

        public StyleCopCore Core
        {
            get
            {
                return this.core;
            }
        }
    }
}

