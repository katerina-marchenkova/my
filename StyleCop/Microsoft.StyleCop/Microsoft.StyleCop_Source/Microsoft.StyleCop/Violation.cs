namespace Microsoft.StyleCop
{
    using System;

    public class Violation
    {
        private CodeElement element;
        private int line;
        private string message;
        private Microsoft.StyleCop.Rule rule;
        private Microsoft.StyleCop.SourceCode sourceCode;

        internal Violation(Microsoft.StyleCop.Rule rule, CodeElement element, int line, string message)
        {
            this.rule = rule;
            this.element = element;
            this.line = line;
            this.message = message;
            if ((this.element != null) && (this.element.Document != null))
            {
                this.sourceCode = this.element.Document.SourceCode;
            }
        }

        internal Violation(Microsoft.StyleCop.Rule rule, Microsoft.StyleCop.SourceCode sourceCode, int line, string message)
        {
            this.rule = rule;
            this.sourceCode = sourceCode;
            this.line = line;
            this.message = message;
        }

        public CodeElement Element
        {
            get
            {
                return this.element;
            }
        }

        internal string Key
        {
            get
            {
                return (this.rule.Name + this.line);
            }
        }

        public int Line
        {
            get
            {
                return this.line;
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }
        }

        public Microsoft.StyleCop.Rule Rule
        {
            get
            {
                return this.rule;
            }
        }

        public Microsoft.StyleCop.SourceCode SourceCode
        {
            get
            {
                return this.sourceCode;
            }
        }
    }
}

