namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class LabelStatement : Statement
    {
        private LiteralExpression identifier;

        internal LabelStatement(CsTokenList tokens, LiteralExpression identifier) : base(StatementType.Label, tokens)
        {
            this.identifier = identifier;
            base.AddExpression(identifier);
        }

        public LiteralExpression Identifier
        {
            get
            {
                return this.identifier;
            }
        }
    }
}

