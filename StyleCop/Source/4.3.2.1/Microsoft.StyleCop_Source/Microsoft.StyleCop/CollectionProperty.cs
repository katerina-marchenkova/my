namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification="The name should not end in Collection as it will resuilt in a very confusing name.")]
    public class CollectionProperty : PropertyValue, ICollection<string>, IEnumerable<string>, IEnumerable
    {
        private List<string> collection;

        public CollectionProperty(CollectionPropertyDescriptor propertyDescriptor) : this(propertyDescriptor, null)
        {
        }

        public CollectionProperty(CollectionPropertyDescriptor propertyDescriptor, IEnumerable<string> innerCollection) : base(propertyDescriptor)
        {
            Param.RequireNotNull(propertyDescriptor, "propertyDescriptor");
            if (innerCollection != null)
            {
                this.collection = new List<string>(innerCollection);
            }
            else
            {
                this.collection = new List<string>();
            }
        }

        public CollectionProperty(IPropertyContainer propertyContainer, string propertyName) : this(propertyContainer, propertyName, null)
        {
            Param.RequireNotNull(propertyContainer, "propertyContainer");
            Param.RequireValidString(propertyName, "propertyName");
        }

        public CollectionProperty(IPropertyContainer propertyContainer, string propertyName, IEnumerable<string> innerCollection) : base(propertyContainer, propertyName)
        {
            Param.RequireNotNull(propertyContainer, "propertyContainer");
            Param.RequireValidString(propertyName, "propertyName");
            if (innerCollection != null)
            {
                this.collection = new List<string>(innerCollection);
            }
            else
            {
                this.collection = new List<string>();
            }
        }

        public void Add(string item)
        {
            Param.RequireNotNull(item, "item");
            if (this.IsReadOnly)
            {
                throw new StyleCopException(Strings.ReadOnlyProperty);
            }
            this.collection.Add(item);
        }

        public void Clear()
        {
            if (this.IsReadOnly)
            {
                throw new StyleCopException(Strings.ReadOnlyProperty);
            }
            this.collection.Clear();
        }

        public override PropertyValue Clone()
        {
            return new CollectionProperty((CollectionPropertyDescriptor) base.PropertyDescriptor, this.collection);
        }

        public bool Contains(string item)
        {
            Param.RequireNotNull(item, "item");
            return this.collection.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        public override bool OverridesProperty(PropertyValue parentProperty)
        {
            Param.RequireNotNull(parentProperty, "parentProperty");
            CollectionProperty property = parentProperty as CollectionProperty;
            if ((property == null) || (this.Aggregate != property.Aggregate))
            {
                throw new ArgumentException(Strings.ComparingDifferentPropertyTypes, "parentProperty");
            }
            return OverridesPropertyCollection(this.collection, property.Values, this.Aggregate);
        }

        private static bool OverridesPropertyCollection(ICollection<string> localValues, ICollection<string> parentValues, bool aggregate)
        {
            if ((localValues != null) && (localValues.Count != 0))
            {
                if ((parentValues == null) || (parentValues.Count == 0))
                {
                    return true;
                }
                foreach (string str in localValues)
                {
                    if (!parentValues.Contains(str))
                    {
                        return true;
                    }
                }
                if (!aggregate)
                {
                    foreach (string str2 in parentValues)
                    {
                        if (!localValues.Contains(str2))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool Remove(string item)
        {
            Param.RequireNotNull(item, "item");
            if (this.IsReadOnly)
            {
                throw new StyleCopException(Strings.ReadOnlyProperty);
            }
            return this.collection.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Aggregate
        {
            get
            {
                return ((CollectionPropertyDescriptor) base.PropertyDescriptor).Aggregate;
            }
        }

        public int Count
        {
            get
            {
                return this.collection.Count;
            }
        }

        public override bool HasDefaultValue
        {
            get
            {
                return false;
            }
        }

        public override bool IsDefault
        {
            get
            {
                return false;
            }
        }

        public ICollection<string> Values
        {
            get
            {
                return this.collection;
            }
        }
    }
}

