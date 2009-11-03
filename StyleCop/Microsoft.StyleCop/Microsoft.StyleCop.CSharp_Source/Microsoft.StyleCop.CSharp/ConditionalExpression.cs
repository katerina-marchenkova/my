namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class ConditionalExpression : Expression
    {
        private Expression condition;
        private Expression falseValue;
        private Expression trueValue;

        internal ConditionalExpression(CsTokenList tokens, Expression condition, Expression trueValue, Expression falseValue) : base(ExpressionType.Conditional, tokens)
        {
            this.condition = condition;
            this.trueValue = trueValue;
            this.falseValue = falseValue;
            base.AddExpression(condition);
            base.AddExpression(trueValue);
            base.AddExpression(falseValue);
        }

        public Expression Condition
        {
            get
            {
                return this.condition;
            }
        }

        public Expression FalseExpression
        {
            get
            {
                return this.falseValue;
            }
        }

        public Expression TrueExpression
        {
            get
            {
                return this.trueValue;
            }
        }
    }
}

