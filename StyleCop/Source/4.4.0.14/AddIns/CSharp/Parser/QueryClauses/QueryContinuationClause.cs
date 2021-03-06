//-----------------------------------------------------------------------
// <copyright file="QueryContinuationClause.cs" company="Microsoft">
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

    /// <summary>
    /// Describes a continuation clause in a query expression.
    /// </summary>
    public sealed class QueryContinuationClause : QueryClause
    {
        #region Private Fields

        /// <summary>
        /// The continuation clause variable.
        /// </summary>
        private Variable variable;

        /// <summary>
        /// The list of clauses in the expression.
        /// </summary>
        private CodeUnitCollection<QueryClause> clauses;

        #endregion Private Fields

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the QueryContinuationClause class.
        /// </summary>
        /// <param name="tokens">The list of tokens that form the clause.</param>
        /// <param name="variable">The continuation clause variable.</param>
        /// <param name="clauses">The collection of clauses in the expression.</param>
        internal QueryContinuationClause(CsTokenList tokens, Variable variable, ICollection<QueryClause> clauses) 
            : base(QueryClauseType.Continuation, tokens)
        {
            Param.AssertNotNull(tokens, "tokens");
            Param.AssertNotNull(clauses, "clauses");
            Param.AssertNotNull(variable, "variable");

            this.variable = variable;
            this.clauses = new CodeUnitCollection<QueryClause>(this);
            this.clauses.AddRange(clauses);

            Debug.Assert(clauses.IsReadOnly, "The collection of query clauses should be read-only.");
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// Gets the continuation clause variable.
        /// </summary>
        public Variable Variable
        {
            get
            {
                return this.variable;
            }
        }

        /// <summary>
        /// Gets the list of query clauses within this expression.
        /// </summary>
        public ICollection<QueryClause> ChildClauses
        {
            get
            {
                return this.clauses;
            }
        }

        #endregion Public Properties
    }
}
