namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class Variable
    {
        private bool generated;
        private CodePoint location;
        private VariableModifiers modifiers;
        private string name;
        private TypeToken type;

        internal Variable(TypeToken type, string name, VariableModifiers modifiers, CodePoint location, bool generated)
        {
            this.type = type;
            this.name = name;
            this.modifiers = modifiers;
            this.location = location;
            this.generated = generated;
        }

        public bool Generated
        {
            get
            {
                return this.generated;
            }
        }

        public CodePoint Location
        {
            get
            {
                return this.location;
            }
        }

        public VariableModifiers Modifiers
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

