namespace Microsoft.StyleCop
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NodeIndex
    {
        internal const int Spacer = 5;
        private int bigValue;
        private short smallValue;
        public static bool operator ==(NodeIndex index1, NodeIndex index2)
        {
            return ((index1.bigValue == index2.bigValue) && (index1.smallValue == index2.smallValue));
        }

        public static bool operator !=(NodeIndex index1, NodeIndex index2)
        {
            if (index1.bigValue == index2.bigValue)
            {
                return (index1.smallValue != index2.smallValue);
            }
            return true;
        }

        public static bool operator <(NodeIndex index1, NodeIndex index2)
        {
            return ((index1.bigValue < index2.bigValue) || ((index1.bigValue == index2.bigValue) && (index1.smallValue < index2.smallValue)));
        }

        public static bool operator >(NodeIndex index1, NodeIndex index2)
        {
            return ((index1.bigValue > index2.bigValue) || ((index1.bigValue == index2.bigValue) && (index1.smallValue > index2.smallValue)));
        }

        public static bool operator <=(NodeIndex index1, NodeIndex index2)
        {
            return ((index1.bigValue <= index2.bigValue) && (index1.smallValue <= index2.smallValue));
        }

        public static bool operator >=(NodeIndex index1, NodeIndex index2)
        {
            return ((index1.bigValue >= index2.bigValue) && (index1.smallValue >= index2.smallValue));
        }

        public static int Compare(NodeIndex index1, NodeIndex index2)
        {
            if (index1 < index2)
            {
                return -1;
            }
            if (index1 > index2)
            {
                return 1;
            }
            return 0;
        }

        public override string ToString()
        {
            return (this.bigValue + "." + this.smallValue);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            try
            {
                NodeIndex index = (NodeIndex) obj;
                return ((this.bigValue == index.bigValue) && (this.smallValue == index.smallValue));
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        internal static bool CreateBetween(NodeIndex before, NodeIndex after, out NodeIndex index)
        {
            return Create(before.bigValue, before.smallValue, after.bigValue, after.smallValue, out index);
        }

        internal static bool CreateBefore(NodeIndex after, out NodeIndex index)
        {
            return Create(-2147483648, -32768, after.bigValue, after.smallValue, out index);
        }

        internal static bool CreateAfter(NodeIndex before, out NodeIndex index)
        {
            return Create(before.bigValue, before.smallValue, 0x7fffffff, 0x7fff, out index);
        }

        internal static bool CreateFirst(out NodeIndex index)
        {
            return Create(-2147483648, -32768, 0x7fffffff, 0x7fff, out index);
        }

        internal void Set(int newBigValue)
        {
            this.bigValue = newBigValue;
            this.smallValue = 0;
        }

        private static bool Create(int previousBigValue, short previousSmallValue, int nextBigValue, short nextSmallValue, out NodeIndex index)
        {
            int num;
            short num2;
            index = new NodeIndex();
            if (previousBigValue == nextBigValue)
            {
                num = previousBigValue;
                if (!CreateSmallValue(previousSmallValue, nextSmallValue, out num2))
                {
                    return false;
                }
            }
            else if (CreateBigValue(previousBigValue, nextBigValue, out num))
            {
                num2 = 0;
            }
            else if (num == previousBigValue)
            {
                if (!CreateSmallValue(previousSmallValue, nextSmallValue, out num2))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            index.bigValue = num;
            index.smallValue = num2;
            return true;
        }

        private static bool CreateSmallValue(short previous, short next, out short smallValue)
        {
            smallValue = 0;
            if (previous == next)
            {
                return false;
            }
            if (previous == -32768)
            {
                if (next == 0x7fff)
                {
                    smallValue = 0;
                }
                else
                {
                    short num = ((next - -32768) < 6) ? ((short) (next - -32768)) : ((short) 6);
                    if (num <= 0)
                    {
                        return false;
                    }
                    smallValue = (short) (next - num);
                }
            }
            else if (next == 0x7fff)
            {
                short num2 = ((0x7fff - previous) < 6) ? ((short) (0x7fff - previous)) : ((short) 6);
                if (num2 <= 0)
                {
                    return false;
                }
                smallValue = (short) (previous + num2);
            }
            else
            {
                smallValue = (short) ((next - previous) / 2);
                if ((smallValue == previous) || (smallValue == next))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool CreateBigValue(int previous, int next, out int bigValue)
        {
            bigValue = 0;
            if (previous == next)
            {
                return false;
            }
            if (previous == -2147483648)
            {
                if (next == 0x7fffffff)
                {
                    bigValue = 0;
                }
                else
                {
                    int num = ((next - -2147483648) < 6) ? (next - -2147483648) : 6;
                    if (num <= 0)
                    {
                        return false;
                    }
                    bigValue = next - num;
                }
            }
            else if (next == 0x7fffffff)
            {
                int num2 = ((0x7fffffff - previous) < 6) ? (0x7fffffff - previous) : 6;
                if (num2 <= 0)
                {
                    return false;
                }
                bigValue = previous + num2;
            }
            else
            {
                bigValue = previous + ((next - previous) / 2);
                if ((bigValue == previous) || (bigValue == next))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

