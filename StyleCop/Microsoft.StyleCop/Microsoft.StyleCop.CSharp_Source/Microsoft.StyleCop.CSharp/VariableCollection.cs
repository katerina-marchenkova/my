namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class VariableCollection : ICollection<Variable>, IEnumerable<Variable>, IEnumerable
    {
        private static Variable[] emptyVariableArray = new Variable[0];
        private static LegacyEnumeratorAdapter<Variable> emptyVariableArrayEnumerator;
        private Dictionary<string, Variable> variables;

        internal VariableCollection()
        {
        }

        internal void Add(Variable variable)
        {
            if (this.variables == null)
            {
                this.variables = new Dictionary<string, Variable>();
            }
            if (!this.variables.ContainsKey(variable.Name))
            {
                this.variables.Add(variable.Name, variable);
            }
        }

        internal void AddRange(IEnumerable<Variable> items)
        {
            foreach (Variable variable in items)
            {
                this.Add(variable);
            }
        }

        public bool Contains(string name)
        {
            Param.RequireNotNull(name, "name");
            if (this.variables == null)
            {
                return false;
            }
            return this.variables.ContainsKey(name);
        }

        public void CopyTo(Variable[] array, int arrayIndex)
        {
            Param.RequireNotNull(array, "array");
            Param.RequireGreaterThanOrEqualToZero(arrayIndex, "arrayIndex");
            if (this.variables != null)
            {
                int num = arrayIndex;
                foreach (Variable variable in this.variables.Values)
                {
                    array[num++] = variable;
                }
            }
        }

        public IEnumerator<Variable> GetEnumerator()
        {
            if (this.variables != null)
            {
                return this.variables.Values.GetEnumerator();
            }
            if (emptyVariableArrayEnumerator == null)
            {
                emptyVariableArrayEnumerator = new LegacyEnumeratorAdapter<Variable>(emptyVariableArray.GetEnumerator());
            }
            return emptyVariableArrayEnumerator;
        }

        public Variable GetVariable(string name)
        {
            Variable variable;
            Param.RequireValidString(name, "name");
            if ((this.variables != null) && this.variables.TryGetValue(name, out variable))
            {
                return variable;
            }
            return null;
        }

        void ICollection<Variable>.Add(Variable variable)
        {
            throw new NotSupportedException();
        }

        void ICollection<Variable>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<Variable>.Contains(Variable variable)
        {
            throw new NotSupportedException();
        }

        bool ICollection<Variable>.Remove(Variable variable)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.variables == null)
            {
                return emptyVariableArray.GetEnumerator();
            }
            return this.variables.Values.GetEnumerator();
        }

        public int Count
        {
            get
            {
                if (this.variables == null)
                {
                    return 0;
                }
                return this.variables.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public Variable this[string name]
        {
            get
            {
                return this.GetVariable(name);
            }
        }
    }
}

