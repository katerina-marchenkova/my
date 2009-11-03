namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;

    public sealed class Bracket : CsToken
    {
        private Node<CsToken> matchingBracketNode;

        internal Bracket(string text, CsTokenType tokenType, CodeLocation location, bool generated) : base(text, tokenType, CsTokenClass.Bracket, location, generated)
        {
        }

        public Bracket MatchingBracket
        {
            get
            {
                if (this.matchingBracketNode == null)
                {
                    return null;
                }
                return (this.matchingBracketNode.Value as Bracket);
            }
        }

        public Node<CsToken> MatchingBracketNode
        {
            get
            {
                return this.matchingBracketNode;
            }
            internal set
            {
                this.matchingBracketNode = value;
            }
        }
    }
}

