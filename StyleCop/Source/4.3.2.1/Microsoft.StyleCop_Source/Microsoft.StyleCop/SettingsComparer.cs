namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class SettingsComparer
    {
        private Settings localSettings;
        private Settings parentSettings;

        public SettingsComparer(Settings localSettings, Settings parentSettings)
        {
            Param.RequireNotNull(localSettings, "localSettings");
            this.localSettings = localSettings;
            this.parentSettings = parentSettings;
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="InSetting", Justification="InSetting is two words in this context.")]
        public bool IsAddInSettingOverwritten(StyleCopAddIn addIn, string propertyName, PropertyValue localProperty)
        {
            Param.RequireNotNull(addIn, "addIn");
            Param.RequireValidString(propertyName, "propertyName");
            if (this.parentSettings == null)
            {
                return false;
            }
            PropertyValue parentProperty = null;
            PropertyCollection addInSettings = this.parentSettings.GetAddInSettings(addIn);
            if (addInSettings != null)
            {
                parentProperty = addInSettings[propertyName];
            }
            if (parentProperty != null)
            {
                return IsSettingOverwritten(localProperty, parentProperty);
            }
            if (localProperty.HasDefaultValue)
            {
                return !localProperty.IsDefault;
            }
            return true;
        }

        public bool IsGlobalSettingOverwritten(string propertyName)
        {
            Param.RequireValidString(propertyName, "propertyName");
            if (this.localSettings == null)
            {
                return false;
            }
            PropertyValue localProperty = this.localSettings.GlobalSettings[propertyName];
            if (localProperty == null)
            {
                return false;
            }
            return this.IsGlobalSettingOverwritten(propertyName, localProperty);
        }

        public bool IsGlobalSettingOverwritten(string propertyName, PropertyValue localProperty)
        {
            Param.RequireValidString(propertyName, "propertyName");
            if (this.parentSettings == null)
            {
                return false;
            }
            PropertyValue parentProperty = this.parentSettings.GlobalSettings[propertyName];
            if (parentProperty == null)
            {
                return false;
            }
            return IsSettingOverwritten(localProperty, parentProperty);
        }

        public bool IsParserSettingOverwritten(StyleCopAddIn addIn, string propertyName)
        {
            Param.RequireNotNull(addIn, "addIn");
            Param.RequireValidString(propertyName, "propertyName");
            if (this.localSettings == null)
            {
                return false;
            }
            PropertyCollection addInSettings = this.localSettings.GetAddInSettings(addIn);
            if (addInSettings == null)
            {
                return false;
            }
            PropertyValue localProperty = addInSettings[propertyName];
            if (localProperty == null)
            {
                return false;
            }
            return this.IsAddInSettingOverwritten(addIn, propertyName, localProperty);
        }

        public static bool IsSettingOverwritten(PropertyValue localProperty, PropertyValue parentProperty)
        {
            if ((parentProperty == null) || (localProperty == null))
            {
                return false;
            }
            if (localProperty.PropertyType != parentProperty.PropertyType)
            {
                throw new ArgumentException(Strings.ComparingDifferentPropertyTypes);
            }
            return localProperty.OverridesProperty(parentProperty);
        }

        public Settings LocalSettings
        {
            get
            {
                return this.localSettings;
            }
        }

        public Settings ParentSettings
        {
            get
            {
                return this.parentSettings;
            }
        }
    }
}

