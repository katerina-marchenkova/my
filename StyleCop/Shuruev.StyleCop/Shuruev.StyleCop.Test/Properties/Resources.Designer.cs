﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shuruev.StyleCop.Test.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Shuruev.StyleCop.Test.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to //# [ERROR]
        /////# Method name starts with lower case letter.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public void testMethod()
        ///		{
        ///			int a = 10;
        ///		}
        ///	}
        ///}
        /////# [OK]
        /////# Method name starts with upper case letter.
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
        /////# [ERROR]
        /////# Windows forms event handler is not private.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public void  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SP1300 {
            get {
                return ResourceManager.GetString("SP1300", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //# [OK]
        /////# Well-formed example.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public void TestMethod()
        ///		{
        ///			for (int i = 0; i &lt; 10; i++)
        ///			{
        ///			}
        ///		}
        ///	}
        ///}
        /////# [ERROR]
        /////# Excess blank line.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public void TestMethod()
        ///		{
        ///			for (int i = 0; i &lt; 10; i++)
        ///
        ///			{
        ///			}
        ///		}
        ///	}
        ///}
        /////# [ERROR]
        /////# Excess blank line.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class TestClass
        ///	{
        ///		public void TestMethod( [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SP1509 {
            get {
                return ResourceManager.GetString("SP1509", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //# [ERROR]
        /////# Public class doesn&apos;t have summary.
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
        /////# [ERROR]
        /////# Public method doesn&apos;t have summary.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	/// &lt;summary&gt;
        ///	/// Hello there.
        ///	/// &lt;/summary&gt;
        ///	public class TestClass
        ///	{
        ///		public void TestMethod()
        ///		{
        ///			int a = 10;
        ///		}
        ///	}
        ///}
        /////# [OK]
        /////# Both class and method have summary.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	/// &lt;summary&gt; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SP1600 {
            get {
                return ResourceManager.GetString("SP1600", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //# [ERROR]
        /////# There is excess whitespace at the end of the line.
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
        /////# [ERROR]
        /////# There is excess whitespace at the end of the line.
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
        /////# [ERROR]
        /////# There are excess whitespaces at the end of the line.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	public class [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SP2000 {
            get {
                return ResourceManager.GetString("SP2000", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //# [OK]
        /////# Spacing uses identical characters.
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
        /////# [ERROR]
        /////# One spacing uses tabs and whitespaces.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///	 public class TestClass
        ///	{
        ///		public void TestMethod()
        ///		{
        ///			int a = 10;
        ///		}
        ///	}
        ///}
        /////# [OK]
        /////# Spacing uses identical characters.
        ///namespace Shuruev.StyleCop.Test
        ///{
        ///    public class TestClass
        ///	{
        ///		public void TestMethod()
        ///		{
        ///	 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SP2001 {
            get {
                return ResourceManager.GetString("SP2001", resourceCulture);
            }
        }
    }
}
