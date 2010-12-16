using System;
using System.Windows.Forms;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Control displaying custom rule options.
	/// </summary>
	public partial class CustomRuleOptions : UserControl
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CustomRuleOptions()
		{
			InitializeComponent();
		}

		#region Events

		/// <summary>
		/// Occurs when options data is changed.
		/// </summary>
		public event EventHandler XXX;

		/// <summary>
		/// Raises XXX event.
		/// </summary>
		protected virtual void OnXXX(EventArgs e)
		{
			if (XXX != null)
				XXX(this, e);
		}

		#endregion

		#region Common methods

		/// <summary>
		/// Gets options text for specified setting value.
		/// </summary>
		public string GetOptionsText(string settingValue)
		{
			ICustomRuleOptionsData data = CreateOptionsData();
			data.ConvertFromValue(settingValue);

			return data.GetDescription();
		}

		/// <summary>
		/// Displays specified setting value.
		/// </summary>
		public void DisplayOptions(string settingValue)
		{
			ICustomRuleOptionsData data = CreateOptionsData();
			data.ConvertFromValue(settingValue);

			DisplayOptionsData(data);
		}

		/// <summary>
		/// Gets setting value from user interface.
		/// </summary>
		public string ParseOptions()
		{
			ICustomRuleOptionsData data = CreateOptionsData();
			ParseOptionsData(data);

			return data.ConvertToValue();
		}

		#endregion

		#region Methods to be overrided

		/// <summary>
		/// Creates an empty instance of options data.
		/// </summary>
		protected virtual ICustomRuleOptionsData CreateOptionsData()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Displays specified options data.
		/// </summary>
		protected virtual void DisplayOptionsData(ICustomRuleOptionsData data)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets options data from user interface.
		/// </summary>
		protected virtual void ParseOptionsData(ICustomRuleOptionsData data)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
