namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class IncrementExpression : Expression
    {
        private IncrementType incrementType;
        private Expression value;

        internal IncrementExpression(CsTokenList tokens, Expression value, IncrementType incrementType) : base(ExpressionType.Increment, tokens)
        {
            this.value = value;
            this.incrementType = incrementType;
            base.AddExpression(value);
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification="API has already been published and should not be changed.")]
        public IncrementType Type
        {
            get
            {
                return this.incrementType;
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
        public enum IncrementType
        {
            Prefix,
            Postfix
        }
    }
}

