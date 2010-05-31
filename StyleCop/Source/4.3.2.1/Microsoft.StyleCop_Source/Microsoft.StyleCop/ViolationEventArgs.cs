namespace Microsoft.StyleCop
{
    using System;

    public class ViolationEventArgs : EventArgs
    {
        private Microsoft.StyleCop.Violation violation;

        internal ViolationEventArgs(Microsoft.StyleCop.Violation violation)
        {
            this.violation = violation;
        }

        public CodeElement Element
        {
            get
            {
                return this.violation.Element;
            }
        }

        public int LineNumber
        {
            get
            {
                return this.violation.Line;
            }
        }

        public string Message
        {
            get
            {
                return this.violation.Message;
            }
        }

        public Microsoft.StyleCop.SourceCode SourceCode
        {
            get
            {
                return this.violation.SourceCode;
            }
        }

        public Microsoft.StyleCop.Violation Violation
        {
            get
            {
                return this.violation;
            }
        }

        public bool Warning
        {
            get
            {
                return this.violation.Rule.Warning;
            }
        }
    }
}

