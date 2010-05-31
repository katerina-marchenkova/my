namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class ForStatement : Statement
    {
        private Expression condition;
        private Statement embeddedStatement;
        private ICollection<Expression> initializers;
        private ICollection<Expression> iterators;

        internal ForStatement(CsTokenList tokens, ICollection<Expression> initializers, Expression condition, ICollection<Expression> iterators) : base(StatementType.For, tokens)
        {
            this.initializers = initializers;
            this.condition = condition;
            this.iterators = iterators;
            base.AddExpressions(initializers);
            if (condition != null)
            {
                base.AddExpression(condition);
            }
            base.AddExpressions(iterators);
        }

        public Expression Condition
        {
            get
            {
                return this.condition;
            }
        }

        public Statement EmbeddedStatement
        {
            get
            {
                return this.embeddedStatement;
            }
            internal set
            {
                this.embeddedStatement = value;
                base.AddStatement(this.embeddedStatement);
            }
        }

        public ICollection<Expression> Initializers
        {
            get
            {
                return this.initializers;
            }
        }

        public ICollection<Expression> Iterators
        {
            get
            {
                return this.iterators;
            }
        }
    }
}

