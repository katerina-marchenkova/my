using System.Collections.Generic;
using System.Reflection;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Rules that are based on the default ones with adding some exception cases.
	/// </summary>
	[SourceAnalyzer(typeof(CsParser))]
	public class LimitedDefaultRules : SourceAnalyzer
	{
		/// <summary>
		/// Analyzes source document.
		/// </summary>
		public override void AnalyzeDocument(CodeDocument document)
		{
			StyleCopCore customCore = new StyleCopCore();
			customCore.ViolationEncountered += this.OnCustomViolationEncountered;

			NamingRules customNamingAnalyzer = new NamingRules();
			DocumentationRules customDocumentationAnalyzer = new DocumentationRules();

			InitializeCustomAnalyzer(
				Core,
				customCore,
				"Microsoft.StyleCop.CSharp.NamingRules",
				customNamingAnalyzer);

			InitializeCustomAnalyzer(
				Core,
				customCore,
				"Microsoft.StyleCop.CSharp.DocumentationRules",
				customDocumentationAnalyzer);

			customNamingAnalyzer.AnalyzeDocument(document);
			customDocumentationAnalyzer.AnalyzeDocument(document);
		}

		/// <summary>
		/// Handles encountered custom violations.
		/// </summary>
		private void OnCustomViolationEncountered(object sender, ViolationEventArgs e)
		{
			RemoveCustomViolation(e);

			CsElement element = (CsElement)e.Element;

			if (e.Violation.Rule.CheckId == "SA1300")
			{
				if (!Helper.IsWindowsFormsEventHandler(element))
				{
					AddViolation(element, Rules.ElementMustBeginWithUpperCaseLetter, new object[] { element.FriendlyTypeText, element.Name });
				}
			}

			if (e.Violation.Rule.CheckId == "SA1600")
			{
				if (!Helper.IsWindowsFormsEventHandler(element))
				{
					AddViolation(element, Rules.ElementsMustBeDocumented, new object[] { element.FriendlyTypeText });
				}
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
	}
}
