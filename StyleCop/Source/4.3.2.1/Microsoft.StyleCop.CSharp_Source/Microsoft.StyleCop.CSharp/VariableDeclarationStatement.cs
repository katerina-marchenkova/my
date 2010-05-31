namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public sealed class VariableDeclarationStatement : Statement
    {
        private bool constant;
        private VariableDeclarationExpression expression;

        internal VariableDeclarationStatement(CsTokenList tokens, bool constant, VariableDeclarationExpression expression) : base(StatementType.VariableDeclaration, tokens)
        {
            this.constant = constant;
            this.expression = expression;
            base.AddExpression(expression);
        }

        public bool Constant
        {
            get
            {
                return this.constant;
            }
        }

        public ICollection<VariableDeclaratorExpression> Declarators
        {
            get
            {
                return this.expression.Declarators;
            }
        }

        public VariableDeclarationExpression InnerExpression
        {
            get
            {
                return this.expression;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification="API has already been published and should not be changed.")]
        public TypeToken Type
        {
            get
            {
                return this.expression.Type;
            }
        }
    }
}

