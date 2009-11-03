namespace Microsoft.StyleCop
{
    using System;
    using System.Xml;

    internal static class V42Settings
    {
        public static void Load(XmlDocument document, Settings settings)
        {
            MoveRuleToNewAnalyzer(document, "Microsoft.SourceAnalysis.CSharp.ReadabilityRules", "Microsoft.SourceAnalysis.CSharp.MaintainabilityRules", "StatementMustNotUseUnnecessaryParenthesis");
            V41Settings.ChangeAnalyzerSettingName(document, "Microsoft.SourceAnalysis.CSharp.DocumentationRules", "PublicAndProtectedOnly", "IgnorePrivates");
            V43Settings.Load(document, settings);
        }

        public static void MoveRuleToNewAnalyzer(XmlDocument document, string legacyAnalyzerName, string newAnalyzerName, string ruleName)
        {
            XmlNode node = document.DocumentElement.SelectSingleNode("Analyzers");
            if (node != null)
            {
                XmlNode node2 = node.SelectSingleNode("Analyzer[@AnalyzerId=\"" + legacyAnalyzerName + "\"]");
                if (node2 != null)
                {
                    XmlNode node3 = node2.SelectSingleNode("Rules");
                    if (node3 != null)
                    {
                        XmlNode oldChild = node3.SelectSingleNode("Rule[@Name=\"" + ruleName + "\"]");
                        if (oldChild != null)
                        {
                            XmlNode newChild = node.SelectSingleNode("Analyzer[@AnalyzerId=\"" + newAnalyzerName + "\"]");
                            if (newChild == null)
                            {
                                newChild = document.CreateElement("Analyzer");
                                XmlAttribute attribute = document.CreateAttribute("AnalyzerId");
                                attribute.Value = newAnalyzerName;
                                newChild.Attributes.Append(attribute);
                                node.AppendChild(newChild);
                            }
                            XmlNode node6 = newChild.SelectSingleNode("Rules");
                            if (node6 == null)
                            {
                                node6 = document.CreateElement("Rules");
                                newChild.AppendChild(node6);
                            }
                            node3.RemoveChild(oldChild);
                            node6.AppendChild(oldChild);
                        }
                    }
                }
            }
        }
    }
}

