using System;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Limit options data structure.
	/// </summary>
	public class LimitOptionsData : ICustomRuleOptionsData
	{
		private const int c_defaultLimit = 100;
		private const int c_minLimit = 1;
		private const int c_maxLimit = 100000;

		private readonly string m_textFormat;

		/// <summary>
		/// Gets or sets limit value.
		/// </summary>
		public int Limit { get; set; }

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public LimitOptionsData(string textFormat)
		{
			m_textFormat = textFormat;

			Limit = c_defaultLimit;
		}

		/// <summary>
		/// Initializes object data from setting value.
		/// </summary>
		public void ConvertFromValue(string settingValue)
		{
			int limit;
			if (!Int32.TryParse(settingValue, out limit))
				return;

			if (limit < c_minLimit)
				limit = c_minLimit;

			if (limit > c_maxLimit)
				limit = c_maxLimit;

			Limit = limit;
		}

		/// <summary>
		/// Converts object data to setting value.
		/// </summary>
		public string ConvertToValue()
		{
			return Limit.ToString();
		}

		/// <summary>
		/// Gets a friendly description for options data.
		/// </summary>
		public string GetDescription()
		{
			return String.Format(m_textFormat, Limit);
		}
	}
}
