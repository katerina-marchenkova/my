namespace Microsoft.StyleCop.CSharp
{
    using System.Collections.Generic;

    public interface ITokenContainer
    {
        ICollection<CsToken> Tokens { get; }
    }
}

