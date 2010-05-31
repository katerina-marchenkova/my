namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class AssignmentExpression : Expression
    {
        private Expression leftHandSide;
        private Operator operatorType;
        private Expression rightHandSide;

        internal AssignmentExpression(CsTokenList tokens, Operator operatorType, Expression leftHandSide, Expression rightHandSide) : base(ExpressionType.Assignment, tokens)
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

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification="Leave nested to avoid changing external interface."), SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification="Describes a C# operator type")]
        public enum Operator
        {
            Equals,
            PlusEquals,
            MinusEquals,
            MultiplicationEquals,
            DivisionEquals,
            ModEquals,
            OrEquals,
            AndEquals,
            XorEquals,
            LeftShiftEquals,
            RightShiftEquals
        }
    }
}

