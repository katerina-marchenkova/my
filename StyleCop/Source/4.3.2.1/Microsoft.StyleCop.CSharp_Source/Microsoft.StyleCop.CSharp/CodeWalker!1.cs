namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal class CodeWalker<T>
    {
        private CodeWalkerElementVisitor<T> elementCallback;
        private CodeWalkerExpressionVisitor<T> expressionCallback;
        private CodeWalkerQueryClauseVisitor<T> queryClauseCallback;
        private CodeWalkerStatementVisitor<T> statementCallback;

        private CodeWalker(Expression expression, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            this.statementCallback = statementCallback;
            this.expressionCallback = expressionCallback;
            this.queryClauseCallback = queryClauseCallback;
            this.WalkExpression(expression, expression.ParentExpression, expression.ParentStatement, expression.ParentElement, context);
        }

        private CodeWalker(QueryClause queryClause, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            this.statementCallback = statementCallback;
            this.expressionCallback = expressionCallback;
            this.queryClauseCallback = queryClauseCallback;
            this.WalkQueryClause(queryClause, queryClause.ParentQueryClause, queryClause.ParentExpression, queryClause.ParentStatement, queryClause.ParentElement, context);
        }

        private CodeWalker(Statement statement, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            this.statementCallback = statementCallback;
            this.expressionCallback = expressionCallback;
            this.queryClauseCallback = queryClauseCallback;
            this.WalkStatement(statement, statement.ParentExpression, statement.ParentStatement, statement.ParentElement, context);
        }

        private CodeWalker(CsDocument document, CodeWalkerElementVisitor<T> elementCallback, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            this.elementCallback = elementCallback;
            this.statementCallback = statementCallback;
            this.expressionCallback = expressionCallback;
            this.queryClauseCallback = queryClauseCallback;
            this.WalkElement(document.RootElement, null, context);
        }

        private CodeWalker(CsElement element, CodeWalkerElementVisitor<T> elementCallback, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            this.elementCallback = elementCallback;
            this.statementCallback = statementCallback;
            this.expressionCallback = expressionCallback;
            this.queryClauseCallback = queryClauseCallback;
            this.WalkElement(element, element.ParentElement, context);
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification="The CodeWalker instance is create but not saved because the constructor walks through the elements.")]
        public static void Start(Expression expression, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            new CodeWalker<T>(expression, statementCallback, expressionCallback, queryClauseCallback, context);
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification="The CodeWalker instance is create but not saved because the constructor walks through the elements.")]
        public static void Start(QueryClause queryClause, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            new CodeWalker<T>(queryClause, statementCallback, expressionCallback, queryClauseCallback, context);
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification="The CodeWalker instance is create but not saved because the constructor walks through the elements.")]
        public static void Start(Statement statement, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            new CodeWalker<T>(statement, statementCallback, expressionCallback, queryClauseCallback, context);
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification="The CodeWalker instance is create but not saved because the constructor walks through the elements.")]
        public static void Start(CsDocument document, CodeWalkerElementVisitor<T> elementCallback, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            new CodeWalker<T>(document, elementCallback, statementCallback, expressionCallback, queryClauseCallback, context);
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification="The CodeWalker instance is create but not saved because the constructor walks through the elements.")]
        public static void Start(CsElement element, CodeWalkerElementVisitor<T> elementCallback, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            new CodeWalker<T>(element, elementCallback, statementCallback, expressionCallback, queryClauseCallback, context);
        }

        private bool VisitElement(CsElement element, CsElement parentElement, ref T context)
        {
            if (this.elementCallback != null)
            {
                return this.elementCallback(element, parentElement, context);
            }
            return true;
        }

        private bool VisitExpression(Expression expression, Expression parentExpression, Statement parentStatement, CsElement parentElement, ref T context)
        {
            if (this.expressionCallback != null)
            {
                return this.expressionCallback(expression, parentExpression, parentStatement, parentElement, context);
            }
            return true;
        }

        private bool VisitQueryClause(QueryClause clause, QueryClause parentClause, Expression parentExpression, Statement parentStatement, CsElement parentElement, ref T context)
        {
            if (this.queryClauseCallback != null)
            {
                return this.queryClauseCallback(clause, parentClause, parentExpression, parentStatement, parentElement, context);
            }
            return true;
        }

        private bool VisitStatement(Statement statement, Expression parentExpression, Statement parentStatement, CsElement parentElement, ref T context)
        {
            if (this.statementCallback != null)
            {
                return this.statementCallback(statement, parentExpression, parentStatement, parentElement, context);
            }
            return true;
        }

        private bool WalkElement(CsElement element, CsElement parentElement, T context)
        {
            if (element != null)
            {
                T local = context;
                if (!this.VisitElement(element, parentElement, ref local))
                {
                    return false;
                }
                foreach (Statement statement in element.ChildStatements)
                {
                    if (!this.WalkStatement(statement, null, null, element, local))
                    {
                        return false;
                    }
                }
                foreach (CsElement element2 in element.ChildElements)
                {
                    if (!this.WalkElement(element2, element, local))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool WalkExpression(Expression expression, Expression parentExpression, Statement parentStatement, CsElement parentElement, T context)
        {
            if (expression != null)
            {
                T local = context;
                if (!this.VisitExpression(expression, parentExpression, parentStatement, parentElement, ref local))
                {
                    return false;
                }
                foreach (Expression expression2 in expression.ChildExpressions)
                {
                    if (!this.WalkExpression(expression2, expression, parentStatement, parentElement, local))
                    {
                        return false;
                    }
                }
                if (expression.ExpressionType == ExpressionType.Query)
                {
                    QueryExpression expression3 = (QueryExpression) expression;
                    foreach (QueryClause clause in expression3.ChildClauses)
                    {
                        if (!this.WalkQueryClause(clause, null, expression, parentStatement, parentElement, local))
                        {
                            return false;
                        }
                    }
                }
                foreach (Statement statement in expression.ChildStatements)
                {
                    if (!this.WalkStatement(statement, expression, parentStatement, parentElement, local))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool WalkQueryClause(QueryClause clause, QueryClause parentClause, Expression parentExpression, Statement parentStatement, CsElement parentElement, T context)
        {
            if (clause != null)
            {
                T local = context;
                if (!this.VisitQueryClause(clause, parentClause, parentExpression, parentStatement, parentElement, ref local))
                {
                    return false;
                }
                foreach (Expression expression in clause.ChildExpressions)
                {
                    if (!this.WalkExpression(expression, parentExpression, parentStatement, parentElement, local))
                    {
                        return false;
                    }
                }
                if (clause.QueryClauseType == QueryClauseType.Continuation)
                {
                    QueryContinuationClause clause2 = (QueryContinuationClause) clause;
                    foreach (QueryClause clause3 in clause2.ChildClauses)
                    {
                        if (!this.WalkQueryClause(clause3, clause, parentExpression, parentStatement, parentElement, local))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool WalkStatement(Statement statement, Expression parentExpression, Statement parentStatement, CsElement parentElement, T context)
        {
            if (statement != null)
            {
                T local = context;
                if (!this.VisitStatement(statement, parentExpression, parentStatement, parentElement, ref local))
                {
                    return false;
                }
                foreach (Expression expression in statement.ChildExpressions)
                {
                    if (!this.WalkExpression(expression, parentExpression, statement, parentElement, local))
                    {
                        return false;
                    }
                }
                foreach (Statement statement2 in statement.ChildStatements)
                {
                    if (!this.WalkStatement(statement2, parentExpression, statement, parentElement, local))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

