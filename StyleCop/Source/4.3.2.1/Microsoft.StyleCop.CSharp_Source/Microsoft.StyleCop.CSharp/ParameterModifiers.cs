namespace Microsoft.StyleCop.CSharp
{
    using System;

    [Flags]
    public enum ParameterModifiers
    {
        None = 0,
        Out = 1,
        Params = 4,
        Ref = 2,
        This = 8
    }
}

