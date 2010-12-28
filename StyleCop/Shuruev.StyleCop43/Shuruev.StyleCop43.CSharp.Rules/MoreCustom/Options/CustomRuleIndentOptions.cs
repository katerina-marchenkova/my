using System;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Control displaying indent options.
	/// </summary>
	public partial class CustomRuleIndentOptions : CustomRuleOptions
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CustomRuleIndentOptions()
		{
			InitializeComponent();
		}

		#region Event handlers

		private void radioMode_CheckedChanged(object sender, EventArgs e)
		{
			OnOptionsDataChanged(e);
		}

		#endregion

		#region Override methods

		/// <summary>
		/// Displays specified options data.
		/// </summary>
		protected override void DisplayOptionsData(ICustomRuleOptionsData data)
		{
			IndentOptionsData options = (IndentOptionsData)data;

			radioTabs.Checked = options.Mode == IndentMode.Tabs;
			radioSpaces.Checked = options.Mode == IndentMode.Spaces;
			radioBoth.Checked = options.Mode == IndentMode.Both;
		}

		/// <summary>
		/// Gets options data from user interface.
		/// </summary>
		protected override void ParseOptionsData(ICustomRuleOptionsData data)
		{
			IndentOptionsData options = (IndentOptionsData)data;

			if (radioTabs.Checked)
			{
				options.Mode = IndentMode.Tabs;
				return;
			}

			if (radioSpaces.Checked)
			{
				options.Mode = IndentMode.Spaces;
				return;
			}

			if (radioBoth.Checked)
			{
				options.Mode = IndentMode.Both;
				return;
			}
		}

		#endregion
	}
}
