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
    internal class Errors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Errors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CCNet.ProjectChecker.Properties.Errors", typeof(Errors).Assembly);
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
        ///   Looks up a localized string similar to The following files were considered as forbidden by some reason:
        ///{0}
        ///Please do not use them in your project..
        /// </summary>
        internal static string ForbiddenFiles {
            get {
                return ResourceManager.GetString("ForbiddenFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found unknown configuration {0}.
        ///Use standard configurations only: Debug and Release..
        /// </summary>
        internal static string UnknownConfiguration {
            get {
                return ResourceManager.GetString("UnknownConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Some assembly information seem not to conform the agreements:
        ///{0}.
        /// </summary>
        internal static string WrongAssemblyInfoContents {
            get {
                return ResourceManager.GetString("WrongAssemblyInfoContents", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assembly information file should be named as AssemblyInfo.cs and should be placed in Properties folder..
        /// </summary>
        internal static string WrongAssemblyInfoFileLocation {
            get {
                return ResourceManager.GetString("WrongAssemblyInfoFileLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Some project properties seem not to conform the agreements:
        ///{0}.
        /// </summary>
        internal static string WrongCommonProperties {
            get {
                return ResourceManager.GetString("WrongCommonProperties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The project should have configuration file named as {0} and placed in root folder..
        /// </summary>
        internal static string WrongConfigFileLocation {
            get {
                return ResourceManager.GetString("WrongConfigFileLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Some project properties for Debug configuration seem not to conform the agreements:
        ///{0}.
        /// </summary>
        internal static string WrongDebugProperties {
            get {
                return ResourceManager.GetString("WrongDebugProperties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Some properties for {0} file seem not to conform the agreements:
        ///{1}.
        /// </summary>
        internal static string WrongFileProperties {
            get {
                return ResourceManager.GetString("WrongFileProperties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All files under source control should match the files included into project:
        ///{0}
        ///Include necessary files into project or remove them from source control..
        /// </summary>
        internal static string WrongFileSet {
            get {
                return ResourceManager.GetString("WrongFileSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Some manifest file contents seem not to conform the agreements:
        ///{0}.
        /// </summary>
        internal static string WrongManifestContents {
            get {
                return ResourceManager.GetString("WrongManifestContents", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Manifest file should be named as App.manifest and should be placed in Properties folder..
        /// </summary>
        internal static string WrongManifestFileLocation {
            get {
                return ResourceManager.GetString("WrongManifestFileLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project is supposed to use the only {0} platform, but now it uses {1}..
        /// </summary>
        internal static string WrongPlatform {
            get {
                return ResourceManager.GetString("WrongPlatform", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project file should be named as {0}.csproj and should be placed in project root folder..
        /// </summary>
        internal static string WrongProjectFileLocation {
            get {
                return ResourceManager.GetString("WrongProjectFileLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Some references seem not to conform the agreements:
        ///{0}
        ///Try to resolve errors by removing references and adding them again in the right way..
        /// </summary>
        internal static string WrongReferences {
            get {
                return ResourceManager.GetString("WrongReferences", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Some project properties for Release configuration seem not to conform the agreements:
        ///{0}.
        /// </summary>
        internal static string WrongReleaseProperties {
            get {
                return ResourceManager.GetString("WrongReleaseProperties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project is supposed to be created in Visual Studio {0}, but now it seems to be created in Visual Studio {1}..
        /// </summary>
        internal static string WrongVisualStudioVersion {
            get {
                return ResourceManager.GetString("WrongVisualStudioVersion", resourceCulture);
            }
        }
    }
}
