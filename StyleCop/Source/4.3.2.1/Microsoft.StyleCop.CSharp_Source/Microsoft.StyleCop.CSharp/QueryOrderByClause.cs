namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class QueryOrderByClause : QueryClause
    {
        private QueryOrderByOrdering[] orderings;

        internal QueryOrderByClause(CsTokenList tokens, ICollection<QueryOrderByOrdering> orderings) : base(QueryClauseType.OrderBy, tokens)
        {
            this.orderings = new QueryOrderByOrdering[orderings.Count];
            int num = 0;
            foreach (QueryOrderByOrdering ordering in orderings)
            {
                this.orderings[num++] = ordering;
                base.AddExpression(ordering.Expression);
            }
        }

        public ICollection<QueryOrderByOrdering> Orderings
        {
            get
            {
                return this.orderings;
            }
        }
    }
}

