namespace Microsoft.StyleCop
{
    using System;

    public class Node<T> where T: class
    {
        private LinkedItemList<T> containingList;
        private NodeIndex index;
        private T item;
        private Node<T> next;
        private Node<T> previous;

        internal Node(T item)
        {
            this.item = item;
        }

        internal bool CreateIndex()
        {
            if (this.next == null)
            {
                if (this.previous == null)
                {
                    return NodeIndex.CreateFirst(out this.index);
                }
                return NodeIndex.CreateAfter(this.previous.index, out this.index);
            }
            if (this.previous == null)
            {
                return NodeIndex.CreateBefore(this.next.index, out this.index);
            }
            return NodeIndex.CreateBetween(this.previous.index, this.next.index, out this.index);
        }

        public bool NodesInSameList(Node<T> node)
        {
            if (node == null)
            {
                return false;
            }
            return (node.ContainingList == this.ContainingList);
        }

        internal LinkedItemList<T> ContainingList
        {
            get
            {
                return this.containingList;
            }
            set
            {
                this.containingList = value;
            }
        }

        public NodeIndex Index
        {
            get
            {
                return this.index;
            }
        }

        public Node<T> Next
        {
            get
            {
                return this.next;
            }
            internal set
            {
                this.next = value;
            }
        }

        public Node<T> Previous
        {
            get
            {
                return this.previous;
            }
            internal set
            {
                this.previous = value;
            }
        }

        public T Value
        {
            get
            {
                return this.item;
            }
        }
    }
}

