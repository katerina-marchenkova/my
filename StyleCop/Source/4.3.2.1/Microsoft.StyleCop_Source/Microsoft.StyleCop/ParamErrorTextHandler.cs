namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Param", Justification="This name represents a Parameter, and should be short as it is used often.")]
    public delegate string ParamErrorTextHandler();
}

