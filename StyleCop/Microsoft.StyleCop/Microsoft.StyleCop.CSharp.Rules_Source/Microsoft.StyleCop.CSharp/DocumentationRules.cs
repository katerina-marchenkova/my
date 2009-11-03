namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.XPath;

    [SourceAnalyzer(typeof(CsParser))]
    public class DocumentationRules : SourceAnalyzer
    {
        internal const string CompanyNameProperty = "CompanyName";
        internal const string CopyrightProperty = "Copyright";
        private const string CrefGenericParamsRegex = @"(\s*(<|&lt;)\s*{0}\s*(>|&gt;))|(\s*{{\s*{0}\s*}})";
        private const string CrefRegex = "(?'see'<see\\s+cref\\s*=\\s*\")?(((({0})?{1})(?(see)({2})))|(?(see)T:(({0})?{1}){3}))(?(see)(\"\\s*(/>|>[\\w\\s]*</see>)))";
        internal const string IgnoreInternals = "IgnoreInternals";
        internal const bool IgnoreInternalsDefaultValue = false;
        internal const string IgnorePrivates = "IgnorePrivates";
        internal const bool IgnorePrivatesDefaultValue = false;
        private Dictionary<string, CachedXmlDocument> includedDocs;
        internal const bool IncludeFieldsDefaultValue = true;
        internal const string IncludeFieldsProperty = "IncludeFields";

        public override void AnalyzeDocument(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");
            CsDocument document2 = (CsDocument) document;
            if ((document2.RootElement != null) && !document2.RootElement.Generated)
            {
                this.CheckElementDocumentation(document2);
                this.CheckFileHeader(document2);
                this.CheckSingleLineComments(document2.RootElement);
            }
        }

        private static string BuildCrefValidationStringForType(ClassBase type)
        {
            string name = type.Declaration.Name;
            string[] genericParams = null;
            string str2 = null;
            int index = name.IndexOf('<');
            if (index > 0)
            {
                genericParams = ExtractGenericParametersFromType(name, index);
                if ((genericParams != null) && (genericParams.Length > 0))
                {
                    str2 = BuildGenericParametersRegex(genericParams);
                }
                name = name.Substring(0, index);
            }
            string str3 = BuildNamespaceRegex(type);
            return string.Format(CultureInfo.InvariantCulture, "(?'see'<see\\s+cref\\s*=\\s*\")?(((({0})?{1})(?(see)({2})))|(?(see)T:(({0})?{1}){3}))(?(see)(\"\\s*(/>|>[\\w\\s]*</see>)))", new object[] { str3, name, (str2 == null) ? string.Empty : str2, (genericParams == null) ? string.Empty : ("`" + genericParams.Length.ToString(CultureInfo.InvariantCulture)) });
        }

        private static string BuildGenericParametersRegex(string[] genericParams)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < genericParams.Length; i++)
            {
                builder.Append(genericParams[i]);
                if (i < (genericParams.Length - 1))
                {
                    builder.Append(@"\s*,\s*");
                }
            }
            return string.Format(CultureInfo.InvariantCulture, @"(\s*(<|&lt;)\s*{0}\s*(>|&gt;))|(\s*{{\s*{0}\s*}})", new object[] { builder.ToString() });
        }

        private static string BuildNamespaceRegex(ClassBase type)
        {
            string fullyQualifiedName = type.FullyQualifiedName;
            StringBuilder builder = new StringBuilder();
            int startIndex = 5;
            for (int i = startIndex; i < fullyQualifiedName.Length; i++)
            {
                if (fullyQualifiedName[i] == '.')
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(@"\.");
                    }
                    builder.Append(fullyQualifiedName.Substring(startIndex, i - startIndex));
                    startIndex = i + 1;
                }
                else if (fullyQualifiedName[i] == '<')
                {
                    break;
                }
            }
            if (builder.Length > 0)
            {
                builder.Append(@"\.");
            }
            return builder.ToString();
        }

        private void CheckClassElementHeader(ClassBase classElement, AnalyzerSettings settings)
        {
            AnalyzerSettings settings2 = settings;
            settings2.RequireFields = false;
            if (classElement.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Partial }))
            {
                this.CheckHeader(classElement, settings2, true);
            }
            else
            {
                this.CheckHeader(classElement, settings2, false);
            }
        }

        private void CheckConstructorSummaryText(Constructor constructor, XmlDocument doc)
        {
            XmlNode node = doc.SelectSingleNode("root/summary");
            if (node != null)
            {
                string input = node.InnerXml.Trim();
                string type = (constructor.Parent is Struct) ? "struct" : "class";
                string typeRegex = BuildCrefValidationStringForType((ClassBase) constructor.ParentElement);
                string pattern = GetExpectedSummaryTextForConstructorType(constructor, type, typeRegex);
                if (!Regex.IsMatch(input, pattern, RegexOptions.ExplicitCapture))
                {
                    base.AddViolation(constructor, Microsoft.StyleCop.CSharp.Rules.ConstructorSummaryDocumentationMustBeginWithStandardText, new object[] { GetExampleSummaryTextForConstructorType(constructor, type) });
                }
            }
        }

        private void CheckDestructorSummaryText(Destructor destructor, XmlDocument doc)
        {
            XmlNode node = doc.SelectSingleNode("root/summary");
            if (node != null)
            {
                string input = node.InnerXml.Trim();
                string expectedSummaryTextForDestructor = GetExpectedSummaryTextForDestructor(BuildCrefValidationStringForType((ClassBase) destructor.ParentElement));
                if (!Regex.IsMatch(input, expectedSummaryTextForDestructor))
                {
                    base.AddViolation(destructor, Microsoft.StyleCop.CSharp.Rules.DestructorSummaryDocumentationMustBeginWithStandardText, new object[] { GetExampleSummaryTextForDestructor() });
                }
            }
        }

        private bool CheckDocumentationForElement(CsElement element, CsElement parentElement, AnalyzerSettings settings)
        {
            if (base.Cancel)
            {
                return false;
            }
            if (!element.Generated)
            {
                if (((element.ElementType == ElementType.Class) || (element.ElementType == ElementType.Interface)) || (element.ElementType == ElementType.Struct))
                {
                    ClassBase classElement = element as ClassBase;
                    this.CheckClassElementHeader(classElement, settings);
                }
                else if ((((element.ElementType == ElementType.Enum) || (element.ElementType == ElementType.Delegate)) || ((element.ElementType == ElementType.Event) || (element.ElementType == ElementType.Property))) || (((element.ElementType == ElementType.Indexer) || (element.ElementType == ElementType.Method)) || (((element.ElementType == ElementType.Constructor) || (element.ElementType == ElementType.Destructor)) || (element.ElementType == ElementType.Field))))
                {
                    this.CheckHeader(element, settings, false);
                }
                if (element.ElementType == ElementType.Enum)
                {
                    this.CheckEnumHeaders(element as Microsoft.StyleCop.CSharp.Enum, settings);
                }
                if (((element.ElementType == ElementType.Accessor) || (element.ElementType == ElementType.Constructor)) || ((element.ElementType == ElementType.Destructor) || (element.ElementType == ElementType.Method)))
                {
                    this.CheckElementComments(element);
                }
            }
            return true;
        }

        private void CheckDocumentationValidity(CsElement element, int lineNumber, XmlNode documentationXml, string documentationType)
        {
            InvalidCommentType type = CommentVerifier.IsGarbageComment(documentationXml);
            if ((type & InvalidCommentType.Empty) != InvalidCommentType.Valid)
            {
                base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.DocumentationTextMustNotBeEmpty, new object[] { documentationType });
            }
            if ((type & InvalidCommentType.NoPeriod) != InvalidCommentType.Valid)
            {
                base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.DocumentationTextMustEndWithAPeriod, new object[] { documentationType });
            }
            if (((type & InvalidCommentType.NoCapitalLetter) != InvalidCommentType.Valid) && (!documentationType.Equals("return", StringComparison.Ordinal) || (!documentationXml.InnerText.StartsWith("true", StringComparison.Ordinal) && !documentationXml.InnerText.StartsWith("false", StringComparison.Ordinal))))
            {
                base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.DocumentationTextMustBeginWithACapitalLetter, new object[] { documentationType });
            }
            if ((type & InvalidCommentType.NoWhitespace) != InvalidCommentType.Valid)
            {
                base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.DocumentationTextMustContainWhitespace, new object[] { documentationType });
            }
            if ((type & InvalidCommentType.TooFewCharacters) != InvalidCommentType.Valid)
            {
                base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.DocumentationMustMeetCharacterPercentage, new object[] { documentationType, 40, 60 });
            }
            if ((type & InvalidCommentType.TooShort) != InvalidCommentType.Valid)
            {
                base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.DocumentationTextMustMeetMinimumCharacterLength, new object[] { documentationType, 10 });
            }
        }

        private void CheckElementComments(CsElement element)
        {
            foreach (CsToken token in element.Tokens)
            {
                if ((token.CsTokenType == CsTokenType.XmlHeader) || (token.CsTokenType == CsTokenType.XmlHeaderLine))
                {
                    base.AddViolation(element, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.SingleLineCommentsMustNotUseDocumentationStyleSlashes, new object[0]);
                }
            }
        }

        private void CheckElementDocumentation(CsDocument document)
        {
            AnalyzerSettings context = new AnalyzerSettings();
            context.IgnorePrivates = false;
            context.IgnoreInternals = false;
            context.RequireFields = true;
            if (document.Settings != null)
            {
                BooleanProperty addInSetting = document.Settings.GetAddInSetting(this, "IgnorePrivates") as BooleanProperty;
                if (addInSetting != null)
                {
                    context.IgnorePrivates = addInSetting.Value;
                }
                addInSetting = document.Settings.GetAddInSetting(this, "IgnoreInternals") as BooleanProperty;
                if (addInSetting != null)
                {
                    context.IgnoreInternals = addInSetting.Value;
                }
                addInSetting = document.Settings.GetAddInSetting(this, "IncludeFields") as BooleanProperty;
                if (addInSetting != null)
                {
                    context.RequireFields = addInSetting.Value;
                }
            }
            document.WalkDocument<AnalyzerSettings>(new CodeWalkerElementVisitor<AnalyzerSettings>(this.CheckDocumentationForElement), context);
        }

        private void CheckEnumHeaders(Microsoft.StyleCop.CSharp.Enum element, AnalyzerSettings settings)
        {
            foreach (EnumItem item in element.Items)
            {
                if ((item.Header == null) || (item.Header.Text.Length == 0))
                {
                    if ((!settings.IgnorePrivates || (element.Declaration.AccessModifierType != AccessModifierType.Private)) && (!settings.IgnoreInternals || (element.Declaration.AccessModifierType != AccessModifierType.Internal)))
                    {
                        base.AddViolation(item, Microsoft.StyleCop.CSharp.Rules.EnumerationItemsMustBeDocumented, new object[0]);
                    }
                    continue;
                }
                this.ParseHeader(item, item.Header, item.LineNumber, false);
            }
        }

        private void CheckFileHeader(CsDocument document)
        {
            string companyName = null;
            string copyright = null;
            StringProperty setting = base.GetSetting(document.Settings, "CompanyName") as StringProperty;
            if (setting != null)
            {
                companyName = setting.Value;
            }
            StringProperty property2 = base.GetSetting(document.Settings, "Copyright") as StringProperty;
            if (property2 != null)
            {
                copyright = property2.Value;
            }
            this.CheckFileHeader(document, copyright, companyName);
        }

        private void CheckFileHeader(CsDocument document, string copyright, string companyName)
        {
            if (((document.FileHeader == null) || (document.FileHeader.HeaderXml == null)) || (document.FileHeader.HeaderXml.Length == 0))
            {
                base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileMustHaveHeader, new object[0]);
            }
            else
            {
                try
                {
                    XmlDocument document2 = new XmlDocument();
                    document2.LoadXml(document.FileHeader.HeaderXml);
                    XmlNode node = document2.DocumentElement["copyright"];
                    if (node == null)
                    {
                        base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileHeaderMustShowCopyright, new object[0]);
                    }
                    else
                    {
                        string strB = node.InnerText.Trim();
                        if (strB.Length == 0)
                        {
                            base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileHeaderMustHaveCopyrightText, new object[0]);
                        }
                        else if (!string.IsNullOrEmpty(copyright) && (string.CompareOrdinal(copyright, strB) != 0))
                        {
                            base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileHeaderCopyrightTextMustMatch, new object[] { copyright });
                        }
                        XmlNode node2 = node.Attributes["file"];
                        if (node2 == null)
                        {
                            base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileHeaderMustContainFileName, new object[0]);
                        }
                        else if (string.Compare(node2.InnerText, document.SourceCode.Name, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileHeaderFileNameDocumentationMustMatchFileName, new object[0]);
                        }
                        node2 = node.Attributes["company"];
                        if (node2 == null)
                        {
                            base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileHeaderMustHaveValidCompanyText, new object[0]);
                        }
                        else
                        {
                            strB = node2.Value.Trim();
                            if (strB.Length == 0)
                            {
                                base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileHeaderMustHaveValidCompanyText, new object[0]);
                            }
                            else if (!string.IsNullOrEmpty(companyName) && (string.CompareOrdinal(companyName, strB) != 0))
                            {
                                base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileHeaderCompanyNameTextMustMatch, new object[] { companyName });
                            }
                        }
                    }
                    node = document2.DocumentElement["summary"];
                    if ((node == null) || (node.InnerText.Length == 0))
                    {
                        base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileHeaderMustHaveSummary, new object[0]);
                    }
                }
                catch (XmlException)
                {
                    base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileMustHaveHeader, new object[0]);
                }
                catch (ArgumentException)
                {
                    base.AddViolation(document.RootElement, 1, Microsoft.StyleCop.CSharp.Rules.FileMustHaveHeader, new object[0]);
                }
            }
        }

        private void CheckForBlankLinesInDocumentationHeader(CsElement element, XmlHeader header)
        {
            if (!element.Generated)
            {
                int num = 0;
                for (Microsoft.StyleCop.Node<CsToken> node = header.ChildTokens.First; (node != null) && (node != header.ChildTokens.Last.Next); node = node.Next)
                {
                    CsToken token = node.Value;
                    if (token.CsTokenType == CsTokenType.EndOfLine)
                    {
                        num++;
                        if (num > 1)
                        {
                            base.AddViolation(element, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.DocumentationHeadersMustNotContainBlankLines, new object[0]);
                            return;
                        }
                    }
                    else if (token.CsTokenType == CsTokenType.XmlHeaderLine)
                    {
                        if ((node == header.ChildTokens.First) || (node == header.ChildTokens.Last))
                        {
                            if (IsXmlHeaderLineEmpty(token))
                            {
                                base.AddViolation(element, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.DocumentationHeadersMustNotContainBlankLines, new object[0]);
                                return;
                            }
                        }
                        else if (!IsXmlHeaderLineEmpty(token))
                        {
                            num = 0;
                        }
                    }
                }
            }
        }

        private void CheckForRepeatingComments(CsElement element, XmlDocument doc)
        {
            List<string> list = new List<string>();
            XmlNode node = doc.SelectSingleNode("root/summary");
            if (node != null)
            {
                list.Add(node.InnerXml.Trim());
            }
            node = doc.SelectSingleNode("root/returns");
            if (node != null)
            {
                list.Add(node.InnerXml.Trim());
            }
            node = doc.SelectSingleNode("root/remarks");
            if (node != null)
            {
                list.Add(node.InnerXml.Trim());
            }
            XmlNodeList list2 = doc.SelectNodes("root/param");
            if ((list2 != null) && (list2.Count > 0))
            {
                foreach (XmlNode node2 in list2)
                {
                    list.Add(node2.InnerXml.Trim());
                }
            }
            list2 = doc.SelectNodes("root/typeparam");
            if ((list2 != null) && (list2.Count > 0))
            {
                foreach (XmlNode node3 in list2)
                {
                    list.Add(node3.InnerXml.Trim());
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Length > 0)
                {
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        if ((string.Compare(list[i], list[j], StringComparison.Ordinal) == 0) && (string.Compare(list[i], CachedCodeStrings.ParameterNotUsed, StringComparison.Ordinal) != 0))
                        {
                            base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ElementDocumentationMustNotBeCopiedAndPasted, new object[] { CachedCodeStrings.ParameterNotUsed });
                            return;
                        }
                    }
                }
            }
        }

        private void CheckGenericTypeParams(CsElement element, XmlDocument doc)
        {
            List<string> list = ExtractGenericTypeList(element.Declaration.Name);
            if ((list != null) && (list.Count > 0))
            {
                XmlNodeList list2 = doc.SelectNodes("root/typeparam");
                if ((list2 == null) || (list2.Count == 0))
                {
                    bool flag = element.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Partial });
                    if ((!flag || (doc.SelectSingleNode("root/summary") != null)) || (doc.SelectSingleNode("root/content") == null))
                    {
                        if (flag)
                        {
                            base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.GenericTypeParametersMustBeDocumentedPartialClass, new object[] { element.FriendlyTypeText });
                        }
                        else
                        {
                            base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.GenericTypeParametersMustBeDocumented, new object[] { element.FriendlyTypeText });
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < list2.Count; i++)
                    {
                        XmlNode node = list2[i];
                        if (list.Count <= i)
                        {
                            base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.GenericTypeParameterDocumentationMustMatchTypeParameters, new object[] { element.FriendlyTypeText });
                            break;
                        }
                        XmlNode namedItem = node.Attributes.GetNamedItem("name");
                        if ((namedItem == null) || (namedItem.Value.Length == 0))
                        {
                            base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.GenericTypeParameterDocumentationMustDeclareParameterName, new object[0]);
                        }
                        else if (namedItem.Value != list[i])
                        {
                            base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.GenericTypeParameterDocumentationMustMatchTypeParameters, new object[] { element.FriendlyTypeText });
                            break;
                        }
                    }
                    if (list.Count > list2.Count)
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.GenericTypeParameterDocumentationMustMatchTypeParameters, new object[] { element.FriendlyTypeText });
                    }
                    foreach (XmlNode node3 in list2)
                    {
                        if ((node3.InnerText == null) || (node3.InnerText.Length == 0))
                        {
                            base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.GenericTypeParameterDocumentationMustHaveText, new object[] { node3.OuterXml });
                            continue;
                        }
                        this.CheckDocumentationValidity(element, element.LineNumber, node3, "typeparam");
                    }
                }
            }
        }

        private void CheckHeader(CsElement element, AnalyzerSettings settings, bool partialElement)
        {
            if ((element.Header == null) || (element.Header.Text.Length == 0))
            {
                if ((((element.ElementType == ElementType.Class) || (element.ElementType == ElementType.Interface)) || (element.ElementType == ElementType.Struct)) || (((settings.RequireFields || (element.ElementType != ElementType.Field)) && (!settings.IgnorePrivates || (element.Declaration.AccessModifierType != AccessModifierType.Private))) && (!settings.IgnoreInternals || (element.Declaration.AccessModifierType != AccessModifierType.Internal))))
                {
                    if (partialElement)
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.PartialElementsMustBeDocumented, new object[] { element.FriendlyTypeText });
                    }
                    else if (!IsNonPublicStaticExternDllImport(element))
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ElementsMustBeDocumented, new object[] { element.FriendlyTypeText });
                    }
                }
            }
            else
            {
                this.ParseHeader(element, element.Header, element.LineNumber, partialElement);
            }
        }

        private void CheckHeaderParams(CsElement element, ICollection<Parameter> parameters, XmlDocument doc)
        {
            XmlNodeList list = doc.SelectNodes("root/param");
            if ((list == null) || ((list.Count == 0) && (parameters.Count > 0)))
            {
                base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ElementParametersMustBeDocumented, new object[0]);
            }
            else
            {
                int num = 0;
                foreach (Parameter parameter in parameters)
                {
                    if (list.Count <= num)
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ElementParametersMustBeDocumented, new object[0]);
                        break;
                    }
                    XmlNode node = list[num];
                    XmlNode namedItem = node.Attributes.GetNamedItem("name");
                    if ((namedItem == null) || (namedItem.Value.Length == 0))
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ElementParameterDocumentationMustDeclareParameterName, new object[0]);
                    }
                    else
                    {
                        string name = parameter.Name;
                        if (name.StartsWith("@", StringComparison.Ordinal) && (name.Length > 1))
                        {
                            name = name.Substring(1, name.Length - 1);
                        }
                        if (namedItem.Value != name)
                        {
                            base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ElementParameterDocumentationMustMatchElementParameters, new object[0]);
                        }
                    }
                    num++;
                }
                foreach (XmlNode node3 in list)
                {
                    if ((node3.InnerText == null) || (node3.InnerText.Length == 0))
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ElementParameterDocumentationMustHaveText, new object[] { node3.OuterXml });
                        continue;
                    }
                    this.CheckDocumentationValidity(element, element.LineNumber, node3, "param");
                }
                if (list.Count > num)
                {
                    base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ElementParameterDocumentationMustMatchElementParameters, new object[0]);
                }
            }
        }

        private void CheckHeaderReturnValue(CsElement element, TypeToken returnType, XmlDocument doc)
        {
            if (returnType != null)
            {
                XmlNode documentationXml = doc.SelectSingleNode("root/returns");
                if (returnType.Text != "void")
                {
                    if (documentationXml == null)
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ElementReturnValueMustBeDocumented, new object[0]);
                    }
                    else if ((documentationXml.InnerText == null) || (documentationXml.InnerText.Length == 0))
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ElementReturnValueDocumentationMustHaveText, new object[0]);
                    }
                    else
                    {
                        this.CheckDocumentationValidity(element, element.LineNumber, documentationXml, "return");
                    }
                }
                else if (documentationXml != null)
                {
                    base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.VoidReturnValueMustNotBeDocumented, new object[0]);
                }
            }
        }

        private void CheckHeaderSummary(CsElement element, int lineNumber, bool partialElement, XmlDocument doc)
        {
            XmlNode documentationXml = doc.SelectSingleNode("root/summary");
            if ((documentationXml == null) && partialElement)
            {
                documentationXml = doc.SelectSingleNode("root/content");
            }
            if (documentationXml == null)
            {
                if (partialElement)
                {
                    base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.PartialElementDocumentationMustHaveSummary, new object[0]);
                }
                else
                {
                    base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.ElementDocumentationMustHaveSummary, new object[0]);
                }
            }
            else if ((documentationXml.InnerText == null) || (documentationXml.InnerText.Length == 0))
            {
                if (partialElement)
                {
                    base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.PartialElementDocumentationMustHaveSummaryText, new object[0]);
                }
                else
                {
                    base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.ElementDocumentationMustHaveSummaryText, new object[0]);
                }
            }
            else
            {
                int startIndex = 0;
                startIndex = 0;
                while (startIndex < documentationXml.InnerText.Length)
                {
                    if (((documentationXml.InnerText[startIndex] != '\r') && (documentationXml.InnerText[startIndex] != '\n')) && ((documentationXml.InnerText[startIndex] != '\t') && (documentationXml.InnerText[startIndex] != ' ')))
                    {
                        break;
                    }
                    startIndex++;
                }
                if (documentationXml.InnerText.Substring(startIndex, documentationXml.InnerText.Length - startIndex).StartsWith("Summary description for", StringComparison.Ordinal))
                {
                    base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.ElementDocumentationMustNotHaveDefaultSummary, new object[0]);
                }
                else
                {
                    this.CheckDocumentationValidity(element, lineNumber, documentationXml, "summary");
                }
            }
        }

        private void CheckPropertySummaryFormatting(Property property, XmlDocument doc)
        {
            if (!property.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Override }))
            {
                XmlNode node = doc.SelectSingleNode("root/summary");
                if (node != null)
                {
                    bool flag = ((property.ReturnType.Text == "bool") || (property.ReturnType.Text == "Boolean")) || (property.ReturnType.Text == "System.Boolean");
                    if (property.GetAccessor != null)
                    {
                        if ((property.SetAccessor == null) || !IncludeSetAccessorInDocumentation(property, property.SetAccessor))
                        {
                            string str2 = flag ? CachedCodeStrings.HeaderSummaryForBooleanGetAccessor : CachedCodeStrings.HeaderSummaryForGetAccessor;
                            string str3 = node.InnerText.TrimStart(new char[0]);
                            if (!str3.StartsWith(str2, StringComparison.Ordinal))
                            {
                                base.AddViolation(property, Microsoft.StyleCop.CSharp.Rules.PropertySummaryDocumentationMustMatchAccessors, new object[] { str2 });
                            }
                            string headerSummaryForGetAndSetAccessor = CachedCodeStrings.HeaderSummaryForGetAndSetAccessor;
                            if (str3.StartsWith(headerSummaryForGetAndSetAccessor, StringComparison.Ordinal))
                            {
                                base.AddViolation(property, Microsoft.StyleCop.CSharp.Rules.PropertySummaryDocumentationMustOmitSetAccessorWithRestrictedAccess, new object[] { str2 });
                            }
                        }
                        else
                        {
                            string str = flag ? CachedCodeStrings.HeaderSummaryForBooleanGetAndSetAccessor : CachedCodeStrings.HeaderSummaryForGetAndSetAccessor;
                            if (!node.InnerText.TrimStart(new char[0]).StartsWith(str, StringComparison.Ordinal))
                            {
                                base.AddViolation(property, Microsoft.StyleCop.CSharp.Rules.PropertySummaryDocumentationMustMatchAccessors, new object[] { str });
                            }
                        }
                    }
                    else if (property.SetAccessor != null)
                    {
                        string str5 = flag ? CachedCodeStrings.HeaderSummaryForBooleanSetAccessor : CachedCodeStrings.HeaderSummaryForSetAccessor;
                        if (!node.InnerText.TrimStart(new char[0]).StartsWith(str5, StringComparison.Ordinal))
                        {
                            base.AddViolation(property, Microsoft.StyleCop.CSharp.Rules.PropertySummaryDocumentationMustMatchAccessors, new object[] { str5 });
                        }
                    }
                }
            }
        }

        private void CheckPropertyValueTag(CsElement element, XmlDocument doc)
        {
            XmlNode documentationXml = doc.SelectSingleNode("root/value");
            if (documentationXml == null)
            {
                if (((element.ActualAccess == AccessModifierType.Public) || (element.ActualAccess == AccessModifierType.ProtectedInternal)) || (element.ActualAccess == AccessModifierType.Protected))
                {
                    base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.PropertyDocumentationMustHaveValue, new object[0]);
                }
            }
            else if ((documentationXml.InnerText == null) || (documentationXml.InnerText.Length == 0))
            {
                base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.PropertyDocumentationMustHaveValue, new object[0]);
            }
            else
            {
                this.CheckDocumentationValidity(element, element.LineNumber, documentationXml, "value");
            }
        }

        private void CheckSingleLineComments(DocumentRoot root)
        {
            if (root.Tokens != null)
            {
                foreach (CsToken token in root.Tokens)
                {
                    if (((token.CsTokenType == CsTokenType.SingleLineComment) && token.Text.StartsWith("///", StringComparison.Ordinal)) && ((token.Text.Length == 3) || ((token.Text.Length > 3) && (token.Text[3] != '/'))))
                    {
                        base.AddViolation(root, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.SingleLineCommentsMustNotUseDocumentationStyleSlashes, new object[0]);
                    }
                }
            }
        }

        private static XmlNode ExtractDocumentationNodeFromIncludedFile(XmlDocument document, string xpath)
        {
            XmlNode node = null;
            try
            {
                node = document.SelectSingleNode(xpath);
            }
            catch (XPathException)
            {
            }
            catch (XmlException)
            {
            }
            catch (ArgumentException)
            {
            }
            return node;
        }

        private static string[] ExtractGenericParametersFromType(string typeName, int index)
        {
            List<string> list = new List<string>();
            for (int i = index + 1; i < typeName.Length; i++)
            {
                StringBuilder builder = new StringBuilder();
                while (i < typeName.Length)
                {
                    if (((typeName[i] == '>') || (typeName[i] == ',')) || char.IsWhiteSpace(typeName[i]))
                    {
                        if (builder.Length > 0)
                        {
                            list.Add(builder.ToString());
                        }
                        break;
                    }
                    builder.Append(typeName[i]);
                    if (i == (typeName.Length - 1))
                    {
                        list.Add(builder.ToString());
                    }
                    i++;
                }
                if (typeName[i] == '>')
                {
                    break;
                }
            }
            return list.ToArray();
        }

        private static List<string> ExtractGenericTypeList(string name)
        {
            List<string> list = null;
            if ((!name.StartsWith("operator", StringComparison.Ordinal) || (name.Length <= 8)) || !char.IsWhiteSpace(name[8]))
            {
                int startIndex = name.LastIndexOf(".", StringComparison.Ordinal);
                if (startIndex == -1)
                {
                    startIndex = 0;
                }
                int num2 = name.IndexOf("<", startIndex, StringComparison.Ordinal);
                if (num2 == -1)
                {
                    return list;
                }
                list = new List<string>();
                while (true)
                {
                    int num3 = name.IndexOf(",", num2 + 1, StringComparison.Ordinal);
                    if (num3 == -1)
                    {
                        break;
                    }
                    list.Add(name.Substring(num2 + 1, (num3 - num2) - 1).Trim());
                    num2 = num3;
                }
                int index = name.IndexOf(">", StringComparison.Ordinal);
                if (index != -1)
                {
                    list.Add(name.Substring(num2 + 1, (index - num2) - 1).Trim());
                }
            }
            return list;
        }

        private static void ExtractIncludeTagFileAndPath(XmlNode documentationNode, out string file, out string path)
        {
            file = null;
            XmlAttribute attribute = documentationNode.Attributes["file"];
            if (attribute != null)
            {
                file = attribute.Value;
            }
            path = null;
            attribute = documentationNode.Attributes["path"];
            if (attribute != null)
            {
                path = attribute.Value;
            }
        }

        private static string GetExampleSummaryTextForConstructorType(Constructor constructor, string type)
        {
            if (constructor.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static }))
            {
                return string.Format(CultureInfo.InvariantCulture, CachedCodeStrings.ExampleHeaderSummaryForStaticConstructor, new object[] { type });
            }
            if ((constructor.AccessModifier == AccessModifierType.Private) && ((constructor.Parameters == null) || (constructor.Parameters.Count == 0)))
            {
                return string.Format(CultureInfo.InvariantCulture, CachedCodeStrings.ExampleHeaderSummaryForPrivateInstanceConstructor, new object[] { type });
            }
            return string.Format(CultureInfo.InvariantCulture, CachedCodeStrings.ExampleHeaderSummaryForInstanceConstructor, new object[] { type });
        }

        private static string GetExampleSummaryTextForDestructor()
        {
            return CachedCodeStrings.ExampleHeaderSummaryForDestructor;
        }

        private static string GetExpectedSummaryTextForConstructorType(Constructor constructor, string type, string typeRegex)
        {
            if (constructor.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static }))
            {
                return string.Format(CultureInfo.InvariantCulture, CachedCodeStrings.HeaderSummaryForStaticConstructor, new object[] { typeRegex, type });
            }
            if ((constructor.AccessModifier == AccessModifierType.Private) && ((constructor.Parameters == null) || (constructor.Parameters.Count == 0)))
            {
                return string.Format(CultureInfo.InvariantCulture, CachedCodeStrings.HeaderSummaryForPrivateInstanceConstructor, new object[] { typeRegex, type });
            }
            return string.Format(CultureInfo.InvariantCulture, CachedCodeStrings.HeaderSummaryForInstanceConstructor, new object[] { typeRegex, type });
        }

        private static string GetExpectedSummaryTextForDestructor(string typeRegex)
        {
            return string.Format(CultureInfo.InvariantCulture, CachedCodeStrings.HeaderSummaryForDestructor, new object[] { typeRegex });
        }

        private static bool IncludeSetAccessorInDocumentation(Property property, Accessor setAccessor)
        {
            if ((setAccessor.AccessModifier != property.AccessModifier) && ((setAccessor.AccessModifier != AccessModifierType.Private) || setAccessor.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Private })))
            {
                if ((setAccessor.AccessModifier == AccessModifierType.Internal) && ((property.ActualAccess == AccessModifierType.Internal) || (property.ActualAccess == AccessModifierType.ProtectedAndInternal)))
                {
                    return true;
                }
                if (((property.ActualAccess != AccessModifierType.Private) || setAccessor.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Private })) && ((setAccessor.AccessModifier != AccessModifierType.Protected) && (setAccessor.AccessModifier != AccessModifierType.ProtectedInternal)))
                {
                    return false;
                }
            }
            return true;
        }

        private bool InsertIncludedDocumentation(CsElement element, string basePath, XmlDocument documentation)
        {
            return this.InsertIncludedDocumentationForNode(element, basePath, documentation.DocumentElement);
        }

        private bool InsertIncludedDocumentationForChildNodes(CsElement element, string basePath, XmlNode documentationNode)
        {
            if (documentationNode.ChildNodes.Count == 1)
            {
                if (!this.InsertIncludedDocumentationForNode(element, basePath, documentationNode.FirstChild))
                {
                    return false;
                }
            }
            else if (documentationNode.ChildNodes.Count > 1)
            {
                XmlNode[] nodeArray = new XmlNode[documentationNode.ChildNodes.Count];
                int num = 0;
                for (XmlNode node = documentationNode.FirstChild; node != null; node = node.NextSibling)
                {
                    nodeArray[num++] = node;
                }
                for (int i = 0; i < nodeArray.Length; i++)
                {
                    if (!this.InsertIncludedDocumentationForNode(element, basePath, nodeArray[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool InsertIncludedDocumentationForNode(CsElement element, string basePath, XmlNode documentationNode)
        {
            if ((documentationNode.NodeType == XmlNodeType.Element) && (documentationNode.Name == "include"))
            {
                return this.LoadAndReplaceIncludeTag(element, basePath, documentationNode);
            }
            return this.InsertIncludedDocumentationForChildNodes(element, basePath, documentationNode);
        }

        private static bool IsNonPublicStaticExternDllImport(CsElement element)
        {
            if (element.ActualAccess != AccessModifierType.Public)
            {
                if (!element.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static }) || !element.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Extern }))
                {
                    return false;
                }
                if (element.Attributes != null)
                {
                    foreach (Microsoft.StyleCop.CSharp.Attribute attribute in element.Attributes)
                    {
                        if (attribute.AttributeExpressions != null)
                        {
                            foreach (Expression expression in attribute.AttributeExpressions)
                            {
                                AttributeExpression expression2 = expression as AttributeExpression;
                                if (expression2 != null)
                                {
                                    foreach (Expression expression3 in expression2.ChildExpressions)
                                    {
                                        MethodInvocationExpression expression4 = expression3 as MethodInvocationExpression;
                                        if (((expression4 != null) && (expression4.Name != null)) && ((expression4.Name.Tokens.MatchTokens(new string[] { "DllImport" }) || expression4.Name.Tokens.MatchTokens(new string[] { "DllImportAttribute" })) || (expression4.Name.Tokens.MatchTokens(new string[] { "System", ".", "Runtime", ".", "InteropServices", ".", "DllImport" }) || expression4.Name.Tokens.MatchTokens(new string[] { "System", ".", "Runtime", ".", "InteropServices", ".", "DllImportAttribute" }))))
                                        {
                                            return true;
                                        }
                                    }
                                    continue;
                                }
                            }
                            continue;
                        }
                    }
                }
            }
            return false;
        }

        private static bool IsXmlHeaderLineEmpty(CsToken token)
        {
            int num = 0;
            for (int i = 0; i < token.Text.Length; i++)
            {
                char c = token.Text[i];
                if (num < 3)
                {
                    if (c != '/')
                    {
                        return false;
                    }
                    num++;
                }
                else if (!char.IsWhiteSpace(c))
                {
                    return false;
                }
            }
            return true;
        }

        private bool LoadAndReplaceIncludeTag(CsElement element, string basePath, XmlNode documentationNode)
        {
            string str;
            string str2;
            ExtractIncludeTagFileAndPath(documentationNode, out str, out str2);
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(str2))
            {
                base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.IncludeNodeDoesNotContainValidFileAndPath, new object[] { documentationNode.OuterXml });
            }
            else
            {
                CachedXmlDocument includedDocument = this.LoadIncludedDocumentationFile(element, basePath, str);
                if (includedDocument == null)
                {
                    base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.IncludedDocumentationFileDoesNotExist, new object[] { str });
                }
                else
                {
                    XmlNode includedDocumentationNode = ExtractDocumentationNodeFromIncludedFile(includedDocument.Document, str2);
                    if (includedDocumentationNode == null)
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.IncludedDocumentationXPathDoesNotExist, new object[] { str2, str });
                    }
                    else if (!this.ReplaceIncludeTagWithIncludedDocumentationContents(element, documentationNode, includedDocument, includedDocumentationNode))
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.IncludedDocumentationXPathDoesNotExist, new object[] { str2, str });
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static CachedXmlDocument LoadDocFileFromDisk(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(path);
                    return new CachedXmlDocument(path, document);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (XmlException)
            {
            }
            return null;
        }

        private XmlDocument LoadHeaderIntoDocument(CsElement element, XmlHeader header, int lineNumber)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                string xml = "<root>" + header.Text + "</root>";
                document.LoadXml(xml);
            }
            catch (XmlException exception)
            {
                base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.DocumentationMustContainValidXml, new object[] { exception.Message });
                document = null;
            }
            return document;
        }

        private CachedXmlDocument LoadIncludedDocumentationFile(CsElement element, string basePath, string file)
        {
            CachedXmlDocument document = null;
            string fullPath = file;
            if (!Path.IsPathRooted(file))
            {
                try
                {
                    fullPath = Path.GetFullPath(Path.Combine(basePath, file));
                }
                catch (ArgumentException)
                {
                    fullPath = null;
                }
                catch (SecurityException)
                {
                    fullPath = null;
                }
                catch (NotSupportedException)
                {
                    fullPath = null;
                }
                catch (PathTooLongException)
                {
                    fullPath = null;
                }
            }
            if (fullPath != null)
            {
                string key = fullPath.ToUpper(CultureInfo.InvariantCulture);
                if (this.includedDocs == null)
                {
                    this.includedDocs = new Dictionary<string, CachedXmlDocument>();
                }
                if (!this.includedDocs.TryGetValue(key, out document))
                {
                    document = LoadDocFileFromDisk(key);
                    this.includedDocs.Add(key, document);
                }
            }
            return document;
        }

        private void ParseHeader(CsElement element, XmlHeader header, int lineNumber, bool partialElement)
        {
            XmlDocument documentation = this.LoadHeaderIntoDocument(element, header, lineNumber);
            if ((documentation != null) && this.InsertIncludedDocumentation(element, Path.GetDirectoryName(element.Document.SourceCode.Path), documentation))
            {
                this.CheckForBlankLinesInDocumentationHeader(element, header);
                this.CheckHeaderSummary(element, lineNumber, partialElement, documentation);
                if (element.ElementType == ElementType.Method)
                {
                    Microsoft.StyleCop.CSharp.Method method = element as Microsoft.StyleCop.CSharp.Method;
                    this.CheckHeaderParams(element, method.Parameters, documentation);
                    this.CheckHeaderReturnValue(element, method.ReturnType, documentation);
                }
                else if (element.ElementType == ElementType.Constructor)
                {
                    Constructor constructor = element as Constructor;
                    this.CheckHeaderParams(element, constructor.Parameters, documentation);
                    this.CheckConstructorSummaryText(constructor, documentation);
                }
                else if (element.ElementType == ElementType.Delegate)
                {
                    Microsoft.StyleCop.CSharp.Delegate delegate2 = element as Microsoft.StyleCop.CSharp.Delegate;
                    this.CheckHeaderParams(element, delegate2.Parameters, documentation);
                    this.CheckHeaderReturnValue(element, delegate2.ReturnType, documentation);
                }
                else if (element.ElementType == ElementType.Indexer)
                {
                    Indexer indexer = element as Indexer;
                    this.CheckHeaderParams(element, indexer.Parameters, documentation);
                }
                else if (element.ElementType == ElementType.Property)
                {
                    this.CheckPropertyValueTag(element, documentation);
                    this.CheckPropertySummaryFormatting(element as Property, documentation);
                }
                else if (element.ElementType == ElementType.Destructor)
                {
                    this.CheckDestructorSummaryText((Destructor) element, documentation);
                }
                if ((((element.ElementType == ElementType.Method) || (element.ElementType == ElementType.Constructor)) || ((element.ElementType == ElementType.Delegate) || (element.ElementType == ElementType.Indexer))) || (((element.ElementType == ElementType.Class) || (element.ElementType == ElementType.Struct)) || (element.ElementType == ElementType.Interface)))
                {
                    this.CheckForRepeatingComments(element, documentation);
                }
                if (((element.ElementType == ElementType.Class) || (element.ElementType == ElementType.Method)) || (((element.ElementType == ElementType.Delegate) || (element.ElementType == ElementType.Interface)) || (element.ElementType == ElementType.Struct)))
                {
                    this.CheckGenericTypeParams(element, documentation);
                }
            }
        }

        public override void PostAnalyze()
        {
            base.PostAnalyze();
            this.includedDocs = null;
        }

        public override void PreAnalyze()
        {
            base.PreAnalyze();
            this.includedDocs = null;
        }

        private bool ReplaceIncludeTagWithIncludedDocumentationContents(CsElement element, XmlNode documentationNode, CachedXmlDocument includedDocument, XmlNode includedDocumentationNode)
        {
            try
            {
                XmlNode refChild = documentationNode;
                foreach (XmlNode node2 in includedDocumentationNode.ChildNodes)
                {
                    XmlNode node3 = documentationNode.OwnerDocument.ImportNode(node2, true);
                    this.InsertIncludedDocumentationForNode(element, Path.GetDirectoryName(includedDocument.FilePath), node3);
                    documentationNode.ParentNode.InsertAfter(node3, refChild);
                    refChild = node3;
                }
                documentationNode.ParentNode.RemoveChild(documentationNode);
            }
            catch (XmlException)
            {
                return false;
            }
            return true;
        }

        public override ICollection<IPropertyControlPage> SettingsPages
        {
            get
            {
                return new IPropertyControlPage[] { new CompanyInformation(this) };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AnalyzerSettings
        {
            public bool IgnorePrivates;
            public bool IgnoreInternals;
            public bool RequireFields;
        }

        private class CachedXmlDocument
        {
            private XmlDocument document;
            private string filePath;

            public CachedXmlDocument(string filePath, XmlDocument document)
            {
                this.filePath = filePath;
                this.document = document;
            }

            public XmlDocument Document
            {
                get
                {
                    return this.document;
                }
            }

            public string FilePath
            {
                get
                {
                    return this.filePath;
                }
            }
        }
    }
}

