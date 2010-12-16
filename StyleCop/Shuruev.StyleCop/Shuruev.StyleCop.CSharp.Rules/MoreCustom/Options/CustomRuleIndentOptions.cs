using System;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Control displaying indent options.
	/// </summary>
	public partial class CustomRuleIndentOptions : CustomRuleOptions
	{
		/// <summary>
		/// Options data structure.
		/// </summary>
		private class OptionsData : ICustomRuleOptionsData
		{
			public bool Field1 { get; set; }
			public string Field2 { get; set; }

			/// <summary>
			/// Initializes object data from setting value.
			/// </summary>
			public void ConvertFromValue(string settingValue)
			{
				string[] parts = settingValue.Split(':');
				Field1 = Boolean.Parse(parts[0]);
				Field2 = parts[1];
			}

			/// <summary>
			/// Converts object data to setting value.
			/// </summary>
			public string ConvertToValue()
			{
				return String.Format("{0}:{1}", Field1, Field2);
			}

			/// <summary>
			/// Gets a friendly description for options data.
			/// </summary>
			public string GetDescription()
			{
				return String.Format("{0}, {1}", Field1, Field2);
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

			checkBox1.Checked = options.Field1;
			textBox1.Text = options.Field2;
		}

		/// <summary>
		/// Gets options data from user interface.
		/// </summary>
		protected override void ParseOptionsData(ICustomRuleOptionsData data)
		{
			OptionsData options = (OptionsData)data;

			options.Field1 = checkBox1.Checked;
			options.Field2 = textBox1.Text;
		}

		#endregion

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			OnXXX(e);
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			OnXXX(e);
		}
	}
}
