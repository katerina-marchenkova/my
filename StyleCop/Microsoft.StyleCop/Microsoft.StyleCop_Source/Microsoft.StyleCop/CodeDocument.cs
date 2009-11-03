namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;

    public abstract class CodeDocument : IDisposable
    {
        private Dictionary<string, object> analyzerData = new Dictionary<string, object>();
        private Microsoft.StyleCop.SourceCode sourceCode;

        protected CodeDocument(Microsoft.StyleCop.SourceCode sourceCode)
        {
            Param.RequireNotNull(sourceCode, "sourceCode");
            this.sourceCode = sourceCode;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        internal Dictionary<string, object> AnalyzerData
        {
            get
            {
                return this.analyzerData;
            }
        }

        public abstract CodeElement DocumentContents { get; }

        public abstract IEnumerable<Token> DocumentTokens { get; }

        public Microsoft.StyleCop.Settings Settings
        {
            get
            {
                if ((this.sourceCode != null) && (this.sourceCode.Project != null))
                {
                    return this.sourceCode.Project.Settings;
                }
                return null;
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

