namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class IsExpression : Expression
    {
        private TypeToken type;
        private Expression value;

        internal IsExpression(CsTokenList tokens, Expression value, LiteralExpression type) : base(ExpressionType.Is, tokens)
        {
            this.value = value;
            this.type = CodeParser.ExtractTypeTokenFromLiteralExpression(type);
            base.AddExpression(value);
            base.AddExpression(type);
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification="API has already been published and should not be changed.")]
        public TypeToken Type
        {
            get
            {
                return this.type;
            }
        }

        public Expression Value
        {
            get
            {
                return this.value;
            }
        }
    }
}

