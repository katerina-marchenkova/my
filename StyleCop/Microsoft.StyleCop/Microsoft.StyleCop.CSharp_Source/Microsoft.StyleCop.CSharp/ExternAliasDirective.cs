namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;

    public sealed class ExternAliasDirective : CsElement
    {
        private string identifier;

        internal ExternAliasDirective(CsDocument document, CsElement parent, Declaration declaration, bool generated) : base(document, parent, ElementType.ExternAliasDirective, "extern alias " + declaration.Name, null, null, declaration, false, generated)
        {
            this.identifier = string.Empty;
        }

        internal override void Initialize()
        {
            base.Initialize();
            Node<CsToken> start = null;
            for (Node<CsToken> node2 = base.Tokens.First; !base.Tokens.OutOfBounds(node2); node2 = node2.Next)
            {
                if (node2.Value.CsTokenType == CsTokenType.Alias)
                {
                    start = node2;
                    break;
                }
            }
            if ((start != null) && (start != base.Tokens.Last))
            {
                Node<CsToken> next = start.Next;
                if (!base.Tokens.OutOfBounds(next))
                {
                    start = next;
                    if (CodeParser.MoveToNextCodeToken(base.Tokens, ref start))
                    {
                        this.identifier = CodeParser.TrimType(CodeParser.GetFullName(base.Document, base.Tokens, start, out start));
                    }
                }
            }
        }

        public string Identifier
        {
            get
            {
                return this.identifier;
            }
        }
    }
}

