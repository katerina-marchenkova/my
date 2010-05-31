namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification="Camel case better serves in this case.")]
    public sealed class CsDocument : CodeDocument, ITokenContainer
    {
        private DocumentRoot contents;
        private Microsoft.StyleCop.CSharp.FileHeader fileHeader;
        private CsParser parser;
        private MasterList<CsToken> tokens;

        internal CsDocument(SourceCode sourceCode, CsParser parser) : this(sourceCode, parser, null)
        {
            this.tokens = new MasterList<CsToken>();
        }

        internal CsDocument(SourceCode sourceCode, CsParser parser, MasterList<CsToken> tokens) : base(sourceCode)
        {
            this.parser = parser;
            this.tokens = tokens;
        }

        [SuppressMessage("Microsoft.Usage", "CA2215:DisposeMethodsShouldCallBaseClassDispose", Justification="base.Dispose is called")]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.contents = null;
                this.tokens = null;
                this.fileHeader = null;
            }
        }

        public void WalkDocument(CodeWalkerElementVisitor<object> elementCallback)
        {
            this.WalkDocument<object>(elementCallback, null, null, null, null);
        }

        public void WalkDocument<T>(CodeWalkerElementVisitor<T> elementCallback, T context)
        {
            this.WalkDocument<T>(elementCallback, null, null, null, context);
        }

        public void WalkDocument(CodeWalkerElementVisitor<object> elementCallback, CodeWalkerStatementVisitor<object> statementCallback)
        {
            this.WalkDocument<object>(elementCallback, statementCallback, null, null, null);
        }

        public void WalkDocument<T>(CodeWalkerElementVisitor<T> elementCallback, CodeWalkerStatementVisitor<T> statementCallback, T context)
        {
            this.WalkDocument<T>(elementCallback, statementCallback, null, null, context);
        }

        public void WalkDocument(CodeWalkerElementVisitor<object> elementCallback, CodeWalkerStatementVisitor<object> statementCallback, CodeWalkerExpressionVisitor<object> expressionCallback)
        {
            this.WalkDocument<object>(elementCallback, statementCallback, expressionCallback, null, null);
        }

        public void WalkDocument(CodeWalkerElementVisitor<object> elementCallback, CodeWalkerStatementVisitor<object> statementCallback, CodeWalkerExpressionVisitor<object> expressionCallback, CodeWalkerQueryClauseVisitor<object> queryClauseCallback)
        {
            CodeWalker<object>.Start(this, elementCallback, statementCallback, expressionCallback, queryClauseCallback, null);
        }

        public void WalkDocument<T>(CodeWalkerElementVisitor<T> elementCallback, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, T context)
        {
            this.WalkDocument<T>(elementCallback, statementCallback, expressionCallback, null, context);
        }

        public void WalkDocument<T>(CodeWalkerElementVisitor<T> elementCallback, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            CodeWalker<T>.Start(this, elementCallback, statementCallback, expressionCallback, queryClauseCallback, context);
        }

        public override CodeElement DocumentContents
        {
            get
            {
                return this.contents;
            }
        }

        public override IEnumerable<Token> DocumentTokens
        {
            get
            {
                return new EnumerableAdapter<CsToken, Token>(this.tokens, delegate (CsToken token) {
                    return token;
                });
            }
        }

        public Microsoft.StyleCop.CSharp.FileHeader FileHeader
        {
            get
            {
                return this.fileHeader;
            }
            internal set
            {
                this.fileHeader = value;
            }
        }

        internal MasterList<CsToken> MasterTokenList
        {
            get
            {
                return this.tokens;
            }
        }

        ICollection<CsToken> ITokenContainer.Tokens
        {
            get
            {
                return this.tokens.AsReadOnly;
            }
        }

        public CsParser Parser
        {
            get
            {
                return this.parser;
            }
        }

        public DocumentRoot RootElement
        {
            get
            {
                return this.contents;
            }
            internal set
            {
                this.contents = value;
            }
        }

        public MasterList<CsToken> Tokens
        {
            get
            {
                return this.tokens.AsReadOnly;
            }
        }
    }
}

