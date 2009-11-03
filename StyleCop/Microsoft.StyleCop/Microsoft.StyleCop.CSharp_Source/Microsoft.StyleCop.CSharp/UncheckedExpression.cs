namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class UncheckedExpression : Expression
    {
        private Expression internalExpression;

        internal UncheckedExpression(CsTokenList tokens, Expression internalExpression) : base(ExpressionType.Unchecked, tokens)
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

