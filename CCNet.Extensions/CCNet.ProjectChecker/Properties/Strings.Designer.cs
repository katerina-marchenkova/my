﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CCNet.ProjectChecker.Properties {
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
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CCNet.ProjectChecker.Properties.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to - {0} should not specify &quot;{1}&quot; property directly.
        /// </summary>
        internal static string DontSpecifyPropertyDirectly {
            get {
                return ResourceManager.GetString("DontSpecifyPropertyDirectly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to - {0} should be a reference to a binary, not to a project.
        /// </summary>
        internal static string DontUseProjectReference {
            get {
                return ResourceManager.GetString("DontUseProjectReference", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to - {0} should not use specific version.
        /// </summary>
        internal static string DontUseSpecificVersion {
            get {
                return ResourceManager.GetString("DontUseSpecificVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to - {0} should use specific version.
        /// </summary>
        internal static string UseSpecificVersion {
            get {
                return ResourceManager.GetString("UseSpecificVersion", resourceCulture);
            }
        }
    }
}
