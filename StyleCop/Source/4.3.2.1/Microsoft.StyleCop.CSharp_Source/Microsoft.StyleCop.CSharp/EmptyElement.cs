namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class EmptyElement : CsElement
    {
        internal EmptyElement(CsDocument document, CsElement parent, Declaration declaration, bool unsafeCode, bool generated) : base(document, parent, ElementType.EmptyElement, Strings.EmptyElement, null, null, declaration, unsafeCode, generated)
        {
        }
    }
}

