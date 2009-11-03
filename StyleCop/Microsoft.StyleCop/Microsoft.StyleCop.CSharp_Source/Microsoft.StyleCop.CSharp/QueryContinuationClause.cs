namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class QueryContinuationClause : QueryClause
    {
        private CodeUnitCollection<QueryClause> clauses;
        private Microsoft.StyleCop.CSharp.Variable variable;

        internal QueryContinuationClause(CsTokenList tokens, Microsoft.StyleCop.CSharp.Variable variable, ICollection<QueryClause> clauses) : base(QueryClauseType.Continuation, tokens)
        {
            this.variable = variable;
            this.clauses = new CodeUnitCollection<QueryClause>(this);
            this.clauses.AddRange(clauses);
        }

        public ICollection<QueryClause> ChildClauses
        {
            get
            {
                return this.clauses;
            }
        }

        public Microsoft.StyleCop.CSharp.Variable Variable
        {
            get
            {
                return this.variable;
            }
        }
    }
}

