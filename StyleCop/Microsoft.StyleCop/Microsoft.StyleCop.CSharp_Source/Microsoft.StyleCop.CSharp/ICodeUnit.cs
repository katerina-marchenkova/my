namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;

    public interface ICodeUnit
    {
        ICollection<Expression> ChildExpressions { get; }

        ICollection<Statement> ChildStatements { get; }

        Microsoft.StyleCop.CSharp.CodeUnitType CodeUnitType { get; }

        string FriendlyPluralTypeText { get; }

        string FriendlyTypeText { get; }

        int LineNumber { get; }

        CodeLocation Location { get; }

        ICodeUnit Parent { get; }

        CsElement ParentElement { get; }

        Expression ParentExpression { get; }

        Statement ParentStatement { get; }

        CsTokenList Tokens { get; }

        VariableCollection Variables { get; }
    }
}

