namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class Struct : ClassBase
    {
        internal Struct(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, ICollection<TypeParameterConstraintClause> typeConstraints, bool unsafeCode, bool generated) : base(document, parent, ElementType.Struct, "struct " + declaration.Name, header, attributes, declaration, typeConstraints, unsafeCode, generated)
        {
        }

        internal override void Initialize()
        {
            base.SetInheritedItems(base.Declaration);
        }
    }
}

