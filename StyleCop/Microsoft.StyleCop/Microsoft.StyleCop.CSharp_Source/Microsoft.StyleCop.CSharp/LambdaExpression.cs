namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class LambdaExpression : ExpressionWithParameters
    {
        private CodeUnit anonymousFunctionBody;

        internal LambdaExpression() : base(ExpressionType.Lambda)
        {
        }

        public CodeUnit AnonymousFunctionBody
        {
            get
            {
                return this.anonymousFunctionBody;
            }
            internal set
            {
                this.anonymousFunctionBody = value;
                Statement anonymousFunctionBody = this.anonymousFunctionBody as Statement;
                if (anonymousFunctionBody != null)
                {
                    base.AddStatement(anonymousFunctionBody);
                }
                else
                {
                    Expression expression = (Expression) this.anonymousFunctionBody;
                    base.AddExpression(expression);
                }
            }
        }
    }
}

