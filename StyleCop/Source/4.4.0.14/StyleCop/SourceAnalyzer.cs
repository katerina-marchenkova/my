//-----------------------------------------------------------------------
// <copyright file="SourceAnalyzer.cs" company="Microsoft">
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
namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Xml;

    /// <summary>
    /// Base class for StyleCop code analyzer modules.
    /// </summary>
    public abstract class SourceAnalyzer : StyleCopAddIn
    {
        #region Private Fields

        /// <summary>
        /// The ID of the parser that this analyzer is attached to.
        /// </summary>
        private string parserId;

        /// <summary>
        /// The parser that this analyzer is attached to.
        /// </summary>
        private SourceParser parser;

        #endregion Private Fields

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the SourceAnalyzer class.
        /// </summary>
        protected SourceAnalyzer()
        {
            // Get the SourceAnalyzer attribute from the type.
            object[] attributes = this.GetType().GetCustomAttributes(typeof(SourceAnalyzerAttribute), true);
            if (attributes == null || attributes.Length == 0)
            {
                throw new ArgumentException(Strings.SourceAnalyzerAttributeMissing);
            }

            // Make sure the parser type is set.
            SourceAnalyzerAttribute attribute = (SourceAnalyzerAttribute)attributes[0];
            if (attribute.ParserType == null)
            {
                throw new ArgumentException(Strings.SourceAnalyzerAttributeMissing);
            }

            // Set the parser ID.
            this.parserId = SourceParser.GetIdFromAddInType(attribute.ParserType);
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// Gets the ID of the parser that this analyzer is attached to.
        /// </summary>
        public string ParserId
        {
            get 
            { 
                return this.parserId; 
            }
        }

        /// <summary>
        /// Gets the parser object that this analyzer is attached to.
        /// </summary>
        public SourceParser Parser
        {
            get 
            { 
                return this.parser; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether the analyzer should cancel its analysis and return immediately.
        /// </summary>
        /// <remarks>The analyzer should check the value of this property periodically while
        /// analyzing a document. If the value is set to true, the analyzer should immediately stop
        /// analyzing the document and return.</remarks>
        public bool Cancel
        {
            get
            {
                Debug.Assert(this.Core != null, "The core instance has not been initialized.");
                return this.Core.Cancel;
            }
        }

        #endregion Public Properties

        #region Public Override Methods

        /// <summary>
        /// Gets a value indicating whether the given rule is enabled for the given document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="ruleName">The rule to check.</param>
        /// <returns>Returns true if the rule is enabled; otherwise false.</returns>
        public override bool IsRuleEnabled(CodeDocument document, string ruleName)
        {
            Param.RequireNotNull(document, "document");
            Param.RequireValidString(ruleName, "ruleName");

            if (document.SourceCode == null || document.SourceCode.Settings == null)
            {
                return true;
            }

            return document.SourceCode.Settings.IsRuleEnabled(this, ruleName);
        }

        /// <summary>
        /// Determines whether the given rule is suppressed for the given element.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <param name="rule">The rule to check.</param>
        /// <returns>Returns true is the rule is suppressed; otherwise false.</returns>
        public override bool IsRuleSuppressed(ICodeElement element, Rule rule)
        {
            Param.Ignore(element, rule);
            return this.parser.IsRuleSuppressed(element, rule);
        }

        #endregion Public Override Methods

        #region Public Virtual Methods

        /// <summary>
        /// Analyzes a code document.
        /// </summary>
        /// <param name="document">The document to analyze.</param>
        public virtual void AnalyzeDocument(CodeDocument document)
        {
            Param.Ignore(document);
        }

        /// <summary>
        /// Determines whether the analyzer wishes to delay its analysis until a later pass.
        /// </summary>
        /// <param name="document">The document to analyze.</param>
        /// <param name="passNumber">The current pass number.</param>
        /// <returns>Returns true if the analysis should be delayed until the next pass, or
        /// false if the analysis should be performed in the current pass.</returns>
        public virtual bool DelayAnalysis(CodeDocument document, int passNumber)
        {
            Param.Ignore(document, passNumber);
            return false;
        }

        /// <summary>
        /// Called before an analysis run is initiated.
        /// </summary>
        public virtual void PreAnalyze()
        {
        }

        /// <summary>
        /// Called after an analysis run is completed.
        /// </summary>
        public virtual void PostAnalyze()
        {
        }

        #endregion Public Virtual Methods

        #region Internal Methods

        /// <summary>
        /// Sets the parser that this analyzer is attached to.
        /// </summary>
        /// <param name="item">The parser object that this analyzer is attached to.</param>
        internal void SetParser(SourceParser item)
        {
            Param.Ignore(item);

            // Set the reference to the parser object.
            this.parser = item;
        }

        #endregion Internal Methods

        #region Protected Override Methods

        /// <summary>
        /// Parses the Xml document which initializes the analyzer.
        /// </summary>
        /// <param name="document">The xml document to load.</param>
        /// <param name="topmostType">Indicates whether the xml document comes from the top-most type in the 
        /// add-in's type hierarchy.</param>
        /// <param name="isKnownAssembly">Indicates whether the add-in comes from a known assembly.</param>
        protected override void ImportInitializationXml(XmlDocument document, bool topmostType, bool isKnownAssembly)
        {
            Param.RequireNotNull(document, "document");
            Param.RequireNotNull(document.DocumentElement, "xml.DocumentElement");
            Param.Ignore(topmostType);
            Param.Ignore(isKnownAssembly);

            base.ImportInitializationXml(document, topmostType, isKnownAssembly);

            if (topmostType)
            {
                // Make sure the root element's name is correct.
                if (document.DocumentElement.Name != "SourceAnalyzer")
                {
                    throw new ArgumentException(Strings.SourceAnalyzerRootNodeIsIncorrect);
                }

                // Get the "disabled by default" value. This attribute has been deprecated. If this attribute
                // is exists and is set to true, throw an exception indicating that the attribute is no longer allowed.
                XmlAttribute disabledByDefault = document.DocumentElement.Attributes["DisabledByDefault"];
                if (disabledByDefault != null && disabledByDefault.Value == "true")
                {
                    throw new ArgumentException(Strings.DisabledByDefaultAttributeDeprecatedForAddIns);
                }
            }
        }

        #endregion Protected Override Methods

        #region Protected Methods

        /// <summary>
        /// Gets the data saved by this analyzer within the given document.
        /// </summary>
        /// <param name="document">The document containing the data.</param>
        /// <returns>Returns the data if it exists.</returns>
        protected object GetDocumentData(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");

            object data = null;
            document.AnalyzerData.TryGetValue(this.Id, out data);

            return data;
        }

        /// <summary>
        /// Stores the given data object within the given document.
        /// </summary>
        /// <param name="document">The document to store the data within.</param>
        /// <param name="data">The data to store.</param>
        protected void SetDocumentData(CodeDocument document, object data)
        {
            Param.RequireNotNull(document, "document");
            Param.Ignore(data);

            if (document.AnalyzerData.ContainsKey(this.Id))
            {
                document.AnalyzerData[this.Id] = data;
            }
            else
            {
                document.AnalyzerData.Add(this.Id, data);
            }
        }

        /// <summary>
        /// Writes the given output to the StyleCop log file.
        /// </summary>
        /// <param name="level">The output level.</param>
        /// <param name="output">The output text to write.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "output", Justification = "The method is not yet implemented.")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "level", Justification = "The method is not yet implemented.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "The method is not yet implemented.")]
        protected void Log(StyleCopLogLevel level, string output)
        {
            Param.Ignore(level, output);
        }

        #endregion Protected Methods
    }
}
