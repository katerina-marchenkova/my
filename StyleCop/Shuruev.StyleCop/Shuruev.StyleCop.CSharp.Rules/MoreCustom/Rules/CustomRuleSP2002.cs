namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// SP2002 custom rule.
	/// </summary>
	public class CustomRuleSP2002 : CustomRule
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		internal CustomRuleSP2002()
			: base(
				Rules.LastCodeLineMustNotBeEmpty,
				"SP2002",
				null,
				CustomRulesResources.DescriptionSP2002,
				CustomRulesResources.ExampleSP2002)
		{
		}
	}
}
