namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class MethodInvocationExpression : Expression
    {
        private IList<Expression> arguments;
        private Expression name;

        internal MethodInvocationExpression(CsTokenList tokens, Expression name, IList<Expression> arguments) : base(ExpressionType.MethodInvocation, tokens)
        {
            this.name = name;
            this.arguments = arguments;
            base.AddExpression(name);
            base.AddExpressions(arguments);
        }

        public IList<Expression> Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        public Expression Name
        {
            get
            {
                return this.name;
            }
        }
    }
}

