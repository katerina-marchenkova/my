namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification="The class represents a linked-list."), SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification="Camel case better serves in this case.")]
    public class CsTokenList : ItemList<CsToken>
    {
        public CsTokenList(MasterList<CsToken> masterList) : base(masterList)
        {
        }

        public CsTokenList(MasterList<CsToken> masterList, Node<CsToken> firstItemNode, Node<CsToken> lastItemNode) : base(masterList, firstItemNode, lastItemNode)
        {
        }

        public static Node<CsToken> GetNextCodeToken(Node<CsToken> start)
        {
            Param.RequireNotNull(start, "start");
            for (Node<CsToken> node = start.Next; node != null; node = node.Next)
            {
                if (((node.Value.CsTokenType != CsTokenType.WhiteSpace) && (node.Value.CsTokenType != CsTokenType.EndOfLine)) && ((node.Value.CsTokenType != CsTokenType.SingleLineComment) && (node.Value.CsTokenType != CsTokenType.MultiLineComment)))
                {
                    return node;
                }
            }
            return null;
        }

        public bool MatchTokens(params string[] values)
        {
            Param.RequireNotNull(values, "values");
            return MatchTokens(base.First, values);
        }

        public static bool MatchTokens(Node<CsToken> start, params string[] values)
        {
            Param.RequireNotNull(start, "start");
            Param.RequireNotNull(values, "values");
            int index = 0;
            for (Node<CsToken> node = start; (node != null) && (index < values.Length); node = GetNextCodeToken(node))
            {
                if (!node.Value.Text.Equals(values[index], StringComparison.Ordinal))
                {
                    return false;
                }
                index++;
            }
            return (index >= values.Length);
        }

        internal int Trim()
        {
            Node<CsToken> node = null;
            Node<CsToken> node2 = null;
            int num = 0;
            for (Node<CsToken> node3 = base.First; !base.OutOfBounds(node3); node3 = node3.Next)
            {
                CsTokenType csTokenType = node3.Value.CsTokenType;
                if (((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine)) && ((csTokenType != CsTokenType.SingleLineComment) && (csTokenType != CsTokenType.MultiLineComment)))
                {
                    node = node3;
                    break;
                }
                num++;
            }
            for (Node<CsToken> node4 = base.Last; !base.OutOfBounds(node4); node4 = node4.Previous)
            {
                CsTokenType type2 = node4.Value.CsTokenType;
                if (((type2 != CsTokenType.WhiteSpace) && (type2 != CsTokenType.EndOfLine)) && ((type2 != CsTokenType.SingleLineComment) && (type2 != CsTokenType.MultiLineComment)))
                {
                    node2 = node4;
                    break;
                }
            }
            base.First = node;
            base.Last = node2;
            return num;
        }
    }
}

