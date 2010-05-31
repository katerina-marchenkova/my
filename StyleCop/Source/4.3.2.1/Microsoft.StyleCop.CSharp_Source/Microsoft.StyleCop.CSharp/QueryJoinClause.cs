namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class QueryJoinClause : QueryClause
    {
        private Expression equalsKeyExpression;
        private Expression inExpression;
        private Variable intoVariable;
        private Expression onKeyExpression;
        private Variable rangeVariable;

        internal QueryJoinClause(CsTokenList tokens, Variable rangeVariable, Expression inExpression, Expression onKeyExpression, Expression equalsKeyExpression, Variable intoVariable) : base(QueryClauseType.Join, tokens)
        {
            this.rangeVariable = rangeVariable;
            this.inExpression = inExpression;
            this.onKeyExpression = onKeyExpression;
            this.equalsKeyExpression = equalsKeyExpression;
            this.intoVariable = intoVariable;
            base.AddExpression(this.inExpression);
            base.AddExpression(this.onKeyExpression);
            base.AddExpression(this.equalsKeyExpression);
        }

        public Expression EqualsKeyExpression
        {
            get
            {
                return this.equalsKeyExpression;
            }
        }

        public Expression InExpression
        {
            get
            {
                return this.inExpression;
            }
        }

        public Variable IntoVariable
        {
            get
            {
                return this.intoVariable;
            }
        }

        public Expression OnKeyExpression
        {
            get
            {
                return this.onKeyExpression;
            }
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

