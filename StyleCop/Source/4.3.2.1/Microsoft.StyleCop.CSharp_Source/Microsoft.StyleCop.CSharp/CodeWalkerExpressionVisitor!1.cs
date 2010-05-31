namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate bool CodeWalkerExpressionVisitor<T>(Expression expression, Expression parentExpression, Statement parentStatement, CsElement parentElement, T context);
}

