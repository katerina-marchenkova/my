namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public class Expression : CodeUnit
    {
        private string text;
        private Microsoft.StyleCop.CSharp.ExpressionType type;

        internal Expression(Microsoft.StyleCop.CSharp.ExpressionType type) : base(CodeUnitType.Expression)
        {
            this.type = type;
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification="The tokens property is virtual but it this is safe as expressions are sealed.")]
        internal Expression(Microsoft.StyleCop.CSharp.ExpressionType type, CsTokenList tokens) : base(CodeUnitType.Expression, tokens)
        {
            this.type = type;
            this.Tokens = tokens;
            this.Tokens.Trim();
        }

        private void CreateTextString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (CsToken token in this.Tokens)
            {
                if (((token.CsTokenType != CsTokenType.SingleLineComment) && (token.CsTokenType != CsTokenType.MultiLineComment)) && (token.CsTokenType != CsTokenType.PreprocessorDirective))
                {
                    builder.Append(token.Text);
                }
            }
            this.text = builder.ToString();
        }

        public void WalkExpression(CodeWalkerStatementVisitor<object> statementCallback)
        {
            this.WalkExpression<object>(statementCallback, null, null, null);
        }

        public void WalkExpression<T>(CodeWalkerStatementVisitor<T> statementCallback, T context)
        {
            this.WalkExpression<T>(statementCallback, null, null, context);
        }

        public void WalkExpression(CodeWalkerStatementVisitor<object> statementCallback, CodeWalkerExpressionVisitor<object> expressionCallback)
        {
            this.WalkExpression<object>(statementCallback, expressionCallback, null, null);
        }

        public void WalkExpression(CodeWalkerStatementVisitor<object> statementCallback, CodeWalkerExpressionVisitor<object> expressionCallback, CodeWalkerQueryClauseVisitor<object> queryClauseCallback)
        {
            CodeWalker<object>.Start(this, statementCallback, expressionCallback, queryClauseCallback, null);
        }

        public void WalkExpression<T>(CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, T context)
        {
            this.WalkExpression<T>(statementCallback, expressionCallback, null, context);
        }

        public void WalkExpression<T>(CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            CodeWalker<T>.Start(this, statementCallback, expressionCallback, queryClauseCallback, context);
        }

        public Microsoft.StyleCop.CSharp.ExpressionType ExpressionType
        {
            get
            {
                return this.type;
            }
        }

        public string Text
        {
            get
            {
                if (this.text == null)
                {
                    this.CreateTextString();
                }
                return this.text;
            }
        }
    }
}

