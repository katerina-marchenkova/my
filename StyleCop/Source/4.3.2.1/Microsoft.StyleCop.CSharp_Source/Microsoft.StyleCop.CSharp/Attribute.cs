namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification="The class describes a C# attribute.")]
    public sealed class Attribute : CsToken, ITokenContainer
    {
        private ICollection<AttributeExpression> attributeExpressions;
        private MasterList<CsToken> childTokens;
        private CsElement element;

        internal Attribute(MasterList<CsToken> childTokens, CodeLocation location, ICollection<AttributeExpression> attributeExpressions, bool generated) : base(CsTokenType.Attribute, CsTokenClass.Attribute, location, generated)
        {
            this.childTokens = childTokens;
            this.attributeExpressions = attributeExpressions;
        }

        protected override void CreateTextString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (CsToken token in this.childTokens)
            {
                if (((token.CsTokenType != CsTokenType.SingleLineComment) && (token.CsTokenType != CsTokenType.MultiLineComment)) && (token.CsTokenType != CsTokenType.PreprocessorDirective))
                {
                    builder.Append(token.Text);
                }
            }
            this.Text = builder.ToString();
        }

        public ICollection<AttributeExpression> AttributeExpressions
        {
            get
            {
                return this.attributeExpressions;
            }
        }

        public MasterList<CsToken> ChildTokens
        {
            get
            {
                return this.childTokens.AsReadOnly;
            }
        }

        public CsElement Element
        {
            get
            {
                return this.element;
            }
            internal set
            {
                this.element = value;
            }
        }

        ICollection<CsToken> ITokenContainer.Tokens
        {
            get
            {
                return this.childTokens.AsReadOnly;
            }
        }
    }
}

