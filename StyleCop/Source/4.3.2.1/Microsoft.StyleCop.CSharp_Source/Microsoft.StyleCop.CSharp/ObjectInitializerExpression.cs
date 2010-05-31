namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class ObjectInitializerExpression : Expression
    {
        private ICollection<AssignmentExpression> initializers;

        internal ObjectInitializerExpression(CsTokenList tokens, ICollection<AssignmentExpression> initializers) : base(ExpressionType.ObjectInitializer, tokens)
        {
            this.initializers = initializers;
            foreach (AssignmentExpression expression in initializers)
            {
                base.AddExpression(expression);
            }
        }

        public ICollection<AssignmentExpression> Initializers
        {
            get
            {
                return this.initializers;
            }
        }
    }
}

