namespace Microsoft.StyleCop.CSharp
{
    using System;

    public abstract class QueryClause : CodeUnit
    {
        private Microsoft.StyleCop.CSharp.QueryClauseType type;

        internal QueryClause(Microsoft.StyleCop.CSharp.QueryClauseType type, CsTokenList tokens) : base(CodeUnitType.QueryClause, tokens)
        {
            this.type = type;
        }

        public void WalkQueryClause(CodeWalkerStatementVisitor<object> statementCallback)
        {
            this.WalkQueryClause<object>(statementCallback, null, null, null);
        }

        public void WalkQueryClause<T>(CodeWalkerStatementVisitor<T> statementCallback, T context)
        {
            this.WalkQueryClause<T>(statementCallback, null, null, context);
        }

        public void WalkQueryClause(CodeWalkerStatementVisitor<object> statementCallback, CodeWalkerExpressionVisitor<object> expressionCallback)
        {
            this.WalkQueryClause<object>(statementCallback, expressionCallback, null, null);
        }

        public void WalkQueryClause(CodeWalkerStatementVisitor<object> statementCallback, CodeWalkerExpressionVisitor<object> expressionCallback, CodeWalkerQueryClauseVisitor<object> queryClauseCallback)
        {
            CodeWalker<object>.Start(this, statementCallback, expressionCallback, queryClauseCallback, null);
        }

        public void WalkQueryClause<T>(CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, T context)
        {
            this.WalkQueryClause<T>(statementCallback, expressionCallback, null, context);
        }

        public void WalkQueryClause<T>(CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            CodeWalker<T>.Start(this, statementCallback, expressionCallback, queryClauseCallback, context);
        }

        public QueryClause ParentQueryClause
        {
            get
            {
                return (base.Parent as QueryClause);
            }
        }

        public Microsoft.StyleCop.CSharp.QueryClauseType QueryClauseType
        {
            get
            {
                return this.type;
            }
        }
    }
}

