namespace Microsoft.StyleCop
{
    using System;

    public class AddInPropertyCollection : PropertyCollection
    {
        private StyleCopAddIn addIn;

        internal AddInPropertyCollection(StyleCopAddIn addIn)
        {
            this.addIn = addIn;
        }

        public override PropertyCollection Clone()
        {
            AddInPropertyCollection propertys = new AddInPropertyCollection(this.addIn);
            foreach (PropertyValue value2 in base.Properties)
            {
                propertys.Add(value2.Clone());
            }
            return propertys;
        }

        public StyleCopAddIn AddIn
        {
            get
            {
                return this.addIn;
            }
        }
    }
}

