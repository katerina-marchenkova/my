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

    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
    internal class Strings
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings()
        {
        }

        internal static string AccessModifier
        {
            get
            {
                return ResourceManager.GetString("AccessModifier", resourceCulture);
            }
        }

        internal static string Closing
        {
            get
            {
                return ResourceManager.GetString("Closing", resourceCulture);
            }
        }

        internal static string CompanyInformationTab
        {
            get
            {
                return ResourceManager.GetString("CompanyInformationTab", resourceCulture);
            }
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

        internal static string CurlyBracketsInFileDoNotMatch
        {
            get
            {
                return ResourceManager.GetString("CurlyBracketsInFileDoNotMatch", resourceCulture);
            }
        }

        internal static string EnterValidPrefix
        {
            get
            {
                return ResourceManager.GetString("EnterValidPrefix", resourceCulture);
            }
        }

        internal static string Files
        {
            get
            {
                return ResourceManager.GetString("Files", resourceCulture);
            }
        }

        internal static string HungarianTab
        {
            get
            {
                return ResourceManager.GetString("HungarianTab", resourceCulture);
            }
        }

        internal static string MissingCompanyOrCopyright
        {
            get
            {
                return ResourceManager.GetString("MissingCompanyOrCopyright", resourceCulture);
            }
        }

        internal static string Opening
        {
            get
            {
                return ResourceManager.GetString("Opening", resourceCulture);
            }
        }

        internal static string Other
        {
            get
            {
                return ResourceManager.GetString("Other", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("Microsoft.StyleCop.CSharp.Strings", typeof(Microsoft.StyleCop.CSharp.Strings).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }

        internal static string Static
        {
            get
            {
                return ResourceManager.GetString("Static", resourceCulture);
            }
        }

        internal static string Title
        {
            get
            {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
    }
}

