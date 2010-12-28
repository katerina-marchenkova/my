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

		#region Working with settings

		/// <summary>
		/// Gets options data for specified rule.
		/// </summary>
		private T GetOptionsData<T>(CodeDocument document, Rules rule) where T : ICustomRuleOptionsData
		{
			CustomRule customRule = CustomRules.Get(rule);
			T data = (T)customRule.CreateOptionsData();

			string settingValue = SettingsManager.GetValue<string>(m_parent, document.Settings, customRule.SettingName);
			data.ConvertFromValue(settingValue);

			return data;
		}

		#endregion

		#region Full token research

		/// <summary>
		/// Iterates through all tokens in the document.
		/// </summary>
		private void FullTokenResearch(CsDocument document)
		{
			var indentOptions = GetOptionsData<IndentOptionsData>(document, Rules.CheckAllowedIndentationCharacters);
			var lastLineOptions = GetOptionsData<LastLineOptionsData>(document, Rules.CheckWhetherLastCodeLineIsEmpty);

			for (Node<CsToken> node = document.Tokens.First; node != null; node = node.Next)
			{
				ResearchLineEnding(document, node);
				ResearchLineBeginning(document, node, indentOptions);
			}

			ResearchLastLine(document, lastLineOptions);
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
					Rules.CheckAllowedIndentationCharacters,
					indentOptions.GetContextValues());
			}
		}

		/// <summary>
		/// Researches the last line.
		/// </summary>
		private void ResearchLastLine(CsDocument document, LastLineOptionsData lastLineOptions)
		{
			CsToken token = document.Tokens.Last.Value;

			bool failed = false;
			switch (lastLineOptions.Mode)
			{
				case LastLineMode.Empty:
					failed = token.CsTokenType != CsTokenType.EndOfLine;
					break;

				case LastLineMode.NotEmpty:
					failed = token.CsTokenType == CsTokenType.EndOfLine;
					break;
			}

			if (failed)
			{
				m_parent.AddViolation(
					document.RootElement,
					token.LineNumber,
					Rules.CheckWhetherLastCodeLineIsEmpty,
					lastLineOptions.GetContextValues());
			}
		}

		#endregion
	}
}
