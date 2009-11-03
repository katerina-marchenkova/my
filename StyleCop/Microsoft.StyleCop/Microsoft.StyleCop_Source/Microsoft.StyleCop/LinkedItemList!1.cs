namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class LinkedItemList<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T: class
    {
        private int count;
        private Microsoft.StyleCop.Node<T> head;
        private Microsoft.StyleCop.Node<T> tail;

        public event EventHandler NodeIndexesReset;

        internal LinkedItemList()
        {
        }

        internal LinkedItemList(ICollection<T> items)
        {
            if (items != null)
            {
                this.AddRange(items);
            }
        }

        internal LinkedItemList(ICollection<Microsoft.StyleCop.Node<T>> nodes)
        {
            if (nodes != null)
            {
                this.AddRange((IEnumerable<Microsoft.StyleCop.Node<T>>) nodes);
            }
        }

        public void Add(T item)
        {
            this.InsertLast(item);
        }

        public void Add(Microsoft.StyleCop.Node<T> node)
        {
            this.InsertAfter(node, this.tail);
        }

        public void AddRange(IEnumerable<Microsoft.StyleCop.Node<T>> nodes)
        {
            if (nodes != null)
            {
                Microsoft.StyleCop.Node<T> tail = this.tail;
                foreach (Microsoft.StyleCop.Node<T> node2 in nodes)
                {
                    if (node2.ContainingList != null)
                    {
                        throw new ArgumentException(Strings.NodeAlreadyInCollection);
                    }
                    node2.ContainingList = (LinkedItemList<T>) this;
                    if (tail != null)
                    {
                        tail.Next = node2;
                    }
                    node2.Previous = tail;
                    this.tail = node2;
                    node2.Next = null;
                    if (this.head == null)
                    {
                        this.head = node2;
                    }
                    this.count++;
                    tail = node2;
                    if (!node2.CreateIndex())
                    {
                        this.ResetNodeIndexes();
                    }
                }
            }
        }

        public ICollection<Microsoft.StyleCop.Node<T>> AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                return null;
            }
            List<Microsoft.StyleCop.Node<T>> list = new List<Microsoft.StyleCop.Node<T>>();
            foreach (T local in items)
            {
                list.Add(new Microsoft.StyleCop.Node<T>(local));
            }
            this.AddRange((IEnumerable<Microsoft.StyleCop.Node<T>>) list);
            return list;
        }

        public void Clear()
        {
            for (Microsoft.StyleCop.Node<T> node = this.head; node != null; node = node.Next)
            {
                node.ContainingList = null;
                if (node == this.tail)
                {
                    break;
                }
            }
            this.head = null;
            this.tail = null;
            this.count = 0;
        }

        public bool Contains(Microsoft.StyleCop.Node<T> node)
        {
            for (Microsoft.StyleCop.Node<T> node2 = this.head; node2 != null; node2 = node2.Next)
            {
                if (node2 == node)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(T item)
        {
            for (Microsoft.StyleCop.Node<T> node = this.head; node != null; node = node.Next)
            {
                if (node.Value == item)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int num = arrayIndex;
            for (Microsoft.StyleCop.Node<T> node = this.head; node != null; node = node.Next)
            {
                array[num++] = node.Value;
            }
        }

        public IEnumerable<T> ForwardIterator()
        {
            return this.ForwardIterator(this.head);
        }

        public IEnumerable<T> ForwardIterator(Microsoft.StyleCop.Node<T> start)
        {
            return new LinkedItemListEnumerators<T>.ForwardValueEnumerable(start, this.tail);
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ForwardNodeIterator()
        {
            return this.ForwardNodeIterator(this.head);
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ForwardNodeIterator(Microsoft.StyleCop.Node<T> start)
        {
            return new LinkedItemListEnumerators<T>.ForwardNodeEnumerable(start, this.tail);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LinkedItemListEnumerators<T>.ForwardValueEnumerator(this.head, this.tail);
        }

        public Microsoft.StyleCop.Node<T> InsertAfter(T item, Microsoft.StyleCop.Node<T> nodeToInsertAfter)
        {
            return this.InsertAfter(new Microsoft.StyleCop.Node<T>(item), nodeToInsertAfter);
        }

        private Microsoft.StyleCop.Node<T> InsertAfter(Microsoft.StyleCop.Node<T> node, Microsoft.StyleCop.Node<T> nodeToInsertAfter)
        {
            if (node.ContainingList != null)
            {
                throw new ArgumentException(Strings.NodeAlreadyInCollection);
            }
            node.ContainingList = (LinkedItemList<T>) this;
            if (this.head == null)
            {
                this.head = node;
                this.tail = node;
                node.Next = null;
                node.Previous = null;
            }
            else if (nodeToInsertAfter.Equals(this.tail))
            {
                this.tail.Next = node;
                node.Next = null;
                node.Previous = this.tail;
                this.tail = node;
            }
            else
            {
                nodeToInsertAfter.Next.Previous = node;
                node.Next = nodeToInsertAfter.Next;
                nodeToInsertAfter.Next = node;
                node.Previous = nodeToInsertAfter;
            }
            this.count++;
            if (!node.CreateIndex())
            {
                this.ResetNodeIndexes();
            }
            return node;
        }

        public Microsoft.StyleCop.Node<T> InsertBefore(T item, Microsoft.StyleCop.Node<T> nodeToInsertBefore)
        {
            return this.InsertBefore(new Microsoft.StyleCop.Node<T>(item), nodeToInsertBefore);
        }

        private Microsoft.StyleCop.Node<T> InsertBefore(Microsoft.StyleCop.Node<T> node, Microsoft.StyleCop.Node<T> nodeToInsertBefore)
        {
            if (node.ContainingList != null)
            {
                throw new ArgumentException(Strings.NodeAlreadyInCollection);
            }
            node.ContainingList = (LinkedItemList<T>) this;
            if (this.head == null)
            {
                this.head = node;
                this.tail = node;
                node.Next = null;
                node.Previous = null;
            }
            else if (nodeToInsertBefore.Equals(this.head))
            {
                this.head.Previous = node;
                node.Previous = null;
                node.Next = this.head;
                this.head = node;
            }
            else
            {
                nodeToInsertBefore.Previous.Next = node;
                node.Previous = nodeToInsertBefore.Previous;
                nodeToInsertBefore.Previous = node;
                node.Next = nodeToInsertBefore;
            }
            if (!node.CreateIndex())
            {
                this.ResetNodeIndexes();
            }
            this.count++;
            return node;
        }

        public Microsoft.StyleCop.Node<T> InsertFirst(T item)
        {
            return this.InsertBefore(item, this.head);
        }

        public Microsoft.StyleCop.Node<T> InsertLast(T item)
        {
            return this.InsertAfter(item, this.tail);
        }

        protected virtual void OnNodeIndexesReset(EventArgs e)
        {
            Param.RequireNotNull(e, "e");
            if (this.NodeIndexesReset != null)
            {
                this.NodeIndexesReset(this, e);
            }
        }

        public bool Remove(T item)
        {
            for (Microsoft.StyleCop.Node<T> node = this.head; node != null; node = node.Next)
            {
                if (node.Value.Equals(item))
                {
                    return this.Remove(node);
                }
            }
            return false;
        }

        public bool Remove(Microsoft.StyleCop.Node<T> node)
        {
            if (node.ContainingList != this)
            {
                return false;
            }
            if (this.head == null)
            {
                return false;
            }
            if (this.head.Equals(this.tail))
            {
                if (!this.head.Equals(node))
                {
                    return false;
                }
                this.head = null;
                this.tail = null;
            }
            else if (this.head.Equals(node))
            {
                this.head.Next.Previous = null;
                this.head = this.head.Next;
            }
            else if (this.tail.Equals(node))
            {
                this.tail.Previous.Next = null;
                this.tail = this.tail.Previous;
            }
            else
            {
                node.Previous.Next = node.Next;
                node.Next.Previous = node.Previous;
            }
            node.Next = null;
            node.Previous = null;
            node.ContainingList = null;
            this.count--;
            return true;
        }

        public void RemoveRange(Microsoft.StyleCop.Node<T> start, Microsoft.StyleCop.Node<T> end)
        {
            for (Microsoft.StyleCop.Node<T> node = start; node != null; node = node.Next)
            {
                this.count--;
                node.ContainingList = null;
                if (node == end)
                {
                    break;
                }
            }
            if (start.Equals(this.head))
            {
                this.head = end.Next;
            }
            if (end.Equals(this.tail))
            {
                this.tail = start.Previous;
            }
            if (start.Previous != null)
            {
                start.Previous.Next = end.Next;
            }
            if (end.Next != null)
            {
                end.Next.Previous = start.Previous;
            }
            start.Previous = null;
            end.Next = null;
        }

        public Microsoft.StyleCop.Node<T> Replace(Microsoft.StyleCop.Node<T> node, T newItem)
        {
            Microsoft.StyleCop.Node<T> newNode = new Microsoft.StyleCop.Node<T>(newItem);
            this.Replace(node, newNode);
            return newNode;
        }

        public void Replace(Microsoft.StyleCop.Node<T> node, Microsoft.StyleCop.Node<T> newNode)
        {
            newNode.Next = node.Next;
            newNode.Previous = node.Previous;
            if (node.Previous != null)
            {
                node.Previous.Next = newNode;
            }
            if (node.Next != null)
            {
                node.Next.Previous = newNode;
            }
            if (newNode.ContainingList != null)
            {
                throw new ArgumentException(Strings.NodeAlreadyInCollection);
            }
            node.ContainingList = null;
            newNode.ContainingList = (LinkedItemList<T>) this;
            if (!newNode.CreateIndex())
            {
                this.ResetNodeIndexes();
            }
        }

        private void ResetNodeIndexes()
        {
            int num = 5;
            while (num > 0)
            {
                if ((this.count * (num + 1)) < (Math.Abs(-2147483648) + 0x7fffffff))
                {
                    break;
                }
                num--;
            }
            int num2 = this.count * (num + 1);
            if (num2 >= (Math.Abs(-2147483648) + 0x7fffffff))
            {
                throw new InvalidOperationException();
            }
            int newBigValue = -(num2 / 2);
            for (Microsoft.StyleCop.Node<T> node = this.First; node != null; node = node.Next)
            {
                node.Index.Set(newBigValue);
                newBigValue += num + 1;
            }
            this.OnNodeIndexesReset(new EventArgs());
        }

        public IEnumerable<T> ReverseIterator()
        {
            return this.ReverseIterator(this.tail);
        }

        public IEnumerable<T> ReverseIterator(Microsoft.StyleCop.Node<T> start)
        {
            return new LinkedItemListEnumerators<T>.BackwardValueEnumerable(start, this.head);
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ReverseNodeIterator()
        {
            return this.ReverseNodeIterator(this.tail);
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ReverseNodeIterator(Microsoft.StyleCop.Node<T> start)
        {
            return new LinkedItemListEnumerators<T>.BackwardNodeEnumerable(start, this.head);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public Microsoft.StyleCop.Node<T> First
        {
            get
            {
                return this.head;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public Microsoft.StyleCop.Node<T> Last
        {
            get
            {
                return this.tail;
            }
        }
    }
}

