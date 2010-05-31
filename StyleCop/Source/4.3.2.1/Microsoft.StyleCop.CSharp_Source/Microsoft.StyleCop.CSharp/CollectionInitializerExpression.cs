namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class CollectionInitializerExpression : Expression
    {
        internal CollectionInitializerExpression(CsTokenList tokens, IEnumerable<Expression> initializers) : base(ExpressionType.CollectionInitializer, tokens)
        {
            base.AddExpressions(initializers);
        }

        public ICollection<Expression> Initializers
        {
            get
            {
                return base.ChildExpressions;
            }
        }
    }
}

