namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class ArithmeticExpression : Expression
    {
        private Expression leftHandSide;
        private Operator operatorType;
        private Expression rightHandSide;

        internal ArithmeticExpression(CsTokenList tokens, Operator operatorType, Expression leftHandSide, Expression rightHandSide) : base(ExpressionType.Arithmetic, tokens)
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
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Mod,
            RightShift,
            LeftShift
        }
    }
}

