namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class DefaultValueExpression : Expression
    {
        private TypeToken type;

        internal DefaultValueExpression(CsTokenList tokens, LiteralExpression type) : base(ExpressionType.DefaultValue, tokens)
        {
            this.type = CodeParser.ExtractTypeTokenFromLiteralExpression(type);
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
    }
}

