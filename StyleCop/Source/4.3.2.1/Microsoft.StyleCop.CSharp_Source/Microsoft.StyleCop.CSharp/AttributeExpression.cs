namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class AttributeExpression : Expression
    {
        private Expression initialization;
        private LiteralExpression target;

        internal AttributeExpression(CsTokenList tokens, LiteralExpression target, Expression initialization) : base(ExpressionType.Attribute, tokens)
        {
            this.target = target;
            if (target != null)
            {
                base.AddExpression(target);
            }
            this.initialization = initialization;
            base.AddExpression(initialization);
        }

        public Expression Initialization
        {
            get
            {
                return this.initialization;
            }
        }

        public bool IsAssemblyAttribute
        {
            get
            {
                bool flag = false;
                foreach (CsToken token in this.Tokens)
                {
                    if (!flag)
                    {
                        if (token.Text == "assembly")
                        {
                            flag = true;
                        }
                    }
                    else if ((((token.CsTokenType != CsTokenType.WhiteSpace) && (token.CsTokenType != CsTokenType.EndOfLine)) && ((token.CsTokenType != CsTokenType.SingleLineComment) && (token.CsTokenType != CsTokenType.MultiLineComment))) && ((token.CsTokenType != CsTokenType.PreprocessorDirective) && (token.Text == ":")))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public LiteralExpression Target
        {
            get
            {
                return this.target;
            }
        }
    }
}

