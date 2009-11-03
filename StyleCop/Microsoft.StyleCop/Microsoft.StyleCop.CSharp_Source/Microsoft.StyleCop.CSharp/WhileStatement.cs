namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class WhileStatement : Statement
    {
        private Expression conditionExpression;
        private Statement embeddedStatement;

        internal WhileStatement(CsTokenList tokens, Expression conditionExpression) : base(StatementType.While, tokens)
        {
            this.conditionExpression = conditionExpression;
            base.AddExpression(conditionExpression);
        }

        public Expression ConditionExpression
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
            internal set
            {
                this.embeddedStatement = value;
                base.AddStatement(this.embeddedStatement);
            }
        }
    }
}

