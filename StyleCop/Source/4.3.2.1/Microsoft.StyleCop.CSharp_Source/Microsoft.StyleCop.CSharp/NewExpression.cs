namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class NewExpression : Expression
    {
        private Expression initializerExpression;
        private Expression typeCreationExpression;

        internal NewExpression(CsTokenList tokens, Expression typeCreationExpression, Expression initializerExpression) : base(ExpressionType.New, tokens)
        {
            this.typeCreationExpression = typeCreationExpression;
            this.initializerExpression = initializerExpression;
            if (typeCreationExpression != null)
            {
                base.AddExpression(typeCreationExpression);
            }
            if (initializerExpression != null)
            {
                base.AddExpression(initializerExpression);
            }
        }

        public Expression InitializerExpression
        {
            get
            {
                return this.initializerExpression;
            }
        }

        public Expression TypeCreationExpression
        {
            get
            {
                return this.typeCreationExpression;
            }
        }
    }
}

