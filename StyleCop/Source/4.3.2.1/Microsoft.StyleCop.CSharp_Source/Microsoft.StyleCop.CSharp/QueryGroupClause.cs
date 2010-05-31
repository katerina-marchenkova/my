namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class QueryGroupClause : QueryClauseWithExpression
    {
        private Expression groupByExpression;

        internal QueryGroupClause(CsTokenList tokens, Expression expression, Expression groupByExpression) : base(QueryClauseType.Group, tokens, expression)
        {
            this.groupByExpression = groupByExpression;
            base.AddExpression(this.groupByExpression);
        }

        public Expression GroupByExpression
        {
            get
            {
                return this.groupByExpression;
            }
        }
    }
}

