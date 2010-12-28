using System;
using System.Collections.Generic;
using System.IO;
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
			AnalyzePlainText(doc);
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

		#region Plain text analysis

		/// <summary>
		/// Analyzes source code as plain text.
		/// </summary>
		private void AnalyzePlainText(CsDocument document)
		{
			var indentOptions = GetOptionsData<IndentOptionsData>(document, Rules.CheckAllowedIndentationCharacters);
			var lastLineOptions = GetOptionsData<LastLineOptionsData>(document, Rules.CheckWhetherLastCodeLineIsEmpty);
			var charLimitOptions = GetOptionsData<CharLimitOptionsData>(document, Rules.CodeLineMustNotBeLongerThan);

			string source;
			using (TextReader reader = document.SourceCode.Read())
			{
				source = reader.ReadToEnd();
			}

			List<string> lines = new List<string>(source.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

			for (int i = 0; i < lines.Count; i++)
			{
				int lineNumber = i + 1;
				string lineText = lines[i];

				CheckLineEnding(document, lineText, lineNumber);
				CheckIndentation(document, lineText, lineNumber, indentOptions);
				CheckLineLength(document, lineText, lineNumber, charLimitOptions);
			}

			CheckLastLine(document, source, lines.Count, lastLineOptions);
		}

		/// <summary>
		/// Checks the ending of specified code line.
		/// </summary>
		private void CheckLineEnding(CsDocument document, string lineText, int lineNumber)
		{
			if (lineText.Length == 0)
				return;

			char lastChar = lineText[lineText.Length - 1];
			if (Char.IsWhiteSpace(lastChar))
			{
				m_parent.AddViolation(
					document.RootElement,
					lineNumber,
					Rules.CodeLineMustNotEndWithWhitespace);
			}
		}

		/// <summary>
		/// Checks indentation in specified code line.
		/// </summary>
		private void CheckIndentation(CsDocument document, string lineText, int lineNumber, IndentOptionsData indentOptions)
		{
			if (lineText.Trim().Length == 0)
				return;

			bool containsTabs = false;
			bool containsSpaces = false;
			foreach (char c in lineText)
			{
				if (!Char.IsWhiteSpace(c))
					break;

				switch (c)
				{
					case '\t':
						containsTabs = true;
						break;

					case ' ':
						containsSpaces = true;
						break;
				}
			}

			bool failed = true;
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
					lineNumber,
					Rules.CheckAllowedIndentationCharacters,
					indentOptions.GetContextValues());
			}
		}

		/// <summary>
		/// Checks length of specified code line.
		/// </summary>
		private void CheckLineLength(CsDocument document, IEnumerable<char> lineText, int lineNumber, CharLimitOptionsData charLimitOptions)
		{
			int length = 0;
			foreach (char c in lineText)
			{
				if (c == '\t')
				{
					length += charLimitOptions.TabSize.Value;
				}
				else
				{
					length += 1;
				}
			}

			if (length > charLimitOptions.Limit.Value)
			{
				m_parent.AddViolation(
					document.RootElement,
					lineNumber,
					Rules.CodeLineMustNotBeLongerThan,
					charLimitOptions.Limit.Value,
					length);
			}
		}

		/// <summary>
		/// Checks the last code line.
		/// </summary>
		private void CheckLastLine(CsDocument document, string sourceText, int lineNumber, LastLineOptionsData lastLineOptions)
		{
			bool passed = false;
			switch (lastLineOptions.Mode)
			{
				case LastLineMode.Empty:
					passed = sourceText.EndsWith(Environment.NewLine);
					break;

				case LastLineMode.NotEmpty:
					passed = !sourceText.EndsWith(Environment.NewLine);
					break;
			}

			if (!passed)
			{
				m_parent.AddViolation(
					document.RootElement,
					lineNumber,
					Rules.CheckWhetherLastCodeLineIsEmpty,
					lastLineOptions.GetContextValues());
			}
		}

		#endregion
	}
}
