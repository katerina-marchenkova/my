namespace Microsoft.StyleCop
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public sealed class StyleCopTask : Task
    {
        private const int DefaultViolationLimit = 0x2710;
        private ITaskItem[] inputAdditionalAddinPaths = new ITaskItem[0];
        private bool inputCacheResults;
        private string[] inputDefineConstants = new string[0];
        private bool inputForceFullAnalysis;
        private ITaskItem inputOverrideSettingsFile;
        private ITaskItem inputProjectFullPath;
        private ITaskItem[] inputSourceFiles = new ITaskItem[0];
        private bool inputTreatErrorsAsWarnings;
        private ITaskItem maxViolationCount;
        private const string MSBuildErrorCode = null;
        private const string MSBuildSubCategory = null;
        private ITaskItem outputFile;
        private bool succeeded = true;
        private int violationCount;
        private int violationLimit;

        public override bool Execute()
        {
            this.violationCount = 0;
            this.violationLimit = 0;
            if ((this.maxViolationCount != null) && !int.TryParse(this.maxViolationCount.ItemSpec, out this.violationLimit))
            {
                this.violationLimit = 0;
            }
            if (this.violationLimit == 0)
            {
                this.violationLimit = 0x2710;
            }
            string settings = null;
            if ((this.inputOverrideSettingsFile != null) && (this.inputOverrideSettingsFile.ItemSpec.Length > 0))
            {
                settings = this.inputOverrideSettingsFile.ItemSpec;
            }
            List<string> addInPaths = new List<string>();
            foreach (ITaskItem item in this.inputAdditionalAddinPaths)
            {
                addInPaths.Add(item.GetMetadata("FullPath"));
            }
            StyleCopConsole console = new StyleCopConsole(settings, this.inputCacheResults, (this.outputFile == null) ? null : this.outputFile.ItemSpec, addInPaths, true);
            Configuration configuration = new Configuration(this.inputDefineConstants);
            string metadata = null;
            if (this.inputProjectFullPath != null)
            {
                metadata = this.inputProjectFullPath.GetMetadata("FullPath");
            }
            if (!string.IsNullOrEmpty(metadata))
            {
                CodeProject project = new CodeProject(metadata.GetHashCode(), metadata, configuration);
                foreach (ITaskItem item2 in this.inputSourceFiles)
                {
                    console.Core.Environment.AddSourceCode(project, item2.ItemSpec, null);
                }
                try
                {
                    console.OutputGenerated += new EventHandler<OutputEventArgs>(this.OnOutputGenerated);
                    console.ViolationEncountered += new EventHandler<ViolationEventArgs>(this.OnViolationEncountered);
                    CodeProject[] projects = new CodeProject[] { project };
                    console.Start(projects, this.inputForceFullAnalysis);
                }
                finally
                {
                    console.OutputGenerated -= new EventHandler<OutputEventArgs>(this.OnOutputGenerated);
                    console.ViolationEncountered -= new EventHandler<ViolationEventArgs>(this.OnViolationEncountered);
                }
            }
            return this.succeeded;
        }

        private void OnOutputGenerated(object sender, OutputEventArgs e)
        {
            lock (this)
            {
                base.Log.LogMessage(e.Importance, e.Output.Trim(), new object[0]);
            }
        }

        private void OnViolationEncountered(object sender, ViolationEventArgs e)
        {
            if ((this.violationLimit < 0) || (this.violationCount < this.violationLimit))
            {
                this.violationCount++;
                if (!e.Warning && !this.inputTreatErrorsAsWarnings)
                {
                    this.succeeded = false;
                }
                string file = string.Empty;
                if (((e.SourceCode != null) && (e.SourceCode.Path != null)) && (e.SourceCode.Path.Length > 0))
                {
                    file = e.SourceCode.Path;
                }
                else if (((e.Element != null) && (e.Element.Document != null)) && ((e.Element.Document.SourceCode != null) && (e.Element.Document.SourceCode.Path != null)))
                {
                    file = e.Element.Document.SourceCode.Path;
                }
                string message = e.Violation.Rule.CheckId + ": " + e.Message;
                lock (this)
                {
                    if (e.Warning || this.inputTreatErrorsAsWarnings)
                    {
                        base.Log.LogWarning(null, null, null, file, e.LineNumber, 1, 0, 0, message, new object[0]);
                    }
                    else
                    {
                        base.Log.LogError(null, null, null, file, e.LineNumber, 1, 0, 0, message, new object[0]);
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Addin", Justification="API has already been published and should not be changed.")]
        public ITaskItem[] AdditionalAddinPaths
        {
            get
            {
                return this.inputAdditionalAddinPaths;
            }
            set
            {
                this.inputAdditionalAddinPaths = value;
            }
        }

        public bool CacheResults
        {
            get
            {
                return this.inputCacheResults;
            }
            set
            {
                this.inputCacheResults = value;
            }
        }

        public string[] DefineConstants
        {
            get
            {
                return this.inputDefineConstants;
            }
            set
            {
                this.inputDefineConstants = value;
            }
        }

        public bool ForceFullAnalysis
        {
            get
            {
                return this.inputForceFullAnalysis;
            }
            set
            {
                this.inputForceFullAnalysis = value;
            }
        }

        public ITaskItem MaxViolationCount
        {
            get
            {
                return this.maxViolationCount;
            }
            set
            {
                this.maxViolationCount = value;
            }
        }

        public ITaskItem OutputFile
        {
            get
            {
                return this.outputFile;
            }
            set
            {
                Param.RequireNotNull(value, "OutputFile");
                this.outputFile = value;
            }
        }

        public ITaskItem OverrideSettingsFile
        {
            get
            {
                return this.inputOverrideSettingsFile;
            }
            set
            {
                this.inputOverrideSettingsFile = value;
            }
        }

        public ITaskItem ProjectFullPath
        {
            get
            {
                return this.inputProjectFullPath;
            }
            set
            {
                Param.RequireNotNull(value, "ProjectFullPath");
                this.inputProjectFullPath = value;
            }
        }

        public ITaskItem[] SourceFiles
        {
            get
            {
                return this.inputSourceFiles;
            }
            set
            {
                Param.RequireNotNull(value, "SourceFiles");
                this.inputSourceFiles = value;
            }
        }

        public bool TreatErrorsAsWarnings
        {
            get
            {
                return this.inputTreatErrorsAsWarnings;
            }
            set
            {
                this.inputTreatErrorsAsWarnings = value;
            }
        }
    }
}

