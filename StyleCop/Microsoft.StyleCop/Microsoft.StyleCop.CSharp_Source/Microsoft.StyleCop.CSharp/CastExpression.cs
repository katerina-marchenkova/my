namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class CastExpression : Expression
    {
        private Expression castedExpression;
        private TypeToken type;

        internal CastExpression(CsTokenList tokens, LiteralExpression type, Expression castedExpression) : base(ExpressionType.Cast, tokens)
        {
            this.type = CodeParser.ExtractTypeTokenFromLiteralExpression(type);
            this.castedExpression = castedExpression;
            base.AddExpression(type);
            base.AddExpression(castedExpression);
        }

        public Expression CastedExpression
        {
            get
            {
                return this.castedExpression;
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

