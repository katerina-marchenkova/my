namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true), SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification="The attribute has no arguments.")]
    public sealed class SourceParserAttribute : StyleCopAddInAttribute
    {
        public SourceParserAttribute()
        {
        }

        public SourceParserAttribute(string parserXmlId) : base(parserXmlId)
        {
        }
    }
}

