namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;

    public abstract class SourceAnalyzer : StyleCopAddIn
    {
        private Dictionary<CodeProject, Dictionary<string, Rule>> enabledRules;
        private SourceParser parser;
        private string parserId;

        protected SourceAnalyzer()
        {
            object[] customAttributes = base.GetType().GetCustomAttributes(typeof(SourceAnalyzerAttribute), true);
            if ((customAttributes == null) || (customAttributes.Length == 0))
            {
                throw new ArgumentException(Strings.SourceAnalyzerAttributeMissing);
            }
            SourceAnalyzerAttribute attribute = (SourceAnalyzerAttribute) customAttributes[0];
            if (attribute.ParserType == null)
            {
                throw new ArgumentException(Strings.SourceAnalyzerAttributeMissing);
            }
            this.parserId = StyleCopAddIn.GetIdFromAddInType(attribute.ParserType);
        }

        public virtual void AnalyzeDocument(CodeDocument document)
        {
        }

        public virtual bool DelayAnalysis(CodeDocument document, int passNumber)
        {
            return false;
        }

        protected object GetDocumentData(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");
            object obj2 = null;
            document.AnalyzerData.TryGetValue(base.Id, out obj2);
            return obj2;
        }

        protected override void ImportInitializationXml(XmlDocument document, bool topmostType, bool isKnownAssembly)
        {
            Param.RequireNotNull(document, "document");
            Param.RequireNotNull(document.DocumentElement, "xml.DocumentElement");
            base.ImportInitializationXml(document, topmostType, isKnownAssembly);
            if (topmostType)
            {
                if (document.DocumentElement.Name != "SourceAnalyzer")
                {
                    throw new ArgumentException(Strings.SourceAnalyzerRootNodeIsIncorrect);
                }
                XmlAttribute attribute = document.DocumentElement.Attributes["DisabledByDefault"];
                if ((attribute != null) && (attribute.Value == "true"))
                {
                    throw new ArgumentException(Strings.DisabledByDefaultAttributeDeprecatedForAddIns);
                }
            }
        }

        public override bool IsRuleEnabled(CodeDocument document, string ruleName)
        {
            Param.RequireNotNull(document, "document");
            Param.RequireValidString(ruleName, "ruleName");
            if (this.enabledRules != null)
            {
                Dictionary<string, Rule> dictionary = null;
                if (this.enabledRules.TryGetValue(document.SourceCode.Project, out dictionary))
                {
                    return dictionary.ContainsKey(ruleName);
                }
            }
            return base.IsRuleEnabled(document, ruleName);
        }

        public override bool IsRuleSuppressed(CodeElement element, string ruleCheckId, string ruleName, string ruleNamespace)
        {
            return this.parser.IsRuleSuppressed(element, ruleCheckId, ruleName, ruleNamespace);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="The method is not yet implemented."), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="output", Justification="The method is not yet implemented."), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="level", Justification="The method is not yet implemented.")]
        protected void Log(StyleCopLogLevel level, string output)
        {
        }

        public virtual void PostAnalyze()
        {
        }

        public virtual void PreAnalyze()
        {
        }

        protected void SetDocumentData(CodeDocument document, object data)
        {
            Param.RequireNotNull(document, "document");
            if (document.AnalyzerData.ContainsKey(base.Id))
            {
                document.AnalyzerData[base.Id] = data;
            }
            else
            {
                document.AnalyzerData.Add(base.Id, data);
            }
        }

        internal void SetParser(SourceParser item)
        {
            this.parser = item;
        }

        public bool Cancel
        {
            get
            {
                return base.Core.Cancel;
            }
        }

        internal Dictionary<CodeProject, Dictionary<string, Rule>> EnabledRules
        {
            get
            {
                return this.enabledRules;
            }
            set
            {
                this.enabledRules = value;
            }
        }

        public SourceParser Parser
        {
            get
            {
                return this.parser;
            }
        }

        public string ParserId
        {
            get
            {
                return this.parserId;
            }
        }
    }
}

