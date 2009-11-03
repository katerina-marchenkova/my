namespace Microsoft.StyleCop.CSharp
{
    using System.Collections.Generic;

    public interface IParameterContainer
    {
        IList<Parameter> Parameters { get; }
    }
}

