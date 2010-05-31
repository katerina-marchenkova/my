namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class QueryLetClause : QueryClauseWithExpression
    {
        private Variable rangeVariable;

        internal QueryLetClause(CsTokenList tokens, Variable rangeVariable, Expression expression) : base(QueryClauseType.Let, tokens, expression)
        {
            this.rangeVariable = rangeVariable;
        }

        public Variable RangeVariable
        {
            get
            {
                return this.rangeVariable;
            }
        }
    }
}

