namespace Microsoft.StyleCop
{
    using System;

    public class BooleanProperty : PropertyValue<bool>
    {
        public BooleanProperty(PropertyDescriptor<bool> propertyDescriptor, bool value) : base(propertyDescriptor, value)
        {
            Param.RequireNotNull(propertyDescriptor, "propertyDescriptor");
        }

        public BooleanProperty(IPropertyContainer propertyContainer, string propertyName, bool value) : base(propertyContainer, propertyName, value)
        {
            Param.RequireNotNull(propertyContainer, "propertyContainer");
            Param.RequireValidString(propertyName, "propertyName");
        }

        public override PropertyValue Clone()
        {
            return new BooleanProperty((PropertyDescriptor<bool>) base.PropertyDescriptor, base.Value);
        }
    }
}

