namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal static class LinkedItemListEnumerators<T> where T: class
    {
        public class BackwardNodeEnumerable : IEnumerable<Microsoft.StyleCop.Node<T>>, IEnumerable
        {
            private Microsoft.StyleCop.Node<T> endItem;
            private Microsoft.StyleCop.Node<T> startItem;

            public BackwardNodeEnumerable(Microsoft.StyleCop.Node<T> startItem, Microsoft.StyleCop.Node<T> endItem)
            {
                this.startItem = startItem;
                this.endItem = endItem;
            }

            public IEnumerator<Microsoft.StyleCop.Node<T>> GetEnumerator()
            {
                return new LinkedItemListEnumerators<T>.BackwardNodeEnumerator(this.startItem, this.endItem);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public class BackwardNodeEnumerator : IEnumerator<Microsoft.StyleCop.Node<T>>, IDisposable, IEnumerator
        {
            private Microsoft.StyleCop.Node<T> currentItem;
            private Microsoft.StyleCop.Node<T> endItem;
            private Microsoft.StyleCop.Node<T> startItem;

            public BackwardNodeEnumerator(Microsoft.StyleCop.Node<T> startItem, Microsoft.StyleCop.Node<T> endItem)
            {
                this.startItem = startItem;
                this.endItem = endItem;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            public bool MoveNext()
            {
                if ((this.startItem == null) || (this.endItem == null))
                {
                    return false;
                }
                if (this.currentItem == null)
                {
                    this.currentItem = this.startItem;
                }
                else if (this.currentItem == this.endItem)
                {
                    this.currentItem = null;
                }
                else
                {
                    this.currentItem = this.currentItem.Previous;
                }
                return (this.currentItem != null);
            }

            public void Reset()
            {
                this.currentItem = null;
            }

            public Microsoft.StyleCop.Node<T> Current
            {
                get
                {
                    return this.currentItem;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.currentItem;
                }
            }
        }

        public class BackwardValueEnumerable : IEnumerable<T>, IEnumerable
        {
            private Microsoft.StyleCop.Node<T> endItem;
            private Microsoft.StyleCop.Node<T> startItem;

            public BackwardValueEnumerable(Microsoft.StyleCop.Node<T> startItem, Microsoft.StyleCop.Node<T> endItem)
            {
                this.startItem = startItem;
                this.endItem = endItem;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new LinkedItemListEnumerators<T>.BackwardValueEnumerator(this.startItem, this.endItem);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public class BackwardValueEnumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private Microsoft.StyleCop.Node<T> currentItem;
            private Microsoft.StyleCop.Node<T> endItem;
            private Microsoft.StyleCop.Node<T> startItem;

            public BackwardValueEnumerator(Microsoft.StyleCop.Node<T> startItem, Microsoft.StyleCop.Node<T> endItem)
            {
                this.startItem = startItem;
                this.endItem = endItem;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            public bool MoveNext()
            {
                if ((this.startItem == null) || (this.endItem == null))
                {
                    return false;
                }
                if (this.currentItem == null)
                {
                    this.currentItem = this.startItem;
                }
                else if (this.currentItem == this.endItem)
                {
                    this.currentItem = null;
                }
                else
                {
                    this.currentItem = this.currentItem.Previous;
                }
                return (this.currentItem != null);
            }

            public void Reset()
            {
                this.currentItem = null;
            }

            public T Current
            {
                get
                {
                    if (this.currentItem != null)
                    {
                        return this.currentItem.Value;
                    }
                    return default(T);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return ((this.currentItem == null) ? default(T) : this.currentItem.Value);
                }
            }
        }

        public class ForwardNodeEnumerable : IEnumerable<Microsoft.StyleCop.Node<T>>, IEnumerable
        {
            private Microsoft.StyleCop.Node<T> endItem;
            private Microsoft.StyleCop.Node<T> startItem;

            public ForwardNodeEnumerable(Microsoft.StyleCop.Node<T> startItem, Microsoft.StyleCop.Node<T> endItem)
            {
                this.startItem = startItem;
                this.endItem = endItem;
            }

            public IEnumerator<Microsoft.StyleCop.Node<T>> GetEnumerator()
            {
                return new LinkedItemListEnumerators<T>.ForwardNodeEnumerator(this.startItem, this.endItem);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public class ForwardNodeEnumerator : IEnumerator<Microsoft.StyleCop.Node<T>>, IDisposable, IEnumerator
        {
            private Microsoft.StyleCop.Node<T> currentItem;
            private Microsoft.StyleCop.Node<T> endItem;
            private Microsoft.StyleCop.Node<T> startItem;

            public ForwardNodeEnumerator(Microsoft.StyleCop.Node<T> startItem, Microsoft.StyleCop.Node<T> endItem)
            {
                this.startItem = startItem;
                this.endItem = endItem;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            public bool MoveNext()
            {
                if ((this.startItem == null) || (this.endItem == null))
                {
                    return false;
                }
                if (this.currentItem == null)
                {
                    this.currentItem = this.startItem;
                }
                else if (this.currentItem == this.endItem)
                {
                    this.currentItem = null;
                }
                else
                {
                    this.currentItem = this.currentItem.Next;
                }
                return (this.currentItem != null);
            }

            public void Reset()
            {
                this.currentItem = null;
            }

            public Microsoft.StyleCop.Node<T> Current
            {
                get
                {
                    return this.currentItem;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.currentItem;
                }
            }
        }

        public class ForwardValueEnumerable : IEnumerable<T>, IEnumerable
        {
            private Microsoft.StyleCop.Node<T> endItem;
            private Microsoft.StyleCop.Node<T> startItem;

            public ForwardValueEnumerable(Microsoft.StyleCop.Node<T> startItem, Microsoft.StyleCop.Node<T> endItem)
            {
                this.startItem = startItem;
                this.endItem = endItem;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new LinkedItemListEnumerators<T>.ForwardValueEnumerator(this.startItem, this.endItem);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public class ForwardValueEnumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private Microsoft.StyleCop.Node<T> currentItem;
            private Microsoft.StyleCop.Node<T> endItem;
            private Microsoft.StyleCop.Node<T> startItem;

            public ForwardValueEnumerator(Microsoft.StyleCop.Node<T> startItem, Microsoft.StyleCop.Node<T> endItem)
            {
                this.startItem = startItem;
                this.endItem = endItem;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            public bool MoveNext()
            {
                if ((this.startItem == null) || (this.endItem == null))
                {
                    return false;
                }
                if (this.currentItem == null)
                {
                    this.currentItem = this.startItem;
                }
                else if (this.currentItem == this.endItem)
                {
                    this.currentItem = null;
                }
                else
                {
                    this.currentItem = this.currentItem.Next;
                }
                return (this.currentItem != null);
            }

            public void Reset()
            {
                this.currentItem = null;
            }

            public T Current
            {
                get
                {
                    if (this.currentItem != null)
                    {
                        return this.currentItem.Value;
                    }
                    return default(T);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }
        }
    }
}

