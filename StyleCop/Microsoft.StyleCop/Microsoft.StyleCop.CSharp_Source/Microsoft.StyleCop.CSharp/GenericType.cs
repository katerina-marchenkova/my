namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class GenericType : TypeToken
    {
        private ICollection<string> types;

        internal GenericType(MasterList<CsToken> childTokens, CodeLocation location, bool generated) : base(childTokens, location, CsTokenClass.GenericType, generated)
        {
        }

        private void ExtractGenericTypes()
        {
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();
            for (Microsoft.StyleCop.Node<CsToken> node = base.ChildTokens.First; node != null; node = node.Next)
            {
                if (node.Value.CsTokenType == CsTokenType.Comma)
                {
                    string str = CodeParser.TrimType(builder.ToString());
                    if (!string.IsNullOrEmpty(str))
                    {
                        list.Add(str);
                    }
                    builder.Remove(0, builder.Length);
                }
                else
                {
                    builder.Append(node.Value.Text);
                }
            }
            if (builder.Length > 0)
            {
                string str2 = CodeParser.TrimType(builder.ToString());
                if (!string.IsNullOrEmpty(str2))
                {
                    list.Add(str2);
                }
            }
            this.types = list.ToArray();
        }

        public ICollection<string> GenericTypes
        {
            get
            {
                if (this.types == null)
                {
                    this.ExtractGenericTypes();
                }
                return this.types;
            }
        }
    }
}

