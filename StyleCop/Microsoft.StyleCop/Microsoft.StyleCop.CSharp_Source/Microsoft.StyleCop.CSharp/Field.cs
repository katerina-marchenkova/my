namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public sealed class Field : CsElement
    {
        private Microsoft.StyleCop.CSharp.VariableDeclarationStatement declaration;
        private bool isConst;
        private bool isReadOnly;
        private TypeToken type;

        internal Field(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, TypeToken fieldType, bool unsafeCode, bool generated) : base(document, parent, ElementType.Field, "field " + declaration.Name, header, attributes, declaration, unsafeCode, generated)
        {
            this.type = fieldType;
            this.isConst = base.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Const });
            this.isReadOnly = base.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Readonly });
        }

        public bool Const
        {
            get
            {
                return this.isConst;
            }
        }

        public TypeToken FieldType
        {
            get
            {
                return this.type;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="Readonly", Justification="API has already been published and should not be changed.")]
        public bool Readonly
        {
            get
            {
                return this.isReadOnly;
            }
        }

        public Microsoft.StyleCop.CSharp.VariableDeclarationStatement VariableDeclarationStatement
        {
            get
            {
                return this.declaration;
            }
            internal set
            {
                this.declaration = value;
                if (this.declaration != null)
                {
                    this.AddStatement(this.declaration);
                }
            }
        }
    }
}

