//-----------------------------------------------------------------------
// <copyright file="FileHeader.cs" company="Microsoft">
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
    using System.Globalization;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Describes the header at the top of a C# file.
    /// </summary>
    /// <subcategory>other</subcategory>
    public class FileHeader : ICodePart
    {
        #region Private Fields

        /// <summary>
        /// Indicates whether the file header has the generated attribute.
        /// </summary>
        private bool generated;

        /// <summary>
        /// The header text.
        /// </summary>
        private string headerText;

        /// <summary>
        /// The header text wrapped into an Xml tag.
        /// </summary>
        private string headerXml;

        /// <summary>
        /// The parent of the file header.
        /// </summary>
        private Reference<ICodePart> parent;

        /// <summary>
        /// The collection of tokens in the header.
        /// </summary>
        private CsTokenList tokens;

        /// <summary>
        /// The location of the header.
        /// </summary>
        private CodeLocation location;

        #endregion Private Fields

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the FileHeader class.
        /// </summary>
        /// <param name="headerText">The header text.</param>
        /// <param name="tokens">The collection of tokens in the header.</param>
        /// <param name="parent">The parent of the header.</param>
        internal FileHeader(string headerText, CsTokenList tokens, Reference<ICodePart> parent)
        {
            Param.AssertNotNull(headerText, "headerText");
            Param.AssertNotNull(tokens, "tokens");
            Param.AssertNotNull(parent, "parent");

            this.headerText = headerText;
            this.tokens = tokens;
            this.parent = parent;

            if (this.tokens.First != null)
            {
                this.location = CsToken.JoinLocations(this.tokens.First, this.tokens.Last);
            }
            else
            {
                this.location = CodeLocation.Empty;
            }

            // Attempt to load this into an Xml document.
            try
            {
                if (this.headerText.Length > 0)
                {
                    this.headerXml = string.Format(CultureInfo.InvariantCulture, "<root>{0}</root>", this.headerText);

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(this.headerXml);

                    // Check whether the header has the autogenerated tag.
                    XmlNode node = doc.DocumentElement["autogenerated"];
                    if (node != null)
                    {
                        // Set this as generated code.
                        this.generated = true;
                    }
                    else
                    {
                        node = doc.DocumentElement["auto-generated"];
                        if (node != null)
                        {
                            // Set this as generated code.
                            this.generated = true;
                        }
                    }
                }
            }
            catch (XmlException)
            {
            }
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// Gets the type of this code part.
        /// </summary>
        public CodePartType CodePartType
        {
            get
            {
                return CodePartType.FileHeader;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the file header contains the auto-generated attribute.
        /// </summary>
        public bool Generated
        {
            get
            {
                return this.generated;
            }
        }

        /// <summary>
        /// Gets the header text.
        /// </summary>
        public string HeaderText
        {
            get
            {
                return this.headerText;
            }
        }

        /// <summary>
        /// Gets the header text string modified such that it is loadable into
        /// an <see cref="XmlDocument"/> object.
        /// </summary>
        public string HeaderXml
        {
            get
            {
                return this.headerXml;
            }
        }

        /// <summary>
        /// Gets the parent of the file header.
        /// </summary>
        public ICodePart Parent
        {
            get
            {
                return this.parent.Target;
            }
        }

        /// <summary>
        /// Gets the collection of tokens that form the header.
        /// </summary>
        public CsTokenList Tokens
        {
            get
            {
                return this.tokens;
            }
        }

        /// <summary>
        /// Gets the line number on which the header begins.
        /// </summary>
        public int LineNumber
        {
            get
            {
                return this.location.LineNumber;
            }
        }

        /// <summary>
        /// Gets the location of the token.
        /// </summary>
        public CodeLocation Location
        {
            get
            {
                return this.location;
            }
        }

        #endregion Public Properties
    }
}
