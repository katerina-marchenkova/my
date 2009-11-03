namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification="The class is inherited by other attribute types."), AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class StyleCopAddInAttribute : Attribute
    {
        private string addInXmlId;

        public StyleCopAddInAttribute()
        {
        }

        public StyleCopAddInAttribute(string addInXmlId)
        {
            Param.RequireValidString(addInXmlId, "addInXmlId");
            this.addInXmlId = addInXmlId;
        }

        public string AddInXmlId
        {
            get
            {
                return this.addInXmlId;
            }
        }
    }
}

