namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class Parameter
    {
        internal static readonly Parameter[] EmptyParameterArray = new Parameter[0];
        private bool generated;
        private CodeLocation location;
        private ParameterModifiers modifiers;
        private string name;
        private CsTokenList tokens;
        private TypeToken type;

        internal Parameter(TypeToken type, string name, ParameterModifiers modifiers, CodeLocation location, CsTokenList tokens, bool generated)
        {
            this.type = type;
            this.name = name;
            this.modifiers = modifiers;
            this.location = location;
            this.tokens = tokens;
            this.generated = generated;
        }

        public bool Generated
        {
            get
            {
                return this.generated;
            }
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

        public ParameterModifiers Modifiers
        {
            get
            {
                return this.modifiers;
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

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification="API has already been published and should not be changed.")]
        public TypeToken Type
        {
            get
            {
                return this.type;
            }
        }
    }
}

