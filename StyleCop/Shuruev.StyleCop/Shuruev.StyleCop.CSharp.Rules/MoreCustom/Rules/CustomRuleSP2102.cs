﻿namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// SP2102 custom rule.
	/// </summary>
	public class CustomRuleSP2102 : CustomRule
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		internal CustomRuleSP2102()
			: base(
				Rules.PropertyMustNotContainMoreLinesThan,
				"SP2102",
				"SP2102_Limit",
				CustomRulesResources.DescriptionSP2102,
				CustomRulesResources.ExampleSP2102)
		{
		}

		/// <summary>
		/// Creates control for displaying options.
		/// </summary>
		public override CustomRuleOptions CreateOptionsControl()
		{
			return new CustomRuleLimitOptions(CustomRulesResources.LimitOptionsLineDescription);
		}

		/// <summary>
		/// Creates an empty instance of options data.
		/// </summary>
		public override ICustomRuleOptionsData CreateOptionsData()
		{
			return new LimitOptionsData(
				NumericValue.CreatePropertySize(),
				CustomRulesResources.LimitOptionsLineFormat);
		}
	}
}
