using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Rules that are based on the original ones with adding some exception cases.
	/// </summary>
	public class ExtendedOriginalRules
	{
		internal const string AllowConstructorsFor1502 = "SP1502_AllowConstructors";
		internal const string AllowNestedCodeBlocksFor1509 = "SP1509_AllowNestedCodeBlocks";

		private readonly SourceAnalyzer m_parent;

		private StyleCopCore m_customCore;
		private NamingRules m_customNamingAnalyzer;
		private LayoutRules m_customLayoutAnalyzer;
		private DocumentationRules m_customDocumentationAnalyzer;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ExtendedOriginalRules(SourceAnalyzer parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			m_parent = parent;
		}

		#region Running original analyzers

		/// <summary>
		/// Initializes an add-in.
		/// </summary>
		public void InitializeAddIn()
		{
			m_customCore = new StyleCopCore();
			m_customCore.ViolationEncountered += OnCustomViolationEncountered;

			m_customNamingAnalyzer = new NamingRules();
			m_customLayoutAnalyzer = new LayoutRules();
			m_customDocumentationAnalyzer = new DocumentationRules();

			InitializeCustomAnalyzer(
				m_parent.Core,
				m_customCore,
				"Microsoft.StyleCop.CSharp.NamingRules",
				m_customNamingAnalyzer);

			InitializeCustomAnalyzer(
				m_parent.Core,
				m_customCore,
				"Microsoft.StyleCop.CSharp.LayoutRules",
				m_customLayoutAnalyzer);

			InitializeCustomAnalyzer(
				m_parent.Core,
				m_customCore,
				"Microsoft.StyleCop.CSharp.DocumentationRules",
				m_customDocumentationAnalyzer);
		}

		/// <summary>
		/// Analyzes source document.
		/// </summary>
		public void AnalyzeDocument(CodeDocument document)
		{
			m_customNamingAnalyzer.AnalyzeDocument(document);
			m_customLayoutAnalyzer.AnalyzeDocument(document);
			m_customDocumentationAnalyzer.AnalyzeDocument(document);
		}

		/// <summary>
		/// Handles encountered custom violations.
		/// </summary>
		private void OnCustomViolationEncountered(object sender, ViolationEventArgs e)
		{
			RemoveCustomViolation(e);

			switch (e.Violation.Rule.CheckId)
			{
				case "SA1502":
					Handle1502(e);
					break;

				case "SA1509":
					Handle1509(e);
					break;

				case "SA1642":
					Handle1642(e);
					break;

				case "SA1643":
					Handle1643(e);
					break;
			}
		}

		/// <summary>
		/// Initializes custom analyzer based on the standard one.
		/// </summary>
		private static void InitializeCustomAnalyzer(
			StyleCopCore originalCore,
			StyleCopCore customCore,
			string originalAnalyzerCode,
			SourceAnalyzer customAnalyzer)
		{
			Dictionary<string, SourceAnalyzer> originalAnalyzers = (Dictionary<string, SourceAnalyzer>)typeof(StyleCopCore).InvokeMember(
				"analyzers",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField,
				null,
				originalCore,
				null);

			SourceAnalyzer originalAnalyzer = originalAnalyzers[originalAnalyzerCode];

			Dictionary<string, Rule> originalRules = (Dictionary<string, Rule>)typeof(StyleCopAddIn).InvokeMember(
				"rules",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField,
				null,
				originalAnalyzer,
				null);

			typeof(StyleCopAddIn).InvokeMember(
				"core",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField,
				null,
				customAnalyzer,
				new object[] { customCore });

			Dictionary<string, Rule> customRules = new Dictionary<string, Rule>();
			foreach (KeyValuePair<string, Rule> pair in originalRules)
			{
				Rule originalRule = pair.Value;
				Rule customRule = (Rule)typeof(Rule).InvokeMember(
					"Rule",
					BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
					null,
					customCore,
					new object[] { originalRule.Name, originalRule.Namespace, originalRule.CheckId, originalRule.Context, originalRule.Warning, originalRule.Description, originalRule.RuleGroup, true, false });
				customRules[pair.Key] = customRule;
			}

			typeof(StyleCopAddIn).InvokeMember(
				"rules",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField,
				null,
				customAnalyzer,
				new object[] { customRules });

			CustomCsParser customParser = new CustomCsParser();

			typeof(SourceAnalyzer).InvokeMember(
				"parser",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField,
				null,
				customAnalyzer,
				new object[] { customParser });
		}

		/// <summary>
		/// Removes violation got from the custom analyzer.
		/// </summary>
		private static void RemoveCustomViolation(ViolationEventArgs e)
		{
			string key = (string)typeof(Violation).InvokeMember(
				"Key",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty,
				null,
				e.Violation,
				null);

			Dictionary<string, Violation> violations = (Dictionary<string, Violation>)typeof(CodeElement).InvokeMember(
				"violations",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField,
				null,
				e.Element,
				null);

			violations.Remove(key);
		}

		#endregion

		#region Handling original violations

		/// <summary>
		/// Handles SA1502 violation.
		/// </summary>
		private void Handle1502(ViolationEventArgs e)
		{
			CsElement element = (CsElement)e.Element;

			if (ReadSetting(e, AllowConstructorsFor1502))
			{
				if (element.ElementType == ElementType.Constructor)
					return;
			}

			m_parent.AddViolation(
				element,
				e.LineNumber,
				Rules.OpeningCurlyBracketsMustNotBePrecededByBlankLine,
				element.FriendlyTypeText);
		}

		/// <summary>
		/// Handles SA1509 violation.
		/// </summary>
		private void Handle1509(ViolationEventArgs e)
		{
			CsElement element = (CsElement)e.Element;

			if (ReadSetting(e, AllowNestedCodeBlocksFor1509))
			{
				Node<CsToken> node = CodeHelper.GetNodeByLine(element.Document, e.LineNumber);
				if (node != null)
				{
					Node<CsToken> prev = CodeHelper.FindPreviousValueableNode(node);
					if (prev.Value.CsTokenType == CsTokenType.Semicolon
						|| prev.Value.CsTokenType == CsTokenType.CloseCurlyBracket)
						return;
				}
			}

			m_parent.AddViolation(
				element,
				e.LineNumber,
				Rules.OpeningCurlyBracketsMustNotBePrecededByBlankLine,
				new object[0]);
		}

		/// <summary>
		/// Handles SA1642 violation.
		/// </summary>
		private void Handle1642(ViolationEventArgs e)
		{
			CsElement element = (CsElement)e.Element;

			string text = CodeHelper.GetSummaryText(element);
			if (text == Resources.StandardConstructorSummaryText)
				return;

			m_parent.AddViolation(
				element,
				Rules.ConstructorSummaryDocumentationMustBeginWithStandardText,
				new object[] { GetExampleSummaryTextForConstructor(element) });
		}

		/// <summary>
		/// Handles SA1643 violation.
		/// </summary>
		private void Handle1643(ViolationEventArgs e)
		{
			CsElement element = (CsElement)e.Element;

			string text = CodeHelper.GetSummaryText(element);
			if (text == Resources.StandardDestructorSummaryText)
				return;

			m_parent.AddViolation(
				element,
				Rules.DestructorSummaryDocumentationMustBeginWithStandardText,
				new object[] { GetExampleSummaryTextForDestructor() });
		}

		#endregion

		#region Reading custom settings

		/// <summary>
		/// Reads the value of custom setting.
		/// </summary>
		private bool ReadSetting(ViolationEventArgs e, string settingName)
		{
			return SettingsManager.GetBooleanValue(
				m_parent,
				e.Element.Document.Settings,
				settingName);
		}

		#endregion

		#region Calling specific private methods from original analyzers

		/// <summary>
		/// Gets example summary text for constructor.
		/// </summary>
		private string GetExampleSummaryTextForConstructor(ICodeUnit constructor)
		{
			string type = (constructor.Parent is Struct) ? "struct" : "class";
			return (string)typeof(DocumentationRules).InvokeMember(
				"GetExampleSummaryTextForConstructorType",
				BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
				null,
				m_customDocumentationAnalyzer,
				new object[] { constructor, type });
		}

		/// <summary>
		/// Gets example summary text for destructor.
		/// </summary>
		private string GetExampleSummaryTextForDestructor()
		{
			return (string)typeof(DocumentationRules).InvokeMember(
				"GetExampleSummaryTextForDestructor",
				BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
				null,
				m_customDocumentationAnalyzer,
				null);
		}

		#endregion
	}
}
