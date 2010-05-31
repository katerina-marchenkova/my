namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Xml;

    [StyleCopAddIn]
    public abstract class StyleCopAddIn : IPropertyContainer
    {
        private StyleCopCore core;
        private const string DefaultCheckIdPrefix = "SA";
        private string description;
        private string id;
        private string name;
        private PropertyDescriptorCollection propertyDescriptors = new PropertyDescriptorCollection();
        private Dictionary<string, Rule> rules = new Dictionary<string, Rule>();

        protected StyleCopAddIn()
        {
            this.id = GetIdFromAddInType(base.GetType());
        }

        private void AddRulesFromXml(XmlNode rulesNode, string ruleGroup, bool isKnownAssembly)
        {
            foreach (XmlNode node in rulesNode.ChildNodes)
            {
                if (node.Name == "RuleGroup")
                {
                    XmlAttribute attribute = node.Attributes["Name"];
                    if ((attribute == null) || (attribute.Value.Length == 0))
                    {
                        throw new ArgumentException(Strings.RuleGroupHasNoNameAttribute);
                    }
                    this.AddRulesFromXml(node, attribute.Value, isKnownAssembly);
                    continue;
                }
                if (node.Name == "Rule")
                {
                    XmlNode node2 = node.Attributes["Name"];
                    if ((node2 == null) || (node2.Value.Length == 0))
                    {
                        throw new ArgumentException(Strings.RuleHasNoNameAttribute);
                    }
                    XmlNode node3 = node.Attributes["CheckId"];
                    if ((node3 == null) || (node3.Value.Length == 0))
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.RuleHasNoCheckIdAttribute, new object[] { node2.Value }));
                    }
                    if (node3.Value.StartsWith("SA", StringComparison.Ordinal) && !isKnownAssembly)
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.UnknownAssemblyUsingDefaultCheckIdPrefix, new object[] { node3.Value }));
                    }
                    XmlNode node4 = node["Context"];
                    if ((node4 == null) || (node4.InnerText.Length == 0))
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.RuleHasNoContextElement, new object[] { node2.Value }));
                    }
                    string str = TrimXmlContent(node4.InnerText);
                    if (string.IsNullOrEmpty(str))
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.RuleHasNoContextElement, new object[] { node2.Value }));
                    }
                    XmlNode node5 = node["Description"];
                    XmlAttribute attribute2 = node.Attributes["Warning"];
                    XmlAttribute attribute3 = node.Attributes["DisabledByDefault"];
                    XmlAttribute attribute4 = node.Attributes["CanDisable"];
                    bool flag = (attribute3 != null) && Convert.ToBoolean(attribute3.Value, CultureInfo.InvariantCulture);
                    bool canDisable = (attribute4 == null) || Convert.ToBoolean(attribute4.Value, CultureInfo.InvariantCulture);
                    bool warning = (attribute2 != null) && Convert.ToBoolean(attribute2.Value, CultureInfo.InvariantCulture);
                    if (this.rules.ContainsKey(node2.Value))
                    {
                        throw new ArgumentException(Strings.RuleWithSameNameExists);
                    }
                    Rule rule = new Rule(node2.Value, this.id, node3.Value, str, warning, (node5 == null) ? string.Empty : TrimXmlContent(node5.InnerText), ruleGroup, !flag, canDisable);
                    this.rules.Add(node2.Value, rule);
                }
            }
        }

        public void AddViolation(CodeElement element, Enum ruleName, params object[] values)
        {
            Param.RequireNotNull(ruleName, "ruleName");
            this.AddViolation(element, ruleName.ToString(), values);
        }

        public void AddViolation(CodeElement element, string ruleName, params object[] values)
        {
            int line = 0;
            if (element != null)
            {
                line = element.LineNumber;
            }
            this.AddViolation(element, line, ruleName, values);
        }

        public void AddViolation(CodeElement element, int line, Enum ruleName, params object[] values)
        {
            Param.RequireNotNull(ruleName, "ruleName");
            this.AddViolation(element, line, ruleName.ToString(), values);
        }

        public void AddViolation(CodeElement element, int line, string ruleName, params object[] values)
        {
            Param.RequireNotNull(element, "element");
            Param.RequireValidString(ruleName, "ruleName");
            if (this.IsRuleEnabled(element.Document, ruleName))
            {
                Rule type = this.GetRule(ruleName);
                if (type == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.RuleDoesNotExist, new object[] { ruleName }), "ruleName");
                }
                if (!this.IsRuleSuppressed(element, type.CheckId, type.Name, type.Namespace))
                {
                    this.core.AddViolation(element, type, line, values);
                }
            }
        }

        public void ClearSetting(WritableSettings settings, string propertyName)
        {
            Param.RequireNotNull(settings, "settings");
            Param.RequireValidString(propertyName, "propertyName");
            settings.ClearAddInSetting(this, propertyName);
        }

        internal static string GetIdFromAddInType(Type addInType)
        {
            return addInType.FullName;
        }

        public Rule GetRule(string ruleName)
        {
            Rule rule;
            Param.RequireValidString(ruleName, "ruleName");
            if (this.rules.TryGetValue(ruleName, out rule))
            {
                return rule;
            }
            return null;
        }

        public PropertyValue GetRuleSetting(Settings settings, string ruleName, string propertyName)
        {
            Param.RequireValidString(ruleName, "ruleName");
            Param.RequireValidString(propertyName, "propertyName");
            if (settings == null)
            {
                return null;
            }
            return settings.GetAddInSetting(this, ruleName + "#" + propertyName);
        }

        public PropertyValue GetSetting(Settings settings, string propertyName)
        {
            Param.RequireValidString(propertyName, "propertyName");
            if (settings == null)
            {
                return null;
            }
            return settings.GetAddInSetting(this, propertyName);
        }

        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId="System.Xml.XmlNode", Justification="Compliance would break well-defined public API.")]
        protected virtual void ImportInitializationXml(XmlDocument document, bool topmostType, bool isKnownAssembly)
        {
            Param.RequireNotNull(document, "document");
            Param.RequireNotNull(document.DocumentElement, "xml.DocumentElement");
            if (topmostType)
            {
                XmlAttribute attribute = document.DocumentElement.Attributes["Name"];
                if ((attribute == null) || (attribute.Value.Length == 0))
                {
                    throw new ArgumentException(Strings.MissingNameAttributeOnRootNode);
                }
                this.name = attribute.Value;
                XmlElement element = document.DocumentElement["Description"];
                if (element == null)
                {
                    throw new ArgumentException(Strings.MissingAddInDescription);
                }
                string str = element.InnerText.Trim();
                if (str.Length == 0)
                {
                    throw new ArgumentException(Strings.MissingAddInDescription);
                }
                this.description = str;
            }
            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                if (node.Name == "Rules")
                {
                    this.AddRulesFromXml(node, null, isKnownAssembly);
                }
                else if (node.Name == "Properties")
                {
                    this.propertyDescriptors.InitializeFromXml(node);
                }
            }
        }

        internal void Initialize(StyleCopCore styleCopCore, XmlDocument initializationXml, bool topMostType, bool isKnownAssembly)
        {
            this.core = styleCopCore;
            this.ImportInitializationXml(initializationXml, topMostType, isKnownAssembly);
        }

        public virtual void InitializeAddIn()
        {
        }

        public virtual bool IsRuleEnabled(CodeDocument document, string ruleName)
        {
            return true;
        }

        public virtual bool IsRuleSuppressed(CodeElement element, string ruleCheckId, string ruleName, string ruleNamespace)
        {
            return false;
        }

        public void SetSetting(WritableSettings settings, PropertyValue property)
        {
            Param.RequireNotNull(settings, "settings");
            Param.RequireNotNull(property, "property");
            settings.SetAddInSetting(this, property);
        }

        public virtual void SolutionClosing()
        {
        }

        public virtual void SolutionOpened()
        {
        }

        private static string TrimXmlContent(string content)
        {
            if (content == null)
            {
                return null;
            }
            int length = 0;
            char[] chArray = new char[content.Length];
            bool flag = false;
            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                if (flag)
                {
                    if (char.IsWhiteSpace(c) && (i < (content.Length - 1)))
                    {
                        if (!char.IsWhiteSpace(content[i + 1]))
                        {
                            chArray[length++] = ' ';
                        }
                    }
                    else
                    {
                        chArray[length++] = c;
                    }
                }
                else if (!char.IsWhiteSpace(c))
                {
                    chArray[length++] = c;
                    flag = true;
                }
            }
            if (length == chArray.Length)
            {
                return content;
            }
            return new string(chArray, 0, length);
        }

        internal IEnumerable<Rule> AddInRules
        {
            get
            {
                return this.rules.Values;
            }
        }

        public StyleCopCore Core
        {
            get
            {
                return this.core;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public virtual PropertyDescriptorCollection PropertyDescriptors
        {
            get
            {
                return this.propertyDescriptors;
            }
        }

        public virtual ICollection<IPropertyControlPage> SettingsPages
        {
            get
            {
                return null;
            }
        }
    }
}

