namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public abstract class Statement : CodeUnit
    {
        private static Statement[] emptyStatementArray = new Statement[0];
        private Microsoft.StyleCop.CSharp.StatementType type;

        internal Statement(Microsoft.StyleCop.CSharp.StatementType type) : base(CodeUnitType.Statement)
        {
            this.type = type;
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification="The tokens property is virtual but it this is safe as statements are sealed.")]
        internal Statement(Microsoft.StyleCop.CSharp.StatementType type, CsTokenList tokens) : base(CodeUnitType.Statement, tokens)
        {
            this.type = type;
            this.Tokens = tokens;
            this.Tokens.Trim();
        }

        public void WalkStatement(CodeWalkerStatementVisitor<object> statementCallback)
        {
            this.WalkStatement<object>(statementCallback, null, null, null);
        }

        public void WalkStatement<T>(CodeWalkerStatementVisitor<T> statementCallback, T context)
        {
            this.WalkStatement<T>(statementCallback, null, null, context);
        }

        public void WalkStatement(CodeWalkerStatementVisitor<object> statementCallback, CodeWalkerExpressionVisitor<object> expressionCallback)
        {
            this.WalkStatement<object>(statementCallback, expressionCallback, null, null);
        }

        public void WalkStatement(CodeWalkerStatementVisitor<object> statementCallback, CodeWalkerExpressionVisitor<object> expressionCallback, CodeWalkerQueryClauseVisitor<object> queryClauseCallback)
        {
            CodeWalker<object>.Start(this, statementCallback, expressionCallback, queryClauseCallback, null);
        }

        public void WalkStatement<T>(CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, T context)
        {
            this.WalkStatement<T>(statementCallback, expressionCallback, null, context);
        }

        public void WalkStatement<T>(CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            CodeWalker<T>.Start(this, statementCallback, expressionCallback, queryClauseCallback, context);
        }

        public virtual IEnumerable<Statement> AttachedStatements
        {
            get
            {
                return emptyStatementArray;
            }
        }

        public Microsoft.StyleCop.CSharp.StatementType StatementType
        {
            get
            {
                return this.type;
            }
        }

        public override CsTokenList Tokens
        {
            get
            {
                return base.Tokens;
            }
            internal set
            {
                base.Tokens = value;
                this.Location = null;
            }
        }
    }
}

