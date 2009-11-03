namespace Microsoft.StyleCop
{
    using System;

    internal interface IPropertyControlHost
    {
        void Cancel();
        void Dirty(bool isDirty);
    }
}

