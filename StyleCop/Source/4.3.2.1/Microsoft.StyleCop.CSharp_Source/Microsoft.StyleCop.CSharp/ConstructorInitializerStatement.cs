namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class ConstructorInitializerStatement : Statement
    {
        private MethodInvocationExpression expression;

        internal ConstructorInitializerStatement(CsTokenList tokens, MethodInvocationExpression expression) : base(StatementType.ConstructorInitializer, tokens)
        {
            this.expression = expression;
            base.AddExpression(expression);
        }

        public MethodInvocationExpression Expression
        {
            get
            {
                return this.expression;
            }
        }
    }
}

