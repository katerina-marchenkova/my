namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class QuerySelectClause : QueryClauseWithExpression
    {
        internal QuerySelectClause(CsTokenList tokens, Expression expression) : base(QueryClauseType.Select, tokens, expression)
        {
        }
    }
}

