namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class DecrementExpression : Expression
    {
        private DecrementType decrementType;
        private Expression value;

        internal DecrementExpression(CsTokenList tokens, Expression value, DecrementType decrementType) : base(ExpressionType.Decrement, tokens)
        {
            this.value = value;
            this.decrementType = decrementType;
            base.AddExpression(value);
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification="API has already been published and should not be changed.")]
        public DecrementType Type
        {
            get
            {
                return this.decrementType;
            }
        }

        public Expression Value
        {
            get
            {
                return this.value;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification="API has already been published and should not be changed.")]
        public enum DecrementType
        {
            Prefix,
            Postfix
        }
    }
}

