namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;

    public sealed class Accessor : CsElement, IParameterContainer
    {
        private Microsoft.StyleCop.CSharp.AccessorType accessorType;
        private IList<Parameter> parameters;
        private TypeToken returnType;

        internal Accessor(CsDocument document, CsElement parent, Microsoft.StyleCop.CSharp.AccessorType accessorType, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, bool unsafeCode, bool generated) : base(document, parent, ElementType.Accessor, declaration.Name + " accessor", header, attributes, declaration, unsafeCode, generated)
        {
            this.accessorType = accessorType;
            this.FillDetails(parent);
        }

        private static TypeToken CreateVoidTypeToken()
        {
            return new TypeToken(new MasterList<CsToken>(new CsToken[] { new CsToken("void", CsTokenType.Other, CsTokenClass.Token, new CodeLocation(), false) }), new CodeLocation(), false);
        }

        private void FillDetails(CsElement parent)
        {
            if (this.accessorType == Microsoft.StyleCop.CSharp.AccessorType.Get)
            {
                this.FillGetAccessorDetails(parent);
            }
            else if (this.accessorType == Microsoft.StyleCop.CSharp.AccessorType.Set)
            {
                this.FillSetAccessorDetails(parent);
            }
            else
            {
                Event event2 = (Event) parent;
                this.returnType = CreateVoidTypeToken();
                this.parameters = new Parameter[] { new Parameter(event2.EventHandlerType, "value", ParameterModifiers.None, new CodeLocation(), null, event2.EventHandlerType.Generated) };
            }
        }

        private void FillGetAccessorDetails(CsElement parent)
        {
            Property property = parent as Property;
            if (property != null)
            {
                this.returnType = property.ReturnType;
            }
            else
            {
                Indexer indexer = (Indexer) parent;
                this.returnType = indexer.ReturnType;
                this.parameters = indexer.Parameters;
            }
        }

        private void FillSetAccessorDetails(CsElement parent)
        {
            Property property = parent as Property;
            if (property != null)
            {
                this.returnType = CreateVoidTypeToken();
                this.parameters = new Parameter[] { new Parameter(property.ReturnType, "value", ParameterModifiers.None, new CodeLocation(), null, property.ReturnType.Generated) };
            }
            else
            {
                Indexer indexer = (Indexer) parent;
                this.returnType = CreateVoidTypeToken();
                Parameter[] parameterArray = new Parameter[indexer.Parameters.Count + 1];
                int index = 0;
                foreach (Parameter parameter in indexer.Parameters)
                {
                    parameterArray[index++] = parameter;
                }
                parameterArray[index] = new Parameter(indexer.ReturnType, "value", ParameterModifiers.None, new CodeLocation(), null, indexer.ReturnType.Generated);
                this.parameters = parameterArray;
            }
        }

        internal override void Initialize()
        {
            base.Initialize();
            if (this.parameters != null)
            {
                foreach (Parameter parameter in this.parameters)
                {
                    Variable variable = new Variable(parameter.Type, parameter.Name, VariableModifiers.None, parameter.Location.StartPoint, parameter.Generated);
                    base.Variables.Add(variable);
                }
            }
        }

        public Microsoft.StyleCop.CSharp.AccessorType AccessorType
        {
            get
            {
                return this.accessorType;
            }
        }

        public IList<Parameter> Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    return Parameter.EmptyParameterArray;
                }
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
    }
}

