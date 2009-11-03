namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification="The class describes a C# namespace")]
    public class Namespace : CsElement
    {
        internal Namespace(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, bool unsafeCode, bool generated) : base(document, parent, ElementType.Namespace, "namespace " + declaration.Name, header, attributes, declaration, unsafeCode, generated)
        {
        }

        internal Namespace(CsDocument document, CsElement parent, ElementType type, string name, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, bool unsafeCode, bool generated) : base(document, parent, type, name, header, attributes, declaration, unsafeCode, generated)
        {
        }
    }
}

