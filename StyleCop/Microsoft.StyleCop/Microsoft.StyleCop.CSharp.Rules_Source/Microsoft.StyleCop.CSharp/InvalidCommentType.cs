namespace Microsoft.StyleCop.CSharp
{
    using System;

    [Flags]
    internal enum InvalidCommentType
    {
        Empty = 1,
        NoCapitalLetter = 4,
        NoPeriod = 8,
        NoWhitespace = 0x20,
        TooFewCharacters = 0x10,
        TooShort = 2,
        Valid = 0
    }
}

