namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;

    public class ExpressionWithParameters : Expression, IParameterContainer
    {
        private List<Parameter> parameters;
        private IList<Parameter> readOnlyParameters;

        internal ExpressionWithParameters(ExpressionType type) : base(type)
        {
        }

        internal void AddParameter(Parameter parameter)
        {
            if (this.parameters == null)
            {
                this.parameters = new List<Parameter>();
            }
            this.parameters.Add(parameter);
        }

        internal void AddParameters(IEnumerable<Parameter> items)
        {
            if (this.parameters == null)
            {
                this.parameters = new List<Parameter>();
            }
            this.parameters.AddRange(items);
        }

        public IList<Parameter> Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    return Parameter.EmptyParameterArray;
                }
                if (this.readOnlyParameters == null)
                {
                    this.readOnlyParameters = this.parameters.AsReadOnly();
                }
                return this.readOnlyParameters;
            }
        }
    }
}

