//-----------------------------------------------------------------------
// <copyright file="DocumentationRules.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.
// </copyright>
// <license>
//   This source code is subject to terms and conditions of the Microsoft 
//   Public License. A copy of the license can be found in the License.html 
//   file at the root of this distribution. If you cannot locate the  
//   Microsoft Public License, please send an email to dlr@microsoft.com. 
//   By using this source code in any fashion, you are agreeing to be bound 
//   by the terms of the Microsoft Public License. You must not remove this 
//   notice, or any other, from this software.
// </license>
//-----------------------------------------------------------------------
namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml;
    using System.Xml.XPath;
    using Microsoft.StyleCop;

    /// <summary>
    /// Checks element documentation for the correct contents.
    /// </summary>
    [SourceAnalyzer(typeof(CsParser))]
    public class DocumentationRules : SourceAnalyzer
    {
        #region Internal Constants

        /// <summary>
        /// The name of the property which ignores private elements.
        /// </summary>
        internal const string IgnorePrivates = "IgnorePrivates";

        /// <summary>
        /// The name of the property which ignores internal elements.
        /// </summary>
        internal const string IgnoreInternals = "IgnoreInternals";

        /// <summary>
        /// The name of the property which checks for headers on fields.
        /// </summary>
        internal const string IncludeFieldsProperty = "IncludeFields";

        /// <summary>
        /// The name of the property contains the company name.
        /// </summary>
        internal const string CompanyNameProperty = "CompanyName";

        /// <summary>
        /// The name of the property which contains the copyright.
        /// </summary>
        internal const string CopyrightProperty = "Copyright";
        
        /// <summary>
        /// The default value of the property which ignores private elements.
        /// </summary>
        internal const bool IgnorePrivatesDefaultValue = false;

        /// <summary>
        /// The default value of the property which ignores internal elements.
        /// </summary>
        internal const bool IgnoreInternalsDefaultValue = false;

        /// <summary>
        /// The default value of the property which checks for headers on fields.
        /// </summary>
        internal const bool IncludeFieldsDefaultValue = true;

        #endregion Internal Constants

        #region Private Constants

        /// <summary>
        /// A regular expression to match a type reference included within a cref attribute.
        /// </summary>
        /// <remarks>
        /// {0}: namespace
        /// {1}: type name
        /// {2}: generic parameters regex
        /// {3}: number of generic parameters
        /// </remarks>
        private const string CrefRegex =
            @"(?'see'<see\s+cref\s*=\s*"")?" + // Optionally matches '<see cref="'
            @"(((({0})?{1})(?(see)({2})))" + // Matches 'type', or, if <see> tag is included, matches 'type&lt;T,S&gt;', 'type&lt;T,S>', or 'type{T,S}'
                @"|" +
                @"(?(see)T:(({0})?{1}){3}))" + // Matches T:type'N, only if <see> tag is included.
            @"(?(see)(""\s*(/>|>[\w\s]*</see>)))"; // Optionally matches '"/>' or '">some text</see>'

        /// <summary>
        /// A regular expression to match the generic parameters list for a type.
        /// </summary>
        private const string CrefGenericParamsRegex =
            @"(\s*(<|&lt;)\s*{0}\s*(>|&gt;))|(\s*{{\s*{0}\s*}})";

        /// <summary>
        /// Various version of the @ character.
        /// </summary>
        private static readonly int[] CopyrightCharTable = new int[]
            {
                169, 65533
            };

        #endregion Private Constants

        #region Private Fields

        /// <summary>
        /// Dictionary of external documentation which has been included into the code being analyzed.
        /// </summary>
        private Dictionary<string, CachedXmlDocument> includedDocs;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the DocumentationRules class.
        /// </summary>
        public DocumentationRules()
        {
        }

        #endregion Public Constructors

        #region Public Override Properties

        /// <summary>
        /// Gets the property pages to expose on the StyleCop settings dialog for this analyzer.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The form will be disposed by the caller.")]
        public override ICollection<IPropertyControlPage> SettingsPages
        {
            get
            {
                return new IPropertyControlPage[] { new CompanyInformation(this) };
            }
        }

        #endregion Public Override Properties

        #region Public Override Methods

        /// <summary>
        /// Checks the element headers within the given document.
        /// </summary>
        /// <param name="document">The document to check.</param>
        public override void AnalyzeDocument(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");

            CsDocument csdocument = (CsDocument)document;

            if (csdocument.RootElement != null && !csdocument.RootElement.Generated)
            {
                this.CheckElementDocumentation(csdocument);
                this.CheckFileHeader(csdocument);
                this.CheckSingleLineComments(csdocument.RootElement);
            }
        }

        /// <summary>
        /// Called before an analysis run is initiated.
        /// </summary>
        public override void PreAnalyze()
        {
            base.PreAnalyze();
            this.includedDocs = null;
        }

        /// <summary>
        /// Called after an analysis run is completed.
        /// </summary>
        public override void PostAnalyze()
        {
            base.PostAnalyze();
            this.includedDocs = null;
        }

        #endregion Public Override Methods

        #region Private Static Methods

        /// <summary>
        /// Exracts the type parameters from a generic type string.
        /// </summary>
        /// <param name="name">The generic type string.</param>
        /// <returns>Returns the list of type parameters, if any.</returns>
        private static List<string> ExtractGenericTypeList(string name)
        {
            Param.AssertValidString(name, "name");

            List<string> types = null;

            // If the name starts with "operator", then this is an operator overload and
            // by definition cannot be a generic method.
            const int OperatorKeywordLength = 8;
            if (!name.StartsWith("operator", StringComparison.Ordinal) ||
                name.Length <= OperatorKeywordLength ||
                !char.IsWhiteSpace(name[OperatorKeywordLength]))
            {
                // If the item contains a dot, that means this is an implicit interface
                // implentation, and the part of the name we care about begins past the dot.
                int start = name.LastIndexOf(".", StringComparison.Ordinal);
                if (start == -1)
                {
                    start = 0;
                }

                // Determine whether this is a generic type.
                int index = name.IndexOf("<", start, StringComparison.Ordinal);
                if (index != -1)
                {
                    types = new List<string>();

                    // Get each of the types.
                    while (true)
                    {
                        int comma = name.IndexOf(",", index + 1, StringComparison.Ordinal);
                        if (comma != -1)
                        {
                            string type = name.Substring(index + 1, comma - index - 1);
                            types.Add(type.Trim());
                            index = comma;
                        }
                        else
                        {
                            int closeBracket = name.IndexOf(">", StringComparison.Ordinal);
                            if (closeBracket != -1)
                            {
                                string type = name.Substring(index + 1, closeBracket - index - 1);
                                types.Add(type.Trim());
                            }

                            break;
                        }
                    }
                }
            }

            return types;
        }

        /// <summary>
        /// Determines whether the given xml header is empty.
        /// </summary>
        /// <param name="token">The xml header line token.</param>
        /// <returns>Returns true if the header is empty.</returns>
        private static bool IsXmlHeaderLineEmpty(CsToken token)
        {
            Param.AssertNotNull(token, "token");
            Debug.Assert(token.CsTokenType == CsTokenType.XmlHeaderLine, "The token should be an xml header line");

            int slashCount = 0;
            for (int i = 0; i < token.Text.Length; ++i)
            {
                char character = token.Text[i];

                if (slashCount < 3)
                {
                    if (character == '/')
                    {
                        ++slashCount;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (!char.IsWhiteSpace(character))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the given element is a non-public, static extern element with a DllImport attribute.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <returns>Returns true if the element is a non-public, static extern element.</returns>
        private static bool IsNonPublicStaticExternDllImport(CsElement element)
        {
            Param.AssertNotNull(element, "element");

            // If the method is not public, then it is not a valid DllImport.
            if (element.ActualAccess == AccessModifierType.Public)
            {
                return false;
            }

            // If the method is not static and extern, then it is not a valid DllImport.
            if (!element.Declaration.ContainsModifier(CsTokenType.Static) ||
                !element.Declaration.ContainsModifier(CsTokenType.Extern))
            {
                return false;
            }

            // Look for a DllImport attribute.
            if (element.Attributes != null)
            {
                foreach (Attribute attribute in element.Attributes)
                {
                    if (attribute.AttributeExpressions != null)
                    {
                        foreach (Expression expression in attribute.AttributeExpressions)
                        {
                            AttributeExpression attributeExpression = expression as AttributeExpression;
                            if (attributeExpression != null)
                            {
                                foreach (Expression innerExpression in attributeExpression.ChildExpressions)
                                {
                                    MethodInvocationExpression innerMethod = innerExpression as MethodInvocationExpression;
                                    if (innerMethod != null)
                                    {
                                        if (innerMethod.Name != null)
                                        {
                                            if (innerMethod.Name.Tokens.MatchTokens("DllImport") ||
                                                innerMethod.Name.Tokens.MatchTokens("DllImportAttribute") ||
                                                innerMethod.Name.Tokens.MatchTokens("System", ".", "Runtime", ".", "InteropServices", ".", "DllImport") ||
                                                innerMethod.Name.Tokens.MatchTokens("System", ".", "Runtime", ".", "InteropServices", ".", "DllImportAttribute"))
                                            {
                                                // The method is a public, static, extern DllImport.
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Extracts the node at the given path from the given document.
        /// </summary>
        /// <param name="document">The included documentation object.</param>
        /// <param name="xpath">The xpath pointing to the documentation node to load from the file.</param>
        /// <returns>Returns the included documentation node.</returns>
        private static XmlNode ExtractDocumentationNodeFromIncludedFile(XmlDocument document, string xpath)
        {
            Param.AssertNotNull(document, "document");
            Param.AssertValidString(xpath, "xpath");

            XmlNode node = null;

            // Navigate the xpath to find the documentation node.
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

        /// <summary>
        /// Extracts the file and path information from an 'include' tag.
        /// </summary>
        /// <param name="documentationNode">The include tag node.</param>
        /// <param name="file">Returns the file information.</param>
        /// <param name="path">Returns the path information.</param>
        private static void ExtractIncludeTagFileAndPath(XmlNode documentationNode, out string file, out string path)
        {
            Param.AssertNotNull(documentationNode, "documentationNode");

            // Extract the file and path.
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

        /// <summary>
        /// Loads an Xml documentation file from the disk.
        /// </summary>
        /// <param name="path">The full path to the file to load.</param>
        /// <returns>Returns the document or null if it could not be loaded.</returns>
        private static CachedXmlDocument LoadDocFileFromDisk(string path)
        {
            Param.AssertValidString(path, "path");

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

        /// <summary>
        /// Determines whether to reference the set accessor within the property's summary documentation.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="setAccessor">The set accessor.</param>
        /// <returns>Returns true to reference the set accessor in the summary documentation, or false to omit it.</returns>
        private static bool IncludeSetAccessorInDocumentation(Property property, Accessor setAccessor)
        {
            Param.AssertNotNull(property, "property");
            Param.AssertNotNull(setAccessor, "setAccessor");

            // If the set accessor has the same access modifier as the property, always include it in the documentation.
            // Accessors get 'private' access modifiers by default if no access modifier is defined, in which case they
            // default to having the access of their parent property. Also include documentation for the set accessor
            // if it appears to be private but it does not actually define the 'private' keyword.
            if (setAccessor.AccessModifier == property.AccessModifier ||
                (setAccessor.AccessModifier == AccessModifierType.Private && !setAccessor.Declaration.ContainsModifier(CsTokenType.Private)))
            {
                return true;
            }

            // If the set accessor has internal access, and the property also has internal or protected internal access, 
            // then include the set accessor in the docs since it effectively has the same access as the overall property.
            if (setAccessor.AccessModifier == AccessModifierType.Internal &&
                (property.ActualAccess == AccessModifierType.Internal || property.ActualAccess == AccessModifierType.ProtectedAndInternal))
            {
                return true;
            }

            // If the property is effectively private (contained within a private class), and the set accessor has any access modifier other than private, then
            // include the set accessor in the documentation. Within a private class, other access modifiers on the set accessor are meaningless.
            if (property.ActualAccess == AccessModifierType.Private && !setAccessor.Declaration.ContainsModifier(CsTokenType.Private))
            {
                return true;
            }

            // If the set accessor has protected access, then always include it in the docs since it will be visible to any
            // class that inherits from this class.
            if (setAccessor.AccessModifier == AccessModifierType.Protected || setAccessor.AccessModifier == AccessModifierType.ProtectedInternal)
            {
                return true;
            }
            
            // Otherwise, omit the set accessor from the documentation since its access is more restricted
            // than the access of the property.
            return false;
        }

        /*
        /// <summary>
        /// Determines whether to reference the set accessor within the property's summary documentation.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="setAccessor">The set accessor.</param>
        /// <returns>Returns true to reference the set accessor in the summary documentation, or false to omit it.</returns>
        private static bool IncludeSetAccessorInDocumentation(Property property, Accessor setAccessor)
        {
            Param.AssertNotNull(property, "property");
            Param.AssertNotNull(setAccessor, "setAccessor");

            // If the set accessor has the same access modifier as the property, always include it in the documentation.
            // Accessors get 'private' access modifiers by default if no access modifier is defined, in which case they
            // default to having the access of their parent property. Also include documentation for the set accessor
            // if it appears to be private but it does not actually define the 'private' keyword.
            if (setAccessor.AccessModifier == property.AccessModifier ||
                (setAccessor.AccessModifier == AccessModifierType.Private && !setAccessor.Declaration.ContainsModifier(CsTokenType.Private)))
            {
                return true;
            }

            // If the property is visible externally, then only include the set accessor in the documentation if 
            // it is marked as public or protected.
            if (property.ActualAccess == AccessModifierType.Public ||
                property.ActualAccess == AccessModifierType.Protected ||
                property.ActualAccess == AccessModifierType.ProtectedInternal)
            {
                if (setAccessor.AccessModifier == AccessModifierType.Public ||
                    setAccessor.AccessModifier == AccessModifierType.Protected ||
                    setAccessor.AccessModifier == AccessModifierType.ProtectedInternal)
                {
                    return true;
                }
            }

            // If the property is internal, the include the set accessor in the docs unless it contains the private keyword.
            if (property.ActualAccess == AccessModifierType.Internal || property.ActualAccess == AccessModifierType.ProtectedAndInternal)
            {
                if (!setAccessor.Declaration.ContainsModifier(CsTokenType.Private))
                {
                    return true;
                }
            }

            // If the property is private, always include the set accessor in the docs.
            if (property.ActualAccess == AccessModifierType.Private)
            {
                return true;
            }

            // Otherwise, do not include the set accessor in the docs.
            return false;
        }
        */

        /// <summary>
        /// Builds a regular expression that can be used to validate the name of the given type when 
        /// used within a documentation cref attribute.
        /// </summary>
        /// <param name="type">The type to match against.</param>
        /// <returns>Returns the regular expression object.</returns>
        private static string BuildCrefValidationStringForType(ClassBase type)
        {
            Param.AssertNotNull(type, "type");

            // Determine whether the type is generic.
            string typeName = type.Declaration.Name;
            string[] genericParams = null;
            string genericParametersRegex = null;

            int index = typeName.IndexOf('<');
            if (index > 0)
            {
                // Get the generic types from the type name.
                genericParams = ExtractGenericParametersFromType(typeName, index);
                if (genericParams != null && genericParams.Length > 0)
                {
                    genericParametersRegex = BuildGenericParametersRegex(genericParams);
                }

                // Remove the generic brackets from the type name.
                typeName = typeName.Substring(0, index);
            }

            // Also capture the fully qualified name of the type, without generic parameters included.
            string namespaceRegex = BuildNamespaceRegex(type);

            // Build the regex string to match all possible formats for the type name.
            return string.Format(
                CultureInfo.InvariantCulture,
                CrefRegex,
                namespaceRegex,
                typeName,
                genericParametersRegex == null ? string.Empty : genericParametersRegex,
                genericParams == null ? string.Empty : "`" + genericParams.Length.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets a string to match the namespace of the type in a format that can be inserted into a regular expression for matching.
        /// </summary>
        /// <param name="type">The type to match.</param>
        /// <returns>Returns the namespace string.</returns>
        private static string BuildNamespaceRegex(ClassBase type)
        {
            Param.AssertNotNull(type, "type");

            // The fully-qualified name of a type should always begin with Root. This part should be ignored.
            string fullyQualifiedName = type.FullyQualifiedName;
            Debug.Assert(fullyQualifiedName.StartsWith("Root.", StringComparison.Ordinal), "The fully qualified name of a type should start with Root.");

            StringBuilder namespaceRegex = new StringBuilder();

            int start = 5;

            // Start at position 5 to skip past the 'Root.' prefix.
            for (int i = start; i < fullyQualifiedName.Length; ++i)
            {
                if (fullyQualifiedName[i] == '.')
                {
                    // Since a dot is a special character in regex syntax, we need to escape this character.
                    if (namespaceRegex.Length > 0)
                    {
                        namespaceRegex.Append("\\.");
                    }

                    namespaceRegex.Append(fullyQualifiedName.Substring(start, i - start));

                    start = i + 1;
                }
                else if (fullyQualifiedName[i] == '<')
                {
                    // Stop if we get to an opening generic bracket.
                    break;
                }
            }

            if (namespaceRegex.Length > 0)
            {
                namespaceRegex.Append("\\.");
            }

            return namespaceRegex.ToString();
        }

        /// <summary>
        /// Builds a regex matching string to match the given generic parameter list.
        /// </summary>
        /// <param name="genericParams">The list of generic parameters.</param>
        /// <returns>Returns the regex string.</returns>
        private static string BuildGenericParametersRegex(string[] genericParams)
        {
            Param.AssertNotNull(genericParams, "genericParams");

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < genericParams.Length; ++i)
            {
                builder.Append(genericParams[i]);
                if (i < genericParams.Length - 1)
                {
                    builder.Append("\\s*,\\s*");
                }
            }

            return string.Format(CultureInfo.InvariantCulture, CrefGenericParamsRegex, builder.ToString());
        }

        /// <summary>
        /// Extracts the collection of generic parameters from the given type name.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="index">The index of the opening generic bracket within the type name.</param>
        /// <returns>Returns the array of type names.</returns>
        private static string[] ExtractGenericParametersFromType(string typeName, int index)
        {
            Param.AssertValidString(typeName, "typeName");
            Param.AssertValueBetween(index, 0, typeName.Length - 1, "index");

            // Create an array containing the generic type parameters.
            List<string> genericTypes = new List<string>();
            for (int i = index + 1; i < typeName.Length; ++i)
            {
                StringBuilder builder = new StringBuilder();
                for (; i < typeName.Length; ++i)
                {
                    if (typeName[i] == '>' || typeName[i] == ',' || char.IsWhiteSpace(typeName[i]))
                    {
                        if (builder.Length > 0)
                        {
                            genericTypes.Add(builder.ToString());
                        }

                        break;
                    }
                    else
                    {
                        builder.Append(typeName[i]);

                        if (i == typeName.Length - 1)
                        {
                            genericTypes.Add(builder.ToString());
                        }
                    }
                }

                if (typeName[i] == '>')
                {
                    break;
                }
            }

            return genericTypes.ToArray();
        }

        /// <summary>
        /// Gets the expected summary text regex for a constructor, based on the type of the constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="type">The type of the element containing the constructor.</param>
        /// <param name="typeRegex">The regular expression for matching the type name.</param>
        /// <returns>Returns the expected summary text regex.</returns>
        private static string GetExpectedSummaryTextForConstructorType(Constructor constructor, string type, string typeRegex)
        {
            Param.AssertNotNull(constructor, "constructor");
            Param.AssertValidString(type, "type");
            Param.AssertValidString(typeRegex, "typeRegex");

            if (constructor.Declaration.ContainsModifier(CsTokenType.Static))
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    CachedCodeStrings.HeaderSummaryForStaticConstructor,
                    typeRegex,
                    type);
            }
            else if (constructor.AccessModifier == AccessModifierType.Private &&
                (constructor.Parameters == null || constructor.Parameters.Count == 0))
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    CachedCodeStrings.HeaderSummaryForPrivateInstanceConstructor,
                    typeRegex,
                    type);
            }
            else
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    CachedCodeStrings.HeaderSummaryForInstanceConstructor,
                    typeRegex,
                    type);
            }
        }

        /// <summary>
        /// Gets the example expected summary text for a constructor, based on the type of the constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="type">The type of the element containing the constructor.</param>
        /// <returns>Returns the example summary text.</returns>
        private static string GetExampleSummaryTextForConstructorType(Constructor constructor, string type)
        {
            Param.AssertNotNull(constructor, "constructor");
            Param.AssertValidString(type, "type");

            if (constructor.Declaration.ContainsModifier(CsTokenType.Static))
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    CachedCodeStrings.ExampleHeaderSummaryForStaticConstructor,
                    type);
            }
            else if (constructor.AccessModifier == AccessModifierType.Private &&
                (constructor.Parameters == null || constructor.Parameters.Count == 0))
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    CachedCodeStrings.ExampleHeaderSummaryForPrivateInstanceConstructor,
                    type);
            }
            else
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    CachedCodeStrings.ExampleHeaderSummaryForInstanceConstructor,
                    type);
            }
        }

        /// <summary>
        /// Gets the expected summary text regex for a destructor.
        /// </summary>
        /// <param name="typeRegex">The regular expression for matching the type name.</param>
        /// <returns>Returns the expected summary text regex.</returns>
        private static string GetExpectedSummaryTextForDestructor(string typeRegex)
        {
            Param.AssertValidString(typeRegex, "typeRegex");

            return string.Format(
                CultureInfo.InvariantCulture,
                CachedCodeStrings.HeaderSummaryForDestructor,
                typeRegex);
        }

        /// <summary>
        /// Gets the example expected summary text for a destructor.
        /// </summary>
        /// <returns>Returns the example summary text.</returns>
        private static string GetExampleSummaryTextForDestructor()
        {
            return CachedCodeStrings.ExampleHeaderSummaryForDestructor;
        }

        /// <summary>
        /// Matches the two copyright text strings to determine if they are the same.
        /// </summary>
        /// <param name="copyright1">The first copyright string.</param>
        /// <param name="copyright2">The second copyright string.</param>
        /// <returns>Returns true if they are the same; false otherwise.</returns>
        private static bool MatchCopyrightText(string copyright1, string copyright2)
        {
            Param.Ignore(copyright1, copyright2);

            if (copyright1 == null || copyright2 == null)
            {
                return copyright1 == null && copyright2 == null;
            }

            if (copyright1.Length != copyright2.Length)
            {
                return false;
            }

            for (int i = 0; i < copyright1.Length; ++i)
            {
                char char1 = copyright1[i];
                char char2 = copyright2[i];

                if (char1 != char2)
                {
                    if (!CharacterIsCopyright(char1) || !CharacterIsCopyright(char2))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the given character is a copyright.
        /// </summary>
        /// <param name="character">The character to check.</param>
        /// <returns>Returns true if the character is a copyright; false otherwise.</returns>
        private static bool CharacterIsCopyright(char character)
        {
            Param.Ignore(character);

            for (int i = 0; i < CopyrightCharTable.Length; ++i)
            {
                if ((int)character == CopyrightCharTable[i])
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Private Static Methods

        #region Private Methods

        /// <summary>
        /// Checks the contents of the documentation for the elements in the document.
        /// </summary>
        /// <param name="document">The document to check.</param>
        private void CheckElementDocumentation(CsDocument document)
        {
            Param.AssertNotNull(document, "document");

            // Get the settings.
            AnalyzerSettings settings = new AnalyzerSettings();
            settings.IgnorePrivates = DocumentationRules.IgnorePrivatesDefaultValue;
            settings.IgnoreInternals = DocumentationRules.IgnoreInternalsDefaultValue;
            settings.RequireFields = DocumentationRules.IncludeFieldsDefaultValue;

            if (document.Settings != null)
            {
                BooleanProperty setting = document.Settings.GetAddInSetting(this, DocumentationRules.IgnorePrivates) as BooleanProperty;
                if (setting != null)
                {
                    settings.IgnorePrivates = setting.Value;
                }

                setting = document.Settings.GetAddInSetting(this, DocumentationRules.IgnoreInternals) as BooleanProperty;
                if (setting != null)
                {
                    settings.IgnoreInternals = setting.Value;
                }

                setting = document.Settings.GetAddInSetting(this, DocumentationRules.IncludeFieldsProperty) as BooleanProperty;
                if (setting != null)
                {
                    settings.RequireFields = setting.Value;
                }
            }

            ((CsDocument)document).WalkDocument<AnalyzerSettings>(
                new CodeWalkerElementVisitor<AnalyzerSettings>(this.CheckDocumentationForElement),
                settings);
        }

        /// <summary>
        /// Processes the given element.
        /// </summary>
        /// <param name="element">The element being visited.</param>
        /// <param name="parentElement">The parent element, if any.</param>
        /// <param name="settings">The analyzer settings.</param>
        /// <returns>Returns true to continue, or false to stop the walker.</returns>
        private bool CheckDocumentationForElement(
            CsElement element,
            CsElement parentElement,
            AnalyzerSettings settings)
        {
            Param.AssertNotNull(element, "element");
            Param.Ignore(parentElement);
            Param.Ignore(settings);

            if (this.Cancel)
            {
                return false;
            }
            
            if (!element.Generated)
            {
                if (element.ElementType == ElementType.Class ||
                    element.ElementType == ElementType.Interface ||
                    element.ElementType == ElementType.Struct)
                {
                    ClassBase classElement = element as ClassBase;
                    Debug.Assert(classElement != null, "The element is not a class.");

                    this.CheckClassElementHeader(classElement, settings);
                }
                else if (element.ElementType == ElementType.Enum ||
                    element.ElementType == ElementType.Delegate ||
                    element.ElementType == ElementType.Event ||
                    element.ElementType == ElementType.Property ||
                    element.ElementType == ElementType.Indexer ||
                    element.ElementType == ElementType.Method ||
                    element.ElementType == ElementType.Constructor ||
                    element.ElementType == ElementType.Destructor ||
                    element.ElementType == ElementType.Field)
                {
                    this.CheckHeader(element, settings, false);
                }

                if (element.ElementType == ElementType.Enum)
                {
                    this.CheckEnumHeaders(element as Microsoft.StyleCop.CSharp.Enum, settings);
                }

                // Check the comments within the element, only for
                // elements which contain statements.
                if (element.ElementType == ElementType.Accessor ||
                    element.ElementType == ElementType.Constructor ||
                    element.ElementType == ElementType.Destructor ||
                    element.ElementType == ElementType.Method)
                {
                    this.CheckElementComments(element);
                }
            }

            return true;
        }

        /// <summary>
        /// Checks the Xml header block of the given class for consistency with the class.
        /// </summary>
        /// <param name="classElement">The element to parse.</param>
        /// <param name="settings">The analyzer settings.</param>
        private void CheckClassElementHeader(ClassBase classElement, AnalyzerSettings settings)
        {
            Param.AssertNotNull(classElement, "classElement");
            Param.Ignore(settings);

            AnalyzerSettings adjustedSettings = settings;
            adjustedSettings.RequireFields = false;

            if (classElement.Declaration.ContainsModifier(CsTokenType.Partial))
            {
                // This is a partial element. Check for the possible partial element header types.
                this.CheckHeader(classElement, adjustedSettings, true);
            }
            else
            {
                // Just perform the regular set of checks on the header.
                this.CheckHeader(classElement, adjustedSettings, false);
            }
        }

        /// <summary>
        /// Checks the Xml header block for consistency with the element it belongs to.
        /// </summary>
        /// <param name="element">The element to parse.</param>
        /// <param name="settings">The analyzer settings.</param>
        /// <param name="partialElement">Indicates whether the element has the partial attribute.</param>
        private void CheckHeader(CsElement element, AnalyzerSettings settings, bool partialElement)
        {
            Param.AssertNotNull(element, "element");
            Param.Ignore(settings);
            Param.Ignore(partialElement);

            // See whether this element has a header at all.
            if (null == element.Header || element.Header.Text.Length == 0)
            {
                // Empty or missing header is a violation if:
                // 1. The element is a class, interface, or struct
                // 2. The element is a field and the RequireFields flag is set
                // 3. The element is not private, or the element is private but the IgnorePrivates flag is not set
                // 4. The element is not internal, or the element is internal but the IgnoreInternals flag is not set
                if ((element.ElementType == ElementType.Class || element.ElementType == ElementType.Interface || element.ElementType == ElementType.Struct) ||
                    ((settings.RequireFields || element.ElementType != ElementType.Field) &&
                     (!settings.IgnorePrivates || element.Declaration.AccessModifierType != AccessModifierType.Private) &&
                     (!settings.IgnoreInternals || element.Declaration.AccessModifierType != AccessModifierType.Internal)))
                {
                    if (partialElement)
                    {
                        this.AddViolation(element, Rules.PartialElementsMustBeDocumented, element.FriendlyTypeText);
                    }
                    else
                    {
                        // We do not require headers for non-public static extern methods.
                        if (!IsNonPublicStaticExternDllImport(element))
                        {
                            this.AddViolation(element, Rules.ElementsMustBeDocumented, element.FriendlyTypeText);
                        }
                    }
                }
            }
            else
            {
                this.ParseHeader(element, element.Header, element.LineNumber, partialElement);
            }
        }

        /// <summary>
        /// Checks the Xml header block for consistency with the element it belongs to.
        /// </summary>
        /// <param name="element">The element to parse.</param>
        /// <param name="settings">The analyzer settings.</param>
        private void CheckEnumHeaders(Microsoft.StyleCop.CSharp.Enum element, AnalyzerSettings settings)
        {
            Param.AssertNotNull(element, "element");
            Param.Ignore(settings);
            
            foreach (EnumItem item in element.Items)
            {
                if (null == item.Header || item.Header.Text.Length == 0)
                {
                    if ((!settings.IgnorePrivates || element.Declaration.AccessModifierType != AccessModifierType.Private) &&
                        (!settings.IgnoreInternals || element.Declaration.AccessModifierType != AccessModifierType.Internal))
                    {
                        this.AddViolation(item, Rules.EnumerationItemsMustBeDocumented);
                    }
                }
                else
                {
                    this.ParseHeader(item, item.Header, item.LineNumber, false);
                }
            }
        }

        /// <summary>
        /// Checks the statements for single line comments that start with three slashes.
        /// </summary>
        /// <param name="element">The element to check.</param>
        private void CheckElementComments(CsElement element)
        {
            Param.AssertNotNull(element, "element");

            foreach (CsToken token in element.Tokens)
            {
                if (token.CsTokenType == CsTokenType.XmlHeader ||
                    token.CsTokenType == CsTokenType.XmlHeaderLine)
                {
                    this.AddViolation(
                        element, 
                        token.LineNumber,
                        Rules.SingleLineCommentsMustNotUseDocumentationStyleSlashes);
                }
            }
        }

        /// <summary>
        /// Checks the document for single line comments that start with three slashes.
        /// </summary>
        /// <param name="root">The document root.</param>
        private void CheckSingleLineComments(DocumentRoot root)
        {
            Param.AssertNotNull(root, "root");

            if (root.Tokens != null)
            {
                foreach (CsToken token in root.Tokens)
                {
                    if (token.CsTokenType == CsTokenType.SingleLineComment)
                    {
                        if (token.Text.StartsWith("///", StringComparison.Ordinal) &&
                            (token.Text.Length == 3 || (token.Text.Length > 3 && token.Text[3] != '/')))
                        {
                            this.AddViolation(
                                token.FindParentElement(),
                                token.LineNumber,
                                Rules.SingleLineCommentsMustNotUseDocumentationStyleSlashes);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses the contents of the header for validity.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="header">The header.</param>
        /// <param name="lineNumber">The line number that the header element begins on.</param>
        /// <param name="partialElement">Indicates whether the element has the partial attribute.</param>
        private void ParseHeader(CsElement element, XmlHeader header, int lineNumber, bool partialElement)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(header, "header");
            Param.AssertGreaterThanZero(lineNumber, "lineNumber");
            Param.Ignore(partialElement);

            // Load this into an xml document.
            XmlDocument rawDocs = null;
            XmlDocument formattedDocs = null;

            this.LoadHeaderIntoDocuments(element, header, lineNumber, out rawDocs, out formattedDocs);

            if (rawDocs != null && formattedDocs != null)
            {
                // Insert any documentation present in 'include' tags.
                if (this.InsertIncludedDocumentation(element, Path.GetDirectoryName(element.Document.SourceCode.Path), formattedDocs))
                {
                    this.CheckForBlankLinesInDocumentationHeader(element, header);
                    this.CheckHeaderSummary(element, lineNumber, partialElement, formattedDocs);

                    // Check element parameters and return types.
                    if (element.ElementType == ElementType.Method)
                    {
                        Method item = element as Method;
                        this.CheckHeaderParams(element, item.Parameters, formattedDocs);
                        this.CheckHeaderReturnValue(element, item.ReturnType, formattedDocs);
                    }
                    else if (element.ElementType == ElementType.Constructor)
                    {
                        Constructor item = element as Constructor;
                        this.CheckHeaderParams(element, item.Parameters, formattedDocs);
                        this.CheckConstructorSummaryText(item, formattedDocs);
                    }
                    else if (element.ElementType == ElementType.Delegate)
                    {
                        Microsoft.StyleCop.CSharp.Delegate item = element as Microsoft.StyleCop.CSharp.Delegate;
                        this.CheckHeaderParams(element, item.Parameters, formattedDocs);
                        this.CheckHeaderReturnValue(element, item.ReturnType, formattedDocs);
                    }
                    else if (element.ElementType == ElementType.Indexer)
                    {
                        Indexer item = element as Indexer;
                        this.CheckHeaderParams(element, item.Parameters, formattedDocs);
                    }
                    else if (element.ElementType == ElementType.Property)
                    {
                        // Check value tags on properties.
                        this.CheckPropertyValueTag(element, formattedDocs);

                        // Check that the property summary starts with the correct text.
                        this.CheckPropertySummaryFormatting(element as Property, formattedDocs);
                    }
                    else if (element.ElementType == ElementType.Destructor)
                    {
                        this.CheckDestructorSummaryText((Destructor)element, formattedDocs);
                    }

                    // Check for repeating comments on all element types which can contain params or typeparams.
                    if (element.ElementType == ElementType.Method ||
                        element.ElementType == ElementType.Constructor ||
                        element.ElementType == ElementType.Delegate ||
                        element.ElementType == ElementType.Indexer ||
                        element.ElementType == ElementType.Class ||
                        element.ElementType == ElementType.Struct ||
                        element.ElementType == ElementType.Interface)
                    {
                        this.CheckForRepeatingComments(element, formattedDocs);
                    }

                    // Check generic type parameters.
                    if (element.ElementType == ElementType.Class ||
                        element.ElementType == ElementType.Method ||
                        element.ElementType == ElementType.Delegate ||
                        element.ElementType == ElementType.Interface ||
                        element.ElementType == ElementType.Struct)
                    {
                        this.CheckGenericTypeParams(element, formattedDocs);
                    }
                }
            }
        }

        /// <summary>
        /// Checks the summary tag from a header.
        /// </summary>
        /// <param name="element">The element containing the header.</param>
        /// <param name="lineNumber">The line number of the header.</param>
        /// <param name="partialElement">Indicates whether the element is a partial element.</param>
        /// <param name="formattedDocs">The formatted header documentation.</param>
        private void CheckHeaderSummary(CsElement element, int lineNumber, bool partialElement, XmlDocument formattedDocs)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertGreaterThanZero(lineNumber, "lineNumber");
            Param.Ignore(partialElement);
            Param.AssertNotNull(formattedDocs, "doc");

            XmlNode summary = formattedDocs.SelectSingleNode("root/summary");
            if (summary == null)
            {
                if (partialElement)
                {
                    summary = formattedDocs.SelectSingleNode("root/content");
                }
            }

            if (summary == null)
            {
                if (partialElement)
                {
                    this.AddViolation(element, lineNumber, Rules.PartialElementDocumentationMustHaveSummary);
                }
                else
                {
                    this.AddViolation(element, lineNumber, Rules.ElementDocumentationMustHaveSummary);
                }
            }
            else
            {
                if (summary.InnerText == null || summary.InnerText.Length == 0)
                {
                    if (partialElement)
                    {
                        this.AddViolation(element, lineNumber, Rules.PartialElementDocumentationMustHaveSummaryText);
                    }
                    else
                    {
                        this.AddViolation(element, lineNumber, Rules.ElementDocumentationMustHaveSummaryText);
                    }
                }
                else
                {
                    int i = 0;
                    for (i = 0; i < summary.InnerText.Length; ++i)
                    {
                        if (summary.InnerText[i] != '\r' &&
                            summary.InnerText[i] != '\n' &&
                            summary.InnerText[i] != '\t' &&
                            summary.InnerText[i] != ' ')
                        {
                            break;
                        }
                    }

                    string temp = summary.InnerText.Substring(i, summary.InnerText.Length - i);

                    if (temp.StartsWith("Summary description for", StringComparison.Ordinal))
                    {
                        this.AddViolation(element, lineNumber, Rules.ElementDocumentationMustNotHaveDefaultSummary);
                    }
                    else
                    {
                        this.CheckDocumentationValidity(element, lineNumber, summary, "summary");
                    }
                }
            }
        }

        /// <summary>
        /// Loads the header text for the element into a document and returns it.
        /// </summary>
        /// <param name="element">The element containing the header.</param>
        /// <param name="header">The header to load.</param>
        /// <param name="lineNumber">The line number that the header begins on.</param>
        /// <param name="rawDocs">Returns the docs with whitepace and newlines left in place.</param>
        /// <param name="formattedDocs">Returns the docs with newlines filtered out.</param>
        private void LoadHeaderIntoDocuments(CsElement element, XmlHeader header, int lineNumber, out XmlDocument rawDocs, out XmlDocument formattedDocs)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(header, "header");
            Param.AssertGreaterThanZero(lineNumber, "lineNumber");

            rawDocs = new XmlDocument();
            formattedDocs = new XmlDocument();

            try
            {
                string correctxml = "<root>" + header.RawText + "</root>";
                rawDocs.LoadXml(correctxml);

                correctxml = "<root>" + header.Text + "</root>";
                formattedDocs.LoadXml(correctxml);
            }
            catch (XmlException xmlex)
            {
                this.AddViolation(element, lineNumber, Rules.DocumentationMustContainValidXml, xmlex.Message);
                rawDocs = formattedDocs = null;
            }
        }

        /// <summary>
        /// Checks the documentation header to see whether it contains any blank lines.
        /// </summary>
        /// <param name="element">The element containing the header.</param>
        /// <param name="header">The documentation header.</param>
        private void CheckForBlankLinesInDocumentationHeader(CsElement element, XmlHeader header)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(header, "header");

            if (!element.Generated)
            {
                int blankLineCount = 0;
                for (Node<CsToken> tokenNode = header.ChildTokens.First; tokenNode != null && tokenNode != header.ChildTokens.Last.Next; tokenNode = tokenNode.Next)
                {
                    CsToken token = tokenNode.Value;

                    if (token.CsTokenType == CsTokenType.EndOfLine)
                    {
                        ++blankLineCount;

                        if (blankLineCount > 1)
                        {
                            this.AddViolation(element, token.LineNumber, Rules.DocumentationHeadersMustNotContainBlankLines);
                            break;
                        }
                    }
                    else if (token.CsTokenType == CsTokenType.XmlHeaderLine)
                    {
                        if (tokenNode == header.ChildTokens.First || tokenNode == header.ChildTokens.Last)
                        {
                            if (IsXmlHeaderLineEmpty(token))
                            {
                                this.AddViolation(element, token.LineNumber, Rules.DocumentationHeadersMustNotContainBlankLines);
                                break;
                            }
                        }
                        else if (!IsXmlHeaderLineEmpty(token))
                        {
                            blankLineCount = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the given documentation to ensure that it meets the documentation text rules.
        /// </summary>
        /// <param name="element">The element containing the documentation.</param>
        /// <param name="lineNumber">The line number that the element appears on.</param>
        /// <param name="documentationXml">The documentation text.</param>
        /// <param name="documentationType">The type of the documentation text.</param>
        private void CheckDocumentationValidity(CsElement element, int lineNumber, XmlNode documentationXml, string documentationType)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertGreaterThanZero(lineNumber, "lineNumber");
            Param.AssertNotNull(documentationXml, "documentationXml");
            Param.AssertValidString(documentationType, "documentationType");

            InvalidCommentType commentType = CommentVerifier.IsGarbageComment(documentationXml);

            if ((commentType & InvalidCommentType.Empty) != 0)
            {
                this.AddViolation(element, lineNumber, Rules.DocumentationTextMustNotBeEmpty, documentationType);
            }
            
            if ((commentType & InvalidCommentType.NoPeriod) != 0)
            {
                this.AddViolation(element, lineNumber, Rules.DocumentationTextMustEndWithAPeriod, documentationType);
            }
            
            if ((commentType & InvalidCommentType.NoCapitalLetter) != 0)
            {
                // If the return value documentation starts with "true" or "false", then
                // don't check that the comment starts with a capital letter, because we want to
                // allow this. For example, it is common to write return documentation such as:
                // "true if the method succeeds; false otherwise."
                if (!documentationType.Equals("return", StringComparison.Ordinal) ||
                    (!documentationXml.InnerText.StartsWith("true", StringComparison.Ordinal) &&
                     !documentationXml.InnerText.StartsWith("false", StringComparison.Ordinal)))
                {
                    this.AddViolation(element, lineNumber, Rules.DocumentationTextMustBeginWithACapitalLetter, documentationType);
                }
            }
            
            if ((commentType & InvalidCommentType.NoWhitespace) != 0)
            {
                this.AddViolation(element, lineNumber, Rules.DocumentationTextMustContainWhitespace, documentationType);
            }
            
            if ((commentType & InvalidCommentType.TooFewCharacters) != 0)
            {
                this.AddViolation(
                    element,
                    lineNumber,
                    Rules.DocumentationMustMeetCharacterPercentage,
                    documentationType,
                    CommentVerifier.MinimumCharacterPercentage,
                    100 - CommentVerifier.MinimumCharacterPercentage);
            }
            
            if ((commentType & InvalidCommentType.TooShort) != 0)
            {
                this.AddViolation(
                    element,
                    lineNumber,
                    Rules.DocumentationTextMustMeetMinimumCharacterLength,
                    documentationType,
                    CommentVerifier.MinimumHeaderCommentLength);
            }
        }

        /// <summary>
        /// Checks a property element to ensure that the heade contains a 'value' tag.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <param name="formattedDocs">The formatted header documentation.</param>
        private void CheckPropertyValueTag(CsElement element, XmlDocument formattedDocs)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(formattedDocs, "formattedDocs");

            XmlNode node = formattedDocs.SelectSingleNode("root/value");
            if (node == null)
            {
                // A missing value tag is only an error if this property is public and protected.
                if (element.ActualAccess == AccessModifierType.Public ||
                    element.ActualAccess == AccessModifierType.ProtectedInternal ||
                    element.ActualAccess == AccessModifierType.Protected)
                {
                    this.AddViolation(element, Rules.PropertyDocumentationMustHaveValue);
                }
            }
            else if (node.InnerText == null || node.InnerText.Length == 0)
            {
                this.AddViolation(element, Rules.PropertyDocumentationMustHaveValue);
            }
            else
            {
                this.CheckDocumentationValidity(element, element.LineNumber, node, "value");
            }
        }

        /// <summary>
        /// Checks a property element to ensure that the header contains a 'value' tag.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <param name="formattedDocs">The formatted header documentation.</param>
        private void CheckPropertySummaryFormatting(Property property, XmlDocument formattedDocs)
        {
            Param.AssertNotNull(property, "property");
            Param.AssertNotNull(formattedDocs, "formattedDocs");

            // If the property is an override, skip this rule. Currently StyleCop cannot detect the proper summary documentation for
            // overridden properties, because this depends upon the property in the base class (whether it has a get accessor, set accessor, or both),
            // and currently stylecop has no access to this information. TODO: fix this when moving to new C# language service.
            if (!property.Declaration.ContainsModifier(CsTokenType.Override))
            {
                XmlNode node = formattedDocs.SelectSingleNode("root/summary");
                if (node != null)
                {
                    // Determine whether the property returns a bool.
                    bool returnsBoolean =
                        property.ReturnType.Text == "bool" ||
                        property.ReturnType.Text == "Boolean" ||
                        property.ReturnType.Text == "System.Boolean";

                    // Determine whether the property has a get accessor.
                    if (property.GetAccessor != null)
                    {
                        if (property.SetAccessor != null &&
                            IncludeSetAccessorInDocumentation(property, property.SetAccessor))
                        {
                            // There is a get and a set accessor.
                            string text = returnsBoolean ? CachedCodeStrings.HeaderSummaryForBooleanGetAndSetAccessor : CachedCodeStrings.HeaderSummaryForGetAndSetAccessor;
                            if (!node.InnerText.TrimStart().StartsWith(text, StringComparison.Ordinal))
                            {
                                this.AddViolation(property, Rules.PropertySummaryDocumentationMustMatchAccessors, text);
                            }
                        }
                        else
                        {
                            // There is only a get accessor.
                            string text = returnsBoolean ? CachedCodeStrings.HeaderSummaryForBooleanGetAccessor : CachedCodeStrings.HeaderSummaryForGetAccessor;
                            string summaryText = node.InnerText.TrimStart();
                            if (!summaryText.StartsWith(text, StringComparison.Ordinal))
                            {
                                this.AddViolation(property, Rules.PropertySummaryDocumentationMustMatchAccessors, text);
                            }

                            // Make sure that the summary does not start with "Gets or sets" since there is only a get accessor.
                            string getOrSetText = CachedCodeStrings.HeaderSummaryForGetAndSetAccessor;
                            if (summaryText.StartsWith(getOrSetText, StringComparison.Ordinal))
                            {
                                this.AddViolation(property, Rules.PropertySummaryDocumentationMustOmitSetAccessorWithRestrictedAccess, text);
                            }
                        }
                    }
                    else if (property.SetAccessor != null)
                    {
                        // There is only a set accessor.
                        string text = returnsBoolean ? CachedCodeStrings.HeaderSummaryForBooleanSetAccessor : CachedCodeStrings.HeaderSummaryForSetAccessor;
                        if (!node.InnerText.TrimStart().StartsWith(text, StringComparison.Ordinal))
                        {
                            this.AddViolation(property, Rules.PropertySummaryDocumentationMustMatchAccessors, text);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks a constructor to ensure that the summary text matches the expected text.
        /// </summary>
        /// <param name="constructor">The constructor to check.</param>
        /// <param name="formattedDocs">The formatted header documentation.</param>
        private void CheckConstructorSummaryText(Constructor constructor, XmlDocument formattedDocs)
        {
            Param.AssertNotNull(constructor, "constructor");
            Param.AssertNotNull(formattedDocs, "formattedDocs");

            XmlNode node = formattedDocs.SelectSingleNode("root/summary");
            if (node != null)
            {
                string summaryText = node.InnerXml.Trim();
                string type = constructor.Parent is Struct ? "struct" : "class";

                // Get a regex to match the type name.
                string typeRegex = BuildCrefValidationStringForType((ClassBase)constructor.FindParentElement());

                // Get the full expected summary text.
                string expectedRegex = GetExpectedSummaryTextForConstructorType(constructor, type, typeRegex);

                if (!Regex.IsMatch(summaryText, expectedRegex, RegexOptions.ExplicitCapture))
                {
                    this.AddViolation(
                        constructor, 
                        Rules.ConstructorSummaryDocumentationMustBeginWithStandardText, 
                        GetExampleSummaryTextForConstructorType(constructor, type));
                }
            }
        }

        /// <summary>
        /// Checks a destructor to ensure that the summary text matches the expected text.
        /// </summary>
        /// <param name="destructor">The destructor to check.</param>
        /// <param name="formattedDocs">The formatted header documentation.</param>
        private void CheckDestructorSummaryText(Destructor destructor, XmlDocument formattedDocs)
        {
            Param.AssertNotNull(destructor, "destructor");
            Param.AssertNotNull(formattedDocs, "formattedDocs");

            XmlNode node = formattedDocs.SelectSingleNode("root/summary");
            if (node != null)
            {
                string summaryText = node.InnerXml.Trim();

                // Get a regex to match the type name.
                string typeRegex = BuildCrefValidationStringForType((ClassBase)destructor.FindParentElement());

                // Get the full expected summary text.
                string expectedRegex = GetExpectedSummaryTextForDestructor(typeRegex);

                if (!Regex.IsMatch(summaryText, expectedRegex))
                {
                    this.AddViolation(
                        destructor, 
                        Rules.DestructorSummaryDocumentationMustBeginWithStandardText, 
                        GetExampleSummaryTextForDestructor());
                }
            }
        }

        /// <summary>
        /// Checks for comments that appear more than once in a header.
        /// </summary>
        /// <param name="element">The element containing the header.</param>
        /// <param name="formattedDocs">The formatted Xml documentation.</param>
        private void CheckForRepeatingComments(CsElement element, XmlDocument formattedDocs)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(formattedDocs, "formattedDocs");

            // Extract and trim all of the parts of the documentation.
            List<string> comments = new List<string>();
            XmlNode node = formattedDocs.SelectSingleNode("root/summary");
            if (node != null)
            {
                comments.Add(node.InnerXml.Trim());
            }

            node = formattedDocs.SelectSingleNode("root/returns");
            if (node != null)
            {
                comments.Add(node.InnerXml.Trim());
            }

            node = formattedDocs.SelectSingleNode("root/remarks");
            if (node != null)
            {
                comments.Add(node.InnerXml.Trim());
            }

            XmlNodeList nodes = formattedDocs.SelectNodes("root/param");
            if (nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode item in nodes)
                {
                    comments.Add(item.InnerXml.Trim());
                }
            }

            nodes = formattedDocs.SelectNodes("root/typeparam");
            if (nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode item in nodes)
                {
                    comments.Add(item.InnerXml.Trim());
                }
            }

            // Now check for matching comments.
            for (int i = 0; i < comments.Count; ++i)
            {
                if (comments[i].Length > 0)
                {
                    for (int j = i + 1; j < comments.Count; ++j)
                    {
                        if (string.Compare(comments[i], comments[j], StringComparison.Ordinal) == 0 &&
                            string.Compare(comments[i], CachedCodeStrings.ParameterNotUsed, StringComparison.Ordinal) != 0)
                        {
                            this.AddViolation(element, Rules.ElementDocumentationMustNotBeCopiedAndPasted, CachedCodeStrings.ParameterNotUsed);
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the parameters of a header for consistancy with the method the header belongs to.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <param name="parameters">The list of parameters in the element.</param>
        /// <param name="formattedDocs">The formatted Xml document that comprises the header.</param>
        private void CheckHeaderParams(CsElement element, ICollection<Parameter> parameters, XmlDocument formattedDocs)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(parameters, "parameters");
            Param.AssertNotNull(formattedDocs, "formattedDocs");
            
            XmlNodeList paramNodes = formattedDocs.SelectNodes("root/param");
            if (paramNodes == null || (paramNodes.Count == 0 && parameters.Count > 0))
            {
                this.AddViolation(element, Rules.ElementParametersMustBeDocumented);
            }
            else
            {
                // Go through each param and make sure there is a header for it.
                int i = 0;
                foreach (Parameter parameter in parameters)
                {
                    if (paramNodes.Count <= i)
                    {
                        this.AddViolation(element, Rules.ElementParametersMustBeDocumented);
                        break;
                    }
                    else
                    {
                        XmlNode paramNode = paramNodes[i];

                        XmlNode attrib = paramNode.Attributes.GetNamedItem("name");
                        if (attrib == null || attrib.Value.Length == 0)
                        {
                            this.AddViolation(element, Rules.ElementParameterDocumentationMustDeclareParameterName);
                        }
                        else
                        {
                            string parameterName = parameter.Name;
                            if (parameterName.StartsWith("@", StringComparison.Ordinal) && parameterName.Length > 1)
                            {
                                parameterName = parameterName.Substring(1, parameterName.Length - 1);
                            }

                            if (attrib.Value != parameterName)
                            {
                                this.AddViolation(element, Rules.ElementParameterDocumentationMustMatchElementParameters);
                            }
                        }
                    }

                    ++i;
                }

                // Make sure none of the parameters is empty.
                foreach (XmlNode paramNode in paramNodes)
                {
                    if (paramNode.InnerText == null || paramNode.InnerText.Length == 0)
                    {
                        this.AddViolation(element, Rules.ElementParameterDocumentationMustHaveText, paramNode.OuterXml);
                    }
                    else
                    {
                        this.CheckDocumentationValidity(element, element.LineNumber, paramNode, "param");
                    }
                }

                // Make sure there are not more param nodes than there are parameters.
                if (paramNodes.Count > i)
                {
                    this.AddViolation(element, Rules.ElementParameterDocumentationMustMatchElementParameters);
                }
            }
        }
        
        /// <summary>
        /// Checks for the presence of a return value tag on an element.
        /// </summary>
        /// <param name="element">The element to parse.</param>
        /// <param name="returnType">The return type.</param>
        /// <param name="formattedDocs">The formatted Xml document that comprises the header.</param>
        private void CheckHeaderReturnValue(CsElement element, TypeToken returnType, XmlDocument formattedDocs)
        {
            Param.AssertNotNull(element, "element");
            Param.Ignore(returnType);
            Param.AssertNotNull(formattedDocs, "formattedDocs");
            
            if (returnType != null)
            {
                XmlNode returnNode = formattedDocs.SelectSingleNode("root/returns");
                if (returnType.Text != "void")
                {
                    if (null == returnNode)
                    {
                        this.AddViolation(element, Rules.ElementReturnValueMustBeDocumented);
                    }
                    else if (returnNode.InnerText == null || returnNode.InnerText.Length == 0)
                    {
                        this.AddViolation(element, Rules.ElementReturnValueDocumentationMustHaveText);
                    }
                    else
                    {
                        this.CheckDocumentationValidity(element, element.LineNumber, returnNode, "return");
                    }
                }
                else
                {
                    if (null != returnNode)
                    {
                        this.AddViolation(element, Rules.VoidReturnValueMustNotBeDocumented);
                    }
                }
            }
        }

        /// <summary>
        /// Checks the generic type parameters of a header for consistancy with the item the header belongs to.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <param name="formattedDocs">The formatted Xml document that comprises the header.</param>
        private void CheckGenericTypeParams(CsElement element, XmlDocument formattedDocs)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(formattedDocs, "formattedDocs");

            List<string> types = DocumentationRules.ExtractGenericTypeList(element.Declaration.Name);
            if (types != null && types.Count > 0)
            {
                // Go through each type and make sure there is a header for it.
                XmlNodeList paramNodes = formattedDocs.SelectNodes("root/typeparam");
                if (paramNodes == null || paramNodes.Count == 0)
                {
                    // If this is a partial class and the header contains a content tag rather
                    // than a summary tag, then assume that the typeparams are documented on
                    // another part of the partial class, and ignore this.
                    bool isPartial = element.Declaration.ContainsModifier(CsTokenType.Partial);
                    if (!isPartial || formattedDocs.SelectSingleNode("root/summary") != null || formattedDocs.SelectSingleNode("root/content") == null)
                    {
                        if (isPartial)
                        {
                            // Output a special message for partial classes which explains about content tags.
                            this.AddViolation(element, Rules.GenericTypeParametersMustBeDocumentedPartialClass, element.FriendlyTypeText);
                        }
                        else
                        {
                            this.AddViolation(element, Rules.GenericTypeParametersMustBeDocumented, element.FriendlyTypeText);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < paramNodes.Count; ++i)
                    {
                        XmlNode paramNode = paramNodes[i];
                        if (types.Count <= i)
                        {
                            this.AddViolation(element, Rules.GenericTypeParameterDocumentationMustMatchTypeParameters, element.FriendlyTypeText);
                            break;
                        }
                        else
                        {
                            XmlNode attrib = paramNode.Attributes.GetNamedItem("name");
                            if (attrib == null || attrib.Value.Length == 0)
                            {
                                this.AddViolation(element, Rules.GenericTypeParameterDocumentationMustDeclareParameterName);
                            }
                            else if (attrib.Value != types[i])
                            {
                                this.AddViolation(element, Rules.GenericTypeParameterDocumentationMustMatchTypeParameters, element.FriendlyTypeText);
                                break;
                            }
                        }
                    }

                    // If the element has more parameters than param tags.
                    if (types.Count > paramNodes.Count)
                    {
                        this.AddViolation(element, Rules.GenericTypeParameterDocumentationMustMatchTypeParameters, element.FriendlyTypeText);
                    }

                    // Make sure none of the parameters is empty.
                    foreach (XmlNode paramNode in paramNodes)
                    {
                        if (paramNode.InnerText == null || paramNode.InnerText.Length == 0)
                        {
                            this.AddViolation(element, Rules.GenericTypeParameterDocumentationMustHaveText, paramNode.OuterXml);
                        }
                        else
                        {
                            this.CheckDocumentationValidity(element, element.LineNumber, paramNode, "typeparam");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the contents of the file header.
        /// </summary>
        /// <param name="document">The document containing the header.</param>
        private void CheckFileHeader(CsDocument document)
        {
            Param.AssertNotNull(document, "document");

            // Get the properties.
            string companyName = null;
            string copyright = null;

            StringProperty companyNameProperty = this.GetSetting(document.Settings, DocumentationRules.CompanyNameProperty) as StringProperty;
            if (companyNameProperty != null)
            {
                companyName = companyNameProperty.Value;
            }

            StringProperty copyrightProperty = this.GetSetting(document.Settings, DocumentationRules.CopyrightProperty) as StringProperty;
            if (copyrightProperty != null)
            {
                copyright = copyrightProperty.Value;
            }

            this.CheckFileHeader(document, copyright, companyName);
        }

        /// <summary>
        /// Checks the file header for the given document.
        /// </summary>
        /// <param name="document">The document to check.</param>
        /// <param name="copyright">The required copyright text.</param>
        /// <param name="companyName">The required company name text.</param>
        private void CheckFileHeader(CsDocument document, string copyright, string companyName)
        {
            Param.AssertNotNull(document, "document");
            Param.Ignore(copyright);
            Param.Ignore(companyName);

            if (document.FileHeader == null ||
                document.FileHeader.HeaderXml == null ||
                document.FileHeader.HeaderXml.Length == 0)
            {
                this.AddViolation(document.RootElement, 1, Rules.FileMustHaveHeader);
            }
            else
            {
                // Open the header up in an Xml document.
                try
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(document.FileHeader.HeaderXml);

                    // Check the copyright node.
                    XmlNode node = xml.DocumentElement["copyright"];
                    if (node == null)
                    {
                        this.AddViolation(document.RootElement, document.FileHeader.LineNumber, Rules.FileHeaderMustShowCopyright);
                    }
                    else
                    {
                        string trimmedText = node.InnerText.Trim();

                        if (trimmedText.Length == 0)
                        {
                            this.AddViolation(document.RootElement, document.FileHeader.LineNumber, Rules.FileHeaderMustHaveCopyrightText);
                        }
                        else if (!string.IsNullOrEmpty(copyright) && !MatchCopyrightText(copyright, trimmedText))
                        {
                            this.AddViolation(document.RootElement, document.FileHeader.LineNumber, Rules.FileHeaderCopyrightTextMustMatch, copyright);
                        }

                        // Check the file attribute
                        XmlNode attribute = node.Attributes["file"];
                        if (attribute == null)
                        {
                            this.AddViolation(document.RootElement, document.FileHeader.LineNumber, Rules.FileHeaderMustContainFileName);
                        }
                        else
                        {
                            // Make sure the filename matches the name of the file.
                            if (string.Compare(attribute.InnerText, document.SourceCode.Name, StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                this.AddViolation(document.RootElement, document.FileHeader.LineNumber, Rules.FileHeaderFileNameDocumentationMustMatchFileName);
                            }
                        }

                        // Check the company attribute.
                        attribute = node.Attributes["company"];
                        if (attribute == null)
                        {
                            this.AddViolation(document.RootElement, document.FileHeader.LineNumber, Rules.FileHeaderMustHaveValidCompanyText);
                        }
                        else
                        {
                            trimmedText = attribute.Value.Trim();

                            if (trimmedText.Length == 0)
                            {
                                this.AddViolation(document.RootElement, document.FileHeader.LineNumber, Rules.FileHeaderMustHaveValidCompanyText);
                            }
                            else if (!string.IsNullOrEmpty(companyName) && string.CompareOrdinal(companyName, trimmedText) != 0)
                            {
                                this.AddViolation(document.RootElement, document.FileHeader.LineNumber, Rules.FileHeaderCompanyNameTextMustMatch, companyName);
                            }
                        }
                    }

                    // Check the summary node.
                    node = xml.DocumentElement["summary"];
                    if (node == null || node.InnerText.Length == 0)
                    {
                        this.AddViolation(document.RootElement, document.FileHeader.LineNumber, Rules.FileHeaderMustHaveSummary);
                    }
                }
                catch (XmlException)
                {
                    this.AddViolation(document.RootElement, 1, Rules.FileMustHaveHeader);
                }
                catch (ArgumentException)
                {
                    this.AddViolation(document.RootElement, 1, Rules.FileMustHaveHeader);
                }
            }
        }

        /// <summary>
        /// Inserts any included documentation into the given documentation header.
        /// </summary>
        /// <param name="element">The element containing the documentation.</param>
        /// <param name="basePath">The base path where the includer is located.</param>
        /// <param name="documentation">The documentation header.</param>
        /// <returns>Returns false if a violation was found.</returns>
        private bool InsertIncludedDocumentation(CsElement element, string basePath, XmlDocument documentation)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertValidString(basePath, "basePath");
            Param.AssertNotNull(documentation, "documentation");

            return this.InsertIncludedDocumentationForNode(element, basePath, documentation.DocumentElement);
        }

        /// <summary>
        /// Inserts any included documentation under the given documentation node.
        /// </summary>
        /// <param name="element">The element containing the documentation.</param>
        /// <param name="basePath">The base path where the includer is located.</param>
        /// <param name="documentationNode">The documentation node.</param>
        /// <returns>Returns false if a violation was found.</returns>
        private bool InsertIncludedDocumentationForNode(CsElement element, string basePath, XmlNode documentationNode)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertValidString(basePath, "basePath");
            Param.AssertNotNull(documentationNode, "documentationNode");

            bool result = true;

            if (documentationNode.NodeType == XmlNodeType.Element && documentationNode.Name == "include")
            {
                // Handle the include tag.
                result = this.LoadAndReplaceIncludeTag(element, basePath, documentationNode);
            }
            else
            {
                result = this.InsertIncludedDocumentationForChildNodes(element, basePath, documentationNode);
            }

            return result;
        }

        /// <summary>
        /// Iterates through the children of the given documentation node, and replaces any include tags found there.
        /// </summary>
        /// <param name="element">The element containing the documentation.</param>
        /// <param name="basePath">The base path where the includer is located.</param>
        /// <param name="documentationNode">The documentation node.</param>
        /// <returns>Returns true of success; false otherwise.</returns>
        private bool InsertIncludedDocumentationForChildNodes(CsElement element, string basePath, XmlNode documentationNode)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertValidString(basePath, "basePath");
            Param.AssertNotNull(documentationNode, "documentationNode");

            // This method is optimized for performance. It loops through the collection of child nodes under the given
            // documentation node. Each time it finds an 'include' node, it will remove that node and replace it with
            // the included contents, inline. Doing this replacement unfortunately destroys the enumerator that we are 
            // using to loop through the child nodes. Therefor, whenever the count of child nodes is two or greater,
            // we copy the collection of child nodes into a temporary array before doing the search and replace.
            // We omit this step whenever there is only a single child node.
            if (documentationNode.ChildNodes.Count == 1)
            {
                if (!this.InsertIncludedDocumentationForNode(element, basePath, documentationNode.FirstChild))
                {
                    return false;
                }
            }
            else if (documentationNode.ChildNodes.Count > 1)
            {
                // Create an array just large enough to store the collection of child nodes.
                XmlNode[] childNodes = new XmlNode[documentationNode.ChildNodes.Count];

                // Add each child node to the collection. Looping through the collection as done below
                // avoids the need to initialize up an enumerator object.
                int index = 0;
                for (XmlNode child = documentationNode.FirstChild; child != null; child = child.NextSibling)
                {
                    childNodes[index++] = child;
                }

                // Loop through each of the nodes in the array and check for includes.
                for (int i = 0; i < childNodes.Length; ++i)
                {
                    if (!this.InsertIncludedDocumentationForNode(element, basePath, childNodes[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Loads and inserts included documentation.
        /// </summary>
        /// <param name="element">The element containing the documentation.</param>
        /// <param name="basePath">The base path to the file containing the element.</param>
        /// <param name="documentationNode">The 'include' tag node.</param>
        /// <returns>Returns true on success.</returns>
        private bool LoadAndReplaceIncludeTag(CsElement element, string basePath, XmlNode documentationNode)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertValidString(basePath, "basePath");
            Param.AssertNotNull(documentationNode, "documentationNode");

            Debug.Assert(documentationNode.Name == "include", "The node is not an include tag.");

            // Extract and validate the file and path values.
            string file;
            string path;
            ExtractIncludeTagFileAndPath(documentationNode, out file, out path);

            if (string.IsNullOrEmpty(file) || string.IsNullOrEmpty(path))
            {
                this.AddViolation(element, Rules.IncludeNodeDoesNotContainValidFileAndPath, documentationNode.OuterXml);
            }
            else
            {
                // Load the included documentation file.
                CachedXmlDocument includedDocument = this.LoadIncludedDocumentationFile(basePath, file);
                if (includedDocument == null)
                {
                    this.AddViolation(element, Rules.IncludedDocumentationFileDoesNotExist, file);
                }
                else
                {
                    // Extract the documentation node from the file.
                    XmlNode includedDocumentationNode = ExtractDocumentationNodeFromIncludedFile(includedDocument.Document, path);
                    if (includedDocumentationNode == null)
                    {
                        this.AddViolation(element, Rules.IncludedDocumentationXPathDoesNotExist, path, file);
                    }
                    else
                    {
                        if (!this.ReplaceIncludeTagWithIncludedDocumentationContents(element, documentationNode, includedDocument, includedDocumentationNode))
                        {
                            this.AddViolation(element, Rules.IncludedDocumentationXPathDoesNotExist, path, file);
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Replaces an include tag with the contents of the included documentation.
        /// </summary>
        /// <param name="element">The element containing the documentation.</param>
        /// <param name="documentationNode">The documentation node within the documentation.</param>
        /// <param name="includedDocument">The included document.</param>
        /// <param name="includedDocumentationNode">The included node within the included document.</param>
        /// <returns>Returns true on success; false otherwise.</returns>
        private bool ReplaceIncludeTagWithIncludedDocumentationContents(CsElement element, XmlNode documentationNode, CachedXmlDocument includedDocument, XmlNode includedDocumentationNode)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(documentationNode, "documentationNode");
            Param.AssertNotNull(includedDocument, "includedDocument");
            Param.AssertNotNull(includedDocumentationNode, "includedDocumentationNode");

            try
            {
                // Insert the contents of the included documentation node immediately after the 'include' node.
                XmlNode previousNode = documentationNode;

                foreach (XmlNode includedDocumentationNodeChild in includedDocumentationNode.ChildNodes)
                {
                    // Import the linked documentation node into the element's documentation object.
                    XmlNode importedNode = documentationNode.OwnerDocument.ImportNode(includedDocumentationNodeChild, true);

                    // Resolve any 'include' tags within the linked documentation.
                    this.InsertIncludedDocumentationForNode(element, Path.GetDirectoryName(includedDocument.FilePath), importedNode);

                    // Replace the original 'include' tag with the linked documentation contents.
                    documentationNode.ParentNode.InsertAfter(importedNode, previousNode);

                    previousNode = importedNode;
                }

                // Remove the original 'include' node.
                documentationNode.ParentNode.RemoveChild(documentationNode);
            }
            catch (XmlException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Loads a documentation file included into a documentation header through an 'include' tag.
        /// </summary>
        /// <param name="basePath">The base path where the caller is located.</param>
        /// <param name="file">The documentation file to load.</param>
        /// <returns>Returns the included documentation object.</returns>
        private CachedXmlDocument LoadIncludedDocumentationFile(string basePath, string file)
        {
            Param.AssertValidString(basePath, "basePath");
            Param.AssertValidString(file, "file");

            CachedXmlDocument document = null;

            // Get the full path to the xml docs file and convert it to upper-case for hash resolving.
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
                string adjustedFile = fullPath.ToUpper(CultureInfo.InvariantCulture);

                lock (this)
                {
                    // Initialize the dictionary if needed.
                    if (this.includedDocs == null)
                    {
                        this.includedDocs = new Dictionary<string, CachedXmlDocument>();
                    }

                    // Attempt to load the xml document out of the dictionary.
                    if (!this.includedDocs.TryGetValue(adjustedFile, out document))
                    {
                        // The file is not included in the dictionary. Attempt to load the file and add it to the dictionary.
                        document = LoadDocFileFromDisk(adjustedFile);

                        // We add the doc even if it is null, so that we don't attempt to load it again.
                        this.includedDocs.Add(adjustedFile, document);
                    }
                }
            }

            return document;
        }

        #endregion Private Methods

        #region Private Structs

        /// <summary>
        /// The settings for the analyzer.
        /// </summary>
        private struct AnalyzerSettings
        {
            /// <summary>
            /// Indicates whether to ignore private members.
            /// </summary>
            public bool IgnorePrivates;

            /// <summary>
            /// Indicates whether to ignore internal members.
            /// </summary>
            public bool IgnoreInternals;

            /// <summary>
            /// Indicates whether to require documentation for fields.
            /// </summary>
            public bool RequireFields;
        }

        #endregion Private Structs

        #region Private Classes

        /// <summary>
        /// Represents one Xml documentation document loaded from disk.
        /// </summary>
        private class CachedXmlDocument
        {
            /// <summary>
            /// The path to the file on disk.
            /// </summary>
            private string filePath;

            /// <summary>
            /// The document.
            /// </summary>
            private XmlDocument document;

            /// <summary>
            /// Initializes a new instance of the CachedXmlDocument class.
            /// </summary>
            /// <param name="filePath">The path to the file on disk.</param>
            /// <param name="document">The document.</param>
            public CachedXmlDocument(string filePath, XmlDocument document)
            {
                Param.AssertValidString(filePath, "filePath");
                Param.AssertNotNull(document, "document");

                this.filePath = filePath;
                this.document = document;
            }

            /// <summary>
            /// Gets the path to the file on disk.
            /// </summary>
            public string FilePath
            {
                get { return this.filePath; }
            }

            /// <summary>
            /// Gets the document.
            /// </summary>
            public XmlDocument Document 
            {
                get { return this.document; }
            }
        }

        #endregion Private Classes
    }
}
