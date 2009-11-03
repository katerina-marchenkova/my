namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class SizeofExpression : Expression
    {
        private Expression type;

        internal SizeofExpression(CsTokenList tokens, Expression type) : base(ExpressionType.Sizeof, tokens)
        {
            this.type = type;
            base.AddExpression(type);
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification="API has already been published and should not be changed.")]
        public Expression Type
        {
            get
            {
                return this.type;
            }
        }
    }
}

