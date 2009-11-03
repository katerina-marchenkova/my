namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [CompilerGenerated, GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0"), DebuggerNonUserCode]
    internal class Strings
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static string DocumentMustBeCsDocument
        {
            get
            {
                return ResourceManager.GetString("DocumentMustBeCsDocument", resourceCulture);
            }
        }

        internal static string DocumentRoot
        {
            get
            {
                return ResourceManager.GetString("DocumentRoot", resourceCulture);
            }
        }

        internal static string ElementMustBeInParentsDocument
        {
            get
            {
                return ResourceManager.GetString("ElementMustBeInParentsDocument", resourceCulture);
            }
        }

        internal static string EmptyElement
        {
            get
            {
                return ResourceManager.GetString("EmptyElement", resourceCulture);
            }
        }

        internal static string InvalidNumberOfPasses
        {
            get
            {
                return ResourceManager.GetString("InvalidNumberOfPasses", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("Microsoft.StyleCop.CSharp.Strings", typeof(Strings).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }

        internal static string Root
        {
            get
            {
                return ResourceManager.GetString("Root", resourceCulture);
            }
        }

        internal static string UnexpectedEndOfFile
        {
            get
            {
                return ResourceManager.GetString("UnexpectedEndOfFile", resourceCulture);
            }
        }
    }
}

