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
		#region General tests

		[TestMethod]
		public void Apply_Rules_Without_Macros()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("Oleg:Shuruev", String.Empty);
			Assert.IsTrue(regex.IsMatch("Oleg"));
			Assert.IsTrue(regex.IsMatch("Shuruev"));
			Assert.IsFalse(regex.IsMatch("oleg"));
			Assert.IsFalse(regex.IsMatch("shuruev"));
			Assert.IsFalse(regex.IsMatch("olegus"));
			Assert.IsFalse(regex.IsMatch("nonshuruev"));
		}

		[TestMethod]
		public void Apply_Rules_With_Pascal_Macro()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("Pre$(AaBb)_POST", String.Empty);
			Assert.IsFalse(regex.IsMatch("Style"));
			Assert.IsFalse(regex.IsMatch("StyleCop"));
			Assert.IsTrue(regex.IsMatch("PreStyle_POST"));
			Assert.IsTrue(regex.IsMatch("PreStyleCop_POST"));
			Assert.IsFalse(regex.IsMatch("PrestyleCop_POST"));
			Assert.IsFalse(regex.IsMatch("PreStyleCop+_POST"));
		}

		[TestMethod]
		public void Apply_Rules_With_Any_Macro()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("Pre$(*)_POST", String.Empty);
			Assert.IsFalse(regex.IsMatch("Style"));
			Assert.IsFalse(regex.IsMatch("StyleCop"));
			Assert.IsTrue(regex.IsMatch("PreStyle_POST"));
			Assert.IsFalse(regex.IsMatch("PREStyleCOP+_POST"));
			Assert.IsFalse(regex.IsMatch("PreStyleCOP+POST"));
			Assert.IsFalse(regex.IsMatch("PreStyleCOP+_Post"));
			Assert.IsTrue(regex.IsMatch("PreStyleCOP+_POST"));
		}

		#endregion

		#region Macro "Pascal"

		[TestMethod]
		public void Macro_Pascal_General()
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
		}

		[TestMethod]
		public void Macro_Pascal_Abbreviations()
		{
			Regex regex;

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

		[TestMethod]
		public void Macro_Pascal_Single_Letter()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(AaBb)", String.Empty);
			Assert.IsFalse(regex.IsMatch("a"));
			Assert.IsTrue(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("$(AaBb)", "A");
			Assert.IsFalse(regex.IsMatch("a"));
			Assert.IsTrue(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("Pre$(AaBb)_POST", String.Empty);
			Assert.IsFalse(regex.IsMatch("Prea_POST"));
			Assert.IsTrue(regex.IsMatch("PreA_POST"));

			regex = NamingMacro.BuildRegex("Pre$(AaBb)_POST", "A");
			Assert.IsFalse(regex.IsMatch("Prea_POST"));
			Assert.IsTrue(regex.IsMatch("PreA_POST"));
		}

		#endregion

		#region Macro "Camel"

		[TestMethod]
		public void Macro_Camel_General()
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
		}

		[TestMethod]
		public void Macro_Camel_Abbreviations()
		{
			Regex regex;

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

		[TestMethod]
		public void Macro_Camel_Single_Letter()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(aaBb)", String.Empty);
			Assert.IsTrue(regex.IsMatch("a"));
			Assert.IsFalse(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("$(aaBb)", "A");
			Assert.IsTrue(regex.IsMatch("a"));
			Assert.IsFalse(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("Pre$(aaBb)_POST", String.Empty);
			Assert.IsTrue(regex.IsMatch("Prea_POST"));
			Assert.IsFalse(regex.IsMatch("PreA_POST"));

			regex = NamingMacro.BuildRegex("Pre$(aaBb)_POST", "A");
			Assert.IsTrue(regex.IsMatch("Prea_POST"));
			Assert.IsFalse(regex.IsMatch("PreA_POST"));
		}

		#endregion

		#region Macro "Upper"

		[TestMethod]
		public void Macro_Upper_General()
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
		}

		[TestMethod]
		public void Macro_Upper_Abbreviations()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(AA_BB)", "A B");
			Assert.IsTrue(regex.IsMatch("STYLE"));
			Assert.IsTrue(regex.IsMatch("STYLE_COP"));
		}

		[TestMethod]
		public void Macro_Upper_Single_Letter()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(AA_BB)", String.Empty);
			Assert.IsFalse(regex.IsMatch("a"));
			Assert.IsTrue(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("$(AA_BB)", "A");
			Assert.IsFalse(regex.IsMatch("a"));
			Assert.IsTrue(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("Pre$(AA_BB)_POST", String.Empty);
			Assert.IsFalse(regex.IsMatch("Prea_POST"));
			Assert.IsTrue(regex.IsMatch("PreA_POST"));

			regex = NamingMacro.BuildRegex("Pre$(AA_BB)_POST", "A");
			Assert.IsFalse(regex.IsMatch("Prea_POST"));
			Assert.IsTrue(regex.IsMatch("PreA_POST"));
		}

		#endregion

		#region Macro "Lower"

		[TestMethod]
		public void Macro_Lower_General()
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
		}

		[TestMethod]
		public void Macro_Lower_Abbreviations()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(aa_bb)", "A B");
			Assert.IsTrue(regex.IsMatch("style"));
			Assert.IsTrue(regex.IsMatch("style_cop"));
		}

		[TestMethod]
		public void Macro_Lower_Single_Letter()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(aa_bb)", String.Empty);
			Assert.IsTrue(regex.IsMatch("a"));
			Assert.IsFalse(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("$(aa_bb)", "A");
			Assert.IsTrue(regex.IsMatch("a"));
			Assert.IsFalse(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("Pre$(aa_bb)_POST", String.Empty);
			Assert.IsTrue(regex.IsMatch("Prea_POST"));
			Assert.IsFalse(regex.IsMatch("PreA_POST"));

			regex = NamingMacro.BuildRegex("Pre$(aa_bb)_POST", "A");
			Assert.IsTrue(regex.IsMatch("Prea_POST"));
			Assert.IsFalse(regex.IsMatch("PreA_POST"));
		}

		#endregion

		#region Macro "Capitalized"

		[TestMethod]
		public void Macro_Capitalized_General()
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
		}

		[TestMethod]
		public void Macro_Capitalized_Abbreviations()
		{
			Regex regex;

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

		[TestMethod]
		public void Macro_Capitalized_Single_Letter()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(Aa_Bb)", String.Empty);
			Assert.IsFalse(regex.IsMatch("a"));
			Assert.IsTrue(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("$(Aa_Bb)", "A");
			Assert.IsFalse(regex.IsMatch("a"));
			Assert.IsTrue(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("Pre$(Aa_Bb)_POST", String.Empty);
			Assert.IsFalse(regex.IsMatch("Prea_POST"));
			Assert.IsTrue(regex.IsMatch("PreA_POST"));

			regex = NamingMacro.BuildRegex("Pre$(Aa_Bb)_POST", "A");
			Assert.IsFalse(regex.IsMatch("Prea_POST"));
			Assert.IsTrue(regex.IsMatch("PreA_POST"));
		}

		#endregion

		#region Macro "Any"

		[TestMethod]
		public void Macro_Any_General()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(*)", String.Empty);
			Assert.IsTrue(regex.IsMatch("Style"));
			Assert.IsTrue(regex.IsMatch("Style_COP"));
			Assert.IsTrue(regex.IsMatch("style_Cop++CSharp"));
		}

		[TestMethod]
		public void Macro_Any_Abbreviations()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(*)", String.Empty);
			Assert.IsTrue(regex.IsMatch("Point3D_3D"));

			regex = NamingMacro.BuildRegex("$(*)", "3D");
			Assert.IsTrue(regex.IsMatch("Point3D_3D"));
		}

		[TestMethod]
		public void Macro_Any_Single_Letter()
		{
			Regex regex;

			regex = NamingMacro.BuildRegex("$(*)", String.Empty);
			Assert.IsTrue(regex.IsMatch("a"));
			Assert.IsTrue(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("$(*)", "A");
			Assert.IsTrue(regex.IsMatch("a"));
			Assert.IsTrue(regex.IsMatch("A"));

			regex = NamingMacro.BuildRegex("Pre$(*)_POST", String.Empty);
			Assert.IsTrue(regex.IsMatch("Prea_POST"));
			Assert.IsTrue(regex.IsMatch("PreA_POST"));

			regex = NamingMacro.BuildRegex("Pre$(*)_POST", "A");
			Assert.IsTrue(regex.IsMatch("Prea_POST"));
			Assert.IsTrue(regex.IsMatch("PreA_POST"));
		}

		#endregion
	}
}
