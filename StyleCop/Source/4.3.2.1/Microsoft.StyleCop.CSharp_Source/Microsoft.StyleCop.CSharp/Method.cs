namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class Method : CsElement, IParameterContainer, ITypeConstraintContainer
    {
        private bool extensionMethod;
        private IList<Parameter> parameters;
        private TypeToken returnType;
        private ICollection<TypeParameterConstraintClause> typeConstraints;

        internal Method(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, TypeToken returnType, IList<Parameter> parameters, ICollection<TypeParameterConstraintClause> typeConstraints, bool unsafeCode, bool generated) : base(document, parent, ElementType.Method, "method " + declaration.Name, header, attributes, declaration, unsafeCode, generated)
        {
            this.returnType = returnType;
            this.parameters = parameters;
            this.typeConstraints = typeConstraints;
            if ((this.parameters.Count > 0) && base.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static }))
            {
                foreach (Parameter parameter in this.parameters)
                {
                    if ((parameter.Modifiers & ParameterModifiers.This) != ParameterModifiers.None)
                    {
                        this.extensionMethod = true;
                    }
                    break;
                }
            }
            base.QualifiedName = CodeParser.AddQualifications(this.parameters, base.QualifiedName);
            if ((base.Declaration.Name.IndexOf(".", StringComparison.Ordinal) > -1) && !base.Declaration.Name.StartsWith("this.", StringComparison.Ordinal))
            {
                base.Declaration.AccessModifierType = AccessModifierType.Public;
            }
            if (typeConstraints != null)
            {
                foreach (TypeParameterConstraintClause clause in typeConstraints)
                {
                    clause.ParentElement = this;
                }
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

        public bool IsExtensionMethod
        {
            get
            {
                return this.extensionMethod;
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

        public ICollection<TypeParameterConstraintClause> TypeConstraints
        {
            get
            {
                return this.typeConstraints;
            }
        }
    }
}

