namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    public static class V40Settings
    {
        public const string DefaultFileName = "StyleCop.Settings";

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

        private static void EnableDisableAnalyzerRules(XmlNode disabledAnalyzersNode, Settings settings, bool enabled)
        {
            foreach (string str in disabledAnalyzersNode.InnerText.Split(new char[] { ',' }))
            {
                string analyzerId = MapAnalyzerId(str);
                if (analyzerId != null)
                {
                    SourceAnalyzer addIn = settings.Core.GetAnalyzer(analyzerId);
                    if (addIn != null)
                    {
                        ICollection<string> is2 = MapAnalyzerToRules(str);
                        if (is2 != null)
                        {
                            AddInPropertyCollection addInSettings = settings.GetAddInSettings(addIn);
                            if (addInSettings == null)
                            {
                                addInSettings = new AddInPropertyCollection(addIn);
                                settings.SetAddInSettings(addInSettings);
                            }
                            foreach (string str3 in is2)
                            {
                                AddBooleanProperty(str3 + "#Enabled", enabled, addInSettings, addIn.PropertyDescriptors);
                            }
                        }
                    }
                }
            }
        }

        internal static void Load(XmlDocument document, Settings settings)
        {
            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                switch (node.Name)
                {
                    case "StyleCopDisabledAnalyzers":
                    {
                        EnableDisableAnalyzerRules(node, settings, false);
                        continue;
                    }
                    case "StyleCopExplicitlyEnabledAnalyzers":
                    {
                        EnableDisableAnalyzerRules(node, settings, true);
                        continue;
                    }
                    case "AnalyzeDesignerFiles":
                    {
                        LoadAnalyzeDesignerFilesSetting(settings, node.InnerText);
                        continue;
                    }
                    case "PublicAndProtectedOnly":
                    {
                        LoadAnalyzerSetting(settings, "Microsoft.StyleCop.CSharp.DocumentationRules", "IgnorePrivates", node.InnerText);
                        LoadAnalyzerSetting(settings, "Microsoft.StyleCop.CSharp.DocumentationRules", "IgnoreInternals", node.InnerText);
                        continue;
                    }
                    case "IncludeFields":
                    {
                        LoadAnalyzerSetting(settings, "Microsoft.StyleCop.CSharp.DocumentationRules", "IncludeFields", node.InnerText);
                        continue;
                    }
                    case "GeneratedCodeElementOrder":
                    {
                        LoadAnalyzerSetting(settings, "Microsoft.StyleCop.CSharp.OrderingRules", "GeneratedCodeElementOrder", node.InnerText);
                        continue;
                    }
                    case "RequireValueTags":
                    {
                        LoadLegacyAnalyzerSetting(settings, "Microsoft.StyleCop.CSharp.DocumentationRules", "RequireValueTags", node.InnerText);
                        continue;
                    }
                    case "GlobalSettingsFilePath":
                    {
                        PropertyDescriptor<string> propertyDescriptor = settings.Core.PropertyDescriptors["MergeSettingsFiles"] as PropertyDescriptor<string>;
                        if (propertyDescriptor != null)
                        {
                            settings.GlobalSettings.Add(new StringProperty(propertyDescriptor, "Linked"));
                        }
                        propertyDescriptor = settings.Core.PropertyDescriptors["LinkedSettingsFile"] as PropertyDescriptor<string>;
                        if (propertyDescriptor != null)
                        {
                            settings.GlobalSettings.Add(new StringProperty(propertyDescriptor, node.InnerText));
                        }
                        continue;
                    }
                    case "StyleCopHungarian":
                    {
                        LoadValidPrefixes(node, settings);
                        continue;
                    }
                }
            }
        }

        private static void LoadAnalyzeDesignerFilesSetting(Settings settings, string nodeText)
        {
            SourceParser addIn = settings.Core.GetParser("Microsoft.StyleCop.CSharp.CsParser");
            if (addIn != null)
            {
                PropertyDescriptor<bool> propertyDescriptor = addIn.PropertyDescriptors["AnalyzeDesignerFiles"] as PropertyDescriptor<bool>;
                if (propertyDescriptor != null)
                {
                    settings.SetAddInSettingInternal(addIn, new BooleanProperty(propertyDescriptor, nodeText != "0"));
                }
            }
        }

        private static void LoadAnalyzerSetting(Settings settings, string analyzerId, string propertyName, string nodeText)
        {
            SourceAnalyzer addIn = settings.Core.GetAnalyzer(analyzerId);
            if (addIn != null)
            {
                PropertyDescriptor<bool> propertyDescriptor = addIn.PropertyDescriptors[propertyName] as PropertyDescriptor<bool>;
                if (propertyDescriptor != null)
                {
                    settings.SetAddInSettingInternal(addIn, new BooleanProperty(propertyDescriptor, nodeText != "0"));
                }
            }
        }

        private static void LoadLegacyAnalyzerSetting(Settings settings, string analyzerId, string propertyName, string nodeText)
        {
            string str;
            if ((((str = analyzerId) != null) && (str == "Microsoft.StyleCop.CSharp.DocumentationRules")) && (propertyName == "RequireValueTags"))
            {
                SourceAnalyzer addIn = settings.Core.GetAnalyzer(analyzerId);
                if (addIn != null)
                {
                    AddInPropertyCollection addInSettings = settings.GetAddInSettings(addIn);
                    if (addInSettings == null)
                    {
                        addInSettings = new AddInPropertyCollection(addIn);
                        settings.SetAddInSettings(addInSettings);
                    }
                    bool flag = nodeText != "0";
                    AddOrUpdateLegacyBooleanProperty("PropertyDocumentationMustHaveValue", flag, addInSettings, addIn.PropertyDescriptors);
                    AddOrUpdateLegacyBooleanProperty("PropertyDocumentationMustHaveValueText", flag, addInSettings, addIn.PropertyDescriptors);
                }
            }
        }

        private static void LoadValidPrefixes(XmlNode validPrefixesNode, Settings settings)
        {
            string[] innerCollection = validPrefixesNode.InnerText.Split(new char[] { ',' });
            SourceAnalyzer addIn = settings.Core.GetAnalyzer("Microsoft.StyleCop.CSharp.NamingRules");
            if (addIn != null)
            {
                CollectionPropertyDescriptor propertyDescriptor = addIn.PropertyDescriptors["Hungarian"] as CollectionPropertyDescriptor;
                if (propertyDescriptor != null)
                {
                    settings.SetAddInSettingInternal(addIn, new CollectionProperty(propertyDescriptor, innerCollection));
                }
            }
        }

        private static string MapAnalyzerId(string analyzerId)
        {
            switch (analyzerId.ToUpperInvariant())
            {
                case "E508A3D4-B487-4D5F-8386-5827FA1334CD":
                    return "Microsoft.StyleCop.CSharp.NamingRules";

                case "C0F8B61A-DC6C-4550-8652-1074C95520B6":
                    return "Microsoft.StyleCop.CSharp.NamingRules";

                case "F3EA01DF-3F2F-42AF-865D-84768B8CF4B0":
                    return "Microsoft.StyleCop.CSharp.NamingRules";

                case "25474DA2-9B71-48A9-BA76-A4726EC8C48E":
                    return "Microsoft.StyleCop.CSharp.NamingRules";

                case "C17BFE16-544B-11DA-8BDE-F66BAD1E3F3A":
                    return "Microsoft.StyleCop.CSharp.ReadabilityRules";

                case "8DE5A506-0BD9-478B-95AF-2B3EC20C2093":
                    return "Microsoft.StyleCop.CSharp.ReadabilityRules";

                case "3B74A427-6D8D-4808-9FCD-520F15E8517C":
                    return "Microsoft.StyleCop.CSharp.ReadabilityRules";

                case "2771D7CA-F585-4832-B2C2-88DA370EBAC1":
                    return "Microsoft.StyleCop.CSharp.ReadabilityRules";

                case "64C9DA50-FC1A-4F20-986E-72AAA00B8ED4":
                    return "Microsoft.StyleCop.CSharp.SpacingRules";

                case "19F875BA-368A-40D8-B70A-5D26DF5BEBDD":
                    return "Microsoft.StyleCop.CSharp.SpacingRules";

                case "9ADAF7F0-E57D-4DF7-9885-ABAB2A9149FC":
                    return "Microsoft.StyleCop.CSharp.MaintainabilityRules";

                case "C0FC9515-97A4-4D61-89CD-9D87FEDD5B24":
                    return "Microsoft.StyleCop.CSharp.MaintainabilityRules";

                case "F6912B0F-C5FC-453D-BA02-183C3C9A2A8B":
                    return "Microsoft.StyleCop.CSharp.LayoutRules";

                case "5937033A-122C-492E-9C08-6F1AE80D1710":
                    return "Microsoft.StyleCop.CSharp.LayoutRules";

                case "305C458B-4CEC-4E49-96A9-E8012B333C7B":
                    return "Microsoft.StyleCop.CSharp.LayoutRules";

                case "31B0AB2A-8EED-4815-9F2D-C5A439EA9809":
                    return "Microsoft.StyleCop.CSharp.DocumentationRules";

                case "A2B149D9-1F5E-4D79-8E8B-2273E956B9DD":
                    return "Microsoft.StyleCop.CSharp.DocumentationRules";

                case "E2283402-D0B2-468D-81BC-DE32B48B7A4C":
                    return "Microsoft.StyleCop.CSharp.OrderingRules";

                case "6D18499A-AA21-4BE5-9590-7C8BEC56C1F8":
                    return "Microsoft.StyleCop.CSharp.OrderingRules";
            }
            return null;
        }

        private static ICollection<string> MapAnalyzerToRules(string analyzerId)
        {
            switch (analyzerId.ToUpperInvariant())
            {
                case "E508A3D4-B487-4D5F-8386-5827FA1334CD":
                    return new string[] { "ConstFieldNamesMustBeginWithUpperCaseLetter", "NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter", "FieldNamesMustNotUseHungarianNotation", "FieldNamesMustBeginWithLowerCaseLetter", "AccessibleFieldsMustBeginWithUpperCaseLetter" };

                case "C0F8B61A-DC6C-4550-8652-1074C95520B6":
                    return new string[] { "VariableNamesMustNotBePrefixed", "FieldNamesMustNotBeginWithUnderscore", "FieldNamesMustNotContainUnderscore" };

                case "F3EA01DF-3F2F-42AF-865D-84768B8CF4B0":
                    return new string[] { "ElementMustBeginWithUpperCaseLetter", "ElementMustBeginWithLowerCaseLetter" };

                case "25474DA2-9B71-48A9-BA76-A4726EC8C48E":
                    return new string[] { "InterfaceNamesMustBeginWithI" };

                case "C17BFE16-544B-11DA-8BDE-F66BAD1E3F3A":
                    return new string[] { "CodeMustNotContainEmptyStatements", "CodeMustNotContainMultipleStatementsOnOneLine" };

                case "C0FC9515-97A4-4D61-89CD-9D87FEDD5B24":
                    return new string[] { "StatementMustNotUseUnnecessaryParenthesis" };

                case "8DE5A506-0BD9-478B-95AF-2B3EC20C2093":
                    return new string[] { "OpeningParenthesisMustBeOnDeclarationLine", "ClosingParenthesisMustBeOnLineOfLastParameter", "ClosingParenthesisMustBeOnLineOfOpeningParenthesis", "CommaMustBeOnSameLineAsPreviousParameter", "ParameterListMustFollowDeclaration", "ParameterMustFollowComma", "SplitParametersMustStartOnLineAfterDeclaration", "ParametersMustBeOnSameLineOrSeparateLines", "ParameterMustNotSpanMultipleLines" };

                case "2771D7CA-F585-4832-B2C2-88DA370EBAC1":
                    return new string[] { "DoNotPrefixCallsWithBaseUnlessLocalImplementationExists", "PrefixLocalCallsWithThis" };

                case "64C9DA50-FC1A-4F20-986E-72AAA00B8ED4":
                    return new string[] { 
                        "KeywordsMustBeSpacedCorrectly", "CommasMustBeSpacedCorrectly", "SemicolonsMustBeSpacedCorrectly", "SymbolsMustBeSpacedCorrectly", "DocumentationLinesMustBeginWithSingleSpace", "SingleLineCommentsMustBeginWithSingleSpace", "PreprocessorKeywordsMustNotBePrecededBySpace", "OperatorKeywordMustBeFollowedBySpace", "OpeningParenthesisMustBeSpacedCorrectly", "ClosingParenthesisMustBeSpacedCorrectly", "OpeningSquareBracketsMustBeSpacedCorrectly", "ClosingSquareBracketsMustBeSpacedCorrectly", "OpeningCurlyBracketsMustBeSpacedCorrectly", "ClosingCurlyBracketsMustBeSpacedCorrectly", "OpeningGenericBracketsMustBeSpacedCorrectly", "ClosingGenericBracketsMustBeSpacedCorrectly", 
                        "OpeningAttributeBracketsMustBeSpacedCorrectly", "ClosingAttributeBracketsMustBeSpacedCorrectly", "NullableTypeSymbolsMustNotBePrecededBySpace", "MemberAccessSymbolsMustBeSpacedCorrectly", "IncrementDecrementSymbolsMustBeSpacedCorrectly", "NegativeSignsMustBeSpacedCorrectly", "PositiveSignsMustBeSpacedCorrectly", "DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly", "ColonsMustBeSpacedCorrectly", "CodeMustNotContainMultipleWhitespaceInARow", "CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation"
                     };

                case "19F875BA-368A-40D8-B70A-5D26DF5BEBDD":
                    return new string[] { "TabsMustNotBeUsed" };

                case "9ADAF7F0-E57D-4DF7-9885-ABAB2A9149FC":
                    return new string[] { "FieldsMustBePrivate" };

                case "F6912B0F-C5FC-453D-BA02-183C3C9A2A8B":
                    return new string[] { "AccessModifierMustBeDeclared", "FieldsMustBePrivate" };

                case "5937033A-122C-492E-9C08-6F1AE80D1710":
                    return new string[] { "OpeningCurlyBracketsMustNotBeFollowedByBlankLine", "ElementDocumentationHeadersMustNotBeFollowedByBlankLine", "CodeMustNotContainMultipleBlankLinesInARow", "ClosingCurlyBracketsMustNotBePrecededByBlankLine", "OpeningCurlyBracketsMustNotBePrecededByBlankLine", "ChainedStatementBlocksMustNotBePrecededByBlankLine", "WhileDoFooterMustNotBePrecededByBlankLine", "SingleLineCommentsMustNotBeFollowedByBlankLine", "ClosingCurlyBracketMustBeFollowedByBlankLine", "ElementDocumentationHeaderMustBePrecededByBlankLine", "SingleLineCommentMustBePrecededByBlankLine" };

                case "305C458B-4CEC-4E49-96A9-E8012B333C7B":
                    return new string[] { "CurlyBracketsForMultiLineStatementsMustNotShareLine", "StatementMustNotBeOnSingleLine", "ElementMustNotBeOnSingleLine", "CurlyBracketsMustNotBeOmitted", "AllAccessorsMustBeMultiLineOrSingleLine" };

                case "31B0AB2A-8EED-4815-9F2D-C5A439EA9809":
                    return new string[] { "FileMustHaveHeader", "FileHeaderMustShowCopyright", "FileHeaderMustHaveCopyrightText", "FileHeaderMustContainFileName", "FileHeaderFileNameDocumentationMustMatchFileName", "FileHeaderMustHaveSummary", "FileHeaderMustHaveValidCompanyText" };

                case "A2B149D9-1F5E-4D79-8E8B-2273E956B9DD":
                    return new string[] { 
                        "ElementsMustBeDocumented", "PartialElementsMustBeDocumented", "EnumerationItemsMustBeDocumented", "DocumentationMustContainValidXml", "ElementDocumentationMustHaveSummary", "PartialElementDocumentationMustHaveSummary", "ElementDocumentationMustHaveSummaryText", "PartialElementDocumentationMustHaveSummaryText", "ElementDocumentationMustNotHaveDefaultSummary", "PropertyDocumentationMustHaveValue", "PropertyDocumentationMustHaveValueText", "ElementParametersMustBeDocumented", "ElementParameterDocumentationMustMatchElementParameters", "ElementParameterDocumentationMustDeclareParameterName", "ElementParameterDocumentationMustHaveText", "ElementReturnValueMustBeDocumented", 
                        "ElementReturnValueDocumentationMustHaveText", "VoidReturnValueMustNotBeDocumented", "GenericTypeParametersMustBeDocumented", "GenericTypeParametersMustBeDocumentedPartialClass", "GenericTypeParameterDocumentationMustMatchTypeParameters", "GenericTypeParameterDocumentationMustDeclareParameterName", "GenericTypeParameterDocumentationMustHaveText", "PropertySummaryDocumentationMustMatchAccessors", "PropertySummaryDocumentationMustOmitSetAccessorWithRestrictedAccess", "ElementDocumentationMustNotBeCopiedAndPasted", "SingleLineCommentsMustNotUseDocumentationStyleSlashes", "DocumentationTextMustNotBeEmpty", "DocumentationTextMustBeginWithACapitalLetter", "DocumentationTextMustEndWithAPeriod", "DocumentationTextMustContainWhitespace", "DocumentationMustMeetCharacterPercentage", 
                        "DocumentationTextMustMeetMinimumCharacterLength"
                     };

                case "E2283402-D0B2-468D-81BC-DE32B48B7A4C":
                    return new string[] { "UsingDirectivesMustBePlacedWithinNamespace", "ElementsMustAppearInTheCorrectOrder", "ElementsMustBeOrderedByAccess", "ConstantsMustAppearBeforeFields", "StaticElementsMustAppearBeforeInstanceElements", "PartialElementsMustDeclareAccess" };

                case "6D18499A-AA21-4BE5-9590-7C8BEC56C1F8":
                    return new string[] { "DeclarationKeywordsMustFollowOrder", "ProtectedMustComeBeforeInternal" };

                case "3B74A427-6D8D-4808-9FCD-520F15E8517C":
                    return new string[] { "CommentsMustContainText" };
            }
            return new string[0];
        }
    }
}

