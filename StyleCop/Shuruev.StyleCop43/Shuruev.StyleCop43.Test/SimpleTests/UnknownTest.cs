using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shuruev.StyleCop.CSharp;

namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// XXX.
	/// </summary>
	[TestClass]
	public class UnknownTest
	{
		/// <summary>
		/// Internal caller.
		/// </summary>
		private static string SplitIntoWords(string text)
		{
			List<string> words = AdvancedNamingRules.SplitIntoWords(text);
			return String.Join(", ", words);
		}

		[TestMethod]
		public void Split_Into_Words()
		{
			Assert.AreEqual("style", SplitIntoWords("style"));
			Assert.AreEqual("style, cop", SplitIntoWords("style cop"));

			Assert.AreEqual("Style, Cop", SplitIntoWords("StyleCop"));
			Assert.AreEqual("Style, Cop", SplitIntoWords("StyleCop+"));
			Assert.AreEqual("Style, Cop", SplitIntoWords("Style_Cop"));
			Assert.AreEqual("Style, Cop", SplitIntoWords("Style Cop"));
			Assert.AreEqual("Style, Cop", SplitIntoWords("Style,   Cop."));

			Assert.AreEqual("Style, Cop, 123", SplitIntoWords("StyleCop123"));
			Assert.AreEqual("Style, 123, Cop", SplitIntoWords("Style123Cop"));
			Assert.AreEqual("Style, Cop, 123", SplitIntoWords("Style, Cop_123!..."));
			Assert.AreEqual("STYLE, COP, 123", SplitIntoWords("STYLE_COP_123"));
			Assert.AreEqual("STYLE, COP, 123", SplitIntoWords("STYLE_COP123"));

			Assert.AreEqual("style, Cop", SplitIntoWords("styleCop"));
			Assert.AreEqual("STYLE, COP", SplitIntoWords("STYLE_COP"));
			Assert.AreEqual("style, cop", SplitIntoWords("style_cop"));
			Assert.AreEqual("Style, Cop", SplitIntoWords("Style_Cop"));
		}
	}
}
