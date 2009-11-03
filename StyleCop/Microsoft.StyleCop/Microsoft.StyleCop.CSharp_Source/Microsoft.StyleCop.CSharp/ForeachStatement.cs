namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class ForeachStatement : Statement
    {
        private Statement embeddedStatement;
        private Expression item;
        private VariableDeclarationExpression variable;

        internal ForeachStatement(CsTokenList tokens, VariableDeclarationExpression variable, Expression item) : base(StatementType.Foreach, tokens)
        {
            this.variable = variable;
            this.item = item;
            base.AddExpression(variable);
            base.AddExpression(item);
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

        public Expression Item
        {
            get
            {
                return this.item;
            }
        }

        public VariableDeclarationExpression Variable
        {
            get
            {
                return this.variable;
            }
        }
    }
}

