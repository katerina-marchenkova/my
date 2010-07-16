using System.Collections.Generic;
using System.Reflection;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Rules that are based on the original ones with adding some exception cases.
	/// </summary>
	public partial class ExtendedOriginalRules
	{
		#region Running original analyzers

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