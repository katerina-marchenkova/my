namespace Microsoft.StyleCop
{
    using System;

    public class PropertyDescriptor<T> : PropertyDescriptor
    {
        private T defaultValue;

        internal PropertyDescriptor(string propertyName, PropertyType propertyType, string friendlyName, string description, bool merge, bool displaySettings, T defaultValue) : base(propertyName, propertyType, friendlyName, description, merge, displaySettings)
        {
            this.defaultValue = defaultValue;
        }

        public T DefaultValue
        {
            get
            {
                return this.defaultValue;
            }
            internal set
            {
                this.defaultValue = value;
            }
        }
    }
}

