namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate bool CodeWalkerStatementVisitor<T>(Statement statement, Expression parentExpression, Statement parentStatement, CsElement parentElement, T context);
}

