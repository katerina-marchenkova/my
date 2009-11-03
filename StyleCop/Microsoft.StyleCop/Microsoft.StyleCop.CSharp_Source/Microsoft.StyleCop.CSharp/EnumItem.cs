namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class EnumItem : CsElement
    {
        private Expression initialization;

        internal EnumItem(CsDocument document, Microsoft.StyleCop.CSharp.Enum parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, Expression initialization, bool unsafeCode, bool generated) : base(document, parent, ElementType.EnumItem, "enum item " + declaration.Name, header, attributes, declaration, unsafeCode, generated)
        {
            this.initialization = initialization;
        }

        public Expression Initialization
        {
            get
            {
                return this.initialization;
            }
        }
    }
}

