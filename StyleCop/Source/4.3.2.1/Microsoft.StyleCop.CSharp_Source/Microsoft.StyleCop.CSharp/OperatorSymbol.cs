namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;

    public class OperatorSymbol : CsToken
    {
        private OperatorCategory category;
        private OperatorType symbolType;

        internal OperatorSymbol(string text, OperatorCategory category, OperatorType symbolType, CodeLocation location, bool generated) : base(text, CsTokenType.OperatorSymbol, CsTokenClass.OperatorSymbol, location, generated)
        {
            this.category = category;
            this.symbolType = symbolType;
        }

        public OperatorCategory Category
        {
            get
            {
                return this.category;
            }
        }

        public OperatorType SymbolType
        {
            get
            {
                return this.symbolType;
            }
        }
    }
}

