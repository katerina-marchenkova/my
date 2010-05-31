namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml;

    internal static class V41Settings
    {
        private static void AddBooleanProperty(string propertyName, bool value, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors)
        {
            PropertyDescriptor<bool> propertyDescriptor = propertyDescriptors[propertyName] as PropertyDescriptor<bool>;
            if (propertyDescriptor != null)
            {
                properties.Add(new BooleanProperty(propertyDescriptor, value));
            }
        }

        private static void AddOrUpdateLegacyBooleanProperty(string ruleName, bool value, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors)
        {
            string propertyName = ruleName + "#Enabled";
            BooleanProperty property = properties[propertyName] as BooleanProperty;
            if (property == null)
            {
                AddBooleanProperty(propertyName, value, properties, propertyDescriptors);
            }
            else if (!value)
            {
                property.Value = false;
            }
        }

        public static void ChangeAnalyzerSettingName(XmlDocument document, string analyzerName, string legacyPropertyName, string newPropertyName)
        {
            XmlNode node = document.DocumentElement.SelectSingleNode("Analyzers");
            if (node != null)
            {
                XmlNode node2 = node.SelectSingleNode("Analyzer[@AnalyzerId=\"" + analyzerName + "\"]");
                if (node2 != null)
                {
                    XmlNode node3 = node2.SelectSingleNode("AnalyzerSettings");
                    if (node3 != null)
                    {
                        XmlNode node4 = node3.SelectSingleNode("*[@Name=\"" + legacyPropertyName + "\"]");
                        if (node4 != null)
                        {
                            XmlAttribute attribute = node4.Attributes["Name"];
                            if (attribute != null)
                            {
                                attribute.Value = newPropertyName;
                            }
                        }
                    }
                }
            }
        }

        public static void Load(XmlDocument document, Settings settings)
        {
            ChangeAnalyzerSettingName(document, "Microsoft.StyleCop.CSharp.Documentation", "PublicAndProtectedOnly", "IgnorePrivates");
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
                        string analyzerId = MapAnalyzerId(attribute.Value);
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
                                LoadPropertyCollection(propertyCollectionNode, addInSettings, addIn.PropertyDescriptors, attribute.Value);
                            }
                        }
                    }
                }
            }
        }

        private static void LoadBooleanProperty(string propertyName, XmlNode propertyNode, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors, string legacyAnalyzerId)
        {
            bool flag;
            if (bool.TryParse(propertyNode.InnerText, out flag))
            {
                if (string.IsNullOrEmpty(legacyAnalyzerId))
                {
                    AddBooleanProperty(propertyName, flag, properties, propertyDescriptors);
                }
                else if (propertyName == "Enabled")
                {
                    ICollection<string> is2 = MapAnalyzerToRules(legacyAnalyzerId);
                    if (is2 != null)
                    {
                        foreach (string str in is2)
                        {
                            AddBooleanProperty(str + "#Enabled", flag, properties, propertyDescriptors);
                        }
                    }
                }
                else if (legacyAnalyzerId == "Microsoft.SourceAnalysis.CSharp.Documentation")
                {
                    if (propertyName == "PublicAndProtectedOnly")
                    {
                        AddBooleanProperty("IgnorePrivates", flag, properties, propertyDescriptors);
                        AddBooleanProperty("IgnoreInternals", flag, properties, propertyDescriptors);
                    }
                    else if (propertyName == "RequireValueTags")
                    {
                        AddOrUpdateLegacyBooleanProperty("PropertyDocumentationMustHaveValue", flag, properties, propertyDescriptors);
                        AddOrUpdateLegacyBooleanProperty("PropertyDocumentationMustHaveValueText", flag, properties, propertyDescriptors);
                    }
                    else if (propertyName == "RequireCapitalLetter")
                    {
                        AddOrUpdateLegacyBooleanProperty("DocumentationTextMustBeginWithACapitalLetter", flag, properties, propertyDescriptors);
                    }
                    else if (propertyName == "RequirePeriod")
                    {
                        AddOrUpdateLegacyBooleanProperty("DocumentationTextMustEndWithAPeriod", flag, properties, propertyDescriptors);
                    }
                    else if (propertyName != "RequireProperFormatting")
                    {
                        AddBooleanProperty(propertyName, flag, properties, propertyDescriptors);
                    }
                    else
                    {
                        AddOrUpdateLegacyBooleanProperty("DocumentationTextMustContainWhitespace", flag, properties, propertyDescriptors);
                        AddOrUpdateLegacyBooleanProperty("DocumentationMustMeetCharacterPercentage", flag, properties, propertyDescriptors);
                        AddOrUpdateLegacyBooleanProperty("DocumentationTextMustMeetMinimumCharacterLength", flag, properties, propertyDescriptors);
                        if (!flag)
                        {
                            AddOrUpdateLegacyBooleanProperty("DocumentationTextMustEndWithAPeriod", flag, properties, propertyDescriptors);
                            AddOrUpdateLegacyBooleanProperty("DocumentationTextMustBeginWithACapitalLetter", flag, properties, propertyDescriptors);
                        }
                    }
                }
                else if (legacyAnalyzerId == "Microsoft.SourceAnalysis.CSharp.FileHeaders")
                {
                    if (propertyName == "RequireSummary")
                    {
                        AddOrUpdateLegacyBooleanProperty("FileHeaderMustHaveSummary", flag, properties, propertyDescriptors);
                    }
                    else
                    {
                        AddBooleanProperty(propertyName, flag, properties, propertyDescriptors);
                    }
                }
                else
                {
                    AddBooleanProperty(propertyName, flag, properties, propertyDescriptors);
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
                CollectionProperty property = new CollectionProperty(propertyDescriptor, innerCollection);
                properties.Add(property);
            }
        }

        private static void LoadIntProperty(string propertyName, XmlNode propertyNode, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors)
        {
            int num;
            if (int.TryParse(propertyNode.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
            {
                PropertyDescriptor<int> propertyDescriptor = propertyDescriptors[propertyName] as PropertyDescriptor<int>;
                properties.Add(new IntProperty(propertyDescriptor, num));
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
                        string parserId = attribute.Value;
                        if (parserId.Equals("Microsoft.SourceAnalysis.CSharp.CsParser", StringComparison.Ordinal))
                        {
                            parserId = "Microsoft.StyleCop.CSharp.CsParser";
                        }
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

        private static void LoadPropertyCollection(XmlNode propertyCollectionNode, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors, string legacyAnalyzerId)
        {
            foreach (XmlNode node in propertyCollectionNode.ChildNodes)
            {
                string str;
                XmlAttribute attribute = node.Attributes["Name"];
                if (((attribute != null) && !string.IsNullOrEmpty(attribute.Value)) && ((str = node.Name) != null))
                {
                    if (!(str == "BooleanProperty"))
                    {
                        if (str == "IntegerProperty")
                        {
                            goto Label_0092;
                        }
                        if (str == "StringProperty")
                        {
                            goto Label_00A2;
                        }
                        if (str == "CollectionProperty")
                        {
                            goto Label_00B2;
                        }
                    }
                    else
                    {
                        LoadBooleanProperty(attribute.InnerText, node, properties, propertyDescriptors, legacyAnalyzerId);
                    }
                }
                continue;
            Label_0092:
                LoadIntProperty(attribute.InnerText, node, properties, propertyDescriptors);
                continue;
            Label_00A2:
                LoadStringProperty(attribute.InnerText, node, properties, propertyDescriptors);
                continue;
            Label_00B2:
                LoadCollectionProperty(attribute.InnerText, node, properties, propertyDescriptors);
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
                            string legacyAnalyzerId = attribute.Value;
                            XmlNode propertyCollectionNode = node2["RuleSettings"];
                            if (propertyCollectionNode != null)
                            {
                                LoadPropertyCollection(propertyCollectionNode, properties, propertyDescriptors, legacyAnalyzerId);
                            }
                        }
                    }
                }
            }
        }

        private static void LoadStringProperty(string propertyName, XmlNode propertyNode, PropertyCollection properties, PropertyDescriptorCollection propertyDescriptors)
        {
            PropertyDescriptor<string> propertyDescriptor = propertyDescriptors[propertyName] as PropertyDescriptor<string>;
            properties.Add(new StringProperty(propertyDescriptor, propertyNode.InnerText));
        }

        private static string MapAnalyzerId(string analyzerId)
        {
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.AccessModifiers", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.MaintainabilityRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.ClassMembers", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.ReadabilityRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Comments", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.ReadabilityRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.CurlyBrackets", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.LayoutRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.DeclarationKeywordOrder", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.OrderingRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Documentation", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.DocumentationRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.ElementOrder", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.OrderingRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.FileHeaders", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.DocumentationRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.LineSpacing", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.LayoutRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.MethodParameters", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.ReadabilityRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Naming", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.NamingRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Parenthesis", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.MaintainabilityRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Spacing", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.SpacingRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Statements", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.ReadabilityRules";
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Tabs", StringComparison.OrdinalIgnoreCase))
            {
                return "Microsoft.StyleCop.CSharp.SpacingRules";
            }
            return analyzerId;
        }

        private static ICollection<string> MapAnalyzerToRules(string analyzerId)
        {
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.AccessModifiers", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "AccessModifierMustBeDeclared", "FieldsMustBePrivate" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.ClassMembers", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "DoNotPrefixCallsWithBaseUnlessLocalImplementationExists", "PrefixLocalCallsWithThis" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Comments", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "CommentsMustContainText" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.CurlyBrackets", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "CurlyBracketsForMultiLineStatementsMustNotShareLine", "StatementMustNotBeOnSingleLine", "ElementMustNotBeOnSingleLine", "CurlyBracketsMustNotBeOmitted", "AllAccessorsMustBeMultiLineOrSingleLine" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.DeclarationKeywordOrder", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "DeclarationKeywordsMustFollowOrder", "ProtectedMustComeBeforeInternal" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Documentation", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { 
                    "ElementsMustBeDocumented", "PartialElementsMustBeDocumented", "EnumerationItemsMustBeDocumented", "DocumentationMustContainValidXml", "ElementDocumentationMustHaveSummary", "PartialElementDocumentationMustHaveSummary", "ElementDocumentationMustHaveSummaryText", "PartialElementDocumentationMustHaveSummaryText", "ElementDocumentationMustNotHaveDefaultSummary", "PropertyDocumentationMustHaveValue", "PropertyDocumentationMustHaveValueText", "ElementParametersMustBeDocumented", "ElementParameterDocumentationMustMatchElementParameters", "ElementParameterDocumentationMustDeclareParameterName", "ElementParameterDocumentationMustHaveText", "ElementReturnValueMustBeDocumented", 
                    "ElementReturnValueDocumentationMustHaveText", "VoidReturnValueMustNotBeDocumented", "GenericTypeParametersMustBeDocumented", "GenericTypeParametersMustBeDocumentedPartialClass", "GenericTypeParameterDocumentationMustMatchTypeParameters", "GenericTypeParameterDocumentationMustDeclareParameterName", "GenericTypeParameterDocumentationMustHaveText", "PropertySummaryDocumentationMustMatchAccessors", "PropertySummaryDocumentationMustOmitSetAccessorWithRestrictedAccess", "ElementDocumentationMustNotBeCopiedAndPasted", "SingleLineCommentsMustNotUseDocumentationStyleSlashes", "DocumentationTextMustNotBeEmpty", "DocumentationTextMustBeginWithACapitalLetter", "DocumentationTextMustEndWithAPeriod", "DocumentationTextMustContainWhitespace", "DocumentationMustMeetCharacterPercentage", 
                    "DocumentationTextMustMeetMinimumCharacterLength"
                 };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.ElementOrder", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "UsingDirectivesMustBePlacedWithinNamespace", "ElementsMustAppearInTheCorrectOrder", "ElementsMustBeOrderedByAccess", "ConstantsMustAppearBeforeFields", "StaticElementsMustAppearBeforeInstanceElements", "PartialElementsMustDeclareAccess" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.FileHeaders", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "FileMustHaveHeader", "FileHeaderMustShowCopyright", "FileHeaderMustHaveCopyrightText", "FileHeaderMustContainFileName", "FileHeaderFileNameDocumentationMustMatchFileName", "FileHeaderMustHaveSummary", "FileHeaderMustHaveValidCompanyText" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.LineSpacing", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "OpeningCurlyBracketsMustNotBeFollowedByBlankLine", "ElementDocumentationHeadersMustNotBeFollowedByBlankLine", "CodeMustNotContainMultipleBlankLinesInARow", "ClosingCurlyBracketsMustNotBePrecededByBlankLine", "OpeningCurlyBracketsMustNotBePrecededByBlankLine", "ChainedStatementBlocksMustNotBePrecededByBlankLine", "WhileDoFooterMustNotBePrecededByBlankLine", "SingleLineCommentsMustNotBeFollowedByBlankLine", "ClosingCurlyBracketMustBeFollowedByBlankLine", "ElementDocumentationHeaderMustBePrecededByBlankLine", "SingleLineCommentMustBePrecededByBlankLine" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.MethodParameters", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "OpeningParenthesisMustBeOnDeclarationLine", "ClosingParenthesisMustBeOnLineOfLastParameter", "ClosingParenthesisMustBeOnLineOfOpeningParenthesis", "CommaMustBeOnSameLineAsPreviousParameter", "ParameterListMustFollowDeclaration", "ParameterMustFollowComma", "SplitParametersMustStartOnLineAfterDeclaration", "ParametersMustBeOnSameLineOrSeparateLines", "ParameterMustNotSpanMultipleLines" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Naming", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "ElementMustBeginWithUpperCaseLetter", "ElementMustBeginWithLowerCaseLetter", "InterfaceNamesMustBeginWithI", "ConstFieldNamesMustBeginWithUpperCaseLetter", "NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter", "FieldNamesMustNotUseHungarianNotation", "FieldNamesMustBeginWithLowerCaseLetter", "AccessibleFieldsMustBeginWithUpperCaseLetter", "VariableNamesMustNotBePrefixed", "FieldNamesMustNotBeginWithUnderscore", "FieldNamesMustNotContainUnderscore" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Parenthesis", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "StatementMustNotUseUnnecessaryParenthesis" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Spacing", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { 
                    "KeywordsMustBeSpacedCorrectly", "CommasMustBeSpacedCorrectly", "SemicolonsMustBeSpacedCorrectly", "SymbolsMustBeSpacedCorrectly", "DocumentationLinesMustBeginWithSingleSpace", "SingleLineCommentsMustBeginWithSingleSpace", "PreprocessorKeywordsMustNotBePrecededBySpace", "OperatorKeywordMustBeFollowedBySpace", "OpeningParenthesisMustBeSpacedCorrectly", "ClosingParenthesisMustBeSpacedCorrectly", "OpeningSquareBracketsMustBeSpacedCorrectly", "ClosingSquareBracketsMustBeSpacedCorrectly", "OpeningCurlyBracketsMustBeSpacedCorrectly", "ClosingCurlyBracketsMustBeSpacedCorrectly", "OpeningGenericBracketsMustBeSpacedCorrectly", "ClosingGenericBracketsMustBeSpacedCorrectly", 
                    "OpeningAttributeBracketsMustBeSpacedCorrectly", "ClosingAttributeBracketsMustBeSpacedCorrectly", "NullableTypeSymbolsMustNotBePrecededBySpace", "MemberAccessSymbolsMustBeSpacedCorrectly", "IncrementDecrementSymbolsMustBeSpacedCorrectly", "NegativeSignsMustBeSpacedCorrectly", "PositiveSignsMustBeSpacedCorrectly", "DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly", "ColonsMustBeSpacedCorrectly", "CodeMustNotContainMultipleWhitespaceInARow", "CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation", "TabsMustNotBeUsed"
                 };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Statements", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "CodeMustNotContainEmptyStatements", "CodeMustNotContainMultipleStatementsOnOneLine", "BlockStatementsMustNotContainEmbeddedComments", "BlockStatementsMustNotContainEmbeddedRegions" };
            }
            if (string.Equals(analyzerId, "Microsoft.SourceAnalysis.CSharp.Tabs", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { "TabsMustNotBeUsed" };
            }
            return new string[0];
        }
    }
}

