namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public sealed class IfStatement : Statement
    {
        private Expression conditionExpression;
        private ElseStatement elseStatement;
        private Statement embeddedStatement;

        internal IfStatement(CsTokenList tokens, Expression conditionExpression) : base(StatementType.If, tokens)
        {
            this.conditionExpression = conditionExpression;
            base.AddExpression(conditionExpression);
        }

        public ElseStatement AttachedElseStatement
        {
            get
            {
                return this.elseStatement;
            }
            internal set
            {
                this.elseStatement = value;
            }
        }

        public override IEnumerable<Statement> AttachedStatements
        {
            get
            {
                _get_AttachedStatements_d__0 d__ = new _get_AttachedStatements_d__0(-2);
                d__.__4__this = this;
                return d__;
            }
        }

        public Expression ConditionExpression
        {
            get
            {
                return this.conditionExpression;
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

        [CompilerGenerated]
        private sealed class _get_AttachedStatements_d__0 : IEnumerable<Statement>, IEnumerable, IEnumerator<Statement>, IEnumerator, IDisposable
        {
            private int __1__state;
            private Statement __2__current;
            public IfStatement __4__this;
            private int __l__initialThreadId;
            public ElseStatement _elseStatement_5__1;

            [DebuggerHidden]
            public _get_AttachedStatements_d__0(int __1__state)
            {
                this.__1__state = __1__state;
                this.__l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            public bool MoveNext()
            {
                switch (this.__1__state)
                {
                    case 0:
                        this.__1__state = -1;
                        this._elseStatement_5__1 = this.__4__this.elseStatement;
                        break;

                    case 1:
                        this.__1__state = -1;
                        this._elseStatement_5__1 = this._elseStatement_5__1.AttachedElseStatement;
                        break;

                    default:
                        goto Label_0066;
                }
                if (this._elseStatement_5__1 != null)
                {
                    this.__2__current = this._elseStatement_5__1;
                    this.__1__state = 1;
                    return true;
                }
            Label_0066:
                return false;
            }

            [DebuggerHidden]
            public IEnumerator<Statement> GetEnumerator()
            {
                if ((Thread.CurrentThread.ManagedThreadId == this.__l__initialThreadId) && (this.__1__state == -2))
                {
                    this.__1__state = 0;
                    return this;
                }
                IfStatement._get_AttachedStatements_d__0 d__ = new IfStatement._get_AttachedStatements_d__0(0);
                d__.__4__this = this.__4__this;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            Statement IEnumerator<Statement>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.__2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.__2__current;
                }
            }
        }
    }
}

