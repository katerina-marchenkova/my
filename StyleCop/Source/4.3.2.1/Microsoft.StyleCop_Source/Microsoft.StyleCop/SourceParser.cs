namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Xml;

    public abstract class SourceParser : StyleCopAddIn
    {
        private List<SourceAnalyzer> analyzers = new List<SourceAnalyzer>();
        private List<string> fileTypes = new List<string>(1);

        protected SourceParser()
        {
        }

        public void AddGlobalViolation(int line, Enum ruleName, params object[] values)
        {
            Param.RequireNotNull(ruleName, "ruleName");
            this.AddGlobalViolation(line, ruleName.ToString(), values);
        }

        public void AddGlobalViolation(int line, string ruleName, params object[] values)
        {
            Rule type = base.GetRule(ruleName);
            if (type == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.RuleDoesNotExist, new object[] { ruleName }), "ruleName");
            }
            base.Core.AddViolation((CodeElement) null, type, line, values);
        }

        public void AddViolation(Violation violation)
        {
            Param.RequireNotNull(violation, "violation");
            if (violation.Element != null)
            {
                base.Core.AddViolation(violation.Element, violation);
            }
            else if (violation.SourceCode != null)
            {
                base.Core.AddViolation(violation.SourceCode, violation);
            }
            else
            {
                base.Core.AddViolation((CodeElement) null, violation);
            }
        }

        public void AddViolation(SourceCode sourceCode, int line, Enum ruleName, params object[] values)
        {
            Param.RequireNotNull(ruleName, "ruleName");
            this.AddViolation(sourceCode, line, ruleName.ToString(), values);
        }

        public void AddViolation(SourceCode sourceCode, int line, string ruleName, params object[] values)
        {
            Rule type = base.GetRule(ruleName);
            if (type == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.RuleDoesNotExist, new object[] { ruleName }), "ruleName");
            }
            base.Core.AddViolation(sourceCode, type, line, values);
        }

        internal static void ClearAnalyzerTags(CodeDocument document)
        {
            if ((document != null) && (document.DocumentContents != null))
            {
                document.DocumentContents.ClearAnalyzerTags();
            }
        }

        private static void ExportElementViolations(CodeElement element, XmlDocument violationsDocument, XmlNode parentNode)
        {
            foreach (Violation violation in element.Violations)
            {
                ExportViolation(violation, violationsDocument, parentNode);
            }
            IEnumerable<CodeElement> childCodeElements = element.ChildCodeElements;
            if (childCodeElements != null)
            {
                foreach (CodeElement element2 in childCodeElements)
                {
                    ExportElementViolations(element2, violationsDocument, parentNode);
                }
            }
        }

        private static void ExportViolation(Violation violation, XmlDocument violationsDocument, XmlNode parentNode)
        {
            XmlElement newChild = violationsDocument.CreateElement("violation");
            parentNode.AppendChild(newChild);
            XmlAttribute node = violationsDocument.CreateAttribute("namespace");
            node.Value = violation.Rule.Namespace;
            newChild.Attributes.Append(node);
            XmlAttribute attribute2 = violationsDocument.CreateAttribute("rule");
            attribute2.Value = violation.Rule.Name;
            newChild.Attributes.Append(attribute2);
            XmlAttribute attribute3 = violationsDocument.CreateAttribute("ruleCheckId");
            attribute3.Value = violation.Rule.CheckId;
            newChild.Attributes.Append(attribute3);
            XmlElement element2 = violationsDocument.CreateElement("context");
            element2.InnerText = violation.Message;
            newChild.AppendChild(element2);
            XmlElement element3 = violationsDocument.CreateElement("line");
            element3.InnerText = violation.Line.ToString(CultureInfo.InvariantCulture);
            newChild.AppendChild(element3);
            XmlElement element4 = violationsDocument.CreateElement("warning");
            element4.InnerText = violation.Rule.Warning.ToString(CultureInfo.InvariantCulture);
            newChild.AppendChild(element4);
        }

        internal static void ExportViolations(CodeDocument document, XmlDocument violationsDocument, XmlNode parentNode)
        {
            if (document.DocumentContents != null)
            {
                ExportElementViolations(document.DocumentContents, violationsDocument, parentNode);
            }
            if (document.SourceCode != null)
            {
                foreach (Violation violation in document.SourceCode.Violations)
                {
                    ExportViolation(violation, violationsDocument, parentNode);
                }
            }
        }

        protected override void ImportInitializationXml(XmlDocument document, bool topmostType, bool isKnownAssembly)
        {
            Param.RequireNotNull(document, "document");
            Param.RequireNotNull(document.DocumentElement, "document.DocumentElement");
            base.ImportInitializationXml(document, topmostType, isKnownAssembly);
            if (topmostType && (document.DocumentElement.Name != "SourceParser"))
            {
                throw new ArgumentException(Strings.RootNodeMustBeSourceParser);
            }
            List<string> collection = null;
            XmlNodeList list2 = document.DocumentElement.SelectNodes("FileTypes/FileType");
            if (list2 != null)
            {
                foreach (XmlNode node in list2)
                {
                    if (collection == null)
                    {
                        collection = new List<string>();
                    }
                    collection.Add(node.InnerText.Trim(new char[] { ' ', '\t', '.' }).ToUpperInvariant());
                }
            }
            if ((collection != null) && (collection.Count > 0))
            {
                this.fileTypes.AddRange(collection);
            }
        }

        internal bool ImportViolations(SourceCode sourceCode, XmlNode parentNode)
        {
            bool flag = true;
            try
            {
                XmlNodeList list = parentNode.SelectNodes("violation");
                if ((list != null) && (list.Count > 0))
                {
                    foreach (XmlNode node in list)
                    {
                        XmlNode node2 = node.SelectSingleNode("@namespace");
                        XmlNode node3 = node.SelectSingleNode("@rule");
                        XmlNode node4 = node.SelectSingleNode("@ruleCheckId");
                        XmlNode node5 = node.SelectSingleNode("context");
                        XmlNode node6 = node.SelectSingleNode("line");
                        XmlNode node7 = node.SelectSingleNode("warning");
                        Rule rule = new Rule(node3.InnerText, node2.InnerText, node4.InnerText, node5.InnerText, Convert.ToBoolean(node7.InnerText, CultureInfo.InvariantCulture));
                        Violation violation = new Violation(rule, sourceCode, Convert.ToInt32(node6.InnerText, (IFormatProvider) null), node5.InnerText);
                        this.AddViolation(violation);
                    }
                }
                return flag;
            }
            catch (ArgumentException)
            {
                flag = false;
            }
            catch (XmlException)
            {
                flag = false;
            }
            catch (FormatException)
            {
                flag = false;
            }
            catch (OverflowException)
            {
                flag = false;
            }
            return flag;
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="output", Justification="The method is not yet implemented."), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="level", Justification="The method is not yet implemented."), SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="The method is not yet implemented.")]
        protected void Log(StyleCopLogLevel level, string output)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId="2#", Justification="The design of the method is consistent with other .Net Framework methods such as int.TryParse, etc."), SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", Justification="The method is abstract")]
        public abstract bool ParseFile(SourceCode sourceCode, int passNumber, ref CodeDocument document);
        public virtual void PostParse()
        {
        }

        public virtual void PreParse()
        {
        }

        public virtual bool SkipAnalysisForDocument(CodeDocument document)
        {
            return false;
        }

        public ICollection<SourceAnalyzer> Analyzers
        {
            get
            {
                return this.analyzers;
            }
        }

        public ICollection<string> FileTypes
        {
            get
            {
                return this.fileTypes;
            }
        }
    }
}

