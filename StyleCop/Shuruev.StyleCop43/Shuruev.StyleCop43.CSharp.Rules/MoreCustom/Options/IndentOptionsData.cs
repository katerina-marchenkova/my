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
		/// Initializes a new instance.
		/// </summary>
		public IndentOptionsData()
		{
			Mode = c_defaultMode;
		}

		#region Properties

		/// <summary>
		/// Gets or sets indentation mode.
		/// </summary>
		public IndentMode Mode { get; set; }

		#endregion

		#region Implementation of ICustomRuleOptionsData

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
					return CustomRulesResources.IndentOptionsTabs;
				case IndentMode.Spaces:
					return CustomRulesResources.IndentOptionsSpaces;
				case IndentMode.Both:
					return CustomRulesResources.IndentOptionsBoth;
				default:
					throw new InvalidOperationException();
			}
		}

		#endregion

		#region Additional methods

		/// <summary>
		/// Gets objects for constructing context string.
		/// </summary>
		public object[] GetContextValues()
		{
			switch (Mode)
			{
				case IndentMode.Tabs:
					return new object[] { CustomRulesResources.IndentContextTabs };
				case IndentMode.Spaces:
					return new object[] { CustomRulesResources.IndentContextSpaces };
				case IndentMode.Both:
					return new object[] { CustomRulesResources.IndentContextBoth };
				default:
					throw new InvalidOperationException();
			}
		}

		#endregion
	}
}
