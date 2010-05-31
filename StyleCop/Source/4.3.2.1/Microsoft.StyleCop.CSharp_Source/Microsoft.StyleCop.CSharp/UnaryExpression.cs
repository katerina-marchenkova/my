namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class UnaryExpression : Expression
    {
        private Operator operatorType;
        private Expression value;

        internal UnaryExpression(CsTokenList tokens, Operator operatorType, Expression value) : base(ExpressionType.Unary, tokens)
        {
            this.operatorType = operatorType;
            this.value = value;
            base.AddExpression(value);
        }

        public Operator OperatorType
        {
            get
            {
                return this.operatorType;
            }
        }

        public Expression Value
        {
            get
            {
                return this.value;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification="The enum describes a C# operator type."), SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification="Leave nested to avoid changing external interface.")]
        public enum Operator
        {
            Positive,
            Negative,
            Not,
            BitwiseCompliment
        }
    }
}

