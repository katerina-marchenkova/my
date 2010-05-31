namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public sealed class Constructor : CsElement, IParameterContainer
    {
        private MethodInvocationExpression initializer;
        private IList<Parameter> parameters;

        internal Constructor(CsDocument document, CsElement parent, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Declaration declaration, IList<Parameter> parameters, MethodInvocationExpression initializerExpression, bool unsafeCode, bool generated) : base(document, parent, ElementType.Constructor, "constructor " + declaration.Name, header, attributes, declaration, unsafeCode, generated)
        {
            if (base.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static }))
            {
                base.Declaration.AccessModifierType = AccessModifierType.Public;
            }
            this.parameters = parameters;
            base.QualifiedName = CodeParser.AddQualifications(this.parameters, base.QualifiedName);
            if (initializerExpression != null)
            {
                this.initializer = initializerExpression;
                ConstructorInitializerStatement statement = new ConstructorInitializerStatement(initializerExpression.Tokens, initializerExpression);
                this.AddStatement(statement);
            }
        }

        internal override void Initialize()
        {
            if (this.parameters != null)
            {
                foreach (Parameter parameter in this.parameters)
                {
                    base.Variables.Add(new Variable(parameter.Type, parameter.Name, VariableModifiers.None, parameter.Location.StartPoint, parameter.Generated));
                }
            }
        }

        public MethodInvocationExpression Initializer
        {
            get
            {
                return this.initializer;
            }
        }

        public IList<Parameter> Parameters
        {
            get
            {
                return this.parameters;
            }
        }
    }
}

