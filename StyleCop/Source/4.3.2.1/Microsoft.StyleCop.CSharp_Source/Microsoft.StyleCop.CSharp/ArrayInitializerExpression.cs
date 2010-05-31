namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class ArrayInitializerExpression : Expression
    {
        private ICollection<Expression> initializers;

        internal ArrayInitializerExpression(CsTokenList tokens, ICollection<Expression> initializers) : base(ExpressionType.ArrayInitializer, tokens)
        {
            this.initializers = initializers;
            base.AddExpressions(initializers);
        }

        public ICollection<Expression> Initializers
        {
            get
            {
                return this.initializers;
            }
        }
    }
}

