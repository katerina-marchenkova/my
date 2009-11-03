namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;

    public class Settings
    {
        public const string AlternateFileName = "Settings.SourceAnalysis";
        private Dictionary<string, AddInPropertyCollection> analyzerSettings;
        private XmlDocument contents;
        private StyleCopCore core;
        public const string DefaultFileName = "Settings.StyleCop";
        private bool defaultSettings;
        private PropertyCollection globalSettings;
        private string location;
        private Dictionary<string, AddInPropertyCollection> parserSettings;
        private string path;
        private DateTime writeTime;

        internal Settings(StyleCopCore core)
        {
            this.globalSettings = new PropertyCollection();
            this.parserSettings = new Dictionary<string, AddInPropertyCollection>();
            this.analyzerSettings = new Dictionary<string, AddInPropertyCollection>();
            this.core = core;
        }

        internal Settings(StyleCopCore core, string path, string location) : this(core, path, location, null, new DateTime())
        {
        }

        internal Settings(StyleCopCore core, string path, string location, XmlDocument contents, DateTime writeTime)
        {
            this.globalSettings = new PropertyCollection();
            this.parserSettings = new Dictionary<string, AddInPropertyCollection>();
            this.analyzerSettings = new Dictionary<string, AddInPropertyCollection>();
            this.core = core;
            this.path = path;
            this.location = location;
            this.contents = contents;
            this.writeTime = writeTime;
            this.LoadSettingsDocument();
        }

        internal void ClearAddInSettingInternal(StyleCopAddIn addIn, string propertyName)
        {
            PropertyCollection addInSettings = this.GetAddInSettings(addIn);
            if (addInSettings != null)
            {
                addInSettings.Remove(propertyName);
                if (addInSettings.Count == 0)
                {
                    this.GetPropertyCollectionDictionary(addIn).Remove(addIn.Id);
                }
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="InSetting", Justification="InSetting is two words in this context.")]
        public PropertyValue GetAddInSetting(StyleCopAddIn addIn, string propertyName)
        {
            Param.RequireNotNull(addIn, "addIn");
            Param.RequireValidString(propertyName, "propertyName");
            PropertyCollection addInSettings = this.GetAddInSettings(addIn);
            if (addInSettings != null)
            {
                return addInSettings[propertyName];
            }
            return null;
        }

        public AddInPropertyCollection GetAddInSettings(StyleCopAddIn addIn)
        {
            AddInPropertyCollection propertys;
            Param.RequireNotNull(addIn, "addIn");
            if (this.GetPropertyCollectionDictionary(addIn).TryGetValue(addIn.Id, out propertys))
            {
                return propertys;
            }
            return null;
        }

        private Dictionary<string, AddInPropertyCollection> GetPropertyCollectionDictionary(StyleCopAddIn addIn)
        {
            Dictionary<string, AddInPropertyCollection> parserSettings = this.parserSettings;
            if (addIn is SourceAnalyzer)
            {
                parserSettings = this.analyzerSettings;
            }
            return parserSettings;
        }

        private void LoadSettingsDocument()
        {
            this.globalSettings.Clear();
            this.parserSettings.Clear();
            this.analyzerSettings.Clear();
            if (this.contents != null)
            {
                XmlAttribute attribute = this.contents.DocumentElement.Attributes["Version"];
                string a = (attribute == null) ? string.Empty : attribute.Value;
                if (string.Equals(a, "4.3", StringComparison.Ordinal))
                {
                    V43Settings.Load(this.contents, this);
                }
                else if (string.Equals(a, "4.2", StringComparison.Ordinal))
                {
                    V42Settings.Load(this.contents, this);
                }
                else if (string.Equals(a, "4.1", StringComparison.Ordinal))
                {
                    V41Settings.Load(this.contents, this);
                }
                else
                {
                    V40Settings.Load(this.contents, this);
                }
            }
        }

        internal void SetAddInSettingInternal(StyleCopAddIn addIn, PropertyValue property)
        {
            AddInPropertyCollection addInSettings = this.GetAddInSettings(addIn);
            if (addInSettings == null)
            {
                addInSettings = new AddInPropertyCollection(addIn);
                this.SetAddInSettings(addInSettings);
            }
            addInSettings.Add(property);
        }

        internal void SetAddInSettings(AddInPropertyCollection properties)
        {
            Dictionary<string, AddInPropertyCollection> propertyCollectionDictionary = this.GetPropertyCollectionDictionary(properties.AddIn);
            if (propertyCollectionDictionary.ContainsKey(properties.AddIn.Id))
            {
                propertyCollectionDictionary[properties.AddIn.Id] = properties;
            }
            else
            {
                propertyCollectionDictionary.Add(properties.AddIn.Id, properties);
            }
        }

        protected Dictionary<string, AddInPropertyCollection> AnalyzerDictionary
        {
            get
            {
                return this.analyzerSettings;
            }
        }

        public ICollection<AddInPropertyCollection> AnalyzerSettings
        {
            get
            {
                return this.analyzerSettings.Values;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId="System.Xml.XmlNode", Justification="Compliance would break public API.")]
        public XmlDocument Contents
        {
            get
            {
                return this.contents;
            }
        }

        internal StyleCopCore Core
        {
            get
            {
                return this.core;
            }
        }

        internal bool DefaultSettings
        {
            get
            {
                return this.defaultSettings;
            }
            set
            {
                this.defaultSettings = value;
            }
        }

        public PropertyCollection GlobalSettings
        {
            get
            {
                return this.globalSettings;
            }
        }

        public bool Loaded
        {
            get
            {
                return (this.contents != null);
            }
        }

        public string Location
        {
            get
            {
                return this.location;
            }
        }

        protected Dictionary<string, AddInPropertyCollection> ParserDictionary
        {
            get
            {
                return this.parserSettings;
            }
        }

        public ICollection<AddInPropertyCollection> ParserSettings
        {
            get
            {
                return this.parserSettings.Values;
            }
        }

        public string Path
        {
            get
            {
                return this.path;
            }
        }

        public DateTime WriteTime
        {
            get
            {
                return this.writeTime;
            }
            internal set
            {
                this.writeTime = value;
            }
        }
    }
}

