//-----------------------------------------------------------------------
// <copyright file="VariableDeclarationStatement.cs" company="Microsoft">
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
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A statement declaring a new variable.
    /// </summary>
    /// <subcategory>statement</subcategory>
    public sealed class VariableDeclarationStatement : Statement
    {
        #region Private Fields

        /// <summary>
        /// Indicates whether the item is constant.
        /// </summary>
        private bool constant;

        /// <summary>
        /// The inner expression.
        /// </summary>
        private VariableDeclarationExpression expression;

        #endregion Private Fields

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the VariableDeclarationStatement class.
        /// </summary>
        /// <param name="tokens">The list of tokens that form the statement.</param>
        /// <param name="constant">Indicates whether the item is constant.</param>
        /// <param name="expression">The inner expression.</param>
        internal VariableDeclarationStatement(
            CsTokenList tokens, bool constant, VariableDeclarationExpression expression)
            : base(StatementType.VariableDeclaration, tokens)
        {
            Param.AssertNotNull(tokens, "tokens");
            Param.Ignore(constant);
            Param.AssertNotNull(expression, "expression");

            this.constant = constant;
            this.expression = expression;

            this.AddExpression(expression);
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the item is constant.
        /// </summary>
        public bool Constant
        {
            get
            {
                return this.constant;
            }
        }

        /// <summary>
        /// Gets the inner expression for this statement.
        /// </summary>
        public VariableDeclarationExpression InnerExpression
        {
            get
            {
                return this.expression;
            }
        }

        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Naming", 
            "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "API has already been published and should not be changed.")]
        public TypeToken Type
        {
            get
            {
                return this.expression.Type;
            }
        }

        /// <summary>
        /// Gets the list of declarators for the expression.
        /// </summary>
        public ICollection<VariableDeclaratorExpression> Declarators
        {
            get
            {
                return this.expression.Declarators;
            }
        }

        #endregion Public Properties
    }
}
