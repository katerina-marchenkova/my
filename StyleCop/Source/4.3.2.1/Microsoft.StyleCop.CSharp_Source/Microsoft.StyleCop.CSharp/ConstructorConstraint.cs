namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class ConstructorConstraint : CsToken, ITokenContainer
    {
        private MasterList<CsToken> childTokens;

        internal ConstructorConstraint(MasterList<CsToken> childTokens, CodeLocation location, bool generated) : base(CsTokenType.Other, CsTokenClass.ConstructorConstraint, location, generated)
        {
            this.childTokens = childTokens;
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

        public MasterList<CsToken> ChildTokens
        {
            get
            {
                return this.childTokens.AsReadOnly;
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

