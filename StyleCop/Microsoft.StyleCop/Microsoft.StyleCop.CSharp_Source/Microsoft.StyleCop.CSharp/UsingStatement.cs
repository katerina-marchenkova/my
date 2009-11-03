namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class UsingStatement : Statement
    {
        private Statement embeddedStatement;
        private Expression resource;

        internal UsingStatement(CsTokenList tokens, Expression resource) : base(StatementType.Using, tokens)
        {
            this.resource = resource;
            base.AddExpression(resource);
        }

        public Statement EmbeddedStatement
        {
            get
            {
                return this.embeddedStatement;
            }
            internal set
            {
                this.embeddedStatement = value;
                base.AddStatement(this.embeddedStatement);
            }
        }

        public Expression Resource
        {
            get
            {
                return this.resource;
            }
        }
    }
}

