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
			CustomRulesSettings settings = new CustomRulesSettings();
			settings.Initialize(m_parent, document);

			CsDocument doc = (CsDocument)document;
			AnalyzePlainText(doc, settings);
			AnalyzeElements(doc.RootElement.ChildElements, settings);
		}

		#region Plain text analysis

		/// <summary>
		/// Analyzes source code as plain text.
		/// </summary>
		private void AnalyzePlainText(CsDocument document, CustomRulesSettings settings)
		{
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
				CheckIndentation(document, lineText, lineNumber, settings);
				CheckLineLength(document, lineText, lineNumber, settings);
			}

			CheckLastLine(document, source, lines.Count, settings);
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
		private void CheckIndentation(CsDocument document, string lineText, int lineNumber, CustomRulesSettings settings)
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
			switch (settings.IndentOptions.Mode)
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
					settings.IndentOptions.GetContextValues());
			}
		}

		/// <summary>
		/// Checks length of specified code line.
		/// </summary>
		private void CheckLineLength(CsDocument document, string lineText, int lineNumber, CustomRulesSettings settings)
		{
			int length = 0;
			foreach (char c in lineText)
			{
				if (c == '\t')
				{
					length += settings.CharLimitOptions.TabSize.Value;
				}
				else
				{
					length += 1;
				}
			}

			if (length > settings.CharLimitOptions.Limit.Value)
			{
				m_parent.AddViolation(
					document.RootElement,
					lineNumber,
					Rules.CodeLineMustNotBeLongerThan,
					settings.CharLimitOptions.Limit.Value,
					length);
			}
		}

		/// <summary>
		/// Checks the last code line.
		/// </summary>
		private void CheckLastLine(CsDocument document, string sourceText, int lineNumber, CustomRulesSettings settings)
		{
			bool passed = false;
			switch (settings.LastLineOptions.Mode)
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
					settings.LastLineOptions.GetContextValues());
			}
		}

		#endregion

		#region Analysis by elements

		/// <summary>
		/// Analyzes a collection of elements.
		/// </summary>
		private void AnalyzeElements(IEnumerable<CsElement> elements, CustomRulesSettings settings)
		{
			foreach (CsElement element in elements)
			{
				AnalyzeElement(element, settings);
				AnalyzeElements(element.ChildElements, settings);
			}
		}

		/// <summary>
		/// Analyzes specified element.
		/// </summary>
		private void AnalyzeElement(CsElement element, CustomRulesSettings settings)
		{
			switch (element.ElementType)
			{
				case ElementType.Constructor:
					AnalyzeConstructor(element, settings);
					break;
				case ElementType.Destructor:
					AnalyzeDestructor(element, settings);
					break;
				case ElementType.Indexer:
					AnalyzeIndexer(element, settings);
					break;
				case ElementType.Method:
					AnalyzeMethod(element, settings);
					break;
				case ElementType.Property:
					AnalyzeProperty(element, settings);
					break;
			}
		}

		/// <summary>
		/// Analyzes constructor element.
		/// </summary>
		private void AnalyzeConstructor(CsElement element, CustomRulesSettings settings)
		{
			CheckSizeLimit(
				element,
				Rules.MethodMustNotContainMoreLinesThan,
				settings.MethodSizeOptions.Limit);
		}

		/// <summary>
		/// Analyzes destructor element.
		/// </summary>
		private void AnalyzeDestructor(CsElement element, CustomRulesSettings settings)
		{
			CheckSizeLimit(
				element,
				Rules.MethodMustNotContainMoreLinesThan,
				settings.MethodSizeOptions.Limit);
		}

		/// <summary>
		/// Analyzes indexer element.
		/// </summary>
		private void AnalyzeIndexer(CsElement element, CustomRulesSettings settings)
		{
			Indexer indexer = (Indexer)element;

			if (indexer.GetAccessor != null)
			{
				CheckSizeLimit(
					indexer.GetAccessor,
					Rules.PropertyMustNotContainMoreLinesThan,
					settings.PropertySizeOptions.Limit);
			}

			if (indexer.SetAccessor != null)
			{
				CheckSizeLimit(
					indexer.SetAccessor,
					Rules.PropertyMustNotContainMoreLinesThan,
					settings.PropertySizeOptions.Limit);
			}
		}

		/// <summary>
		/// Analyzes method element.
		/// </summary>
		private void AnalyzeMethod(CsElement element, CustomRulesSettings settings)
		{
			CheckSizeLimit(
				element,
				Rules.MethodMustNotContainMoreLinesThan,
				settings.MethodSizeOptions.Limit);
		}

		/// <summary>
		/// Analyzes property element.
		/// </summary>
		private void AnalyzeProperty(CsElement element, CustomRulesSettings settings)
		{
			Property property = (Property)element;

			if (property.GetAccessor != null)
			{
				CheckSizeLimit(
					property.GetAccessor,
					Rules.PropertyMustNotContainMoreLinesThan,
					settings.PropertySizeOptions.Limit);
			}

			if (property.SetAccessor != null)
			{
				CheckSizeLimit(
					property.SetAccessor,
					Rules.PropertyMustNotContainMoreLinesThan,
					settings.PropertySizeOptions.Limit);
			}
		}

		/// <summary>
		/// Checks if specified element violates size limit.
		/// </summary>
		private void CheckSizeLimit(CsElement element, Rules rule, NumericValue limit)
		{
			int size = CodeHelper.GetElementSizeByDeclaration(element);

			if (size > limit.Value)
			{
				m_parent.AddViolation(
					element,
					rule,
					limit.Value,
					size);
			}
		}

		#endregion
	}
}
