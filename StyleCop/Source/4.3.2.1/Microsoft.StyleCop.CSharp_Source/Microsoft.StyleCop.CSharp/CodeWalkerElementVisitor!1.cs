namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate bool CodeWalkerElementVisitor<T>(CsElement element, CsElement parentElement, T context);
}

