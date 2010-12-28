using System;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Last line options data structure.
	/// </summary>
	public class LastLineOptionsData : ICustomRuleOptionsData
	{
		private const LastLineMode c_defaultMode = LastLineMode.Empty;

		/// <summary>
		/// Gets or sets last line mode.
		/// </summary>
		public LastLineMode Mode { get; set; }

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public LastLineOptionsData()
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
				Mode = (LastLineMode)Enum.Parse(typeof(LastLineMode), settingValue);
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
				case LastLineMode.Empty:
					return CustomRulesResources.LastLineOptionsEmpty;
				case LastLineMode.NotEmpty:
					return CustomRulesResources.LastLineOptionsNotEmpty;
				default:
					throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Gets objects for constructing context string.
		/// </summary>
		public object[] GetContextValues()
		{
			switch (Mode)
			{
				case LastLineMode.Empty:
					return new object[] { CustomRulesResources.LastLineContextEmpty };
				case LastLineMode.NotEmpty:
					return new object[] { CustomRulesResources.LastLineContextNotEmpty };
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
