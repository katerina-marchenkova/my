namespace Microsoft.StyleCop
{
    using System;

    public class StringProperty : PropertyValue<string>
    {
        public StringProperty(PropertyDescriptor<string> propertyDescriptor, string value) : base(propertyDescriptor, value)
        {
            Param.RequireNotNull(propertyDescriptor, "propertyDescriptor");
        }

        public StringProperty(IPropertyContainer propertyContainer, string propertyName, string value) : base(propertyContainer, propertyName, value)
        {
            Param.RequireNotNull(propertyContainer, "propertyContainer");
            Param.RequireValidString(propertyName, "propertyName");
        }

        public override PropertyValue Clone()
        {
            return new StringProperty((PropertyDescriptor<string>) base.PropertyDescriptor, base.Value);
        }
    }
}

