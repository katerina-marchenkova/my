using System;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Indentation options data structure.
	/// </summary>
	public class IndentOptionsData : ICustomRuleOptionsData
	{
		private const IndentMode c_defaultMode = IndentMode.Tabs;

		/// <summary>
		/// Gets or sets indentation mode.
		/// </summary>
		public IndentMode Mode { get; set; }

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public IndentOptionsData()
		{
			Mode = c_defaultMode;
		}

		/// <summary>
		/// Initializes object data from setting value.
		/// </summary>
		public void ConvertFromValue(string settingValue)
		{
			try
			{
				Mode = (IndentMode)Enum.Parse(typeof(IndentMode), settingValue);
			}
			catch
			{
			}
		}

		/// <summary>
		/// Converts object data to setting value.
		/// </summary>
		public string ConvertToValue()
		{
			return Mode.ToString();
		}

		/// <summary>
		/// Gets a friendly description for options data.
		/// </summary>
		public string GetDescription()
		{
			switch (Mode)
			{
				case IndentMode.Tabs:
					return CustomRulesResources.IndentOptionsTabsOnly;
				case IndentMode.Spaces:
					return CustomRulesResources.IndentOptionsSpacesOnly;
				case IndentMode.Both:
					return CustomRulesResources.IndentOptionsTabsAndSpaces;
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
