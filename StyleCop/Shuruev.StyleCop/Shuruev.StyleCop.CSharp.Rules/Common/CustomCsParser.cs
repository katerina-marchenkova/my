using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// C# parser used for running default analyzers in custom mode.
	/// </summary>
	public class CustomCsParser : CsParser
	{
		/// <summary>
		/// Checks whether specified rule is enabled.
		/// </summary>
		public override bool IsRuleEnabled(CodeDocument document, string ruleName)
		{
			return true;
		}

		/// <summary>
		/// Checks whether specified rule is suppressed.
		/// </summary>
		public override bool IsRuleSuppressed(CodeElement element, string ruleCheckId, string ruleName, string ruleNamespace)
		{
			return false;
		}
	}
}
