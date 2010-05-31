namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;

    public abstract class CodeElement
    {
        private object analyzerTag;
        private CodeDocument document;
        private bool generated;
        private string name;
        private Dictionary<string, Violation> violations = new Dictionary<string, Violation>();

        protected internal CodeElement(CodeDocument document, string name, bool generated)
        {
            this.document = document;
            this.name = name;
            this.generated = generated;
        }

        internal bool AddViolation(Violation violation)
        {
            string key = violation.Key;
            if (!this.violations.ContainsKey(key))
            {
                this.violations.Add(violation.Key, violation);
                return true;
            }
            return false;
        }

        public virtual void ClearAnalyzerTags()
        {
            this.analyzerTag = null;
        }

        public object AnalyzerTag
        {
            get
            {
                return this.analyzerTag;
            }
            set
            {
                this.analyzerTag = value;
            }
        }

        public abstract IEnumerable<CodeElement> ChildCodeElements { get; }

        public CodeDocument Document
        {
            get
            {
                return this.document;
            }
        }

        public abstract IEnumerable<Token> ElementTokens { get; }

        public abstract string FullyQualifiedName { get; }

        public bool Generated
        {
            get
            {
                return this.generated;
            }
            protected set
            {
                this.generated = value;
            }
        }

        public abstract int LineNumber { get; }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public ICollection<Violation> Violations
        {
            get
            {
                return this.violations.Values;
            }
        }
    }
}

