﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CCNet.Common.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CCNet.Common.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to - found forbidden entry &quot;{0}&quot;.
        /// </summary>
        internal static string ForbiddenEntry {
            get {
                return ResourceManager.GetString("ForbiddenEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to - property {0} is supposed to have value &quot;{1}&quot;, but now it is &quot;{2}&quot;.
        /// </summary>
        internal static string InvalidPropertyValue {
            get {
                return ResourceManager.GetString("InvalidPropertyValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to - missing required entry &quot;{0}&quot;.
        /// </summary>
        internal static string MissingRequiredEntry {
            get {
                return ResourceManager.GetString("MissingRequiredEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to - missing required property {0}: &quot;{1}&quot;.
        /// </summary>
        internal static string MissingRequiredProperty {
            get {
                return ResourceManager.GetString("MissingRequiredProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to - found unexpected property {0}: &quot;{1}&quot;.
        /// </summary>
        internal static string UnexpectedProperty {
            get {
                return ResourceManager.GetString("UnexpectedProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UNKNOWN.
        /// </summary>
        internal static string Unknown {
            get {
                return ResourceManager.GetString("Unknown", resourceCulture);
            }
        }
    }
}
