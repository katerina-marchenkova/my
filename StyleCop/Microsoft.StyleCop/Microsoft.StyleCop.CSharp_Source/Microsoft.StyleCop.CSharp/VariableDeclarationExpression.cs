namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public sealed class VariableDeclarationExpression : Expression
    {
        private ICollection<VariableDeclaratorExpression> declarators;
        private TypeToken type;

        internal VariableDeclarationExpression(CsTokenList tokens, LiteralExpression type, ICollection<VariableDeclaratorExpression> declarators) : base(ExpressionType.VariableDeclaration, tokens)
        {
            this.declarators = declarators;
            base.AddExpression(type);
            this.type = CodeParser.ExtractTypeTokenFromLiteralExpression(type);
            foreach (VariableDeclaratorExpression expression in declarators)
            {
                base.AddExpression(expression);
                expression.ParentVariable = this;
            }
        }

        public ICollection<VariableDeclaratorExpression> Declarators
        {
            get
            {
                return this.declarators;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification="API has already been published and should not be changed.")]
        public TypeToken Type
        {
            get
            {
                return this.type;
            }
        }
    }
}

