namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class MemberAccessExpression : Expression
    {
        private Expression leftHandSide;
        private Operator operatorType;
        private LiteralExpression rightHandSide;

        internal MemberAccessExpression(CsTokenList tokens, Operator operatorType, Expression leftHandSide, LiteralExpression rightHandSide) : base(ExpressionType.MemberAccess, tokens)
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

        public LiteralExpression RightHandSide
        {
            get
            {
                return this.rightHandSide;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification="Describes a C# operator type"), SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification="Leave nested to avoid changing external interface.")]
        public enum Operator
        {
            Pointer,
            Dot,
            QualifiedAlias
        }
    }
}

