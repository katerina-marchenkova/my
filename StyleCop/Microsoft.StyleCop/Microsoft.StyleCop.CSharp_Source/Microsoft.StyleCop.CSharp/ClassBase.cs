namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;

    public abstract class ClassBase : CsElement, ITypeConstraintContainer
    {
        private string baseClass;
        private string[] implementedInterfaces;
        private ICollection<TypeParameterConstraintClause> typeConstraints;

        internal ClassBase(CsDocument document, CsElement parent, ElementType type, string name, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, ICollection<TypeParameterConstraintClause> typeConstraints, bool unsafeCode, bool generated) : base(document, parent, type, name, header, attributes, declaration, unsafeCode, generated)
        {
            this.baseClass = string.Empty;
            this.implementedInterfaces = new string[0];
            this.typeConstraints = typeConstraints;
            if (typeConstraints != null)
            {
                foreach (TypeParameterConstraintClause clause in typeConstraints)
                {
                    clause.ParentElement = this;
                }
            }
        }

        protected void SetInheritedItems(Declaration declaration)
        {
            Param.RequireNotNull(declaration, "declaration");
            bool flag = false;
            bool flag2 = false;
            List<string> list = new List<string>();
            foreach (CsToken token in declaration.Tokens)
            {
                if (flag)
                {
                    if ((((token.CsTokenType != CsTokenType.WhiteSpace) && (token.CsTokenType != CsTokenType.EndOfLine)) && ((token.CsTokenType != CsTokenType.SingleLineComment) && (token.CsTokenType != CsTokenType.MultiLineComment))) && (token.CsTokenType != CsTokenType.PreprocessorDirective))
                    {
                        if (((token.Text.Length >= 2) && (token.Text[0] == 'I')) && char.IsUpper(token.Text[1]))
                        {
                            list.Add(CodeParser.TrimType(token.Text));
                        }
                        else
                        {
                            this.baseClass = CodeParser.TrimType(token.Text);
                        }
                        flag = false;
                    }
                    continue;
                }
                if (flag2)
                {
                    if ((((token.CsTokenType != CsTokenType.WhiteSpace) && (token.CsTokenType != CsTokenType.EndOfLine)) && ((token.CsTokenType != CsTokenType.SingleLineComment) && (token.CsTokenType != CsTokenType.MultiLineComment))) && (token.CsTokenType != CsTokenType.PreprocessorDirective))
                    {
                        list.Add(CodeParser.TrimType(token.Text));
                        flag2 = false;
                    }
                }
                else
                {
                    if (token.CsTokenType == CsTokenType.Where)
                    {
                        break;
                    }
                    if (token.Text == ":")
                    {
                        if (this.baseClass.Length > 0)
                        {
                            break;
                        }
                        flag = true;
                        continue;
                    }
                    if (token.CsTokenType == CsTokenType.Comma)
                    {
                        flag2 = true;
                    }
                }
            }
            if (list.Count > 0)
            {
                this.implementedInterfaces = list.ToArray();
            }
        }

        public string BaseClass
        {
            get
            {
                return this.baseClass;
            }
        }

        public ICollection<string> ImplementedInterfaces
        {
            get
            {
                return this.implementedInterfaces;
            }
        }

        public ICollection<CsElement> PartialElementList
        {
            get
            {
                if (base.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Partial }))
                {
                    lock (base.Document.Parser.PartialElements)
                    {
                        List<CsElement> list;
                        if (base.Document.Parser.PartialElements.TryGetValue(base.FullNamespaceName, out list))
                        {
                            return list.AsReadOnly();
                        }
                    }
                }
                return null;
            }
        }

        public ICollection<TypeParameterConstraintClause> TypeConstraints
        {
            get
            {
                return this.typeConstraints;
            }
        }
    }
}

