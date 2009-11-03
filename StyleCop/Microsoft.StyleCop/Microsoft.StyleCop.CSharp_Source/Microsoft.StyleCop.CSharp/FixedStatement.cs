namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class FixedStatement : Statement
    {
        private Statement embeddedStatement;
        private VariableDeclarationExpression fixedVariable;

        internal FixedStatement(CsTokenList tokens, VariableDeclarationExpression fixedVariable) : base(StatementType.Fixed, tokens)
        {
            this.fixedVariable = fixedVariable;
            base.AddExpression(fixedVariable);
        }

        public Statement EmbeddedStatement
        {
            get
            {
                return this.embeddedStatement;
            }
            internal set
            {
                this.embeddedStatement = value;
                base.AddStatement(this.embeddedStatement);
            }
        }

        public VariableDeclarationExpression FixedVariable
        {
            get
            {
                return this.fixedVariable;
            }
        }
    }
}

