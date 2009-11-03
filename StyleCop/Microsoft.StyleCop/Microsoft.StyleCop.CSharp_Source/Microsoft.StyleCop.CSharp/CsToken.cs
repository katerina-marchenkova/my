namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification="Camel case better serves in this case.")]
    public class CsToken : Token
    {
        private Microsoft.StyleCop.CSharp.CsTokenClass tokenClass;
        private Microsoft.StyleCop.CSharp.CsTokenType tokenType;

        internal CsToken(Microsoft.StyleCop.CSharp.CsTokenType tokenType, Microsoft.StyleCop.CSharp.CsTokenClass tokenClass, CodeLocation location, bool generated) : base(location, generated)
        {
            this.tokenType = tokenType;
            this.tokenClass = tokenClass;
        }

        internal CsToken(string text, Microsoft.StyleCop.CSharp.CsTokenType tokenType, CodeLocation location, bool generated) : this(text, tokenType, Microsoft.StyleCop.CSharp.CsTokenClass.Token, location, generated)
        {
        }

        internal CsToken(string text, Microsoft.StyleCop.CSharp.CsTokenType tokenType, Microsoft.StyleCop.CSharp.CsTokenClass tokenClass, CodeLocation location, bool generated) : base(text, location, generated)
        {
            this.tokenType = tokenType;
            this.tokenClass = tokenClass;
        }

        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId="Member", Justification="Camel case better serves in this case.")]
        public Microsoft.StyleCop.CSharp.CsTokenClass CsTokenClass
        {
            get
            {
                return this.tokenClass;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId="Member", Justification="Camel case better serves in this case.")]
        public Microsoft.StyleCop.CSharp.CsTokenType CsTokenType
        {
            get
            {
                return this.tokenType;
            }
        }
    }
}

