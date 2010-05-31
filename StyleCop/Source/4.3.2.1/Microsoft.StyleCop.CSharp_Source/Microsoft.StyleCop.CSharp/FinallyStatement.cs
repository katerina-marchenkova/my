namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class FinallyStatement : Statement
    {
        private BlockStatement embeddedStatement;
        private Microsoft.StyleCop.CSharp.TryStatement tryStatement;

        internal FinallyStatement(CsTokenList tokens, Microsoft.StyleCop.CSharp.TryStatement tryStatement, BlockStatement embeddedStatement) : base(StatementType.Finally, tokens)
        {
            this.tryStatement = tryStatement;
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

        public Microsoft.StyleCop.CSharp.TryStatement TryStatement
        {
            get
            {
                return this.tryStatement;
            }
        }
    }
}

