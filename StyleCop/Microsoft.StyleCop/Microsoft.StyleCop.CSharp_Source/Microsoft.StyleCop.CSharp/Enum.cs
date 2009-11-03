namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification="The class name does not need any suffix."), SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification="The class describes a C# enum.")]
    public sealed class Enum : CsElement
    {
        private string baseType;
        private ICollection<EnumItem> items;

        internal Enum(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, bool unsafeCode, bool generated) : base(document, parent, ElementType.Enum, "enum " + declaration.Name, header, attributes, declaration, unsafeCode, generated)
        {
        }

        private string GetBaseType()
        {
            bool flag = false;
            foreach (CsToken token in base.Declaration.Tokens)
            {
                if (flag)
                {
                    if ((((token.CsTokenType != CsTokenType.WhiteSpace) && (token.CsTokenType != CsTokenType.EndOfLine)) && ((token.CsTokenType != CsTokenType.SingleLineComment) && (token.CsTokenType != CsTokenType.MultiLineComment))) && (token.CsTokenType != CsTokenType.PreprocessorDirective))
                    {
                        return token.Text;
                    }
                }
                else if (token.Text == ":")
                {
                    flag = true;
                }
            }
            return null;
        }

        internal override void Initialize()
        {
            this.baseType = this.GetBaseType();
        }

        public string BaseType
        {
            get
            {
                return this.baseType;
            }
        }

        public ICollection<EnumItem> Items
        {
            get
            {
                return this.items;
            }
            internal set
            {
                this.items = value;
            }
        }
    }
}

