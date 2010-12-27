using System;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// More custom rules represented by StyleCop+ plug-in.
	/// </summary>
	public class MoreCustomRules
	{
		private readonly StyleCopPlus m_parent;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public MoreCustomRules(StyleCopPlus parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			m_parent = parent;
		}

		/// <summary>
		/// Analyzes source document.
		/// </summary>
		public void AnalyzeDocument(CodeDocument document)
		{
			CsDocument doc = (CsDocument)document;
			FullTokenResearch(doc);
		}

		#region Full token research

		/// <summary>
		/// Iterates through all tokens in the document
		/// </summary>
		private void FullTokenResearch(CsDocument document)
		{
			//xxx
			CustomRule rule = CustomRules.Get(Rules.CodeLineMustBeginWithIdenticalWhitespaces);
			IndentOptionsData data = (IndentOptionsData)rule.CreateOptionsData();
			string settingValue = SettingsManager.GetValue<string>(
				m_parent,
				document.Settings,
				rule.SettingName);
			data.ConvertFromValue(settingValue);

			for (Node<CsToken> node = document.Tokens.First; node != null; node = node.Next)
			{
				ResearchLineEnding(document, node);
				ResearchLineBeginning(document, node, data);
			}
		}

		/// <summary>
		/// Researches the ending of the line.
		/// </summary>
		private void ResearchLineEnding(CsDocument document, Node<CsToken> node)
		{
			if (node.Next != null && node.Next.Value.CsTokenType != CsTokenType.EndOfLine)
				return;

			if (node.Value.CsTokenType == CsTokenType.WhiteSpace)
			{
				m_parent.AddViolation(
					document.RootElement,
					node.Value.LineNumber,
					Rules.CodeLineMustNotEndWithWhitespace);
			}
		}

		/// <summary>
		/// Researches the beginning of the line.
		/// </summary>
		private void ResearchLineBeginning(CsDocument document, Node<CsToken> node, IndentOptionsData indentOptions)
		{
			if (node.Previous == null || node.Previous.Value.CsTokenType != CsTokenType.EndOfLine)
				return;

			if (node.Value.CsTokenType != CsTokenType.WhiteSpace)
				return;

			bool containsTabs = node.Value.Text.Contains("\t");
			bool containsSpaces = node.Value.Text.Contains(" ");

			bool failed = false;
			switch (indentOptions.Mode)
			{
				case IndentMode.Tabs:
					failed = containsSpaces;
					break;

				case IndentMode.Spaces:
					failed = containsTabs;
					break;

				case IndentMode.Both:
					failed = containsSpaces && containsTabs;
					break;
			}

			if (failed)
			{
				m_parent.AddViolation(
					document.RootElement,
					node.Value.LineNumber,
					Rules.CodeLineMustBeginWithIdenticalWhitespaces);
			}
		}

		#endregion
	}
}
