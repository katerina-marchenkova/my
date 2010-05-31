namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class DocumentRoot : Namespace
    {
        internal DocumentRoot(CsDocument document, Declaration declaration, bool generated) : base(document, null, ElementType.Root, Strings.DocumentRoot, null, null, declaration, false, generated)
        {
        }
    }
}

