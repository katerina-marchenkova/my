using System;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Control displaying indent options.
	/// </summary>
	public partial class CustomRuleIndentOptions : CustomRuleOptions
	{
		/// <summary>
		/// Specifies different indentation modes.
		/// </summary>
		public enum IndentMode
		{
			/// <summary>
			/// Only tabs are allowed for indentation.
			/// </summary>
			Tabs,

			/// <summary>
			/// Only spaces are allowed for indentation.
			/// </summary>
			Spaces,

			/// <summary>
			/// Both tabs and spaces are allowed for indentation.
			/// </summary>
			Both
		}

		/// <summary>
		/// Options data structure.
		/// </summary>
		public class OptionsData : ICustomRuleOptionsData
		{
			/// <summary>
			/// Indentation mode.
			/// </summary>
			public IndentMode Mode { get; set; }

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

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CustomRuleIndentOptions()
		{
			InitializeComponent();
		}

		#region Override methods

		/// <summary>
		/// Creates an empty instance of options data.
		/// </summary>
		protected override ICustomRuleOptionsData CreateOptionsData()
		{
			return new OptionsData();
		}

		/// <summary>
		/// Displays specified options data.
		/// </summary>
		protected override void DisplayOptionsData(ICustomRuleOptionsData data)
		{
			OptionsData options = (OptionsData)data;

			/*xxxcheckBox1.Checked = options.Field1;
			textBox1.Text = options.Field2;*/
		}

		/// <summary>
		/// Gets options data from user interface.
		/// </summary>
		protected override void ParseOptionsData(ICustomRuleOptionsData data)
		{
			OptionsData options = (OptionsData)data;

			/*xxxoptions.Field1 = checkBox1.Checked;
			options.Field2 = textBox1.Text;*/
		}

		#endregion

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			OnOptionsDataChanged(e);
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			OnOptionsDataChanged(e);
		}
	}
}
