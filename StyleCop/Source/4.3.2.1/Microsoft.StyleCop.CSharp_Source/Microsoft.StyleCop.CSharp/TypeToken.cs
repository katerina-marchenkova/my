namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public class TypeToken : CsToken, ITokenContainer
    {
        private MasterList<CsToken> childTokens;

        internal TypeToken(MasterList<CsToken> childTokens, CodeLocation location, bool generated) : base(CsTokenType.Other, CsTokenClass.Type, location, generated)
        {
            this.childTokens = childTokens;
        }

        internal TypeToken(MasterList<CsToken> childTokens, CodeLocation location, CsTokenClass tokenClass, bool generated) : base(CsTokenType.Other, tokenClass, location, generated)
        {
            this.childTokens = childTokens;
        }

        protected override void CreateTextString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (CsToken token in this.childTokens)
            {
                if ((((token.CsTokenType != CsTokenType.WhiteSpace) && (token.CsTokenType != CsTokenType.EndOfLine)) && ((token.CsTokenType != CsTokenType.SingleLineComment) && (token.CsTokenType != CsTokenType.MultiLineComment))) && (token.CsTokenType != CsTokenType.PreprocessorDirective))
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

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification="The tokens list should be hidden")]
        ICollection<CsToken> ITokenContainer.Tokens
        {
            get
            {
                return this.childTokens.AsReadOnly;
            }
        }
    }
}

