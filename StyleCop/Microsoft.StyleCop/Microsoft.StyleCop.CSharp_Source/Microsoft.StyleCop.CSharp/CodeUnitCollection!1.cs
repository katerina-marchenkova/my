namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class CodeUnitCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T: IWriteableCodeUnit
    {
        private List<T> items;
        private ICodeUnit parent;

        public CodeUnitCollection(ICodeUnit parent)
        {
            this.parent = parent;
        }

        internal void Add(T item)
        {
            this.InitializeItem(item);
            if (this.items == null)
            {
                this.items = new List<T>();
            }
            this.items.Add(item);
        }

        internal void AddRange(IEnumerable<T> codeUnits)
        {
            if (this.items == null)
            {
                this.items = new List<T>();
            }
            if (codeUnits != null)
            {
                foreach (T local in codeUnits)
                {
                    this.InitializeItem(local);
                }
            }
            this.items.AddRange(codeUnits);
        }

        internal void Clear()
        {
            if (this.items != null)
            {
                for (int i = 0; i < this.items.Count; i++)
                {
                    this.UninitializeItem(this.items[i]);
                }
                if (this.items != null)
                {
                    this.items.Clear();
                }
            }
        }

        public bool Contains(T item)
        {
            return this.items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        protected virtual void InitializeItem(T item)
        {
            if (item != null)
            {
                item.SetParent(this.parent);
            }
        }

        internal bool Remove(T item)
        {
            if (item != null)
            {
                this.UninitializeItem(item);
            }
            return ((this.items != null) && this.items.Remove(item));
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        protected virtual void UninitializeItem(T item)
        {
            if (item != null)
            {
                item.SetParent(null);
            }
        }

        public int Count
        {
            get
            {
                if (this.items == null)
                {
                    return 0;
                }
                return this.items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public ICodeUnit Parent
        {
            get
            {
                return this.parent;
            }
        }
    }
}

