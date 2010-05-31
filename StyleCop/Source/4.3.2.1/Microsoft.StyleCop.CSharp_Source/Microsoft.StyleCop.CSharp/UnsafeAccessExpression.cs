namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class UnsafeAccessExpression : Expression
    {
        private Operator operatorType;
        private Expression value;

        internal UnsafeAccessExpression(CsTokenList tokens, Operator operatorType, Expression value) : base(ExpressionType.UnsafeAccess, tokens)
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

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification="The enum describes a C# operator."), SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification="Leave nested to avoid changing external interface.")]
        public enum Operator
        {
            Dereference,
            AddressOf
        }
    }
}

