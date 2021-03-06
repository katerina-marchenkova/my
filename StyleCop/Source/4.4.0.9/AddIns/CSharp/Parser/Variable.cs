//-----------------------------------------------------------------------
// <copyright file="Variable.cs" company="Microsoft">
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
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Describes a field, variable or constant.
    /// </summary>
    /// <subcategory>other</subcategory>
    public class Variable : ICodePart
    {
        #region Private Fields

        /// <summary>
        /// The variable type.
        /// </summary>
        private TypeToken type;

        /// <summary>
        /// The variable name.
        /// </summary>
        private string name;

        /// <summary>
        /// The variable modifiers, if any.
        /// </summary>
        private VariableModifiers modifiers;

        /// <summary>
        /// The location of the variable.
        /// </summary>
        private CodeLocation location;

        /// <summary>
        /// Indicates whether the variable is located within a block of generated code.
        /// </summary>
        private bool generated;

        /// <summary>
        /// The parent of the variable.
        /// </summary>
        private Reference<ICodePart> parent;

        #endregion Private Fields

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the Variable class.
        /// </summary>
        /// <param name="type">The type of the variable.</param>
        /// <param name="name">The name of the variable.</param>
        /// <param name="modifiers">Modifers applied to this variable.</param>
        /// <param name="location">The location of the variable.</param>
        /// <param name="parent">The parent code part.</param>
        /// <param name="generated">Indicates whethre the variable is located within a block of generated code.</param>
        internal Variable(TypeToken type, string name, VariableModifiers modifiers, CodeLocation location, Reference<ICodePart> parent, bool generated)
        {
            Param.Ignore(type);
            Param.AssertValidString(name, "name");
            Param.Ignore(modifiers);
            Param.AssertNotNull(location, "location");
            Param.AssertNotNull(parent, "parent");
            Param.Ignore(generated);

            this.type = type;
            this.name = name;
            this.modifiers = modifiers;
            this.location = location;
            this.parent = parent;
            this.generated = generated;
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// Gets the variable name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the type of this code part.
        /// </summary>
        public CodePartType CodePartType
        {
            get
            {
                return CodePartType.Variable;
            }
        }

        /// <summary>
        /// Gets the variable type.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Naming", 
            "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "API has already been published and should not be changed.")]
        public TypeToken Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// Gets the line number on which this variable appears.
        /// </summary>
        public int LineNumber
        {
            get
            {
                return this.location.LineNumber;
            }
        }

        /// <summary>
        /// Gets the modifiers applied to this variable.
        /// </summary>
        public VariableModifiers Modifiers
        {
            get
            {
                return this.modifiers;
            }
        }

        /// <summary>
        /// Gets the location of the variable.
        /// </summary>
        public CodeLocation Location
        {
            get
            {
                return this.location;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the variable is located within a block of generated code.
        /// </summary>
        public bool Generated
        {
            get
            {
                return this.generated;
            }
        }

        /// <summary>
        /// Gets the parent of the variable.
        /// </summary>
        public ICodePart Parent
        {
            get
            {
                return this.parent.Target;
            }
        }

        #endregion Public Properties
    }
}
