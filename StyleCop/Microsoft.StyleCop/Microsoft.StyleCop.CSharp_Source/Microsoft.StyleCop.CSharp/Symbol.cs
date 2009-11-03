namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;

    internal class Symbol
    {
        private CodeLocation location;
        private Microsoft.StyleCop.CSharp.SymbolType symbolType;
        private string text = string.Empty;

        internal Symbol(string text, Microsoft.StyleCop.CSharp.SymbolType symbolType, CodeLocation location)
        {
            this.text = text;
            this.symbolType = symbolType;
            this.location = location;
        }

        public override string ToString()
        {
            return this.text;
        }

        public int LineNumber
        {
            get
            {
                return this.location.StartPoint.LineNumber;
            }
        }

        public CodeLocation Location
        {
            get
            {
                return this.location;
            }
        }

        public Microsoft.StyleCop.CSharp.SymbolType SymbolType
        {
            get
            {
                return this.symbolType;
            }
            set
            {
                this.symbolType = value;
            }
        }

        public string Text
        {
            get
            {
                return this.text;
            }
        }
    }
}

