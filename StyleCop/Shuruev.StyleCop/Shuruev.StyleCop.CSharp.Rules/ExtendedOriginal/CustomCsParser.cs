using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// C# parser used for running default analyzers in custom mode.
	/// </summary>
	internal class CustomCsParser : CsParser
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
		public override bool IsRuleSuppressed(ICodeElement element, Rule rule)
		{
			return false;
		}
	}
}
