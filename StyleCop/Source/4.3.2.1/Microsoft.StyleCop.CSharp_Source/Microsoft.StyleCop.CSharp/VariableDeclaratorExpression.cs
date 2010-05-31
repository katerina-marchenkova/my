namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class VariableDeclaratorExpression : Expression
    {
        private LiteralExpression identifier;
        private Expression initializer;
        private VariableDeclarationExpression parent;

        internal VariableDeclaratorExpression(CsTokenList tokens, LiteralExpression identifier, Expression initializer) : base(ExpressionType.VariableDeclarator, tokens)
        {
            this.identifier = identifier;
            this.initializer = initializer;
            base.AddExpression(identifier);
            if (initializer != null)
            {
                base.AddExpression(initializer);
            }
        }

        public LiteralExpression Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        public Expression Initializer
        {
            get
            {
                return this.initializer;
            }
        }

        public VariableDeclarationExpression ParentVariable
        {
            get
            {
                return this.parent;
            }
            internal set
            {
                this.parent = value;
            }
        }
    }
}

