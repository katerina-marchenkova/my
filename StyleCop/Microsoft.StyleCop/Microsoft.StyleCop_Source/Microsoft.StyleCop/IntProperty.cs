namespace Microsoft.StyleCop
{
    using System;

    public class IntProperty : PropertyValue<int>
    {
        public IntProperty(PropertyDescriptor<int> propertyDescriptor, int value) : base(propertyDescriptor, value)
        {
            Param.RequireNotNull(propertyDescriptor, "propertyDescriptor");
        }

        public IntProperty(IPropertyContainer propertyContainer, string propertyName, int value) : base(propertyContainer, propertyName, value)
        {
            Param.RequireNotNull(propertyContainer, "propertyContainer");
            Param.RequireValidString(propertyName, "propertyName");
        }

        public override PropertyValue Clone()
        {
            return new IntProperty((PropertyDescriptor<int>) base.PropertyDescriptor, base.Value);
        }
    }
}

