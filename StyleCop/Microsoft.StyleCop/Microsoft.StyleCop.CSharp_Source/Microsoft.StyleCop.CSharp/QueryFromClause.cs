namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class QueryFromClause : QueryClauseWithExpression
    {
        private Variable rangeVariable;

        internal QueryFromClause(CsTokenList tokens, Variable rangeVariable, Expression expression) : base(QueryClauseType.From, tokens, expression)
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

