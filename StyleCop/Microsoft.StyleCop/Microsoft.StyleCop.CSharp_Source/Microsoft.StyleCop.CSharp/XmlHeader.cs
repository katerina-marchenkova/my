namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public sealed class XmlHeader : CsToken, ITokenContainer
    {
        private MasterList<CsToken> childTokens;
        private CsElement element;

        internal XmlHeader(MasterList<CsToken> childTokens, CodeLocation location, bool generated) : base(CsTokenType.XmlHeader, CsTokenClass.XmlHeader, location, generated)
        {
            this.childTokens = childTokens;
        }

        protected override void CreateTextString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (CsToken token in this.childTokens)
            {
                if (token.CsTokenType == CsTokenType.XmlHeaderLine)
                {
                    builder.Append(token.Text.Substring(3, token.Text.Length - 3));
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

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification="The tokens list should be hidden.")]
        ICollection<CsToken> ITokenContainer.Tokens
        {
            get
            {
                return this.childTokens.AsReadOnly;
            }
        }
    }
}

