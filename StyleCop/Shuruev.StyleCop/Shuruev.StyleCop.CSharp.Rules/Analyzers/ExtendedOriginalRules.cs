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
	[SourceAnalyzer(typeof(CsParser))]
	public class ExtendedOriginalRules : SourceAnalyzer
	{
		private StyleCopCore customCore;
		private NamingRules customNamingAnalyzer;
		private LayoutRules customLayoutAnalyzer;
		private DocumentationRules customDocumentationAnalyzer;

		#region Running original analyzers

		/// <summary>
		/// Initializes an add-in.
		/// </summary>
		public override void InitializeAddIn()
		{
			base.InitializeAddIn();

			this.customCore = new StyleCopCore();
			this.customCore.ViolationEncountered += this.OnCustomViolationEncountered;

			this.customNamingAnalyzer = new NamingRules();
			this.customLayoutAnalyzer = new LayoutRules();
			this.customDocumentationAnalyzer = new DocumentationRules();

			InitializeCustomAnalyzer(
				Core,
				this.customCore,
				"Microsoft.StyleCop.CSharp.NamingRules",
				this.customNamingAnalyzer);

			InitializeCustomAnalyzer(
				Core,
				this.customCore,
				"Microsoft.StyleCop.CSharp.LayoutRules",
				this.customLayoutAnalyzer);

			InitializeCustomAnalyzer(
				Core,
				this.customCore,
				"Microsoft.StyleCop.CSharp.DocumentationRules",
				this.customDocumentationAnalyzer);
		}

		/// <summary>
		/// Analyzes source document.
		/// </summary>
		public override void AnalyzeDocument(CodeDocument document)
		{
			this.customNamingAnalyzer.AnalyzeDocument(document);
			this.customLayoutAnalyzer.AnalyzeDocument(document);
			this.customDocumentationAnalyzer.AnalyzeDocument(document);
		}

		/// <summary>
		/// Handles encountered custom violations.
		/// </summary>
		private void OnCustomViolationEncountered(object sender, ViolationEventArgs e)
		{
			RemoveCustomViolation(e);

			switch (e.Violation.Rule.CheckId)
			{
				case "SA1300":
					Handle1300(e);
					break;

				case "SA1509":
					Handle1509(e);
					break;

				case "SA1600":
					Handle1600(e);
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
		/// Handles SA1300 violation.
		/// </summary>
		private void Handle1300(ViolationEventArgs e)
		{
			CsElement element = (CsElement)e.Element;

			if (Helper.IsWindowsFormsEventHandler(element))
				return;

			AddViolation(
				element,
				Rules.ElementMustBeginWithUpperCaseLetter,
				new object[] { element.FriendlyTypeText, element.Name });
		}

		/// <summary>
		/// Handles SA1509 violation.
		/// </summary>
		private void Handle1509(ViolationEventArgs e)
		{
			CsElement element = (CsElement)e.Element;

			Node<CsToken> node = Helper.GetNodeByLine(element.Document, e.LineNumber);
			if (node != null)
			{
				Node<CsToken> prev = Helper.FindPreviousValueableNode(node);
				if (prev.Value.CsTokenType == CsTokenType.Semicolon
					|| prev.Value.CsTokenType == CsTokenType.CloseCurlyBracket)
					return;
			}

			AddViolation(
				element,
				e.LineNumber,
				Rules.OpeningCurlyBracketsMustNotBePrecededByBlankLine,
				new object[0]);
		}

		/// <summary>
		/// Handles SA1600 violation.
		/// </summary>
		private void Handle1600(ViolationEventArgs e)
		{
			CsElement element = (CsElement)e.Element;

			if (Helper.IsWindowsFormsEventHandler(element))
				return;

			AddViolation(
				element,
				Rules.ElementsMustBeDocumented,
				new object[] { element.FriendlyTypeText });
		}

		/// <summary>
		/// Handles SA1642 violation.
		/// </summary>
		private void Handle1642(ViolationEventArgs e)
		{
			CsElement element = (CsElement)e.Element;

			string text = Helper.GetSummaryText(element);
			if (text == Resources.StandardConstructorSummaryText)
				return;

			AddViolation(
				element,
				Rules.ConstructorSummaryDocumentationMustBeginWithStandardText,
				new object[] { this.GetExampleSummaryTextForConstructor(element) });
		}

		/// <summary>
		/// Handles SA1643 violation.
		/// </summary>
		private void Handle1643(ViolationEventArgs e)
		{
			CsElement element = (CsElement)e.Element;

			string text = Helper.GetSummaryText(element);
			if (text == Resources.StandardDestructorSummaryText)
				return;

			AddViolation(
				element,
				Rules.DestructorSummaryDocumentationMustBeginWithStandardText,
				new object[] { this.GetExampleSummaryTextForDestructor() });
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
				this.customDocumentationAnalyzer,
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
				this.customDocumentationAnalyzer,
				null);
		}

		#endregion
	}
}
