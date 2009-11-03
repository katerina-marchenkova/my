namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class LegacyEnumeratorAdapter<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        private IEnumerator innerEnumerator;

        public LegacyEnumeratorAdapter(IEnumerator enumerator)
        {
            this.innerEnumerator = enumerator;
        }

        public void Dispose()
        {
        }

        bool IEnumerator.MoveNext()
        {
            if (this.innerEnumerator == null)
            {
                return false;
            }
            return this.innerEnumerator.MoveNext();
        }

        void IEnumerator.Reset()
        {
            if (this.innerEnumerator != null)
            {
                this.innerEnumerator.Reset();
            }
        }

        public IEnumerator InnerEnumerator
        {
            get
            {
                return this.innerEnumerator;
            }
        }

        T IEnumerator<T>.Current
        {
            get
            {
                if (this.innerEnumerator == null)
                {
                    return default(T);
                }
                return (T) this.innerEnumerator.Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                if (this.innerEnumerator == null)
                {
                    return null;
                }
                return this.innerEnumerator.Current;
            }
        }
    }
}

