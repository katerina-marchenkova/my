namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class ParenthesizedExpression : Expression
    {
        private Expression innerExpression;

        internal ParenthesizedExpression(CsTokenList tokens, Expression innerExpression) : base(ExpressionType.Parenthesized, tokens)
        {
            this.innerExpression = innerExpression;
            base.AddExpression(innerExpression);
        }

        public Expression InnerExpression
        {
            get
            {
                return this.innerExpression;
            }
        }
    }
}

