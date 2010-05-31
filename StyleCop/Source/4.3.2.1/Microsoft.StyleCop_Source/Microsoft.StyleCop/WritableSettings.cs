namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Xml;

    public class WritableSettings : Settings
    {
        internal WritableSettings(StyleCopCore core, string path, string location, XmlDocument contents, DateTime writeTime) : base(core, path, location, contents, writeTime)
        {
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="InSetting", Justification="InSetting is two words in this context.")]
        public void ClearAddInSetting(StyleCopAddIn addIn, string propertyName)
        {
            Param.RequireNotNull(addIn, "addIn");
            Param.RequireValidString(propertyName, "propertyName");
            base.ClearAddInSettingInternal(addIn, propertyName);
        }

        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId="System.Xml.XmlNode", Justification="Compliance would break the well-defined public API.")]
        public static XmlDocument NewDocument()
        {
            XmlDocument document = new XmlDocument();
            XmlElement newChild = document.CreateElement("StyleCopSettings");
            document.AppendChild(newChild);
            XmlAttribute node = document.CreateAttribute("Version");
            node.Value = "4.3";
            newChild.Attributes.Append(node);
            return document;
        }

        private static bool SaveBooleanProperty(XmlNode rootNode, BooleanProperty property, string propertyName)
        {
            XmlNode newChild = rootNode.OwnerDocument.CreateElement("BooleanProperty");
            rootNode.AppendChild(newChild);
            XmlAttribute node = rootNode.OwnerDocument.CreateAttribute("Name");
            node.Value = propertyName;
            newChild.Attributes.Append(node);
            newChild.InnerText = property.Value.ToString();
            return true;
        }

        private static bool SaveCollectionProperty(XmlNode rootNode, CollectionProperty property, string propertyName)
        {
            if (property.Values.Count <= 0)
            {
                return false;
            }
            XmlNode newChild = rootNode.OwnerDocument.CreateElement("CollectionProperty");
            rootNode.AppendChild(newChild);
            XmlAttribute node = rootNode.OwnerDocument.CreateAttribute("Name");
            node.Value = propertyName;
            newChild.Attributes.Append(node);
            foreach (string str in property.Values)
            {
                XmlNode node2 = rootNode.OwnerDocument.CreateElement("Value");
                node2.InnerText = str;
                newChild.AppendChild(node2);
            }
            return true;
        }

        private static bool SaveIntProperty(XmlNode rootNode, IntProperty property, string propertyName)
        {
            XmlNode newChild = rootNode.OwnerDocument.CreateElement("IntegerProperty");
            rootNode.AppendChild(newChild);
            XmlAttribute node = rootNode.OwnerDocument.CreateAttribute("Name");
            node.Value = propertyName;
            newChild.Attributes.Append(node);
            newChild.InnerText = property.Value.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        private static bool SavePropertyCollection(XmlNode rootNode, string nodeName, PropertyCollection properties, PropertyCollection parentProperties, bool aggregate, string nodeNameAttribute)
        {
            if (properties.Count == 0)
            {
                return false;
            }
            bool flag = false;
            XmlElement rootCollectionNode = rootNode.OwnerDocument.CreateElement(nodeName);
            if (!string.IsNullOrEmpty(nodeNameAttribute))
            {
                XmlAttribute node = rootNode.OwnerDocument.CreateAttribute("Name");
                node.Value = nodeNameAttribute;
                rootCollectionNode.Attributes.Append(node);
            }
            foreach (PropertyValue value2 in properties)
            {
                bool flag2 = false;
                PropertyValue parentProperty = null;
                if (parentProperties != null)
                {
                    parentProperty = parentProperties[value2.PropertyName];
                    if (aggregate && (parentProperty != null))
                    {
                        if (!SettingsComparer.IsSettingOverwritten(value2, parentProperty))
                        {
                            flag2 = true;
                        }
                    }
                    else if (value2.IsDefault)
                    {
                        flag2 = true;
                    }
                }
                else if (value2.IsDefault)
                {
                    flag2 = true;
                }
                if (!flag2)
                {
                    int index = value2.PropertyName.IndexOf('#');
                    if (index > 0)
                    {
                        flag |= SaveRuleProperty(rootNode, value2, value2.PropertyName.Substring(0, index), value2.PropertyName.Substring(index + 1, (value2.PropertyName.Length - index) - 1));
                        continue;
                    }
                    flag |= SavePropertyValue(rootCollectionNode, value2, value2.PropertyName);
                }
            }
            if (flag)
            {
                rootNode.AppendChild(rootCollectionNode);
            }
            return flag;
        }

        private static bool SavePropertyValue(XmlNode rootCollectionNode, PropertyValue property, string propertyName)
        {
            bool flag = false;
            switch (property.PropertyType)
            {
                case PropertyType.String:
                    return (flag | SaveStringProperty(rootCollectionNode, property as StringProperty, propertyName));

                case PropertyType.Boolean:
                    return (flag | SaveBooleanProperty(rootCollectionNode, property as BooleanProperty, propertyName));

                case PropertyType.Int:
                    return (flag | SaveIntProperty(rootCollectionNode, property as IntProperty, propertyName));

                case PropertyType.Collection:
                    return (flag | SaveCollectionProperty(rootCollectionNode, property as CollectionProperty, propertyName));
            }
            return flag;
        }

        private static bool SaveRuleProperty(XmlNode rootNode, PropertyValue property, string ruleName, string propertyName)
        {
            XmlNode newChild = rootNode.SelectSingleNode("Rules");
            if (newChild == null)
            {
                newChild = rootNode.OwnerDocument.CreateElement("Rules");
                rootNode.AppendChild(newChild);
            }
            XmlNode node2 = newChild.SelectSingleNode("Rule[@Name=\"" + ruleName + "\"]");
            if (node2 == null)
            {
                node2 = rootNode.OwnerDocument.CreateElement("Rule");
                newChild.AppendChild(node2);
                XmlAttribute node = rootNode.OwnerDocument.CreateAttribute("Name");
                node.Value = ruleName;
                node2.Attributes.Append(node);
            }
            XmlNode node3 = node2.SelectSingleNode("RuleSettings");
            if (node3 == null)
            {
                node3 = rootNode.OwnerDocument.CreateElement("RuleSettings");
                node2.AppendChild(node3);
            }
            return SavePropertyValue(node3, property, propertyName);
        }

        private static bool SaveStringProperty(XmlNode rootNode, StringProperty property, string propertyName)
        {
            XmlNode newChild = rootNode.OwnerDocument.CreateElement("StringProperty");
            rootNode.AppendChild(newChild);
            XmlAttribute node = rootNode.OwnerDocument.CreateAttribute("Name");
            node.Value = propertyName;
            newChild.Attributes.Append(node);
            newChild.InnerText = property.Value;
            return true;
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="InSetting", Justification="InSetting is two words in this context.")]
        public void SetAddInSetting(StyleCopAddIn addIn, PropertyValue property)
        {
            Param.RequireNotNull(addIn, "addIn");
            Param.RequireNotNull(property, "property");
            base.SetAddInSettingInternal(addIn, property);
        }

        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId="System.Xml.XmlNode", Justification="Compliance would break the well-defined public API.")]
        public XmlDocument WriteSettingsToDocument(StyleCopEnvironment environment)
        {
            Param.RequireNotNull(environment, "environment");
            XmlDocument document = NewDocument();
            SettingsMerger merger = new SettingsMerger(this, environment);
            Settings parentMergedSettings = merger.ParentMergedSettings;
            if ((base.GlobalSettings != null) && (base.GlobalSettings.Count > 0))
            {
                PropertyCollection parentProperties = null;
                if (parentMergedSettings != null)
                {
                    parentProperties = parentMergedSettings.GlobalSettings;
                }
                SavePropertyCollection(document.DocumentElement, "GlobalSettings", base.GlobalSettings, parentProperties, true, null);
            }
            if (base.ParserSettings.Count > 0)
            {
                bool flag = false;
                XmlElement newChild = document.CreateElement("Parsers");
                foreach (AddInPropertyCollection propertys2 in base.ParserSettings)
                {
                    if (propertys2.Count > 0)
                    {
                        XmlElement rootNode = document.CreateElement("Parser");
                        XmlAttribute node = document.CreateAttribute("ParserId");
                        node.Value = propertys2.AddIn.Id;
                        rootNode.Attributes.Append(node);
                        PropertyCollection addInSettings = null;
                        if (parentMergedSettings != null)
                        {
                            addInSettings = parentMergedSettings.GetAddInSettings(propertys2.AddIn);
                        }
                        if (SavePropertyCollection(rootNode, "ParserSettings", propertys2, addInSettings, true, null))
                        {
                            newChild.AppendChild(rootNode);
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    document.DocumentElement.AppendChild(newChild);
                }
            }
            if (base.AnalyzerSettings.Count > 0)
            {
                bool flag2 = false;
                XmlElement element3 = document.CreateElement("Analyzers");
                foreach (AddInPropertyCollection propertys4 in base.AnalyzerSettings)
                {
                    if (propertys4.Count > 0)
                    {
                        XmlElement element4 = document.CreateElement("Analyzer");
                        XmlAttribute attribute2 = document.CreateAttribute("AnalyzerId");
                        attribute2.Value = propertys4.AddIn.Id;
                        element4.Attributes.Append(attribute2);
                        PropertyCollection propertys5 = null;
                        if (parentMergedSettings != null)
                        {
                            propertys5 = parentMergedSettings.GetAddInSettings(propertys4.AddIn);
                        }
                        if (SavePropertyCollection(element4, "AnalyzerSettings", propertys4, propertys5, true, null))
                        {
                            element3.AppendChild(element4);
                            flag2 = true;
                        }
                    }
                }
                if (flag2)
                {
                    document.DocumentElement.AppendChild(element3);
                }
            }
            return document;
        }
    }
}

