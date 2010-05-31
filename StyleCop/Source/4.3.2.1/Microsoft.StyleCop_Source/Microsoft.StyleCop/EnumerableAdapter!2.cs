namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope="type", Justification="The type is an adapter for a collection, not a simple collection.")]
    public sealed class EnumerableAdapter<TOriginal, TAdapted> : IEnumerable<TAdapted>, IEnumerable
    {
        private AdapterConverterHandler<TOriginal, TAdapted> converter;
        private IEnumerable<TOriginal> innerEnumerable;

        public EnumerableAdapter(IEnumerable<TOriginal> enumerable, AdapterConverterHandler<TOriginal, TAdapted> converter)
        {
            Param.RequireNotNull(converter, "converter");
            this.innerEnumerable = enumerable;
            this.converter = converter;
        }

        private TAdapted Convert(TOriginal item)
        {
            return this.converter(item);
        }

        IEnumerator<TAdapted> IEnumerable<TAdapted>.GetEnumerator()
        {
            GetEnumerator_d__0 d__ = new GetEnumerator_d__0(0);
            d__.__4__this = (EnumerableAdapter<TOriginal, TAdapted>) this;
            return d__;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            GetEnumerator_d__5 d__ = new GetEnumerator_d__5(0);
            d__.__4__this = (EnumerableAdapter<TOriginal, TAdapted>) this;
            return d__;
        }

        public IEnumerable<TOriginal> InnerEnumerable
        {
            get
            {
                return this.innerEnumerable;
            }
        }

        [CompilerGenerated]
        private sealed class GetEnumerator_d__0 : IEnumerator<TAdapted>, IEnumerator, IDisposable
        {
            private int __1__state;
            private TAdapted __2__current;
            public EnumerableAdapter<TOriginal, TAdapted> __4__this;
            public IEnumerator<TOriginal> __7__wrap2;
            public TOriginal _item_5__1;

            [DebuggerHidden]
            public GetEnumerator_d__0(int __1__state)
            {
                this.__1__state = __1__state;
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
                            if (this.__4__this.innerEnumerable == null)
                            {
                                goto Label_009E;
                            }
                            this.__7__wrap2 = this.__4__this.innerEnumerable.GetEnumerator();
                            this.__1__state = 1;
                            goto Label_008B;

                        case 2:
                            this.__1__state = 1;
                            goto Label_008B;

                        default:
                            goto Label_009E;
                    }
                Label_0051:
                    this._item_5__1 = this.__7__wrap2.Current;
                    this.__2__current = this.__4__this.Convert(this._item_5__1);
                    this.__1__state = 2;
                    return true;
                Label_008B:
                    if (this.__7__wrap2.MoveNext())
                    {
                        goto Label_0051;
                    }
                    this.__m__Finally3();
                Label_009E:
                    flag = false;
                }
                finally
                {
                    Dispose();
                }
                return flag;
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

            TAdapted IEnumerator<TAdapted>.Current
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

        [CompilerGenerated]
        private sealed class GetEnumerator_d__5 : IEnumerator<object>, IEnumerator, IDisposable
        {
            private int __1__state;
            private object __2__current;
            public EnumerableAdapter<TOriginal, TAdapted> __4__this;
            public IEnumerator<TOriginal> __7__wrap7;
            public TOriginal _item_5__6;

            [DebuggerHidden]
            public GetEnumerator_d__5(int __1__state)
            {
                this.__1__state = __1__state;
            }

            private void __m__Finally8()
            {
                this.__1__state = -1;
                if (this.__7__wrap7 != null)
                {
                    this.__7__wrap7.Dispose();
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
                            if (this.__4__this.innerEnumerable == null)
                            {
                                goto Label_00A3;
                            }
                            this.__7__wrap7 = this.__4__this.innerEnumerable.GetEnumerator();
                            this.__1__state = 1;
                            goto Label_0090;

                        case 2:
                            this.__1__state = 1;
                            goto Label_0090;

                        default:
                            goto Label_00A3;
                    }
                Label_0051:
                    this._item_5__6 = this.__7__wrap7.Current;
                    this.__2__current = this.__4__this.Convert(this._item_5__6);
                    this.__1__state = 2;
                    return true;
                Label_0090:
                    if (this.__7__wrap7.MoveNext())
                    {
                        goto Label_0051;
                    }
                    this.__m__Finally8();
                Label_00A3:
                    flag = false;
                }
                finally
                {
                    Dispose();
                }
                return flag;
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

            object IEnumerator<object>.Current
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

