namespace Microsoft.StyleCop
{
    using System;

    public class CollectionPropertyDescriptor : PropertyDescriptor
    {
        private bool aggregate;

        internal CollectionPropertyDescriptor(string propertyName, string friendlyName, string description, bool merge, bool aggregate) : base(propertyName, PropertyType.Collection, friendlyName, description, merge, false)
        {
            this.aggregate = aggregate;
        }

        public bool Aggregate
        {
            get
            {
                return this.aggregate;
            }
        }
    }
}

