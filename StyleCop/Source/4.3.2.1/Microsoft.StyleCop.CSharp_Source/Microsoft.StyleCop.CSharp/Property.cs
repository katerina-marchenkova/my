namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification="The class describes a C# property.")]
    public sealed class Property : CsElement
    {
        private Accessor get;
        private TypeToken returnType;
        private Accessor set;

        internal Property(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, TypeToken returnType, bool unsafeCode, bool generated) : base(document, parent, ElementType.Property, "property " + declaration.Name, header, attributes, declaration, unsafeCode, generated)
        {
            this.returnType = returnType;
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

