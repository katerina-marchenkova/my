namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification="The class stores a linked list.")]
    public class ItemList<T> : INodeList<T>, IEnumerable<T>, IEnumerable where T: class
    {
        private Microsoft.StyleCop.Node<T> firstItem;
        private Microsoft.StyleCop.Node<T> lastItem;
        private MasterList<T> masterList;

        public ItemList(MasterList<T> masterList)
        {
            Param.RequireNotNull(masterList, "masterList");
            this.masterList = masterList;
        }

        public ItemList(MasterList<T> masterList, Microsoft.StyleCop.Node<T> firstItem, Microsoft.StyleCop.Node<T> lastItem)
        {
            Param.RequireNotNull(masterList, "masterList");
            Param.RequireNotNull(firstItem, "firstItem");
            Param.RequireNotNull(lastItem, "lastItem");
            this.masterList = masterList.AsReadWrite;
            this.firstItem = firstItem;
            this.lastItem = lastItem;
        }

        public virtual ItemList<T> Clone()
        {
            return new ItemList<T>(this.masterList, this.firstItem, this.lastItem);
        }

        public IEnumerable<T> ForwardIterator()
        {
            return new LinkedItemListEnumerators<T>.ForwardValueEnumerable(this.firstItem, this.lastItem);
        }

        public IEnumerable<T> ForwardIterator(Microsoft.StyleCop.Node<T> start)
        {
            if (this.IsAfterLast(start))
            {
                return new T[0];
            }
            return new LinkedItemListEnumerators<T>.ForwardValueEnumerable(start, this.lastItem);
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ForwardNodeIterator()
        {
            return new LinkedItemListEnumerators<T>.ForwardNodeEnumerable(this.firstItem, this.lastItem);
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ForwardNodeIterator(Microsoft.StyleCop.Node<T> start)
        {
            if (this.IsAfterLast(start))
            {
                return new Microsoft.StyleCop.Node<T>[0];
            }
            return new LinkedItemListEnumerators<T>.ForwardNodeEnumerable(start, this.lastItem);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LinkedItemListEnumerators<T>.ForwardValueEnumerator(this.firstItem, this.lastItem);
        }

        private bool IsAfterLast(Microsoft.StyleCop.Node<T> node)
        {
            if ((node != null) && (this.lastItem != null))
            {
                return (node.Index > this.lastItem.Index);
            }
            return true;
        }

        private bool IsBeforeFirst(Microsoft.StyleCop.Node<T> node)
        {
            if ((node != null) && (this.firstItem != null))
            {
                return (node.Index < this.firstItem.Index);
            }
            return true;
        }

        public bool OutOfBounds(Microsoft.StyleCop.Node<T> node)
        {
            if ((((node != null) && (this.firstItem != null)) && (this.lastItem != null)) && (node.Index >= this.firstItem.Index))
            {
                return (node.Index > this.lastItem.Index);
            }
            return true;
        }

        public IEnumerable<T> ReverseIterator()
        {
            return new LinkedItemListEnumerators<T>.BackwardValueEnumerable(this.lastItem, this.firstItem);
        }

        public IEnumerable<T> ReverseIterator(Microsoft.StyleCop.Node<T> start)
        {
            if (this.IsBeforeFirst(start))
            {
                return new T[0];
            }
            return new LinkedItemListEnumerators<T>.BackwardValueEnumerable(start, this.firstItem);
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ReverseNodeIterator()
        {
            return new LinkedItemListEnumerators<T>.BackwardNodeEnumerable(this.lastItem, this.firstItem);
        }

        public IEnumerable<Microsoft.StyleCop.Node<T>> ReverseNodeIterator(Microsoft.StyleCop.Node<T> start)
        {
            if (this.IsBeforeFirst(start))
            {
                return new Microsoft.StyleCop.Node<T>[0];
            }
            return new LinkedItemListEnumerators<T>.BackwardNodeEnumerable(start, this.firstItem);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Microsoft.StyleCop.Node<T> First
        {
            get
            {
                return this.firstItem;
            }
            protected set
            {
                this.firstItem = value;
            }
        }

        public Microsoft.StyleCop.Node<T> Last
        {
            get
            {
                return this.lastItem;
            }
            protected set
            {
                this.lastItem = value;
            }
        }

        public MasterList<T> MasterList
        {
            get
            {
                return this.masterList.AsReadOnly;
            }
        }
    }
}

