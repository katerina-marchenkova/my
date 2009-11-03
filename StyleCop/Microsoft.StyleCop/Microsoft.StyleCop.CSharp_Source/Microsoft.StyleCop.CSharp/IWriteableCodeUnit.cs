namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    internal interface IWriteableCodeUnit : ICodeUnit
    {
        void AddExpression(Expression expression);
        void AddExpressions(IEnumerable<Expression> expressions);
        void AddStatement(Statement statement);
        void AddStatements(IEnumerable<Statement> statements);
        void SetParent(ICodeUnit parent);
    }
}

