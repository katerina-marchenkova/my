namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class Destructor : CsElement
    {
        internal Destructor(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, bool unsafeCode, bool generated) : base(document, parent, ElementType.Destructor, "destructor " + declaration.Name, header, attributes, declaration, unsafeCode, generated)
        {
            if (base.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static }))
            {
                base.Declaration.AccessModifierType = AccessModifierType.Public;
            }
        }
    }
}

