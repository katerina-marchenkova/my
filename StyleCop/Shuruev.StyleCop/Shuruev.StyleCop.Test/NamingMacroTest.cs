using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shuruev.StyleCop.CSharp;

namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Testing naming macros work.
	/// </summary>
	[TestClass]
	public class NamingMacroTest
	{
		/// <summary>
		/// Tests engine for applying one or several rules.
		/// </summary>
		[TestMethod]
		public void TestRulesApplying()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("Oleg:Shuruev", String.Empty);
			Assert.IsTrue(regex.IsMatch("Oleg"));
			Assert.IsTrue(regex.IsMatch("Shuruev"));
			Assert.IsFalse(regex.IsMatch("oleg"));
			Assert.IsFalse(regex.IsMatch("shuruev"));
			Assert.IsFalse(regex.IsMatch("olegus"));
			Assert.IsFalse(regex.IsMatch("nonshuruev"));

			regex = NamingMacro.BuildRegex("Pre$(AaBb)_Post", String.Empty);
			Assert.IsFalse(regex.IsMatch("Style"));
			Assert.IsFalse(regex.IsMatch("StyleCop"));
			Assert.IsTrue(regex.IsMatch("PreStyle_Post"));
			Assert.IsTrue(regex.IsMatch("PreStyleCop_Post"));
			Assert.IsFalse(regex.IsMatch("PrestyleCop_Post"));
			Assert.IsFalse(regex.IsMatch("PreStyleCop+_Post"));
		}

		/// <summary>
		/// Tests $(AaBb) macro.
		/// </summary>
		[TestMethod]
		public void TestMacroPascal()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(AaBb)", String.Empty);
			Assert.IsTrue(regex.IsMatch("Style"));
			Assert.IsTrue(regex.IsMatch("StyleCop"));
			Assert.IsFalse(regex.IsMatch("styleCop"));
			Assert.IsFalse(regex.IsMatch("StyleCop+"));
			Assert.IsFalse(regex.IsMatch("Style_Cop"));
			Assert.IsFalse(regex.IsMatch("StyleC"));
			Assert.IsFalse(regex.IsMatch("CSharpStyle"));

			regex = NamingMacro.BuildRegex("$(AaBb)", "C");
			Assert.IsTrue(regex.IsMatch("Style"));
			Assert.IsTrue(regex.IsMatch("StyleCop"));
			Assert.IsFalse(regex.IsMatch("styleCop"));
			Assert.IsFalse(regex.IsMatch("StyleCop+"));
			Assert.IsFalse(regex.IsMatch("Style_Cop"));
			Assert.IsTrue(regex.IsMatch("StyleC"));
			Assert.IsTrue(regex.IsMatch("CSharpStyle"));

			regex = NamingMacro.BuildRegex("$(AaBb)", "C X");
			Assert.IsTrue(regex.IsMatch("Style"));
			Assert.IsTrue(regex.IsMatch("StyleCop"));
			Assert.IsTrue(regex.IsMatch("StyleCX"));
			Assert.IsTrue(regex.IsMatch("CXSharpStyle"));

			regex = NamingMacro.BuildRegex("$(AaBb)", String.Empty);
			Assert.IsFalse(regex.IsMatch("Point3D"));

			regex = NamingMacro.BuildRegex("$(AaBb)", "3D");
			Assert.IsTrue(regex.IsMatch("Point3D"));
		}

		/// <summary>
		/// Tests $(aaBb) macro.
		/// </summary>
		[TestMethod]
		public void TestMacroCamel()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(aaBb)", String.Empty);
			Assert.IsTrue(regex.IsMatch("style"));
			Assert.IsTrue(regex.IsMatch("styleCop"));
			Assert.IsTrue(regex.IsMatch("styleCopPlus"));
			Assert.IsFalse(regex.IsMatch("StyleCop"));
			Assert.IsFalse(regex.IsMatch("styleCop+"));
			Assert.IsFalse(regex.IsMatch("style_Cop"));
			Assert.IsFalse(regex.IsMatch("styleC"));
			Assert.IsFalse(regex.IsMatch("styleCSharp"));

			regex = NamingMacro.BuildRegex("$(aaBb)", "C");
			Assert.IsTrue(regex.IsMatch("style"));
			Assert.IsTrue(regex.IsMatch("styleCop"));
			Assert.IsTrue(regex.IsMatch("styleCopPlus"));
			Assert.IsFalse(regex.IsMatch("StyleCop"));
			Assert.IsFalse(regex.IsMatch("styleCop+"));
			Assert.IsFalse(regex.IsMatch("style_Cop"));
			Assert.IsTrue(regex.IsMatch("styleC"));
			Assert.IsTrue(regex.IsMatch("styleCSharp"));

			regex = NamingMacro.BuildRegex("$(aaBb)", "C X");
			Assert.IsTrue(regex.IsMatch("style"));
			Assert.IsTrue(regex.IsMatch("styleCop"));
			Assert.IsTrue(regex.IsMatch("styleCX"));
			Assert.IsTrue(regex.IsMatch("styleCXSharp"));

			regex = NamingMacro.BuildRegex("$(aaBb)", String.Empty);
			Assert.IsFalse(regex.IsMatch("point3D"));

			regex = NamingMacro.BuildRegex("$(aaBb)", "3D");
			Assert.IsTrue(regex.IsMatch("point3D"));
		}

		/// <summary>
		/// Tests $(AA_BB) macro.
		/// </summary>
		[TestMethod]
		public void TestMacroUpper()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(AA_BB)", String.Empty);
			Assert.IsTrue(regex.IsMatch("STYLE"));
			Assert.IsTrue(regex.IsMatch("STYLE_COP"));
			Assert.IsFalse(regex.IsMatch("sTYLE_COP"));
			Assert.IsFalse(regex.IsMatch("STYLE_COP+"));
			Assert.IsFalse(regex.IsMatch("STYLE__COP"));
			Assert.IsFalse(regex.IsMatch("_STYLE"));
			Assert.IsFalse(regex.IsMatch("STYLE_"));
			Assert.IsTrue(regex.IsMatch("STYLE_C"));
			Assert.IsTrue(regex.IsMatch("C_SHARP_STYLE"));

			regex = NamingMacro.BuildRegex("$(AA_BB)", "A B");
			Assert.IsTrue(regex.IsMatch("STYLE"));
			Assert.IsTrue(regex.IsMatch("STYLE_COP"));
		}

		/// <summary>
		/// Tests $(aa_bb) macro.
		/// </summary>
		[TestMethod]
		public void TestMacroLower()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(aa_bb)", String.Empty);
			Assert.IsTrue(regex.IsMatch("style"));
			Assert.IsTrue(regex.IsMatch("style_cop"));
			Assert.IsFalse(regex.IsMatch("Style_cop"));
			Assert.IsFalse(regex.IsMatch("style_cop+"));
			Assert.IsFalse(regex.IsMatch("style__cop"));
			Assert.IsFalse(regex.IsMatch("_style"));
			Assert.IsFalse(regex.IsMatch("style_"));
			Assert.IsTrue(regex.IsMatch("style_c"));
			Assert.IsTrue(regex.IsMatch("c_sharp_style"));

			regex = NamingMacro.BuildRegex("$(aa_bb)", "A B");
			Assert.IsTrue(regex.IsMatch("style"));
			Assert.IsTrue(regex.IsMatch("style_cop"));
		}

		/// <summary>
		/// Tests $(Aa_Bb) macro.
		/// </summary>
		[TestMethod]
		public void TestMacroCapitalized()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(Aa_Bb)", String.Empty);
			Assert.IsTrue(regex.IsMatch("Style"));
			Assert.IsTrue(regex.IsMatch("Style_Cop"));
			Assert.IsFalse(regex.IsMatch("style_Cop"));
			Assert.IsFalse(regex.IsMatch("styleCop"));
			Assert.IsFalse(regex.IsMatch("StyleCop"));
			Assert.IsFalse(regex.IsMatch("Style_Cop+"));
			Assert.IsFalse(regex.IsMatch("Style__Cop"));
			Assert.IsFalse(regex.IsMatch("_Style"));
			Assert.IsFalse(regex.IsMatch("Style_"));
			Assert.IsFalse(regex.IsMatch("StyleC"));
			Assert.IsFalse(regex.IsMatch("Style_C"));
			Assert.IsFalse(regex.IsMatch("CSharpStyle"));
			Assert.IsFalse(regex.IsMatch("CSharp_Style"));
			Assert.IsFalse(regex.IsMatch("C_Sharp_Style"));

			regex = NamingMacro.BuildRegex("$(Aa_Bb)", "C");
			Assert.IsTrue(regex.IsMatch("Style"));
			Assert.IsTrue(regex.IsMatch("Style_Cop"));
			Assert.IsFalse(regex.IsMatch("style_Cop"));
			Assert.IsFalse(regex.IsMatch("styleCop"));
			Assert.IsFalse(regex.IsMatch("StyleCop"));
			Assert.IsFalse(regex.IsMatch("Style_Cop+"));
			Assert.IsFalse(regex.IsMatch("Style__Cop"));
			Assert.IsFalse(regex.IsMatch("StyleC"));
			Assert.IsTrue(regex.IsMatch("Style_C"));
			Assert.IsFalse(regex.IsMatch("CSharpStyle"));
			Assert.IsFalse(regex.IsMatch("CSharp_Style"));
			Assert.IsTrue(regex.IsMatch("C_Sharp_Style"));

			regex = NamingMacro.BuildRegex("$(Aa_Bb)", "C X");
			Assert.IsTrue(regex.IsMatch("Style"));
			Assert.IsTrue(regex.IsMatch("Style_Cop"));
			Assert.IsFalse(regex.IsMatch("StyleCX"));
			Assert.IsFalse(regex.IsMatch("Style_CX"));
			Assert.IsTrue(regex.IsMatch("Style_C_X"));
			Assert.IsFalse(regex.IsMatch("StyleC_X"));
			Assert.IsFalse(regex.IsMatch("CXSharpStyle"));
			Assert.IsFalse(regex.IsMatch("C_XSharpStyle"));
			Assert.IsFalse(regex.IsMatch("C_X_SharpStyle"));
			Assert.IsTrue(regex.IsMatch("C_X_Sharp_Style"));
			Assert.IsFalse(regex.IsMatch("CX_Sharp_Style"));
			Assert.IsFalse(regex.IsMatch("CXSharp_Style"));

			regex = NamingMacro.BuildRegex("$(Aa_Bb)", "CX");
			Assert.IsTrue(regex.IsMatch("Style"));
			Assert.IsTrue(regex.IsMatch("Style_Cop"));
			Assert.IsFalse(regex.IsMatch("StyleCX"));
			Assert.IsTrue(regex.IsMatch("Style_CX"));
			Assert.IsFalse(regex.IsMatch("Style_C_X"));
			Assert.IsFalse(regex.IsMatch("StyleC_X"));
			Assert.IsFalse(regex.IsMatch("CXSharpStyle"));
			Assert.IsFalse(regex.IsMatch("C_XSharpStyle"));
			Assert.IsFalse(regex.IsMatch("C_X_SharpStyle"));
			Assert.IsFalse(regex.IsMatch("C_X_Sharp_Style"));
			Assert.IsTrue(regex.IsMatch("CX_Sharp_Style"));
			Assert.IsFalse(regex.IsMatch("CXSharp_Style"));

			regex = NamingMacro.BuildRegex("$(Aa_Bb)", String.Empty);
			Assert.IsFalse(regex.IsMatch("Point_3D"));

			regex = NamingMacro.BuildRegex("$(Aa_Bb)", "3D");
			Assert.IsTrue(regex.IsMatch("Point_3D"));
		}
	}
}
