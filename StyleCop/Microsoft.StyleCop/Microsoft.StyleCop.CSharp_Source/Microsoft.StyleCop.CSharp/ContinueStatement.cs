namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class ContinueStatement : Statement
    {
        internal ContinueStatement(CsTokenList tokens) : base(StatementType.Continue, tokens)
        {
        }
    }
}

