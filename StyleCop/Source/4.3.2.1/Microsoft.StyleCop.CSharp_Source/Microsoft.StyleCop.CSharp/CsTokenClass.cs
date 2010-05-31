namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification="Camel case better serves in this case.")]
    public enum CsTokenClass
    {
        Attribute = 0,
        Bracket = 10,
        ConditionalCompilationDirective = 5,
        ConstructorConstraint = 11,
        GenericType = 1,
        Number = 2,
        OperatorSymbol = 9,
        PreprocessorDirective = 3,
        RegionDirective = 4,
        Token = 12,
        Type = 6,
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="Whitespace", Justification="API has already been published and should not be changed.")]
        Whitespace = 7,
        XmlHeader = 8
    }
}

