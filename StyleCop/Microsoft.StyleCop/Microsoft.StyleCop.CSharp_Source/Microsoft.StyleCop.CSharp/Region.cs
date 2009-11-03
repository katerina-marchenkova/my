namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;

    public sealed class Region : Preprocessor
    {
        private bool beginning;
        private Region partner;

        internal Region(string text, CodeLocation location, bool beginning, bool generated) : base(text, CsTokenClass.RegionDirective, location, generated)
        {
            this.beginning = beginning;
        }

        public bool Beginning
        {
            get
            {
                return this.beginning;
            }
        }

        public bool IsGeneratedCodeRegion
        {
            get
            {
                return ((base.PreprocessorType == "region") && (this.Text.IndexOf("generated code", StringComparison.OrdinalIgnoreCase) != -1));
            }
        }

        public Region Partner
        {
            get
            {
                return this.partner;
            }
            internal set
            {
                this.partner = value;
            }
        }
    }
}

