//-----------------------------------------------------------------------
// <copyright file="Field.cs" company="Microsoft">
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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Describes a field within a class or struct.
    /// </summary>
    /// <subcategory>element</subcategory>
    public sealed class Field : CsElement
    {
        #region Private Fields

        /// <summary>
        /// The type of the field.
        /// </summary>
        private TypeToken type;

        /// <summary>
        /// Indicates whether the item is declared const.
        /// </summary>
        private bool isConst;

        /// <summary>
        /// Indicates whether the item is declared readonly.
        /// </summary>
        private bool isReadOnly;

        /// <summary>
        /// The variable declaration statement within this field.
        /// </summary>
        private VariableDeclarationStatement declaration;

        #endregion Private Fields

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the Field class.
        /// </summary>
        /// <param name="document">The documenent that contains the element.</param>
        /// <param name="parent">The parent of the element.</param>
        /// <param name="header">The Xml header for this element.</param>
        /// <param name="attributes">The list of attributes attached to this element.</param>
        /// <param name="declaration">The declaration code for this element.</param>
        /// <param name="fieldType">The type of the field.</param>
        /// <param name="unsafeCode">Indicates whether the element resides within a block of unsafe code.</param>
        /// <param name="generated">Indicates whether the code element was generated or written by hand.</param>
        internal Field(
            CsDocument document,
            CsElement parent,
            XmlHeader header,
            ICollection<Attribute> attributes,
            Declaration declaration,
            TypeToken fieldType,
            bool unsafeCode,
            bool generated)
            : base(
            document, 
            parent, 
            ElementType.Field, 
            "field " + declaration.Name, 
            header,
            attributes,
            declaration, 
            unsafeCode,
            generated)
        {
            Param.AssertNotNull(document, "document");
            Param.AssertNotNull(parent, "parent");
            Param.Ignore(header);
            Param.Ignore(attributes);
            Param.AssertNotNull(declaration, "declaration");
            Param.AssertNotNull(fieldType, "fieldType");
            Param.Ignore(unsafeCode);
            Param.Ignore(generated);

            this.type = fieldType;

            // Determine whether the item is const or readonly.
            this.isConst = this.Declaration.ContainsModifier(CsTokenType.Const);
            this.isReadOnly = this.Declaration.ContainsModifier(CsTokenType.Readonly);
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the field is declared const.
        /// </summary>
        public bool Const
        {
            get
            {
                return this.isConst;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the field is declared readonly.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Naming", 
            "CA1702:CompoundWordsShouldBeCasedCorrectly", 
            MessageId = "Readonly",
            Justification = "API has already been published and should not be changed.")]
        public bool Readonly
        {
            get
            {
                return this.isReadOnly;
            }
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        public TypeToken FieldType
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// Gets the variable declaration statement within this field.
        /// </summary>
        public VariableDeclarationStatement VariableDeclarationStatement
        {
            get
            {
                return this.declaration;
            }

            internal set
            {
                this.declaration = value;
                if (this.declaration != null)
                {
                    this.AddStatement(this.declaration);
                }
            }
        }

        #endregion Public Properties
    }
}
