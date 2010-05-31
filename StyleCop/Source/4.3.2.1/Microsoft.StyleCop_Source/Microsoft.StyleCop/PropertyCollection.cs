namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    public class PropertyCollection : ICollection<PropertyValue>, IEnumerable<PropertyValue>, IEnumerable
    {
        private Dictionary<string, PropertyValue> properties = new Dictionary<string, PropertyValue>();
        private bool readOnly;

        internal PropertyCollection()
        {
            this.properties = new Dictionary<string, PropertyValue>();
        }

        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId="0#", Justification="'Property' is a more appropriate name than 'item' for a property.")]
        public void Add(PropertyValue property)
        {
            Param.RequireNotNull(property, "property");
            this.SetProperty(property);
        }

        public void Clear()
        {
            if (this.readOnly)
            {
                throw new StyleCopException(Strings.ReadOnlyProperty);
            }
            this.properties.Clear();
        }

        public virtual PropertyCollection Clone()
        {
            PropertyCollection propertys = new PropertyCollection();
            foreach (KeyValuePair<string, PropertyValue> pair in this.properties)
            {
                propertys.Add(pair.Value.Clone());
            }
            return propertys;
        }

        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId="0#", Justification="'Property' is a more appropriate name than 'item' for a property.")]
        public bool Contains(PropertyValue property)
        {
            Param.RequireNotNull(property, "property");
            return this.properties.ContainsKey(property.PropertyName);
        }

        public bool Contains(string propertyName)
        {
            Param.RequireValidString(propertyName, "propertyName");
            return this.properties.ContainsKey(propertyName);
        }

        public void CopyTo(PropertyValue[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<PropertyValue> GetEnumerator()
        {
            return this.properties.Values.GetEnumerator();
        }

        public PropertyValue GetProperty(string propertyName)
        {
            Param.RequireValidString(propertyName, "propertyName");
            PropertyValue value2 = null;
            if (this.properties.TryGetValue(propertyName, out value2))
            {
                return value2;
            }
            return null;
        }

        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId="0#", Justification="'Property' is a more appropriate name than 'item' for a property.")]
        public bool Remove(PropertyValue property)
        {
            Param.RequireNotNull(property, "property");
            if (this.readOnly)
            {
                throw new StyleCopException(Strings.ReadOnlyProperty);
            }
            return this.properties.Remove(property.PropertyName);
        }

        public bool Remove(string propertyName)
        {
            Param.RequireValidString(propertyName, "propertyName");
            if (this.readOnly)
            {
                throw new StyleCopException(Strings.ReadOnlyProperty);
            }
            return this.properties.Remove(propertyName);
        }

        public void SetProperty(PropertyValue property)
        {
            Param.RequireNotNull(property, "property");
            if (this.readOnly)
            {
                throw new StyleCopException(Strings.ReadOnlyProperty);
            }
            if (this.properties.ContainsKey(property.PropertyName))
            {
                this.properties[property.PropertyName] = property;
            }
            else
            {
                this.properties.Add(property.PropertyName, property);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return this.properties.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.readOnly;
            }
            internal set
            {
                if (this.readOnly != value)
                {
                    this.readOnly = value;
                    foreach (PropertyValue value2 in this.properties.Values)
                    {
                        value2.IsReadOnly = value;
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="propertyName", Justification="The set accessor does not use the propertyName parameter since the name of the property is built-in to the PropertyValue object.")]
        public PropertyValue this[string propertyName]
        {
            get
            {
                return this.GetProperty(propertyName);
            }
            set
            {
                this.SetProperty(value);
            }
        }

        public ICollection<PropertyValue> Properties
        {
            get
            {
                return this.properties.Values;
            }
        }
    }
}

