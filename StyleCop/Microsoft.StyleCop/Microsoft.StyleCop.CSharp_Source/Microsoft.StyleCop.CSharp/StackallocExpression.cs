namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class StackallocExpression : Expression
    {
        private ArrayAccessExpression type;

        internal StackallocExpression(CsTokenList tokens, ArrayAccessExpression type) : base(ExpressionType.Stackalloc, tokens)
        {
            this.type = type;
            base.AddExpression(type);
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

