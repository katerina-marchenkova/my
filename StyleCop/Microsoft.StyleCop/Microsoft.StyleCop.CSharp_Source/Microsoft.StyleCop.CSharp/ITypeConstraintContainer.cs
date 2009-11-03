namespace Microsoft.StyleCop.CSharp
{
    using System.Collections.Generic;

    public interface ITypeConstraintContainer
    {
        ICollection<TypeParameterConstraintClause> TypeConstraints { get; }
    }
}

