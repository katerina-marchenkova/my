namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class ArrayAccessExpression : Expression
    {
        private ICollection<Expression> arguments;
        private Expression array;

        internal ArrayAccessExpression(CsTokenList tokens, Expression array, ICollection<Expression> arguments) : base(ExpressionType.ArrayAccess, tokens)
        {
            this.array = array;
            this.arguments = arguments;
            base.AddExpression(array);
            base.AddExpressions(arguments);
        }

        public ICollection<Expression> Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        public Expression Array
        {
            get
            {
                return this.array;
            }
        }
    }
}

