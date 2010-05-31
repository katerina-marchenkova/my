namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class DoWhileStatement : Statement
    {
        private Expression conditionExpression;
        private Statement embeddedStatement;

        internal DoWhileStatement(CsTokenList tokens, Expression conditionExpression, Statement embeddedStatement) : base(StatementType.DoWhile, tokens)
        {
            this.conditionExpression = conditionExpression;
            this.embeddedStatement = embeddedStatement;
            base.AddExpression(conditionExpression);
            base.AddStatement(embeddedStatement);
        }

        public Expression ConditionalExpression
        {
            get
            {
                return this.conditionExpression;
            }
        }

        public Statement EmbeddedStatement
        {
            get
            {
                return this.embeddedStatement;
            }
        }
    }
}

