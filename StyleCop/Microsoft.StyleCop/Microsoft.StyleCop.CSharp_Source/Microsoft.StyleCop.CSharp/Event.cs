namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification="The class describes a C# event")]
    public sealed class Event : CsElement
    {
        private Accessor add;
        private TypeToken eventHandlerType;
        private Expression initializationExpression;
        private Accessor remove;

        internal Event(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, TypeToken eventHandlerType, bool unsafeCode, bool generated) : base(document, parent, ElementType.Event, "event " + declaration.Name, header, attributes, declaration, unsafeCode, generated)
        {
            this.eventHandlerType = eventHandlerType;
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
                if (accessor.AccessorType == AccessorType.Add)
                {
                    if (this.add != null)
                    {
                        throw new SyntaxException(base.Document.SourceCode, accessor.LineNumber);
                    }
                    this.add = accessor;
                }
                else
                {
                    if (accessor.AccessorType != AccessorType.Remove)
                    {
                        throw new SyntaxException(base.Document.SourceCode, accessor.LineNumber);
                    }
                    if (this.remove != null)
                    {
                        throw new SyntaxException(base.Document.SourceCode, accessor.LineNumber);
                    }
                    this.remove = accessor;
                }
            }
        }

        public Accessor AddAccessor
        {
            get
            {
                return this.add;
            }
        }

        public TypeToken EventHandlerType
        {
            get
            {
                return this.eventHandlerType;
            }
        }

        public Expression InitializationExpression
        {
            get
            {
                return this.initializationExpression;
            }
            internal set
            {
                this.initializationExpression = value;
                this.AddExpression(this.initializationExpression);
            }
        }

        public Accessor RemoveAccessor
        {
            get
            {
                return this.remove;
            }
        }
    }
}

