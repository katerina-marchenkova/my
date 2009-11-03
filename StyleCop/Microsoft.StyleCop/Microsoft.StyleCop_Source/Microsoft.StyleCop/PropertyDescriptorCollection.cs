namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Xml;

    public class PropertyDescriptorCollection : ICollection<PropertyDescriptor>, IEnumerable<PropertyDescriptor>, IEnumerable
    {
        private Dictionary<string, PropertyDescriptor> propertyDescriptors;

        internal PropertyDescriptorCollection()
        {
        }

        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId="0#", Justification="'Property' is a more appropriate name than 'item' for a property.")]
        public void Add(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        private void AddBooleanPropertyDescriptor(XmlNode propertyNode)
        {
            XmlAttribute attribute = propertyNode.Attributes["DefaultValue"];
            if ((attribute == null) || string.IsNullOrEmpty(attribute.Value))
            {
                throw new ArgumentException(Strings.PropertyDescriptorHasNoDefaultValue);
            }
            bool defaultValue = bool.Parse(attribute.Value);
            string propertyName = ExtractPropertyName(propertyNode);
            PropertyDescriptor<bool> descriptor = new PropertyDescriptor<bool>(propertyName, PropertyType.Boolean, ExtractFriendlyName(propertyNode), ExtractDescription(propertyNode), ExtractMerge(propertyNode), ExtractDisplaySettings(propertyNode), defaultValue);
            this.propertyDescriptors.Add(propertyName, descriptor);
        }

        private void AddCollectionPropertyDescriptor(XmlNode propertyNode)
        {
            bool aggregate = false;
            XmlAttribute attribute = propertyNode.Attributes["Aggregate"];
            if (attribute != null)
            {
                aggregate = bool.Parse(attribute.Value);
            }
            string propertyName = ExtractPropertyName(propertyNode);
            CollectionPropertyDescriptor descriptor = new CollectionPropertyDescriptor(propertyName, ExtractFriendlyName(propertyNode), ExtractDescription(propertyNode), ExtractMerge(propertyNode), aggregate);
            this.propertyDescriptors.Add(propertyName, descriptor);
        }

        private void AddIntPropertyDescriptor(XmlNode propertyNode)
        {
            XmlAttribute attribute = propertyNode.Attributes["DefaultValue"];
            if ((attribute == null) || string.IsNullOrEmpty(attribute.Value))
            {
                throw new ArgumentException(Strings.PropertyDescriptorHasNoDefaultValue);
            }
            int defaultValue = int.Parse(attribute.Value, CultureInfo.InvariantCulture);
            string propertyName = ExtractPropertyName(propertyNode);
            PropertyDescriptor<int> descriptor = new PropertyDescriptor<int>(propertyName, PropertyType.Int, ExtractFriendlyName(propertyNode), ExtractDescription(propertyNode), ExtractMerge(propertyNode), ExtractDisplaySettings(propertyNode), defaultValue);
            this.propertyDescriptors.Add(propertyName, descriptor);
        }

        internal void AddPropertyDescriptor(PropertyDescriptor descriptor)
        {
            if (this.propertyDescriptors == null)
            {
                this.propertyDescriptors = new Dictionary<string, PropertyDescriptor>();
            }
            this.propertyDescriptors.Add(descriptor.PropertyName, descriptor);
        }

        private void AddStringPropertyDescriptor(XmlNode propertyNode)
        {
            XmlAttribute attribute = propertyNode.Attributes["DefaultValue"];
            if ((attribute == null) || (attribute.Value == null))
            {
                throw new ArgumentException(Strings.PropertyDescriptorHasNoDefaultValue);
            }
            string propertyName = ExtractPropertyName(propertyNode);
            PropertyDescriptor<string> descriptor = new PropertyDescriptor<string>(propertyName, PropertyType.String, ExtractFriendlyName(propertyNode), ExtractDescription(propertyNode), ExtractMerge(propertyNode), ExtractDisplaySettings(propertyNode), attribute.Value);
            this.propertyDescriptors.Add(propertyName, descriptor);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(PropertyDescriptor item)
        {
            Param.RequireNotNull(item, "item");
            return this.propertyDescriptors.ContainsKey(item.PropertyName);
        }

        public bool Contains(string propertyName)
        {
            Param.RequireValidString(propertyName, "propertyName");
            return this.propertyDescriptors.ContainsKey(propertyName);
        }

        public void CopyTo(PropertyDescriptor[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        private static string ExtractDescription(XmlNode propertyNode)
        {
            XmlAttribute attribute = propertyNode.Attributes["Description"];
            if ((attribute == null) || string.IsNullOrEmpty(attribute.Value))
            {
                throw new ArgumentException(Strings.PropertyHasNoDescription);
            }
            return attribute.Value;
        }

        private static bool ExtractDisplaySettings(XmlNode propertyNode)
        {
            XmlAttribute attribute = propertyNode.Attributes["DisplaySettings"];
            if ((attribute != null) && !string.IsNullOrEmpty(attribute.Value))
            {
                return bool.Parse(attribute.Value);
            }
            return true;
        }

        private static string ExtractFriendlyName(XmlNode propertyNode)
        {
            XmlAttribute attribute = propertyNode.Attributes["FriendlyName"];
            if ((attribute == null) || string.IsNullOrEmpty(attribute.Value))
            {
                throw new ArgumentException(Strings.PropertyHasNoFriendlyName);
            }
            return attribute.Value;
        }

        private static bool ExtractMerge(XmlNode propertyNode)
        {
            XmlAttribute attribute = propertyNode.Attributes["Merge"];
            if ((attribute != null) && !string.IsNullOrEmpty(attribute.Value))
            {
                return bool.Parse(attribute.Value);
            }
            return true;
        }

        private static string ExtractPropertyName(XmlNode propertyNode)
        {
            XmlAttribute attribute = propertyNode.Attributes["Name"];
            if ((attribute == null) || string.IsNullOrEmpty(attribute.Value))
            {
                throw new ArgumentException(Strings.PropertyHasNoName);
            }
            return attribute.Value;
        }

        public IEnumerator<PropertyDescriptor> GetEnumerator()
        {
            return this.propertyDescriptors.Values.GetEnumerator();
        }

        public PropertyDescriptor GetPropertyDescriptor(string propertyName)
        {
            Param.RequireValidString(propertyName, "propertyName");
            PropertyDescriptor descriptor = null;
            if (this.propertyDescriptors.TryGetValue(propertyName, out descriptor))
            {
                return descriptor;
            }
            return null;
        }

        internal void InitializeFromXml(XmlNode propertiesNode)
        {
            if ((propertiesNode.ChildNodes == null) || (propertiesNode.ChildNodes.Count == 0))
            {
                this.propertyDescriptors = new Dictionary<string, PropertyDescriptor>(0);
            }
            else
            {
                if (this.propertyDescriptors == null)
                {
                    this.propertyDescriptors = new Dictionary<string, PropertyDescriptor>(propertiesNode.ChildNodes.Count);
                }
                foreach (XmlNode node in propertiesNode.ChildNodes)
                {
                    string name = node.Name;
                    if (name != null)
                    {
                        if (!(name == "BooleanProperty"))
                        {
                            if (name == "IntegerProperty")
                            {
                                goto Label_00A3;
                            }
                            if (name == "StringProperty")
                            {
                                goto Label_00AC;
                            }
                            if (name == "CollectionProperty")
                            {
                                goto Label_00B5;
                            }
                        }
                        else
                        {
                            this.AddBooleanPropertyDescriptor(node);
                        }
                    }
                    continue;
                Label_00A3:
                    this.AddIntPropertyDescriptor(node);
                    continue;
                Label_00AC:
                    this.AddStringPropertyDescriptor(node);
                    continue;
                Label_00B5:
                    this.AddCollectionPropertyDescriptor(node);
                }
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId="0#", Justification="'Property' is a more appropriate name than 'item' for a property.")]
        public bool Remove(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return this.propertyDescriptors.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public PropertyDescriptor this[string propertyName]
        {
            get
            {
                return this.GetPropertyDescriptor(propertyName);
            }
        }

        public ICollection<PropertyDescriptor> PropertyDescriptors
        {
            get
            {
                return this.propertyDescriptors.Values;
            }
        }

        public ICollection<string> PropertyNames
        {
            get
            {
                return this.propertyDescriptors.Keys;
            }
        }
    }
}

