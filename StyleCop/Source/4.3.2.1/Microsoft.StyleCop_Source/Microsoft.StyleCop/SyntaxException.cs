using System.Globalization;

namespace Microsoft.StyleCop
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public sealed class SyntaxException : Exception
    {
        private int lineNumber;
        private Microsoft.StyleCop.SourceCode sourceCode;

        public SyntaxException()
        {
            this.lineNumber = 1;
        }

        public SyntaxException(string message) : base(message)
        {
            this.lineNumber = 1;
        }

        public SyntaxException(Microsoft.StyleCop.SourceCode sourceCode, int lineNumber) : base(string.Format(CultureInfo.CurrentCulture, Strings.SyntaxErrorInFile, new object[] { sourceCode.Path, lineNumber }))
        {
            this.lineNumber = 1;
            Param.RequireNotNull(sourceCode, "sourceCode");
            Param.RequireGreaterThanZero(lineNumber, "lineNumber");
            this.sourceCode = sourceCode;
            this.lineNumber = lineNumber;
        }

        private SyntaxException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
            this.lineNumber = 1;
        }

        public SyntaxException(string message, Exception innerException) : base(message, innerException)
        {
            this.lineNumber = 1;
        }

        public SyntaxException(Microsoft.StyleCop.SourceCode sourceCode, int lineNumber, Exception innerException) : base(string.Format(CultureInfo.CurrentCulture, Strings.SyntaxErrorInFile, new object[] { sourceCode.Path, lineNumber }), innerException)
        {
            this.lineNumber = 1;
            Param.RequireNotNull(sourceCode, "sourceCode");
            Param.RequireGreaterThanZero(lineNumber, "lineNumber");
            this.sourceCode = sourceCode;
            this.lineNumber = lineNumber;
        }

        public SyntaxException(Microsoft.StyleCop.SourceCode sourceCode, int lineNumber, string message) : base(string.Format(CultureInfo.CurrentCulture, Strings.SyntaxErrorInFileWithMessage, new object[] { sourceCode.Path, lineNumber, message }))
        {
            this.lineNumber = 1;
            Param.RequireNotNull(sourceCode, "sourceCode");
            Param.RequireGreaterThanZero(lineNumber, "lineNumber");
            Param.RequireValidString(message, "message");
            this.sourceCode = sourceCode;
            this.lineNumber = lineNumber;
        }

        public SyntaxException(Microsoft.StyleCop.SourceCode sourceCode, int lineNumber, string message, Exception innerException) : base(string.Format(CultureInfo.CurrentCulture, Strings.SyntaxErrorInFileWithMessage, new object[] { sourceCode.Path, lineNumber, message }), innerException)
        {
            this.lineNumber = 1;
            Param.RequireNotNull(sourceCode, "sourceCode");
            Param.RequireGreaterThanZero(lineNumber, "lineNumber");
            Param.RequireValidString(message, "message");
            this.sourceCode = sourceCode;
            this.lineNumber = lineNumber;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public int LineNumber
        {
            get
            {
                return this.lineNumber;
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

