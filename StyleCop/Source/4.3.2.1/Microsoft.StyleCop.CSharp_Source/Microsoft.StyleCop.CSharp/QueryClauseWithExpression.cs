namespace Microsoft.StyleCop.CSharp
{
    using System;

    public abstract class QueryClauseWithExpression : QueryClause
    {
        private Microsoft.StyleCop.CSharp.Expression expression;

        internal QueryClauseWithExpression(QueryClauseType type, CsTokenList tokens, Microsoft.StyleCop.CSharp.Expression expression) : base(type, tokens)
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

