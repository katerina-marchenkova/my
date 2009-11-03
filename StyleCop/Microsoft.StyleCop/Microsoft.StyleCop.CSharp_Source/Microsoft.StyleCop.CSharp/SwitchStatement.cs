namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class SwitchStatement : Statement
    {
        private ICollection<SwitchCaseStatement> caseStatements;
        private SwitchDefaultStatement defaultStatement;
        private Expression switchItem;

        internal SwitchStatement(CsTokenList tokens, Expression switchItem, ICollection<SwitchCaseStatement> caseStatements, SwitchDefaultStatement defaultStatement) : base(StatementType.Switch, tokens)
        {
            this.switchItem = switchItem;
            this.caseStatements = caseStatements;
            this.defaultStatement = defaultStatement;
            base.AddExpression(switchItem);
            foreach (Statement statement in caseStatements)
            {
                base.AddStatement(statement);
            }
            if (defaultStatement != null)
            {
                base.AddStatement(defaultStatement);
            }
        }

        public ICollection<SwitchCaseStatement> CaseStatements
        {
            get
            {
                return this.caseStatements;
            }
        }

        public SwitchDefaultStatement DefaultStatement
        {
            get
            {
                return this.defaultStatement;
            }
        }

        public Expression SwitchItem
        {
            get
            {
                return this.switchItem;
            }
        }
    }
}

