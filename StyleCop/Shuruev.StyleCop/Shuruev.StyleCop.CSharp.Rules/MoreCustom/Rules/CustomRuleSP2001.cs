namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// SP2001 custom rule.
	/// </summary>
	public class CustomRuleSP2001 : CustomRule
	{
		internal CustomRuleSP2001()
			: base(
				Rules.CodeLineMustBeginWithIdenticalWhitespaces,
				"SP2001",
				null,
				CustomRulesResources.DescriptionSP2001,
				CustomRulesResources.ExampleSP2001)
		{
		}
	}
}
