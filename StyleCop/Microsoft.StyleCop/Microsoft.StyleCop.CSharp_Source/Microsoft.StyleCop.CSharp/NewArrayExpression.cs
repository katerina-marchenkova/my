namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class NewArrayExpression : Expression
    {
        private ArrayInitializerExpression initializer;
        private ArrayAccessExpression type;

        internal NewArrayExpression(CsTokenList tokens, ArrayAccessExpression type, ArrayInitializerExpression initializer) : base(ExpressionType.NewArray, tokens)
        {
            this.type = type;
            this.initializer = initializer;
            if (type != null)
            {
                base.AddExpression(type);
            }
            if (initializer != null)
            {
                base.AddExpression(initializer);
            }
        }

        public ArrayInitializerExpression Initializer
        {
            get
            {
                return this.initializer;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification="API has already been published and should not be changed.")]
        public ArrayAccessExpression Type
        {
            get
            {
                return this.type;
            }
        }
    }
}

