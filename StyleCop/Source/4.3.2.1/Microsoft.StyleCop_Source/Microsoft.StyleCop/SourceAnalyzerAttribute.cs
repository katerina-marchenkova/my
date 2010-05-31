namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification="The attribute has no other arguments."), AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class SourceAnalyzerAttribute : StyleCopAddInAttribute
    {
        private Type parserType;

        public SourceAnalyzerAttribute(Type parserType)
        {
            Param.RequireNotNull(parserType, "parserType");
            this.parserType = parserType;
        }

        public SourceAnalyzerAttribute(Type parserType, string analyzerXmlId) : base(analyzerXmlId)
        {
            Param.RequireNotNull(parserType, "parserType");
            Param.RequireValidString(analyzerXmlId, "analyzerXmlId");
            this.parserType = parserType;
        }

        public Type ParserType
        {
            get
            {
                return this.parserType;
            }
        }
    }
}

