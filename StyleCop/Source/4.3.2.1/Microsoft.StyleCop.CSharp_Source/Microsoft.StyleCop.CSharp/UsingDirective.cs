namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;

    public sealed class UsingDirective : CsElement
    {
        private string alias;
        private string namespaceType;

        internal UsingDirective(CsDocument document, CsElement parent, Declaration declaration, bool generated, string @namespace, string alias) : base(document, parent, ElementType.UsingDirective, "using " + declaration.Name, null, null, declaration, false, generated)
        {
            this.namespaceType = string.Empty;
            this.alias = string.Empty;
            this.namespaceType = @namespace;
            if (alias != null)
            {
                this.alias = alias;
            }
        }

        internal override void Initialize()
        {
            base.Initialize();
            Node<CsToken> node = null;
            for (Node<CsToken> node2 = base.Tokens.First; !base.Tokens.OutOfBounds(node2); node2 = node2.Next)
            {
                if (node2.Value.CsTokenType == CsTokenType.UsingDirective)
                {
                    node = node2;
                    break;
                }
            }
            if (node != null)
            {
                Node<CsToken> next = node.Next;
                if (base.Tokens.OutOfBounds(next))
                {
                    next = null;
                }
                if (CodeParser.MoveToNextCodeToken(base.Tokens, ref next))
                {
                    this.namespaceType = CodeParser.TrimType(CodeParser.GetFullName(base.Document, base.Tokens, next, out next));
                    next = next.Next;
                    if (base.Tokens.OutOfBounds(next))
                    {
                        next = null;
                    }
                    if (CodeParser.MoveToNextCodeToken(base.Tokens, ref next) && (next.Value.Text == "="))
                    {
                        next = next.Next;
                        if (base.Tokens.OutOfBounds(next))
                        {
                            next = null;
                        }
                        if (CodeParser.MoveToNextCodeToken(base.Tokens, ref next))
                        {
                            this.alias = this.namespaceType;
                            this.namespaceType = CodeParser.TrimType(next.Value.Text);
                        }
                    }
                }
            }
        }

        public string Alias
        {
            get
            {
                return this.alias;
            }
        }

        public string NamespaceType
        {
            get
            {
                return this.namespaceType;
            }
        }
    }
}

