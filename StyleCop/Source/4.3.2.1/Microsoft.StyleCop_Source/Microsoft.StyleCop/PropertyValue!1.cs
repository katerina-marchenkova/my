namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public abstract class PropertyValue<T> : PropertyValue
    {
        private T value;

        protected PropertyValue(PropertyDescriptor<T> propertyDescriptor, T value) : base(propertyDescriptor)
        {
            Param.RequireNotNull(propertyDescriptor, "propertyDescriptor");
            this.value = value;
        }

        protected PropertyValue(IPropertyContainer propertyContainer, string propertyName, T value) : base(propertyContainer, propertyName)
        {
            Param.RequireNotNull(propertyContainer, "propertyContainer");
            Param.RequireValidString(propertyName, "propertyName");
            this.value = value;
        }

        public override PropertyValue Clone()
        {
            throw new NotImplementedException();
        }

        public override bool OverridesProperty(PropertyValue parentProperty)
        {
            Param.RequireNotNull(parentProperty, "parentProperty");
            PropertyValue<T> value2 = parentProperty as PropertyValue<T>;
            if ((value2 == null) || !this.DefaultValue.Equals(value2.DefaultValue))
            {
                throw new ArgumentException(Strings.ComparingDifferentPropertyTypes, "parentProperty");
            }
            return !this.value.Equals(value2.Value);
        }

        public T DefaultValue
        {
            get
            {
                PropertyDescriptor<T> propertyDescriptor = (PropertyDescriptor<T>) base.PropertyDescriptor;
                return propertyDescriptor.DefaultValue;
            }
        }

        public override bool HasDefaultValue
        {
            get
            {
                return (this.DefaultValue != null);
            }
        }

        public override bool IsDefault
        {
            get
            {
                T defaultValue = this.DefaultValue;
                return ((defaultValue != null) && defaultValue.Equals(this.value));
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId="value", Justification="It is not possible to change the name")]
        public T Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.IsReadOnly)
                {
                    throw new StyleCopException(Strings.ReadOnlyProperty);
                }
                this.value = value;
            }
        }
    }
}

