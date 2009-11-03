namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class GotoStatement : Statement
    {
        private Expression identifier;

        internal GotoStatement(CsTokenList tokens, Expression identifier) : base(StatementType.Goto, tokens)
        {
            this.identifier = identifier;
            base.AddExpression(identifier);
        }

        public Expression Identifier
        {
            get
            {
                return this.identifier;
            }
        }
    }
}

