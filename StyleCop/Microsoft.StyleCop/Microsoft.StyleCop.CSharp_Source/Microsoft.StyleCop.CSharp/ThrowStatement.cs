namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class ThrowStatement : Statement
    {
        private Expression thrownExpression;

        internal ThrowStatement(CsTokenList tokens, Expression thrownExpression) : base(StatementType.Throw, tokens)
        {
            this.thrownExpression = thrownExpression;
            if (thrownExpression != null)
            {
                base.AddExpression(thrownExpression);
            }
        }

        public Expression ThrownExpression
        {
            get
            {
                return this.thrownExpression;
            }
        }
    }
}

