namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;

    public sealed class Indexer : CsElement, IParameterContainer
    {
        private Accessor get;
        private IList<Parameter> parameters;
        private TypeToken returnType;
        private Accessor set;

        internal Indexer(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, TypeToken returnType, IList<Parameter> parameters, bool unsafeCode, bool generated) : base(document, parent, ElementType.Indexer, "indexer " + declaration.Name, header, attributes, declaration, unsafeCode, generated)
        {
            this.returnType = returnType;
            this.parameters = parameters;
            base.QualifiedName = CodeParser.AddQualifications(this.parameters, base.QualifiedName);
            if ((base.Declaration.Name.IndexOf(".", StringComparison.Ordinal) > -1) && !base.Declaration.Name.StartsWith("this.", StringComparison.Ordinal))
            {
                base.Declaration.AccessModifierType = AccessModifierType.Public;
            }
        }

        internal override void Initialize()
        {
            base.Initialize();
            foreach (CsElement element in base.ChildElements)
            {
                Accessor accessor = element as Accessor;
                if (accessor == null)
                {
                    throw new SyntaxException(base.Document.SourceCode, accessor.LineNumber);
                }
                if (accessor.AccessorType == AccessorType.Get)
                {
                    if (this.get != null)
                    {
                        throw new SyntaxException(base.Document.SourceCode, accessor.LineNumber);
                    }
                    this.get = accessor;
                }
                else
                {
                    if (accessor.AccessorType != AccessorType.Set)
                    {
                        throw new SyntaxException(base.Document.SourceCode, accessor.LineNumber);
                    }
                    if (this.set != null)
                    {
                        throw new SyntaxException(base.Document.SourceCode, accessor.LineNumber);
                    }
                    this.set = accessor;
                }
            }
        }

        public Accessor GetAccessor
        {
            get
            {
                return this.get;
            }
        }

        public IList<Parameter> Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        public TypeToken ReturnType
        {
            get
            {
                return this.returnType;
            }
        }

        public Accessor SetAccessor
        {
            get
            {
                return this.set;
            }
        }
    }
}

