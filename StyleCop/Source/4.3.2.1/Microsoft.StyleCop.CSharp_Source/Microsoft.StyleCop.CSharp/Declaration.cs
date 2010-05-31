namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;

    public sealed class Declaration
    {
        private Microsoft.StyleCop.CSharp.AccessModifierType accessModifierType;
        private Microsoft.StyleCop.CSharp.ElementType elementType;
        private Dictionary<CsTokenType, CsToken> modifiers;
        private string name;
        private CsTokenList tokens;

        internal Declaration(CsTokenList tokens, string name, Microsoft.StyleCop.CSharp.ElementType elementType, Microsoft.StyleCop.CSharp.AccessModifierType accessModifierType) : this(tokens, name, elementType, accessModifierType, new Dictionary<CsTokenType, CsToken>(0))
        {
        }

        internal Declaration(CsTokenList tokens, string name, Microsoft.StyleCop.CSharp.ElementType elementType, Microsoft.StyleCop.CSharp.AccessModifierType accessModifierType, Dictionary<CsTokenType, CsToken> modifiers)
        {
            this.accessModifierType = Microsoft.StyleCop.CSharp.AccessModifierType.Private;
            this.tokens = tokens;
            this.name = name;
            this.elementType = elementType;
            this.accessModifierType = accessModifierType;
            this.modifiers = modifiers;
            this.tokens.Trim();
        }

        public bool ContainsModifier(params CsTokenType[] types)
        {
            Param.RequireNotNull(types, "types");
            for (int i = 0; i < types.Length; i++)
            {
                if (this.modifiers.ContainsKey(types[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool AccessModifier
        {
            get
            {
                return this.ContainsModifier(new CsTokenType[] { CsTokenType.Public, CsTokenType.Private, CsTokenType.Protected, CsTokenType.Internal });
            }
        }

        public Microsoft.StyleCop.CSharp.AccessModifierType AccessModifierType
        {
            get
            {
                return this.accessModifierType;
            }
            internal set
            {
                this.accessModifierType = value;
            }
        }

        public Microsoft.StyleCop.CSharp.ElementType ElementType
        {
            get
            {
                return this.elementType;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public CsTokenList Tokens
        {
            get
            {
                return this.tokens;
            }
        }
    }
}

