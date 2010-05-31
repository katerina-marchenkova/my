namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class CheckedStatement : Statement
    {
        private BlockStatement embeddedStatement;

        internal CheckedStatement(CsTokenList tokens, BlockStatement embeddedStatement) : base(StatementType.Checked, tokens)
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

