namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class ExpressionStatement : Statement
    {
        private Microsoft.StyleCop.CSharp.Expression expression;

        internal ExpressionStatement(CsTokenList tokens, Microsoft.StyleCop.CSharp.Expression expression) : base(StatementType.Expression, tokens)
        {
            this.expression = expression;
            base.AddExpression(expression);
        }

        public Microsoft.StyleCop.CSharp.Expression Expression
        {
            get
            {
                return this.expression;
            }
        }
    }
}

