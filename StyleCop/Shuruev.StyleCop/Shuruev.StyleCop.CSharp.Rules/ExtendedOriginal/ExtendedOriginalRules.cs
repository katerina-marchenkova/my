using System;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Rules that are based on the original ones with adding some exception cases.
	/// </summary>
	public partial class ExtendedOriginalRules
	{
		internal const string AllowConstructorsFor1502 = "SP1502_AllowConstructors";
		internal const string AllowNestedCodeBlocksFor1509 = "SP1509_AllowNestedCodeBlocks";
		internal const string AllowJoinedAccessorsFor1513 = "SP1513_AllowJoinedAccessors";
		internal const string AllowJoinedAccessorsFor1516 = "SP1516_AllowJoinedAccessors";

		private readonly StyleCopPlus m_parent;

		private StyleCopCore m_customCore;
		private CustomNamingRules m_customNamingAnalyzer;
		private CustomLayoutRules m_customLayoutAnalyzer;
		private CustomDocumentationRules m_customDocumentationAnalyzer;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ExtendedOriginalRules(StyleCopPlus parent)
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

			m_customNamingAnalyzer = new CustomNamingRules();
			m_customLayoutAnalyzer = new CustomLayoutRules();
			m_customDocumentationAnalyzer = new CustomDocumentationRules();

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
			CheckOriginalRule(document, "LayoutRules", Rules.ElementMustNotBeOnSingleLine);
			CheckOriginalRule(document, "LayoutRules", Rules.OpeningCurlyBracketsMustNotBePrecededByBlankLine);
			CheckOriginalRule(document, "LayoutRules", Rules.ClosingCurlyBracketMustBeFollowedByBlankLine);
			CheckOriginalRule(document, "LayoutRules", Rules.ElementsMustBeSeparatedByBlankLine);

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

				case "SA1513":
					Handle1513(e);
					break;

				case "SA1516":
					Handle1516(e);
					break;

				case "SA1642":
					Handle1642(e);
					break;

				case "SA1643":
					Handle1643(e);
					break;
			}
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
				Node<CsToken> node = CodeHelper.GetNodeByLine((CsDocument)element.Document, e.LineNumber);
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
		/// Handles SA1513 violation.
		/// </summary>
		private void Handle1513(ViolationEventArgs e)
		{
			CsElement element = (CsElement)e.Element;

			if (ReadSetting(e, AllowJoinedAccessorsFor1513))
			{
				if (element.ElementType == ElementType.Accessor)
					return;

				if (CodeHelper.IsStyleCop43())
				{
					Node<CsToken> node = CodeHelper.GetNodeByLine((CsDocument)element.Document, e.LineNumber);
					if (node != null)
					{
						Node<CsToken> next1 = CodeHelper.FindNextValueableNode(node);
						if (next1.Value.CsTokenType == CsTokenType.CloseCurlyBracket)
						{
							Node<CsToken> next2 = CodeHelper.FindNextValueableNode(next1);
							if (next2.Value.CsTokenType == CsTokenType.Get
								|| next2.Value.CsTokenType == CsTokenType.Set)
								return;
						}
					}
				}
			}

			m_parent.AddViolation(
				element,
				e.LineNumber,
				Rules.ClosingCurlyBracketMustBeFollowedByBlankLine);
		}

		/// <summary>
		/// Handles SA1516 violation.
		/// </summary>
		private void Handle1516(ViolationEventArgs e)
		{
			CsElement element = (CsElement)e.Element;

			if (ReadSetting(e, AllowJoinedAccessorsFor1516))
			{
				if (element.ElementType == ElementType.Accessor)
					return;

				if (CodeHelper.IsStyleCop43())
				{
					Node<CsToken> node = CodeHelper.GetNodeByLine((CsDocument)element.Document, e.LineNumber);
					if (node != null)
					{
						Node<CsToken> next1 = CodeHelper.FindNextValueableNode(node);
						if (next1.Value.CsTokenType == CsTokenType.CloseCurlyBracket)
						{
							Node<CsToken> next2 = CodeHelper.FindNextValueableNode(next1);
							if (next2.Value.CsTokenType == CsTokenType.Get
								|| next2.Value.CsTokenType == CsTokenType.Set)
								return;
						}
					}
				}
			}

			m_parent.AddViolation(
				element,
				Rules.ElementsMustBeSeparatedByBlankLine);
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
		private void CheckOriginalRule(CodeDocument document, string analyzerName, Rules rule)
		{
			string ruleName = rule.ToString();

			if (!m_parent.IsRuleEnabled(document, ruleName))
				return;

			string fullName = String.Format("Microsoft.StyleCop.CSharp.{0}", analyzerName);
			SourceAnalyzer analyzer = m_parent.Core.GetAnalyzer(fullName);

			if (CodeHelper.IsStyleCop43())
			{
				string settingName = String.Format("{0}#Enabled", rule);
				BooleanProperty enabled = (BooleanProperty)analyzer.GetSetting(document.Settings, settingName);
				if (!enabled.Value)
					return;
			}
			else
			{
				if (!analyzer.IsRuleEnabled(document, ruleName))
					return;
			}

			string message = String.Format(
				Resources.ExtendedRuleConflictError,
				m_parent.GetRule(ruleName).CheckId,
				analyzer.GetRule(ruleName).CheckId);

			throw new Exception(message);
		}

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
	}
}
