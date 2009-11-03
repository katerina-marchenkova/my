namespace Microsoft.StyleCop
{
    using System;
    using System.IO;

    public class SettingsMerger
    {
        private StyleCopEnvironment environment;
        internal const string LinkedSettingsProperty = "LinkedSettingsFile";
        private Settings localSettings;
        internal const string MergeSettingsFilesProperty = "MergeSettingsFiles";
        internal const string MergeStyleLinked = "Linked";
        internal const string MergeStyleNone = "NoMerge";
        internal const string MergeStyleParent = "Parent";

        public SettingsMerger(Settings localSettings, StyleCopEnvironment environment)
        {
            Param.RequireNotNull(localSettings, "localSettings");
            Param.RequireNotNull(environment, "environment");
            this.localSettings = localSettings;
            this.environment = environment;
        }

        private Settings FindMergedSettingsThroughLinkedSettings(Settings originalSettings, bool mergeOriginal)
        {
            StringProperty property = originalSettings.GlobalSettings.GetProperty("LinkedSettingsFile") as StringProperty;
            if ((property == null) || string.IsNullOrEmpty(property.Value))
            {
                return originalSettings;
            }
            string relativePath = Environment.ExpandEnvironmentVariables(property.Value);
            if (relativePath.StartsWith(".", StringComparison.Ordinal) || !relativePath.Contains(@"\"))
            {
                relativePath = StyleCopCore.MakeAbsolutePath(originalSettings.Location, relativePath);
            }
            if (!File.Exists(relativePath))
            {
                return originalSettings;
            }
            Settings settings = this.environment.GetSettings(relativePath, true);
            if (settings == null)
            {
                return originalSettings;
            }
            if (mergeOriginal)
            {
                return MergeSettings(settings, originalSettings);
            }
            return settings;
        }

        private Settings FindMergedSettingsThroughParentPaths(Settings originalSettings, bool mergeOriginal)
        {
            if (!originalSettings.DefaultSettings)
            {
                bool flag = false;
                string parentSettingsPath = this.environment.GetParentSettingsPath(originalSettings.Location);
                if (string.IsNullOrEmpty(parentSettingsPath) && !originalSettings.DefaultSettings)
                {
                    flag = true;
                    parentSettingsPath = this.environment.GetDefaultSettingsPath();
                }
                if (!string.IsNullOrEmpty(parentSettingsPath))
                {
                    Settings settings = this.environment.GetSettings(parentSettingsPath, !flag);
                    settings.DefaultSettings = flag;
                    if (settings != null)
                    {
                        if (mergeOriginal)
                        {
                            Settings settings2 = MergeSettings(settings, originalSettings);
                            settings2.DefaultSettings = flag;
                            return settings2;
                        }
                        return settings;
                    }
                }
            }
            if (!mergeOriginal)
            {
                return null;
            }
            return originalSettings;
        }

        private static void MergeCollectionProperties(PropertyCollection mergedPropertyCollection, PropertyValue originalProperty, PropertyValue overridingProperty)
        {
            CollectionProperty property = (CollectionProperty) originalProperty;
            CollectionProperty property2 = (CollectionProperty) overridingProperty;
            CollectionProperty property3 = new CollectionProperty((CollectionPropertyDescriptor) property.PropertyDescriptor);
            mergedPropertyCollection.Add(property3);
            foreach (string str in property2.Values)
            {
                property3.Add(str);
            }
            if (property.Aggregate)
            {
                foreach (string str2 in property.Values)
                {
                    if (!property3.Contains(str2))
                    {
                        property3.Add(str2);
                    }
                }
            }
        }

        internal static void MergePropertyCollections(PropertyCollection originalPropertyCollection, PropertyCollection overridingPropertyCollection, PropertyCollection mergedPropertyCollection)
        {
            if ((originalPropertyCollection == null) && (overridingPropertyCollection != null))
            {
                foreach (PropertyValue value2 in overridingPropertyCollection)
                {
                    mergedPropertyCollection.Add(value2.Clone());
                }
            }
            else if ((originalPropertyCollection != null) && (overridingPropertyCollection == null))
            {
                foreach (PropertyValue value3 in originalPropertyCollection)
                {
                    if (value3.PropertyDescriptor.Merge)
                    {
                        mergedPropertyCollection.Add(value3.Clone());
                    }
                }
            }
            else if ((originalPropertyCollection != null) && (overridingPropertyCollection != null))
            {
            Label_0109:
                foreach (PropertyValue value4 in originalPropertyCollection)
                {
                    if (value4.PropertyDescriptor.Merge)
                    {
                        PropertyValue overridingProperty = overridingPropertyCollection[value4.PropertyName];
                        if (overridingProperty == null)
                        {
                            mergedPropertyCollection.Add(value4.Clone());
                        }
                        else
                        {
                            switch (value4.PropertyType)
                            {
                                case PropertyType.String:
                                case PropertyType.Boolean:
                                case PropertyType.Int:
                                    mergedPropertyCollection.Add(overridingProperty.Clone());
                                    goto Label_0109;

                                case PropertyType.Collection:
                                    MergeCollectionProperties(mergedPropertyCollection, value4, overridingProperty);
                                    goto Label_0109;
                            }
                        }
                    }
                }
                foreach (PropertyValue value6 in overridingPropertyCollection)
                {
                    if (mergedPropertyCollection[value6.PropertyName] == null)
                    {
                        mergedPropertyCollection.Add(value6.Clone());
                    }
                }
            }
            mergedPropertyCollection.IsReadOnly = true;
        }

        private static Settings MergeSettings(Settings originalSettings, Settings overridingSettings)
        {
            Settings settings = new Settings(originalSettings.Core);
            MergePropertyCollections(originalSettings.GlobalSettings, overridingSettings.GlobalSettings, settings.GlobalSettings);
            foreach (AddInPropertyCollection propertys in originalSettings.ParserSettings)
            {
                AddInPropertyCollection addInSettings = overridingSettings.GetAddInSettings(propertys.AddIn);
                AddInPropertyCollection properties = new AddInPropertyCollection(propertys.AddIn);
                settings.SetAddInSettings(properties);
                MergePropertyCollections(propertys, addInSettings, properties);
            }
            foreach (AddInPropertyCollection propertys4 in overridingSettings.ParserSettings)
            {
                if (settings.GetAddInSettings(propertys4.AddIn) == null)
                {
                    settings.SetAddInSettings((AddInPropertyCollection) propertys4.Clone());
                }
            }
            foreach (AddInPropertyCollection propertys6 in originalSettings.AnalyzerSettings)
            {
                AddInPropertyCollection overridingPropertyCollection = overridingSettings.GetAddInSettings(propertys6.AddIn);
                AddInPropertyCollection propertys8 = new AddInPropertyCollection(propertys6.AddIn);
                settings.SetAddInSettings(propertys8);
                MergePropertyCollections(propertys6, overridingPropertyCollection, propertys8);
            }
            foreach (AddInPropertyCollection propertys9 in overridingSettings.AnalyzerSettings)
            {
                if (settings.GetAddInSettings(propertys9.AddIn) == null)
                {
                    settings.SetAddInSettings((AddInPropertyCollection) propertys9.Clone());
                }
            }
            if (originalSettings.WriteTime.CompareTo(overridingSettings.WriteTime) > 0)
            {
                settings.WriteTime = originalSettings.WriteTime;
                return settings;
            }
            settings.WriteTime = overridingSettings.WriteTime;
            return settings;
        }

        public Settings MergedSettings
        {
            get
            {
                StringProperty property = this.localSettings.GlobalSettings.GetProperty("MergeSettingsFiles") as StringProperty;
                string strA = "Parent";
                if (property != null)
                {
                    strA = property.Value;
                }
                if (!this.environment.SupportsLinkedSettings && (string.CompareOrdinal(strA, "Linked") == 0))
                {
                    strA = "Parent";
                }
                if (string.CompareOrdinal(strA, "Linked") == 0)
                {
                    return this.FindMergedSettingsThroughLinkedSettings(this.localSettings, true);
                }
                if (string.CompareOrdinal(strA, "NoMerge") != 0)
                {
                    return this.FindMergedSettingsThroughParentPaths(this.localSettings, true);
                }
                return this.localSettings;
            }
        }

        public Settings ParentMergedSettings
        {
            get
            {
                StringProperty property = this.localSettings.GlobalSettings.GetProperty("MergeSettingsFiles") as StringProperty;
                string strA = "Parent";
                if (property != null)
                {
                    strA = property.Value;
                }
                if (!this.environment.SupportsLinkedSettings && (string.CompareOrdinal(strA, "Linked") == 0))
                {
                    strA = "Parent";
                }
                if (string.CompareOrdinal(strA, "Parent") == 0)
                {
                    return this.FindMergedSettingsThroughParentPaths(this.localSettings, false);
                }
                if (string.CompareOrdinal(strA, "Linked") == 0)
                {
                    return this.FindMergedSettingsThroughLinkedSettings(this.localSettings, false);
                }
                return null;
            }
        }
    }
}

