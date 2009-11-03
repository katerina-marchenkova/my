namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;

    public sealed class LiteralExpression : Expression
    {
        private Node<CsToken> tokenNode;

        internal LiteralExpression(CsTokenList tokens, Node<CsToken> tokenNode) : base(ExpressionType.Literal, tokens)
        {
            this.tokenNode = tokenNode;
        }

        internal LiteralExpression(MasterList<CsToken> masterList, Node<CsToken> tokenNode) : this(new CsTokenList(masterList, tokenNode, tokenNode), tokenNode)
        {
        }

        public override string ToString()
        {
            return this.tokenNode.Value.Text;
        }

        public CsToken Token
        {
            get
            {
                return this.tokenNode.Value;
            }
        }

        public Node<CsToken> TokenNode
        {
            get
            {
                return this.tokenNode;
            }
        }
    }
}

