namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// SP2001 custom rule.
	/// </summary>
	public class CustomRuleSP2001 : CustomRule
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		internal CustomRuleSP2001()
			: base(
				Rules.CodeLineMustBeginWithIdenticalWhitespaces,
				"SP2001",
				"SP2001_Demo",
				CustomRulesResources.DescriptionSP2001,
				CustomRulesResources.ExampleSP2001)
		{
		}

		/// <summary>
		/// Creates control for displaying options.
		/// </summary>
		public override CustomRuleOptions CreateOptionsControl()
		{
			return new CustomRuleIndentOptions();
		}
	}
}
