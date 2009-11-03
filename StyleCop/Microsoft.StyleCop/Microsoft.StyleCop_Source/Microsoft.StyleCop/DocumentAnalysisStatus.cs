namespace Microsoft.StyleCop
{
    using System;

    internal class DocumentAnalysisStatus
    {
        private bool complete;
        private CodeDocument document;
        private bool initialized;

        public bool Complete
        {
            get
            {
                return this.complete;
            }
            set
            {
                this.complete = value;
            }
        }

        public CodeDocument Document
        {
            get
            {
                return this.document;
            }
            set
            {
                Param.RequireNotNull(value, "Document");
                this.document = value;
            }
        }

        public bool Initialized
        {
            get
            {
                return this.initialized;
            }
            set
            {
                this.initialized = value;
            }
        }
    }
}

