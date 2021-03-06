﻿//-----------------------------------------------------------------------
// <copyright file="CachedCodeStrings.cs" company="Microsoft">
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

    /// <summary>
    /// Loaded and cached code strings used within the DocumentationRules analyzer.
    /// </summary>
    internal static class CachedCodeStrings
    {
        /// <summary>
        /// Header text string "Gets a value indicating whether".
        /// </summary>
        private static string headerSummaryForBooleanGetAccessor;

        /// <summary>
        /// Header text string "Gets or sets a value indicating whether".
        /// </summary>
        private static string headerSummaryForBooleanGetAndSetAccessor;

        /// <summary>
        /// Header text string "Sets a value indicating whether".
        /// </summary>
        private static string headerSummaryForBooleanSetAccessor;

        /// <summary>
        /// Header text string "Gets".
        /// </summary>
        private static string headerSummaryForGetAccessor;

        /// <summary>
        /// Header text string "Gets or sets".
        /// </summary>
        private static string headerSummaryForGetAndSetAccessor;

        /// <summary>
        /// Header text string "Sets".
        /// </summary>
        private static string headerSummaryForSetAccessor;

        /// <summary>
        /// Header text string "Initializes static data within the {0} class".
        /// </summary>
        private static string headerSummaryForStaticConstructor;

        /// <summary>
        /// Example header text string "Initializes static data within the {0} class".
        /// </summary>
        private static string exampleHeaderSummaryForStaticConstructor;

        /// <summary>
        /// Header text string "Prevents an instance of the class from being created".
        /// </summary>
        private static string headerSummaryForPrivateInstanceConstructor;

        /// <summary>
        /// Example header text string "Prevents an instance of the class from being created".
        /// </summary>
        private static string exampleHeaderSummaryForPrivateInstanceConstructor;

        /// <summary>
        /// Header text string "Initializes a new instance of the {0} class".
        /// </summary>
        private static string headerSummaryForInstanceConstructor;

        /// <summary>
        /// Example header text string "Initializes a new instance of the {0} class".
        /// </summary>
        private static string exampleHeaderSummaryForInstanceConstructor;

        /// <summary>
        /// Header text string "Finalizes an instance of the {0} class".
        /// </summary>
        private static string headerSummaryForDestructor;

        /// <summary>
        /// Example header text string "Finalizes an instance of the {0} class".
        /// </summary>
        private static string exampleHeaderSummaryForDestructor;

        /// <summary>
        /// Header text string "The parameter is not used".
        /// </summary>
        private static string parameterNotUsed;

        /// <summary>
        /// Gets header text string "Gets a value indicating whether".
        /// </summary>
        public static string HeaderSummaryForBooleanGetAccessor
        {
            get
            {
                if (headerSummaryForBooleanGetAccessor == null)
                {
                    headerSummaryForBooleanGetAccessor = CodeStrings.HeaderSummaryForBooleanGetAccessor;
                }

                return headerSummaryForBooleanGetAccessor;
            }
        }

        /// <summary>
        /// Gets header text string "Gets or sets a value indicating whether".
        /// </summary>
        public static string HeaderSummaryForBooleanGetAndSetAccessor
        {
            get
            {
                if (headerSummaryForBooleanGetAndSetAccessor == null)
                {
                    headerSummaryForBooleanGetAndSetAccessor = CodeStrings.HeaderSummaryForBooleanGetAndSetAccessor;
                }

                return headerSummaryForBooleanGetAndSetAccessor;
            }
        }

        /// <summary>
        /// Gets header text string "Sets a value indicating whether".
        /// </summary>
        public static string HeaderSummaryForBooleanSetAccessor
        {
            get
            {
                if (headerSummaryForBooleanSetAccessor == null)
                {
                    headerSummaryForBooleanSetAccessor = CodeStrings.HeaderSummaryForBooleanSetAccessor;
                }

                return headerSummaryForBooleanSetAccessor;
            }
        }

        /// <summary>
        /// Gets header text string "Gets".
        /// </summary>
        public static string HeaderSummaryForGetAccessor
        {
            get
            {
                if (headerSummaryForGetAccessor == null)
                {
                    headerSummaryForGetAccessor = CodeStrings.HeaderSummaryForGetAccessor;
                }

                return headerSummaryForGetAccessor;
            }
        }

        /// <summary>
        /// Gets header text string "Gets or sets".
        /// </summary>
        public static string HeaderSummaryForGetAndSetAccessor
        {
            get
            {
                if (headerSummaryForGetAndSetAccessor == null)
                {
                    headerSummaryForGetAndSetAccessor = CodeStrings.HeaderSummaryForGetAndSetAccessor;
                }

                return headerSummaryForGetAndSetAccessor;
            }
        }

        /// <summary>
        /// Gets header text string "Sets".
        /// </summary>
        public static string HeaderSummaryForSetAccessor
        {
            get
            {
                if (headerSummaryForSetAccessor == null)
                {
                    headerSummaryForSetAccessor = CodeStrings.HeaderSummaryForSetAccessor;
                }

                return headerSummaryForSetAccessor;
            }
        }

        /// <summary>
        /// Gets header text string "Initializes static data within the {0} class".
        /// </summary>
        public static string HeaderSummaryForStaticConstructor
        {
            get
            {
                if (headerSummaryForStaticConstructor == null)
                {
                    headerSummaryForStaticConstructor = CodeStrings.HeaderSummaryForStaticConstructor;
                }

                return headerSummaryForStaticConstructor;
            }
        }

        /// <summary>
        /// Gets hxample header text string "Initializes static data within the {0} class".
        /// </summary>
        public static string ExampleHeaderSummaryForStaticConstructor
        {
            get
            {
                if (exampleHeaderSummaryForStaticConstructor == null)
                {
                    exampleHeaderSummaryForStaticConstructor = CodeStrings.ExampleHeaderSummaryForStaticConstructor;
                }

                return exampleHeaderSummaryForStaticConstructor;
            }
        }

        /// <summary>
        /// Gets header text string "Prevents an instance of the class from being created".
        /// </summary>
        public static string HeaderSummaryForPrivateInstanceConstructor
        {
            get
            {
                if (headerSummaryForPrivateInstanceConstructor == null)
                {
                    headerSummaryForPrivateInstanceConstructor = CodeStrings.HeaderSummaryForPrivateInstanceConstructor;
                }

                return headerSummaryForPrivateInstanceConstructor;
            }
        }

        /// <summary>
        /// Gets example header text string "Prevents an instance of the class from being created".
        /// </summary>
        public static string ExampleHeaderSummaryForPrivateInstanceConstructor
        {
            get
            {
                if (exampleHeaderSummaryForPrivateInstanceConstructor == null)
                {
                    exampleHeaderSummaryForPrivateInstanceConstructor = CodeStrings.ExampleHeaderSummaryForPrivateInstanceConstructor;
                }

                return exampleHeaderSummaryForPrivateInstanceConstructor;
            }
        }

        /// <summary>
        /// Gets header text string "Initializes a new instance of the {0} class".
        /// </summary>
        public static string HeaderSummaryForInstanceConstructor
        {
            get
            {
                if (headerSummaryForInstanceConstructor == null)
                {
                    headerSummaryForInstanceConstructor = CodeStrings.HeaderSummaryForInstanceConstructor;
                }

                return headerSummaryForInstanceConstructor;
            }
        }

        /// <summary>
        /// Gets example header text string "Initializes a new instance of the {0} class".
        /// </summary>
        public static string ExampleHeaderSummaryForInstanceConstructor
        {
            get
            {
                if (exampleHeaderSummaryForInstanceConstructor == null)
                {
                    exampleHeaderSummaryForInstanceConstructor = CodeStrings.ExampleHeaderSummaryForInstanceConstructor;
                }

                return exampleHeaderSummaryForInstanceConstructor;
            }
        }

        /// <summary>
        /// Gets header text string "Finalizes an instance of the {0} class".
        /// </summary>
        public static string HeaderSummaryForDestructor
        {
            get
            {
                if (headerSummaryForDestructor == null)
                {
                    headerSummaryForDestructor = CodeStrings.HeaderSummaryForDestructor;
                }

                return headerSummaryForDestructor;
            }
        }

        /// <summary>
        /// Gets example header text string "Finalizes an instance of the {0} class".
        /// </summary>
        public static string ExampleHeaderSummaryForDestructor
        {
            get
            {
                if (exampleHeaderSummaryForDestructor == null)
                {
                    exampleHeaderSummaryForDestructor = CodeStrings.ExampleHeaderSummaryForDestructor;
                }

                return exampleHeaderSummaryForDestructor;
            }
        }

        /// <summary>
        /// Gets header text string "The parameter is not used".
        /// </summary>
        public static string ParameterNotUsed
        {
            get
            {
                if (parameterNotUsed == null)
                {
                    parameterNotUsed = CodeStrings.ParameterNotUsed;
                }

                return parameterNotUsed;
            }
        }
    }
}
