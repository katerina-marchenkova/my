namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;

    public sealed class ConditionalCompilationDirective : Preprocessor
    {
        private Expression body;
        private Microsoft.StyleCop.CSharp.ConditionalCompilationDirectiveType type;

        internal ConditionalCompilationDirective(string text, Microsoft.StyleCop.CSharp.ConditionalCompilationDirectiveType type, Expression body, CodeLocation location, bool generated) : base(text, CsTokenClass.ConditionalCompilationDirective, location, generated)
        {
            this.type = type;
            this.body = body;
        }

        public Expression Body
        {
            get
            {
                return this.body;
            }
        }

        public Microsoft.StyleCop.CSharp.ConditionalCompilationDirectiveType ConditionalCompilationDirectiveType
        {
            get
            {
                return this.type;
            }
        }
    }
}

