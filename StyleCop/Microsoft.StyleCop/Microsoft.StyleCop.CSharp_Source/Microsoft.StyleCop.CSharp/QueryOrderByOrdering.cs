namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct QueryOrderByOrdering
    {
        private QueryOrderByDirection direction;
        private Microsoft.StyleCop.CSharp.Expression expression;
        public QueryOrderByDirection Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                this.direction = value;
            }
        }
        public Microsoft.StyleCop.CSharp.Expression Expression
        {
            get
            {
                return this.expression;
            }
            set
            {
                this.expression = value;
            }
        }
        public static bool operator ==(QueryOrderByOrdering item1, QueryOrderByOrdering item2)
        {
            return ((item1.Expression == item2.Expression) && (item1.Direction == item2.Direction));
        }

        public static bool operator !=(QueryOrderByOrdering item1, QueryOrderByOrdering item2)
        {
            if (item1.Expression == item2.Expression)
            {
                return (item1.Direction != item2.Direction);
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            QueryOrderByOrdering ordering = (QueryOrderByOrdering) obj;
            return ((ordering.Expression == this.expression) && (ordering.Direction == this.direction));
        }

        public override int GetHashCode()
        {
            return (this.expression.GetHashCode() ^ this.direction.GetHashCode());
        }
    }
}

