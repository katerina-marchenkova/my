namespace Microsoft.StyleCop
{
    using Microsoft.Build.Framework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class StyleCopThread
    {
        private bool complete;
        private Data data;

        public event EventHandler<StyleCopThreadCompletedEventArgs> ThreadCompleted;

        public StyleCopThread(Data data)
        {
            this.data = data;
        }

        private static ICollection<SourceAnalyzer> DiscoverAnalyzerList(StyleCopCore core, CodeProject project, ICollection<SourceParser> parsers)
        {
            List<SourceAnalyzer> list = new List<SourceAnalyzer>();
            foreach (SourceParser parser in parsers)
            {
                foreach (SourceAnalyzer analyzer in parser.Analyzers)
                {
                    Dictionary<string, Rule> dictionary = new Dictionary<string, Rule>();
                    AddInPropertyCollection propertys = (project.Settings == null) ? null : project.Settings.GetAddInSettings(analyzer);
                    foreach (Rule rule in analyzer.AddInRules)
                    {
                        bool flag = !core.AddinsDisabledByDefault && rule.EnabledByDefault;
                        if ((propertys != null) && (!flag || rule.CanDisable))
                        {
                            BooleanProperty property = propertys[rule.Name + "#Enabled"] as BooleanProperty;
                            if (property != null)
                            {
                                flag = property.Value;
                            }
                        }
                        if (flag)
                        {
                            dictionary.Add(rule.Name, rule);
                        }
                    }
                    if (dictionary.Count > 0)
                    {
                        list.Add(analyzer);
                        analyzer.EnabledRules.Add(project, dictionary);
                    }
                }
            }
            return list;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification="Cannot allow exception from plug-in to kill VS or build")]
        public void DoWork(object sender, DoWorkEventArgs e)
        {
            this.complete = true;
            SourceCode sourceCode = null;
            try
            {
                while (!this.data.Core.Cancel)
                {
                    DocumentAnalysisStatus documentStatus = null;
                    lock (this.data)
                    {
                        sourceCode = this.data.GetNextSourceCodeDocument();
                        if (sourceCode == null)
                        {
                            return;
                        }
                        documentStatus = this.data.GetDocumentStatus(sourceCode);
                    }
                    if (!documentStatus.Initialized)
                    {
                        if (!sourceCode.Exists || this.LoadSourceCodeFromResultsCache(sourceCode))
                        {
                            documentStatus.Complete = true;
                        }
                        documentStatus.Initialized = true;
                    }
                    if (!documentStatus.Complete)
                    {
                        this.ParseAndAnalyzeDocument(sourceCode, documentStatus);
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
                Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Exception occurred: {0}, {1}", new object[] { exception.GetType(), exception.Message }));
                this.data.Core.CoreViolations.AddViolation(sourceCode, 1, Rules.ExceptionOccurred, new object[] { exception.GetType(), exception.Message });
            }
            finally
            {
                e.Result = this.data;
                if (this.ThreadCompleted != null)
                {
                    this.ThreadCompleted(this, new StyleCopThreadCompletedEventArgs(this.data));
                }
            }
        }

        private ICollection<SourceAnalyzer> GetAnalyzersForProjectFile(CodeProject project, SourceCode sourceCode, ICollection<SourceParser> parsers)
        {
            if (!string.IsNullOrEmpty(sourceCode.Type))
            {
                ProjectStatus projectStatus = this.data.GetProjectStatus(project);
                lock (projectStatus.AnalyzerLists)
                {
                    ICollection<SourceAnalyzer> is2 = null;
                    if (!projectStatus.AnalyzerLists.TryGetValue(sourceCode.Type, out is2))
                    {
                        is2 = DiscoverAnalyzerList(this.data.Core, project, parsers);
                        projectStatus.AnalyzerLists.Add(sourceCode.Type, is2);
                        foreach (SourceAnalyzer analyzer in is2)
                        {
                            this.data.Core.SignalOutput(MessageImportance.Low, string.Format(CultureInfo.CurrentCulture, "Loaded Analyzer: {0}...", new object[] { analyzer.Name }));
                        }
                    }
                    return is2;
                }
            }
            return null;
        }

        private bool LoadSourceCodeFromResultsCache(SourceCode sourceCode)
        {
            if ((!this.data.IgnoreResultsCache && this.data.Core.Environment.SupportsResultsCache) && ((this.data.ResultsCache != null) && !this.data.GetProjectStatus(sourceCode.Project).IgnoreResultsCache))
            {
                DateTime timeStamp = sourceCode.TimeStamp;
                if (this.data.ResultsCache.LoadResults(sourceCode, sourceCode.Parser, timeStamp, sourceCode.Project.Settings.WriteTime))
                {
                    return true;
                }
            }
            return false;
        }

        private void ParseAndAnalyzeDocument(SourceCode sourceCode, DocumentAnalysisStatus documentStatus)
        {
            bool flag;
            this.data.Core.SignalOutput(MessageImportance.Low, string.Format(CultureInfo.CurrentCulture, "Pass {0}: {1}...\n", new object[] { this.data.PassNumber + 1, sourceCode.Name }));
            CodeDocument document = documentStatus.Document;
            ICollection<SourceAnalyzer> analyzers = this.GetAnalyzersForProjectFile(sourceCode.Project, sourceCode, this.data.Core.Parsers);
            try
            {
                flag = !sourceCode.Parser.ParseFile(sourceCode, this.data.PassNumber, ref document);
            }
            catch (Exception)
            {
                string output = string.Format(CultureInfo.CurrentCulture, "Exception thrown by parser '{0}' while processing '{1}'.", new object[] { sourceCode.Parser.Name, sourceCode.Path });
                this.data.Core.SignalOutput(MessageImportance.High, output);
                throw;
            }
            if (flag)
            {
                if (document == null)
                {
                    documentStatus.Complete = true;
                }
                else if (this.TestAndRunAnalyzers(document, sourceCode.Parser, analyzers, this.data.PassNumber))
                {
                    documentStatus.Complete = true;
                    if (document != null)
                    {
                        if ((this.data.ResultsCache != null) && sourceCode.Project.WriteCache)
                        {
                            this.data.ResultsCache.SaveDocumentResults(document, sourceCode.Parser, sourceCode.Project.Settings.WriteTime);
                        }
                        document.Dispose();
                        document = null;
                    }
                }
            }
            if (!documentStatus.Complete)
            {
                this.complete = false;
                if (document != null)
                {
                    documentStatus.Document = document;
                }
            }
        }

        private void RunAnalyzers(CodeDocument document, SourceParser parser, ICollection<SourceAnalyzer> analyzers)
        {
            if (analyzers != null)
            {
                if (parser.SkipAnalysisForDocument(document))
                {
                    this.data.Core.SignalOutput(MessageImportance.Normal, string.Format(CultureInfo.CurrentCulture, "Skipping {0}...", new object[] { document.SourceCode.Name }));
                }
                else
                {
                    foreach (SourceAnalyzer analyzer in analyzers)
                    {
                        if (this.data.Core.Cancel)
                        {
                            return;
                        }
                        SourceParser.ClearAnalyzerTags(document);
                        try
                        {
                            analyzer.AnalyzeDocument(document);
                            continue;
                        }
                        catch (Exception)
                        {
                            string output = string.Format(CultureInfo.CurrentCulture, "Exception thrown by analyzer '{0}' while processing '{1}'.", new object[] { analyzer.Name, document.SourceCode.Path });
                            this.data.Core.SignalOutput(MessageImportance.High, output);
                            throw;
                        }
                    }
                }
            }
        }

        private bool TestAndRunAnalyzers(CodeDocument document, SourceParser parser, ICollection<SourceAnalyzer> analyzers, int passNumber)
        {
            if (analyzers == null)
            {
                return true;
            }
            bool flag = false;
            foreach (SourceAnalyzer analyzer in analyzers)
            {
                if (analyzer.DelayAnalysis(document, passNumber))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                this.RunAnalyzers(document, parser, analyzers);
            }
            return !flag;
        }

        public bool Complete
        {
            get
            {
                return this.complete;
            }
        }

        public class Data
        {
            private Microsoft.StyleCop.ResultsCache cache;
            private StyleCopCore core;
            private bool ignoreResultsCache;
            private int passNumber;
            private int projectIndex;
            private IList<CodeProject> projects;
            private Dictionary<CodeProject, ProjectStatus> projectStatus = new Dictionary<CodeProject, ProjectStatus>();
            private Dictionary<int, Settings> settings;
            private string settingsPath;
            private int sourceCodeInstanceIndex = -1;
            private Dictionary<SourceCode, DocumentAnalysisStatus> sourceCodeInstanceStatus = new Dictionary<SourceCode, DocumentAnalysisStatus>();
            private int threads;

            public Data(StyleCopCore core, IList<CodeProject> codeProjects, Microsoft.StyleCop.ResultsCache resultsCache, bool ignoreResultsCache, string settingsPath)
            {
                this.core = core;
                this.projects = codeProjects;
                this.cache = resultsCache;
                this.ignoreResultsCache = ignoreResultsCache;
                this.settingsPath = settingsPath;
            }

            public int DecrementThreadCount()
            {
                return --this.threads;
            }

            public DocumentAnalysisStatus GetDocumentStatus(SourceCode sourceCode)
            {
                DocumentAnalysisStatus status;
                if (!this.sourceCodeInstanceStatus.TryGetValue(sourceCode, out status))
                {
                    status = new DocumentAnalysisStatus();
                    this.sourceCodeInstanceStatus.Add(sourceCode, status);
                }
                return status;
            }

            public SourceCode GetNextSourceCodeDocument()
            {
                while (this.projectIndex < this.projects.Count)
                {
                    CodeProject project = this.projects[this.projectIndex];
                    this.sourceCodeInstanceIndex++;
                    if (this.sourceCodeInstanceIndex >= project.SourceCodeInstances.Count)
                    {
                        this.projectIndex++;
                        this.sourceCodeInstanceIndex = -1;
                    }
                    else
                    {
                        return project.SourceCodeInstances[this.sourceCodeInstanceIndex];
                    }
                }
                return null;
            }

            public ProjectStatus GetProjectStatus(CodeProject project)
            {
                ProjectStatus status;
                if (!this.projectStatus.TryGetValue(project, out status))
                {
                    status = new ProjectStatus();
                    this.projectStatus.Add(project, status);
                }
                return status;
            }

            public Settings GetSettings(CodeProject project)
            {
                int key = project.Key;
                if (this.settingsPath != null)
                {
                    key = this.settingsPath.GetHashCode();
                }
                Settings projectSettings = null;
                if (this.settings != null)
                {
                    this.settings.TryGetValue(key, out projectSettings);
                }
                if (projectSettings == null)
                {
                    if (this.settingsPath != null)
                    {
                        projectSettings = this.core.Environment.GetSettings(this.settingsPath, true);
                    }
                    else
                    {
                        projectSettings = this.core.Environment.GetProjectSettings(project, true);
                    }
                    if (projectSettings == null)
                    {
                        return projectSettings;
                    }
                    if (this.settings == null)
                    {
                        this.settings = new Dictionary<int, Settings>();
                    }
                    this.settings.Add(key, projectSettings);
                }
                return projectSettings;
            }

            public int IncrementThreadCount()
            {
                return ++this.threads;
            }

            public void ResetEmumerator()
            {
                this.sourceCodeInstanceIndex = -1;
                this.projectIndex = 0;
            }

            public StyleCopCore Core
            {
                get
                {
                    return this.core;
                }
            }

            public bool IgnoreResultsCache
            {
                get
                {
                    return this.ignoreResultsCache;
                }
            }

            public int PassNumber
            {
                get
                {
                    return this.passNumber;
                }
                set
                {
                    Param.RequireGreaterThanOrEqualToZero(value, "PassNumber");
                    this.passNumber = value;
                }
            }

            public Microsoft.StyleCop.ResultsCache ResultsCache
            {
                get
                {
                    return this.cache;
                }
            }
        }
    }
}

