namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [Flags]
    public enum VariableModifiers
    {
        Const = 1,
        None = 0,
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="Readonly", Justification="API has already been published and should not be changed.")]
        Readonly = 2
    }
}

