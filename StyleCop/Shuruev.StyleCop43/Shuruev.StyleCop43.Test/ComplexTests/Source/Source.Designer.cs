﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shuruev.StyleCop.Test.ComplexTests {
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
    internal class Source {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Source() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Shuruev.StyleCop.Test.ComplexTests.Source.Source", typeof(Source).Assembly);
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
        ///   Looks up a localized string similar to #region AdvancedNamingRules // Methods
        ///
        /////# [OK]
        /////# Method name is correct.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public void TestMethod()
        ///		{
        ///		}
        ///	}
        ///}
        /////# [END]
        ///
        /////# [ERROR]
        /////# Method name is incorrect.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public void Test_Method()
        ///		{
        ///		}
        ///	}
        ///}
        /////# [END]
        ///
        /////# [OK]
        /////# Test method names are correct.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		[TestMethod]
        ///		public void [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AdvancedNamingRules {
            get {
                return ResourceManager.GetString("AdvancedNamingRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #region AdvancedNamingRules // Skipping auto-generated files
        ///
        /////# (AdvancedNaming_Namespace = $(AaBb))
        ///
        /////# [ERROR]
        /////# Namespace name is incorrect.
        ///namespace Shuruev.StyleCop.TEST
        ///{
        ///}
        /////# [END]
        ///
        /////# [OK]
        /////# Incorrect namespace name is skipped due to &lt;auto-generated&gt; node.
        /////------------------------------------------------------------------------------
        ///// &lt;auto-generated&gt;
        /////     This code was generated by a tool.
        ///// &lt;/auto-generated&gt;
        /////------------------------------------------------------ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Environmental {
            get {
                return ResourceManager.GetString("Environmental", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #region ElementMustNotBeOnSingleLine // SP1502
        ///
        /////# [OK]
        /////# Well-formed example.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public TestClass()
        ///		{
        ///		}
        ///	}
        ///}
        /////# [END]
        ///
        /////# [OK]
        /////# Special style for constructors.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public TestClass()
        ///		{ }
        ///	}
        ///}
        /////# [END]
        ///
        /////# [OK]
        /////# Special style for constructors.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public TestClass() { }
        ///	}
        ///}
        /////# [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ExtendedOriginalRules {
            get {
                return ResourceManager.GetString("ExtendedOriginalRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #region CodeLineMustNotEndWithWhitespace
        ///
        /////# [OK]
        /////# Source file is OK.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public void TestMethod()
        ///		{
        ///			int a = 10;
        ///		}
        ///	}
        ///}
        /////# [END]
        ///
        /////# [OK]
        /////# Source file without a line break at the end.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public void TestMethod()
        ///		{
        ///
        ///			int a = 10;
        ///		}
        ///	}
        ///}//# [END]
        ///
        /////# [ERROR]
        /////# There is excess whitespace at the end of the line.
        ///namespace Shuruev.StyleCop. [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MoreCustomRules {
            get {
                return ResourceManager.GetString("MoreCustomRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string OneRun {
            get {
                return ResourceManager.GetString("OneRun", resourceCulture);
            }
        }
    }
}
