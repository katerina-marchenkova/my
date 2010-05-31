namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class QueryExpression : Expression
    {
        private CodeUnitCollection<QueryClause> clauses;

        internal QueryExpression(CsTokenList tokens, ICollection<QueryClause> clauses) : base(ExpressionType.Query, tokens)
        {
            this.clauses = new CodeUnitCollection<QueryClause>(this);
            this.clauses.AddRange(clauses);
            this.InitializeFromClauses(clauses);
        }

        private void InitializeFromClauses(IEnumerable<QueryClause> items)
        {
            foreach (QueryClause clause in items)
            {
                foreach (Expression expression in clause.ChildExpressions)
                {
                    base.AddExpression(expression);
                }
                QueryContinuationClause clause2 = clause as QueryContinuationClause;
                if (clause2 != null)
                {
                    this.InitializeFromClauses(clause2.ChildClauses);
                }
            }
        }

        public ICollection<QueryClause> ChildClauses
        {
            get
            {
                return this.clauses;
            }
        }
    }
}

