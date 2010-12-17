namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// SP2100 custom rule.
	/// </summary>
	public class CustomRuleSP2100 : CustomRule
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		internal CustomRuleSP2100()
			: base(
				Rules.CodeLineMustNotBeLongerThan,
				"SP2100",
				"SP2100_Limit",
				CustomRulesResources.DescriptionSP2100,
				CustomRulesResources.ExampleSP2100)
		{
		}

		/// <summary>
		/// Creates control for displaying options.
		/// </summary>
		public override CustomRuleOptions CreateOptionsControl()
		{
			return new CustomRuleLimitOptions(
				CustomRulesResources.LimitOptionsCharDescription,
				CustomRulesResources.LimitOptionsCharFormat);
		}
	}
}
