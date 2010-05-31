namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate bool CodeWalkerQueryClauseVisitor<T>(QueryClause clause, QueryClause parentClause, Expression parentExpression, Statement parentStatement, CsElement parentElement, T context);
}

