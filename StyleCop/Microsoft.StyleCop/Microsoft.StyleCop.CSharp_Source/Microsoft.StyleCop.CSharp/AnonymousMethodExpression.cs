namespace Microsoft.StyleCop.CSharp
{
    using System;

    public sealed class AnonymousMethodExpression : ExpressionWithParameters
    {
        internal AnonymousMethodExpression() : base(ExpressionType.AnonymousMethod)
        {
        }
    }
}

