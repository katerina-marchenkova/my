namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;

    public sealed class Number : CsToken
    {
        internal Number(string token, CodeLocation location, bool generated) : base(token, CsTokenType.Number, CsTokenClass.Number, location, generated)
        {
        }
    }
}

