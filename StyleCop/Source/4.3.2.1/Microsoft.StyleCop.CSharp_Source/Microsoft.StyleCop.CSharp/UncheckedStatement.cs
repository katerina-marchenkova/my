namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class UncheckedStatement : Statement
    {
        private BlockStatement embeddedStatement;

        internal UncheckedStatement(CsTokenList tokens, BlockStatement embeddedStatement) : base(StatementType.Unchecked, tokens)
        {
            this.embeddedStatement = embeddedStatement;
            base.AddStatement(embeddedStatement);
        }

        public BlockStatement EmbeddedStatement
        {
            get
            {
                return this.embeddedStatement;
            }
        }
    }
}

