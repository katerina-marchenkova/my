namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class LockStatement : Statement
    {
        private Statement embeddedStatement;
        private Expression lockedExpression;

        internal LockStatement(CsTokenList tokens, Expression lockedExpression) : base(StatementType.Lock, tokens)
        {
            this.lockedExpression = lockedExpression;
            base.AddExpression(lockedExpression);
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

        public Expression LockedExpression
        {
            get
            {
                return this.lockedExpression;
            }
        }
    }
}

