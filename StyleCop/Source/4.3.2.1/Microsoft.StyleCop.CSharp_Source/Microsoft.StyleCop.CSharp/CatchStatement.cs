namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class CatchStatement : Statement
    {
        private Expression catchExpression;
        private TypeToken classType;
        private BlockStatement embeddedStatement;
        private string identifier;
        private Microsoft.StyleCop.CSharp.TryStatement tryStatement;

        internal CatchStatement(CsTokenList tokens, Microsoft.StyleCop.CSharp.TryStatement tryStatement, Expression classExpression, BlockStatement embeddedStatement) : base(StatementType.Catch, tokens)
        {
            this.tryStatement = tryStatement;
            this.catchExpression = classExpression;
            this.embeddedStatement = embeddedStatement;
            if (classExpression != null)
            {
                base.AddExpression(classExpression);
                if (classExpression != null)
                {
                    if (classExpression.ExpressionType == ExpressionType.Literal)
                    {
                        this.classType = ((LiteralExpression) classExpression).Token as TypeToken;
                    }
                    else if (classExpression.ExpressionType == ExpressionType.VariableDeclaration)
                    {
                        VariableDeclarationExpression expression = (VariableDeclarationExpression) classExpression;
                        this.classType = expression.Type;
                        foreach (VariableDeclaratorExpression expression2 in expression.Declarators)
                        {
                            this.identifier = expression2.Identifier.Text;
                            break;
                        }
                    }
                }
            }
            base.AddStatement(embeddedStatement);
        }

        public Expression CatchExpression
        {
            get
            {
                return this.catchExpression;
            }
        }

        public TypeToken ClassType
        {
            get
            {
                return this.classType;
            }
        }

        public BlockStatement EmbeddedStatement
        {
            get
            {
                return this.embeddedStatement;
            }
        }

        public string Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        public Microsoft.StyleCop.CSharp.TryStatement TryStatement
        {
            get
            {
                return this.tryStatement;
            }
        }
    }
}

