namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification="The class stores linked list of items.")]
    public class MasterList<T> : INodeList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T: class
    {
        private LinkedItemList<T> list;
        private bool readOnly;

        public event EventHandler NodeIndexesReset;

        public MasterList()
        {
            this.list = new LinkedItemList<T>();
            this.list.NodeIndexesReset += new EventHandler(this.ListNodeIndexesReset);
        }

        public MasterList(ICollection<T> items) : this()
        {
            Param.RequireNotNull(items, "items");
            this.list.AddRange(items);
        }

        private MasterList(LinkedItemList<T> innerList, bool readOnly)
        {
            this.list = new LinkedItemList<T>();
            this.list = innerList;
            this.readOnly = readOnly;
        }

        public virtual void Add(T item)
        {
            Param.RequireNotNull(item, "item");
            if (this.readOnly)
            {
                throw new NotSupportedException(Strings.ReadOnlyCollection);
            }
            this.InsertLast(item);
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            if (this.readOnly)
            {
                throw new NotSupportedException(Strings.ReadOnlyCollection);
            }
            this.list.AddRange(items);
        }

        public virtual void Clear()
        {
            if (this.readOnly)
            {
                throw new NotSupportedException(Strings.ReadOnlyCollection);
            }
            this.list.Clear();
        }

        public bool Contains(T item)
        {
            return this.list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        public IEnumerable<T> ForwardIterator()
        {
            return this.list.ForwardIterator();
        }

        public IEnumerable<T> ForwardIterator(Microsoft.StyleCop.Node<T> start)
        {
            return this.list.ForwardIterator(start);
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ForwardNodeIterator()
        {
            return this.list.ForwardNodeIterator();
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ForwardNodeIterator(Microsoft.StyleCop.Node<T> start)
        {
            return this.list.ForwardNodeIterator(start);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        public virtual Microsoft.StyleCop.Node<T> InsertAfter(T item, Microsoft.StyleCop.Node<T> nodeToInsertAfter)
        {
            Param.RequireNotNull(item, "item");
            if (this.readOnly)
            {
                throw new NotSupportedException(Strings.ReadOnlyCollection);
            }
            return this.list.InsertAfter(item, nodeToInsertAfter);
        }

        public virtual Microsoft.StyleCop.Node<T> InsertBefore(T item, Microsoft.StyleCop.Node<T> nodeToInsertBefore)
        {
            Param.RequireNotNull(item, "item");
            if (this.readOnly)
            {
                throw new NotSupportedException(Strings.ReadOnlyCollection);
            }
            return this.list.InsertBefore(item, nodeToInsertBefore);
        }

        public virtual Microsoft.StyleCop.Node<T> InsertFirst(T item)
        {
            Param.RequireNotNull(item, "item");
            if (this.readOnly)
            {
                throw new NotSupportedException(Strings.ReadOnlyCollection);
            }
            return this.list.InsertFirst(item);
        }

        public virtual Microsoft.StyleCop.Node<T> InsertLast(T item)
        {
            Param.RequireNotNull(item, "item");
            if (this.readOnly)
            {
                throw new NotSupportedException(Strings.ReadOnlyCollection);
            }
            return this.list.InsertLast(item);
        }

        private void ListNodeIndexesReset(object sender, EventArgs e)
        {
            this.OnNodeIndexesReset(e);
        }

        protected virtual void OnNodeIndexesReset(EventArgs e)
        {
            Param.RequireNotNull(e, "e");
            if (this.NodeIndexesReset != null)
            {
                this.NodeIndexesReset(this, e);
            }
        }

        public bool OutOfBounds(Microsoft.StyleCop.Node<T> node)
        {
            return (node == null);
        }

        public virtual bool Remove(T item)
        {
            Param.RequireNotNull(item, "item");
            if (this.readOnly)
            {
                throw new NotSupportedException(Strings.ReadOnlyCollection);
            }
            for (Microsoft.StyleCop.Node<T> node = this.First; node != null; node = node.Next)
            {
                if (node.Value == item)
                {
                    return this.Remove(node);
                }
            }
            return false;
        }

        public virtual bool Remove(Microsoft.StyleCop.Node<T> node)
        {
            if (this.readOnly)
            {
                throw new NotSupportedException(Strings.ReadOnlyCollection);
            }
            return this.list.Remove(node);
        }

        public virtual void RemoveRange(Microsoft.StyleCop.Node<T> startNode, Microsoft.StyleCop.Node<T> endNode)
        {
            if (this.readOnly)
            {
                throw new NotSupportedException(Strings.ReadOnlyCollection);
            }
            if (((startNode == null) && (endNode != null)) || ((startNode != null) && (endNode == null)))
            {
                throw new ArgumentException(Strings.BothItemsMustBeNullOrNonNull);
            }
            this.list.RemoveRange(startNode, endNode);
        }

        public virtual Microsoft.StyleCop.Node<T> Replace(Microsoft.StyleCop.Node<T> node, T newItem)
        {
            Param.RequireNotNull(node, "node");
            Param.RequireNotNull(newItem, "newItem");
            return this.list.Replace(node, newItem);
        }

        public IEnumerable<T> ReverseIterator()
        {
            return this.list.ReverseIterator();
        }

        public IEnumerable<T> ReverseIterator(Microsoft.StyleCop.Node<T> start)
        {
            return this.list.ReverseIterator(start);
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ReverseNodeIterator()
        {
            return this.list.ReverseNodeIterator();
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ReverseNodeIterator(Microsoft.StyleCop.Node<T> start)
        {
            return this.list.ReverseNodeIterator(start);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        public MasterList<T> AsReadOnly
        {
            get
            {
                if (this.readOnly)
                {
                    return (MasterList<T>) this;
                }
                return new MasterList<T>(this.list, true);
            }
        }

        internal MasterList<T> AsReadWrite
        {
            get
            {
                if (!this.readOnly)
                {
                    return (MasterList<T>) this;
                }
                return new MasterList<T>(this.list, false);
            }
        }

        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        public Microsoft.StyleCop.Node<T> First
        {
            get
            {
                return this.list.First;
            }
        }

        internal LinkedItemList<T> InnerList
        {
            get
            {
                return this.list;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.readOnly;
            }
        }

        public Microsoft.StyleCop.Node<T> Last
        {
            get
            {
                return this.list.Last;
            }
        }
    }
}

