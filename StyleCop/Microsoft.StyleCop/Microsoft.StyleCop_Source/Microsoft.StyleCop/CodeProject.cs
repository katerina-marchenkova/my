namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;

    public class CodeProject
    {
        private Microsoft.StyleCop.Configuration configuration;
        private int key;
        private string location;
        private Microsoft.StyleCop.Settings settings;
        private bool settingsLoaded;
        private List<SourceCode> sourceCodes = new List<SourceCode>();
        private bool? writeCache;

        public CodeProject(int key, string location, Microsoft.StyleCop.Configuration configuration)
        {
            Param.RequireNotNull(configuration, "configuration");
            this.key = key;
            this.configuration = configuration;
            if (location != null)
            {
                this.location = StyleCopCore.CleanPath(location);
            }
        }

        internal void AddSourceCode(SourceCode sourceCode)
        {
            if (string.IsNullOrEmpty(sourceCode.Type))
            {
                throw new ArgumentException(Strings.SourceCodeTypePropertyNotSet);
            }
            this.sourceCodes.Add(sourceCode);
        }

        public Microsoft.StyleCop.Configuration Configuration
        {
            get
            {
                return this.configuration;
            }
        }

        public int Key
        {
            get
            {
                return this.key;
            }
        }

        public string Location
        {
            get
            {
                return this.location;
            }
        }

        public Microsoft.StyleCop.Settings Settings
        {
            get
            {
                return this.settings;
            }
            set
            {
                this.settings = value;
            }
        }

        public bool SettingsLoaded
        {
            get
            {
                return this.settingsLoaded;
            }
            set
            {
                this.settingsLoaded = value;
            }
        }

        public IList<SourceCode> SourceCodeInstances
        {
            get
            {
                return this.sourceCodes.ToArray();
            }
        }

        public bool WriteCache
        {
            get
            {
                if (!this.writeCache.HasValue && this.settingsLoaded)
                {
                    if (this.settings != null)
                    {
                        PropertyDescriptor<bool> descriptor = this.settings.Core.PropertyDescriptors["WriteCache"] as PropertyDescriptor<bool>;
                        if (descriptor != null)
                        {
                            BooleanProperty property = this.settings.GlobalSettings.GetProperty(descriptor.PropertyName) as BooleanProperty;
                            if (property == null)
                            {
                                this.writeCache = new bool?(descriptor.DefaultValue);
                            }
                            else
                            {
                                this.writeCache = new bool?(property.Value);
                            }
                        }
                        else
                        {
                            this.writeCache = true;
                        }
                    }
                    else
                    {
                        this.writeCache = true;
                    }
                }
                return (!this.writeCache.HasValue || this.writeCache.Value);
            }
        }
    }
}

