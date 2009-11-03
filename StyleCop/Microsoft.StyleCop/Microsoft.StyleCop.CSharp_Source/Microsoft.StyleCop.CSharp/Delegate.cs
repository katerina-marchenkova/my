namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification="The class should not have any suffix."), SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification="The class describes a C# delegate.")]
    public sealed class Delegate : CsElement, IParameterContainer, ITypeConstraintContainer
    {
        private IList<Parameter> parameters;
        private TypeToken returnType;
        private ICollection<TypeParameterConstraintClause> typeConstraints;

        internal Delegate(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, TypeToken returnType, IList<Parameter> parameters, ICollection<TypeParameterConstraintClause> typeConstraints, bool unsafeCode, bool generated) : base(document, parent, ElementType.Delegate, "delegate " + declaration.Name, header, attributes, declaration, unsafeCode, generated)
        {
            this.returnType = returnType;
            this.typeConstraints = typeConstraints;
            this.parameters = parameters;
            base.QualifiedName = CodeParser.AddQualifications(this.parameters, base.QualifiedName);
            if (typeConstraints != null)
            {
                foreach (TypeParameterConstraintClause clause in typeConstraints)
                {
                    clause.ParentElement = this;
                }
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

