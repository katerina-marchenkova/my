namespace Microsoft.StyleCop
{
    using System;
    using System.Globalization;

    public abstract class PropertyValue
    {
        private Microsoft.StyleCop.PropertyDescriptor propertyDescriptor;
        private bool readOnly;

        protected PropertyValue(Microsoft.StyleCop.PropertyDescriptor propertyDescriptor)
        {
            Param.RequireNotNull(propertyDescriptor, "propertyDescriptor");
            this.propertyDescriptor = propertyDescriptor;
        }

        protected PropertyValue(IPropertyContainer propertyContainer, string propertyName)
        {
            Param.RequireNotNull(propertyContainer, "propertyContainer");
            Param.RequireValidString(propertyName, "propertyName");
            Microsoft.StyleCop.PropertyDescriptor descriptor = propertyContainer.PropertyDescriptors[propertyName];
            if (descriptor == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.PropertyDescriptorDoesNotExist, new object[] { propertyName }));
            }
            this.propertyDescriptor = descriptor;
        }

        public abstract PropertyValue Clone();
        public abstract bool OverridesProperty(PropertyValue parentProperty);

        public string Description
        {
            get
            {
                return this.propertyDescriptor.Description;
            }
        }

        public string FriendlyName
        {
            get
            {
                return this.propertyDescriptor.FriendlyName;
            }
        }

        public abstract bool HasDefaultValue { get; }

        public abstract bool IsDefault { get; }

        public virtual bool IsReadOnly
        {
            get
            {
                return this.readOnly;
            }
            internal set
            {
                this.readOnly = value;
            }
        }

        public Microsoft.StyleCop.PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return this.propertyDescriptor;
            }
        }

        public string PropertyName
        {
            get
            {
                return this.propertyDescriptor.PropertyName;
            }
        }

        public Microsoft.StyleCop.PropertyType PropertyType
        {
            get
            {
                return this.propertyDescriptor.PropertyType;
            }
        }
    }
}

