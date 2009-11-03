namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Xml;

    public abstract class StyleCopEnvironment
    {
        private StyleCopCore core;

        protected StyleCopEnvironment()
        {
        }

        public abstract void AddParser(SourceParser parser);
        public abstract bool AddSourceCode(CodeProject project, string path, object context);
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification="API has already been published and should not be changed.")]
        public abstract string GetDefaultSettingsPath();
        public abstract string GetParentSettingsPath(string settingsPath);
        public Settings GetProjectSettings(CodeProject project, bool merge)
        {
            Exception exception;
            return this.GetProjectSettings(project, merge, out exception);
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId="2#", Justification="The design is OK.")]
        public abstract Settings GetProjectSettings(CodeProject project, bool merge, out Exception exception);
        public Settings GetSettings(string settingsPath, bool merge)
        {
            Exception exception;
            return this.GetSettings(settingsPath, merge, out exception);
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId="2#", Justification="The design is OK.")]
        public abstract Settings GetSettings(string settingsPath, bool merge, out Exception exception);
        public WritableSettings GetWritableSettings(string settingsPath)
        {
            Exception exception;
            return this.GetWritableSettings(settingsPath, out exception);
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId="1#", Justification="The design is OK.")]
        public abstract WritableSettings GetWritableSettings(string settingsPath, out Exception exception);
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId="System.Xml.XmlNode", Justification="Compliance would break well-defined API.")]
        public abstract XmlDocument LoadResultsCache(string location);
        public abstract void RemoveAnalysisResults(string location);
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId="2#", Justification="The design is OK."), SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId="System.Xml.XmlNode", Justification="Compliance would break the well-defined public API.")]
        public abstract bool SaveAnalysisResults(string location, XmlDocument analysisResults, out Exception exception);
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId="System.Xml.XmlNode", Justification="Compliance would break well-defined API.")]
        public abstract void SaveResultsCache(string location, XmlDocument resultsCache);
        public bool SaveSettings(WritableSettings settings)
        {
            Exception exception;
            return this.SaveSettings(settings, out exception);
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId="1#", Justification="The design is OK.")]
        public abstract bool SaveSettings(WritableSettings settings, out Exception exception);

        public StyleCopCore Core
        {
            get
            {
                return this.core;
            }
            internal set
            {
                this.core = value;
            }
        }

        public abstract bool SupportsLinkedSettings { get; }

        public abstract bool SupportsResultsCache { get; }
    }
}

