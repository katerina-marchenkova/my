namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml;

    internal static class V43Settings
    {
        private static string ConvertLegacyAddInName(string addInName)
        {
            if (addInName.StartsWith("Microsoft.SourceAnalysis", StringComparison.Ordinal))
            {
                return ("Microsoft.StyleCop" + addInName.Substring("Microsoft.SourceAnalysis".Length, addInName.Length - "Microsoft.SourceAnalysis".Length));
            }
            return addInName;
        }

        public static void Load(XmlDocument document, Settings settings)
        {
            XmlNode propertyCollectionNode = document.DocumentElement["GlobalSettings"];
            if (propertyCollectionNode != null)
            {
                LoadPropertyCollection(propertyCollectionNode, settings.GlobalSettings, settings.Core.PropertyDescriptors, null);
            }
            LoadParserSettings(document, settings);
            LoadAnalyzerSettings(document, settings);
        }

        private static void LoadAnalyzerSettings(XmlDocument document, Settings settings)
        {
            XmlNodeList list = document.DocumentElement.SelectNodes("Analyzers/Analyzer");
            if ((list != null) && (list.Count > 0))
            {
                foreach (XmlNode node in list)
                {
                    XmlAttribute attribute = node.Attributes["AnalyzerId"];
                    if ((attribute != null) && !string.IsNullOrEmpty(attribute.Value))
                    {
                        string analyzerId = ConvertLegacyAddInName(attribute.Value);
                        SourceAnalyzer addIn = settings.Core.GetAnalyzer(analyzerId);
                        if (addIn != null)
                        {
                            AddInPropertyCollection addInSettings = settings.GetAddInSettings(addIn);
                            if (addInSettings == null)
                            {
                                addInSettings = new AddInPropertyCollection(addIn);
                                settings.SetAddInSettings(addInSettings);
                            }
                            XmlNode propertyCollectionNode = node["AnalyzerSettings"];
                            if (propertyCollectionNode != null)
                            {
                                LoadPropertyCollection(propertyCollectionNode, addInSettings, addIn.PropertyDescriptors, null);
                            }
                            LoadRulesSettings(node, addInSettings, addIn.PropertyDescriptors);
                        }
                    }
                }
            }
        }

        private static void LoadBooleanProperty(string propertyName, XmlNode propertyNode, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors)
        {
            bool flag;
            if (bool.TryParse(propertyNode.InnerText, out flag))
            {
                PropertyDescriptor<bool> propertyDescriptor = propertyDescriptors[propertyName] as PropertyDescriptor<bool>;
                if (propertyDescriptor != null)
                {
                    properties.Add(new BooleanProperty(propertyDescriptor, flag));
                }
            }
        }

        private static void LoadCollectionProperty(string propertyName, XmlNode propertyNode, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors)
        {
            List<string> innerCollection = new List<string>();
            XmlNodeList list2 = propertyNode.SelectNodes("Value");
            if ((list2 != null) && (list2.Count > 0))
            {
                foreach (XmlNode node in list2)
                {
                    if (!string.IsNullOrEmpty(node.InnerText))
                    {
                        innerCollection.Add(node.InnerText);
                    }
                }
            }
            if (innerCollection.Count > 0)
            {
                CollectionPropertyDescriptor propertyDescriptor = propertyDescriptors[propertyName] as CollectionPropertyDescriptor;
                if (propertyDescriptor != null)
                {
                    CollectionProperty property = new CollectionProperty(propertyDescriptor, innerCollection);
                    properties.Add(property);
                }
            }
        }

        private static void LoadIntProperty(string propertyName, XmlNode propertyNode, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors)
        {
            int num;
            if (int.TryParse(propertyNode.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
            {
                PropertyDescriptor<int> propertyDescriptor = propertyDescriptors[propertyName] as PropertyDescriptor<int>;
                if (propertyDescriptor != null)
                {
                    properties.Add(new IntProperty(propertyDescriptor, num));
                }
            }
        }

        private static void LoadParserSettings(XmlDocument document, Settings settings)
        {
            XmlNodeList list = document.DocumentElement.SelectNodes("Parsers/Parser");
            if ((list != null) && (list.Count > 0))
            {
                foreach (XmlNode node in list)
                {
                    XmlAttribute attribute = node.Attributes["ParserId"];
                    if ((attribute != null) && !string.IsNullOrEmpty(attribute.Value))
                    {
                        string parserId = ConvertLegacyAddInName(attribute.Value);
                        SourceParser addIn = settings.Core.GetParser(parserId);
                        if (addIn != null)
                        {
                            AddInPropertyCollection addInSettings = settings.GetAddInSettings(addIn);
                            if (addInSettings == null)
                            {
                                addInSettings = new AddInPropertyCollection(addIn);
                                settings.SetAddInSettings(addInSettings);
                            }
                            XmlNode propertyCollectionNode = node["ParserSettings"];
                            if (propertyCollectionNode != null)
                            {
                                LoadPropertyCollection(propertyCollectionNode, addInSettings, addIn.PropertyDescriptors, null);
                            }
                            LoadRulesSettings(node, addInSettings, addIn.PropertyDescriptors);
                        }
                    }
                }
            }
        }

        private static void LoadPropertyCollection(XmlNode propertyCollectionNode, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors, string ruleName)
        {
            foreach (XmlNode node in propertyCollectionNode.ChildNodes)
            {
                string innerText;
                XmlAttribute attribute = node.Attributes["Name"];
                if ((attribute != null) && !string.IsNullOrEmpty(attribute.Value))
                {
                    innerText = attribute.InnerText;
                    if (!string.IsNullOrEmpty(ruleName))
                    {
                        innerText = ruleName + "#" + attribute.InnerText;
                    }
                    string name = node.Name;
                    if (name != null)
                    {
                        if (!(name == "BooleanProperty"))
                        {
                            if (name == "IntegerProperty")
                            {
                                goto Label_00B5;
                            }
                            if (name == "StringProperty")
                            {
                                goto Label_00C0;
                            }
                            if (name == "CollectionProperty")
                            {
                                goto Label_00CB;
                            }
                        }
                        else
                        {
                            LoadBooleanProperty(innerText, node, properties, propertyDescriptors);
                        }
                    }
                }
                continue;
            Label_00B5:
                LoadIntProperty(innerText, node, properties, propertyDescriptors);
                continue;
            Label_00C0:
                LoadStringProperty(innerText, node, properties, propertyDescriptors);
                continue;
            Label_00CB:
                LoadCollectionProperty(innerText, node, properties, propertyDescriptors);
            }
        }

        private static void LoadRulesSettings(XmlNode addInNode, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors)
        {
            XmlNode node = addInNode["Rules"];
            if (node != null)
            {
                foreach (XmlNode node2 in node.ChildNodes)
                {
                    if (string.Equals(node2.Name, "Rule", StringComparison.Ordinal))
                    {
                        XmlAttribute attribute = node2.Attributes["Name"];
                        if ((attribute != null) && !string.IsNullOrEmpty(attribute.Value))
                        {
                            string ruleName = attribute.Value;
                            XmlNode propertyCollectionNode = node2["RuleSettings"];
                            if (propertyCollectionNode != null)
                            {
                                LoadPropertyCollection(propertyCollectionNode, properties, propertyDescriptors, ruleName);
                            }
                        }
                    }
                }
            }
        }

        private static void LoadStringProperty(string propertyName, XmlNode propertyNode, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors)
        {
            PropertyDescriptor<string> propertyDescriptor = propertyDescriptors[propertyName] as PropertyDescriptor<string>;
            if (propertyDescriptor != null)
            {
                properties.Add(new StringProperty(propertyDescriptor, propertyNode.InnerText));
            }
        }
    }
}

