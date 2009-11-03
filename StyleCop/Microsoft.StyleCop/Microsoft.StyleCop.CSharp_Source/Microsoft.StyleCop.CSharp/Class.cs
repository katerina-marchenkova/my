namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification="The class describes a C# class.")]
    public sealed class Class : ClassBase
    {
        internal Class(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, ICollection<TypeParameterConstraintClause> typeConstraints, bool unsafeCode, bool generated) : base(document, parent, ElementType.Class, "class " + declaration.Name, header, attributes, declaration, typeConstraints, unsafeCode, generated)
        {
        }

        internal override void Initialize()
        {
            base.SetInheritedItems(base.Declaration);
        }
    }
}

