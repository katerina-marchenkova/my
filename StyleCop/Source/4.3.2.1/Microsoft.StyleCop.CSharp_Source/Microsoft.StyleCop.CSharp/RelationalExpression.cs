namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class RelationalExpression : Expression
    {
        private Expression leftHandSide;
        private Operator operatorType;
        private Expression rightHandSide;

        internal RelationalExpression(CsTokenList tokens, Operator operatorType, Expression leftHandSide, Expression rightHandSide) : base(ExpressionType.Relational, tokens)
        {
            this.operatorType = operatorType;
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

        public Operator OperatorType
        {
            get
            {
                return this.operatorType;
            }
        }

        public Expression RightHandSide
        {
            get
            {
                return this.rightHandSide;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification="Describes a C# operator type"), SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification="Leave nested to avoid changing external interface.")]
        public enum Operator
        {
            EqualTo,
            NotEqualTo,
            GreaterThan,
            LessThan,
            GreaterThanOrEqualTo,
            LessThanOrEqualTo
        }
    }
}

