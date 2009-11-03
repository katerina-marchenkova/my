namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class SwitchCaseStatement : Statement
    {
        private Expression identifier;

        internal SwitchCaseStatement(Expression identifier) : base(StatementType.SwitchCase)
        {
            this.identifier = identifier;
            base.AddExpression(identifier);
        }

        public Expression Identifier
        {
            get
            {
                return this.identifier;
            }
        }
    }
}

