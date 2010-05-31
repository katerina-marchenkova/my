namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public sealed class TryStatement : Statement
    {
        private ICollection<CatchStatement> catchStatements;
        private BlockStatement embeddedStatement;
        private Microsoft.StyleCop.CSharp.FinallyStatement finallyStatement;

        internal TryStatement(BlockStatement embeddedStatement) : base(StatementType.Try)
        {
            this.embeddedStatement = embeddedStatement;
            base.AddStatement(embeddedStatement);
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

        public ICollection<CatchStatement> CatchStatements
        {
            get
            {
                return this.catchStatements;
            }
            internal set
            {
                this.catchStatements = value;
            }
        }

        public BlockStatement EmbeddedStatement
        {
            get
            {
                return this.embeddedStatement;
            }
        }

        public Microsoft.StyleCop.CSharp.FinallyStatement FinallyStatement
        {
            get
            {
                return this.finallyStatement;
            }
            internal set
            {
                this.finallyStatement = value;
            }
        }

        [CompilerGenerated]
        private sealed class _get_AttachedStatements_d__0 : IEnumerable<Statement>, IEnumerable, IEnumerator<Statement>, IEnumerator, IDisposable
        {
            private int __1__state;
            private Statement __2__current;
            public TryStatement __4__this;
            public IEnumerator<CatchStatement> __7__wrap2;
            private int __l__initialThreadId;
            public CatchStatement _catchStatement_5__1;

            [DebuggerHidden]
            public _get_AttachedStatements_d__0(int __1__state)
            {
                this.__1__state = __1__state;
                this.__l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void __m__Finally3()
            {
                this.__1__state = -1;
                if (this.__7__wrap2 != null)
                {
                    this.__7__wrap2.Dispose();
                }
            }

            public bool MoveNext()
            {
                bool flag;
                try
                {
                    switch (this.__1__state)
                    {
                        case 0:
                            this.__1__state = -1;
                            if (this.__4__this.catchStatements.Count <= 0)
                            {
                                goto Label_009D;
                            }
                            this.__7__wrap2 = this.__4__this.catchStatements.GetEnumerator();
                            this.__1__state = 1;
                            goto Label_008A;

                        case 2:
                            this.__1__state = 1;
                            goto Label_008A;

                        case 3:
                            this.__1__state = -1;
                            goto Label_00CD;

                        default:
                            goto Label_00CD;
                    }
                Label_005B:
                    this._catchStatement_5__1 = this.__7__wrap2.Current;
                    this.__2__current = this._catchStatement_5__1;
                    this.__1__state = 2;
                    return true;
                Label_008A:
                    if (this.__7__wrap2.MoveNext())
                    {
                        goto Label_005B;
                    }
                    this.__m__Finally3();
                Label_009D:
                    if (this.__4__this.finallyStatement != null)
                    {
                        this.__2__current = this.__4__this.finallyStatement;
                        this.__1__state = 3;
                        return true;
                    }
                Label_00CD:
                    flag = false;
                }
                finally
                {
                    Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            public IEnumerator<Statement> GetEnumerator()
            {
                if ((Thread.CurrentThread.ManagedThreadId == this.__l__initialThreadId) && (this.__1__state == -2))
                {
                    this.__1__state = 0;
                    return this;
                }
                TryStatement._get_AttachedStatements_d__0 d__ = new TryStatement._get_AttachedStatements_d__0(0);
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

            public void Dispose()
            {
                switch (this.__1__state)
                {
                    case 1:
                    case 2:
                        break;

                    default:
                        return;
                }
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

