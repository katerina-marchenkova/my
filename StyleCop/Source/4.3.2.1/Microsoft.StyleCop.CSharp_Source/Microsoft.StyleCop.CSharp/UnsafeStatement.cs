namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class UnsafeStatement : Statement
    {
        private BlockStatement embeddedStatement;

        internal UnsafeStatement(CsTokenList tokens, BlockStatement embeddedStatement) : base(StatementType.Unsafe, tokens)
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

