namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class BreakStatement : Statement
    {
        internal BreakStatement(CsTokenList tokens) : base(StatementType.Break, tokens)
        {
        }
    }
}

