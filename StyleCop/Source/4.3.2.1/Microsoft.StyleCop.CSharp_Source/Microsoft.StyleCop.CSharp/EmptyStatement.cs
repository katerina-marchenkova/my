namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class EmptyStatement : Statement
    {
        internal EmptyStatement(CsTokenList tokens) : base(StatementType.Empty, tokens)
        {
        }
    }
}

