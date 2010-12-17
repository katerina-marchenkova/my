using System;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Control displaying limit options.
	/// </summary>
	public partial class CustomRuleLimitOptions : CustomRuleOptions
	{
		private readonly string m_textFormat;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CustomRuleLimitOptions()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CustomRuleLimitOptions(string description, string textFormat)
			: this()
		{
			m_textFormat = textFormat;
			label1.Text = description;
		}

		#region Override methods

		/// <summary>
		/// Creates an empty instance of options data.
		/// </summary>
		protected override ICustomRuleOptionsData CreateOptionsData()
		{
			return new LimitOptionsData(m_textFormat);
		}

		/// <summary>
		/// Displays specified options data.
		/// </summary>
		protected override void DisplayOptionsData(ICustomRuleOptionsData data)
		{
			LimitOptionsData options = (LimitOptionsData)data;

			textBox1.Text = options.Limit.ToString();
		}

		/// <summary>
		/// Gets options data from user interface.
		/// </summary>
		protected override void ParseOptionsData(ICustomRuleOptionsData data)
		{
			LimitOptionsData options = (LimitOptionsData)data;

			int limit;
			if (Int32.TryParse(textBox1.Text, out limit))
			{
				options.Limit = limit;
			}
		}

		#endregion

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			//xxx
			OnOptionsDataChanged(e);
		}
	}
}
