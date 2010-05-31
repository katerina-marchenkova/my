namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class ReturnStatement : Statement
    {
        private Expression returnValue;

        internal ReturnStatement(CsTokenList tokens, Expression returnValue) : base(StatementType.Return, tokens)
        {
            this.returnValue = returnValue;
            if (returnValue != null)
            {
                base.AddExpression(returnValue);
            }
        }

        public Expression ReturnValue
        {
            get
            {
                return this.returnValue;
            }
        }
    }
}

