namespace Microsoft.StyleCop
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate TAdapted AdapterConverterHandler<TOriginal, TAdapted>(TOriginal item);
}

