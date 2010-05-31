namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class TypeParameterConstraintClause
    {
        private ICollection<CsToken> constraints;
        private CsElement parentElement;
        private CsTokenList tokens;
        private CsToken type;

        internal TypeParameterConstraintClause(CsTokenList tokens, CsToken type, ICollection<CsToken> constraints)
        {
            this.tokens = tokens;
            this.type = type;
            this.constraints = constraints;
            this.tokens.Trim();
        }

        public ICollection<CsToken> Constraints
        {
            get
            {
                return this.constraints;
            }
        }

        public CsElement ParentElement
        {
            get
            {
                return this.parentElement;
            }
            internal set
            {
                this.parentElement = value;
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
        public CsToken Type
        {
            get
            {
                return this.type;
            }
        }
    }
}

