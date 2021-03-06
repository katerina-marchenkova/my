//-----------------------------------------------------------------------
// <copyright file="GenericType.cs" company="Microsoft">
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
    using System.Text;

    /// <summary>
    /// Describes a generic type token.
    /// </summary>
    /// <subcategory>token</subcategory>
    public sealed class GenericType : TypeToken
    {
        #region Private Fields

        /// <summary>
        /// The types within the generic type.
        /// </summary>
        private ICollection<string> types;

        #endregion Private Fields

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the GenericType class.
        /// </summary>
        /// <param name="childTokens">The list of child tokens that form the generic token.</param>
        /// <param name="location">The location of the generic in the code.</param>
        /// <param name="parent">The parent of the token.</param>
        /// <param name="generated">True if the token is inside of a block of generated code.</param>
        internal GenericType(MasterList<CsToken> childTokens, CodeLocation location, Reference<ICodePart> parent, bool generated)
            : base(childTokens, location, parent, CsTokenClass.GenericType, generated)
        {
            Param.AssertNotNull(childTokens, "childTokens");
            Param.AssertGreaterThanOrEqualTo(childTokens.Count, 3, "childTokens");
            Param.AssertNotNull(location, "location");
            Param.AssertNotNull(parent, "parent");
            Param.Ignore(generated);
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// Gets the types within the generic type.
        /// </summary>
        public ICollection<string> GenericTypes
        {
            get
            {
                if (this.types == null)
                {
                    this.ExtractGenericTypes();
                }

                return this.types;
            }
        }

        #endregion Public Properties
        
        #region Private Methods

        /// <summary>
        /// Extracts the generic types from the type list and saves them.
        /// </summary>
        private void ExtractGenericTypes()
        {
            List<string> genericTypes = new List<string>();
            StringBuilder type = new StringBuilder();

            for (Node<CsToken> tokenNode = this.ChildTokens.First; tokenNode != null; tokenNode = tokenNode.Next)
            {
                if (tokenNode.Value.CsTokenType == CsTokenType.Comma)
                {
                    string trimmedType = CodeParser.TrimType(type.ToString());
                    if (!string.IsNullOrEmpty(trimmedType))
                    {
                        genericTypes.Add(trimmedType);
                    }

                    type.Remove(0, type.Length);
                }
                else
                {
                    type.Append(tokenNode.Value.Text);
                }
            }

            if (type.Length > 0)
            {
                string trimmedType = CodeParser.TrimType(type.ToString());
                if (!string.IsNullOrEmpty(trimmedType))
                {
                    genericTypes.Add(trimmedType);
                }
            }

            this.types = genericTypes.ToArray();
        }

        #endregion Private Methods
    }
}
