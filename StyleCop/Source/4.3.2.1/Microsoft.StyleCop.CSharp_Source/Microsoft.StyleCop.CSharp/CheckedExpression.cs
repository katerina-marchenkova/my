namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class CheckedExpression : Expression
    {
        private Expression internalExpression;

        internal CheckedExpression(CsTokenList tokens, Expression internalExpression) : base(ExpressionType.Checked, tokens)
        {
            this.internalExpression = internalExpression;
            base.AddExpression(internalExpression);
        }

        public Expression InternalExpression
        {
            get
            {
                return this.internalExpression;
            }
        }
    }
}

