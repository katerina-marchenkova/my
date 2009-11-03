namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class QueryWhereClause : QueryClauseWithExpression
    {
        internal QueryWhereClause(CsTokenList tokens, Expression expression) : base(QueryClauseType.Where, tokens, expression)
        {
        }
    }
}

