namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class NullCoalescingExpression : Expression
    {
        private Expression leftHandSide;
        private Expression rightHandSide;

        internal NullCoalescingExpression(CsTokenList tokens, Expression leftHandSide, Expression rightHandSide) : base(ExpressionType.NullCoalescing, tokens)
        {
            this.leftHandSide = leftHandSide;
            this.rightHandSide = rightHandSide;
            base.AddExpression(leftHandSide);
            base.AddExpression(rightHandSide);
        }

        public Expression LeftHandSide
        {
            get
            {
                return this.leftHandSide;
            }
        }

        public Expression RightHandSide
        {
            get
            {
                return this.rightHandSide;
            }
        }
    }
}

