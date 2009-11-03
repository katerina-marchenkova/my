namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class YieldStatement : Statement
    {
        private Expression returnValue;
        private Type type;

        internal YieldStatement(CsTokenList tokens, Type type, Expression returnValue) : base(StatementType.Yield, tokens)
        {
            this.type = type;
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

        public Type YieldType
        {
            get
            {
                return this.type;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification="API has already been published and should not be changed.")]
        public enum Type
        {
            Break,
            Return
        }
    }
}

