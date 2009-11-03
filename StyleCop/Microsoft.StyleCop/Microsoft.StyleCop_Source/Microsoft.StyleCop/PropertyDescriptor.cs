namespace Microsoft.StyleCop
{
    using System;

    public abstract class PropertyDescriptor
    {
        private string description;
        private bool displaySettings;
        private string friendlyName;
        private bool merge;
        private string propertyName;
        private Microsoft.StyleCop.PropertyType propertyType;

        protected PropertyDescriptor(string propertyName, Microsoft.StyleCop.PropertyType propertyType, string friendlyName, string description, bool merge, bool displaySettings)
        {
            Param.RequireValidString(propertyName, "propertyName");
            Param.RequireNotNull(friendlyName, "friendlyName");
            Param.RequireNotNull(description, "description");
            this.propertyName = propertyName;
            this.propertyType = propertyType;
            this.friendlyName = friendlyName;
            this.description = description;
            this.merge = merge;
            this.displaySettings = displaySettings;
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public bool DisplaySettings
        {
            get
            {
                return this.displaySettings;
            }
        }

        public string FriendlyName
        {
            get
            {
                return this.friendlyName;
            }
        }

        public bool Merge
        {
            get
            {
                return this.merge;
            }
        }

        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }

        public Microsoft.StyleCop.PropertyType PropertyType
        {
            get
            {
                return this.propertyType;
            }
        }
    }
}

