namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class SymbolManager
    {
        private int generatedCodeCount;
        private int index = -1;
        private Stack<Region> regions = new Stack<Region>();
        private List<Symbol> symbols;

        public SymbolManager(List<Symbol> symbols)
        {
            this.symbols = symbols;
        }

        public void Advance()
        {
            this.index++;
        }

        public void Combine(int startIndex, int endIndex, string text, SymbolType type)
        {
            int index = startIndex + this.index;
            int num2 = endIndex + this.index;
            CodeLocation location = CodeLocation.Join(this.symbols[index].Location, this.symbols[num2].Location);
            Symbol symbol = new Symbol(text, type, location);
            this.symbols[index] = symbol;
            index++;
            if (index <= num2)
            {
                this.symbols.RemoveRange(index, (num2 - index) + 1);
            }
        }

        public void DecrementGeneratedCodeBlocks()
        {
            this.generatedCodeCount--;
        }

        public void IncrementGeneratedCodeBlocks()
        {
            this.generatedCodeCount++;
        }

        public Symbol Peek(int count)
        {
            if ((this.index + count) < this.symbols.Count)
            {
                return this.symbols[this.index + count];
            }
            return null;
        }

        public Region PopRegion()
        {
            if (this.regions.Count <= 0)
            {
                return null;
            }
            Region region = this.regions.Pop();
            if ((region != null) && region.IsGeneratedCodeRegion)
            {
                this.DecrementGeneratedCodeBlocks();
            }
            return region;
        }

        public void PushRegion(Region region)
        {
            this.regions.Push(region);
            if (region.IsGeneratedCodeRegion)
            {
                this.IncrementGeneratedCodeBlocks();
            }
        }

        public Symbol Current
        {
            get
            {
                if ((this.index >= 0) && (this.index < this.symbols.Count))
                {
                    return this.symbols[this.index];
                }
                return null;
            }
        }

        public int CurrentIndex
        {
            get
            {
                return this.index;
            }
            set
            {
                Param.RequireGreaterThanOrEqualToZero(value, "CurrentIndex");
                this.index = value;
            }
        }

        public bool Generated
        {
            get
            {
                return (this.generatedCodeCount > 0);
            }
        }

        public Symbol this[int symbolIndex]
        {
            get
            {
                if ((symbolIndex >= 0) && (symbolIndex < this.symbols.Count))
                {
                    return this.symbols[symbolIndex];
                }
                return null;
            }
        }
    }
}

