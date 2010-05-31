namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="Whitespace", Justification="API has already been published and should not be changed.")]
    public sealed class Whitespace : CsToken
    {
        private int spaceCount;
        private int tabCount;

        internal Whitespace(string text, CodeLocation location, bool generated) : base(text, CsTokenType.WhiteSpace, CsTokenClass.Whitespace, location, generated)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    this.spaceCount++;
                }
                else if (text[i] == '\t')
                {
                    this.tabCount++;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (this.tabCount >= 1)
            {
                builder.Append("\t");
            }
            if (this.spaceCount == 1)
            {
                builder.Append(" ");
            }
            else if (this.spaceCount > 1)
            {
                builder.Append("  ");
            }
            return builder.ToString();
        }

        public int SpaceCount
        {
            get
            {
                return this.spaceCount;
            }
        }

        public int TabCount
        {
            get
            {
                return this.tabCount;
            }
        }
    }
}

