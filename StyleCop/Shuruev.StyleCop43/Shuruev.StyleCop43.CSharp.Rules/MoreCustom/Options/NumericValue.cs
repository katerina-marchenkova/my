﻿using System;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Holds numeric value.
	/// </summary>
	public class NumericValue
	{
		private readonly int m_defaultValue;
		private readonly int m_minValue;
		private readonly int m_maxValue;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public NumericValue(int defaultValue, int minValue, int maxValue)
		{
			if (minValue > maxValue)
				throw new InvalidOperationException("Failed constraint: minValue <= maxValue.");

			if (defaultValue > maxValue
				|| defaultValue < minValue)
				throw new InvalidOperationException("Failed constraint: minValue <= defaultValue <= maxValue.");

			m_defaultValue = defaultValue;
			m_minValue = minValue;
			m_maxValue = maxValue;

			Value = m_defaultValue;
		}

		#region Properties

		/// <summary>
		/// Gets numeric value.
		/// </summary>
		public int Value { get; private set; }

		#endregion

		#region Converting from text

		/// <summary>
		/// Parses numeric value from text.
		/// </summary>
		public void Parse(string text)
		{
			int value;
			if (!Int32.TryParse(text, out value))
				return;

			if (value < m_minValue)
				value = m_minValue;

			if (value > m_maxValue)
				value = m_maxValue;

			Value = value;
		}

		#endregion

		#region Creating different numeric instances

		/// <summary>
		/// Creates numeric value holding char count limit.
		/// </summary>
		public static NumericValue CreateCharLimit()
		{
			return new NumericValue(140, 1, 10000);
		}

		/// <summary>
		/// Creates numeric value holding line count limit.
		/// </summary>
		public static NumericValue CreateLineLimit()
		{
			return new NumericValue(120, 1, 100000);
		}

		/// <summary>
		/// Creates numeric value holding tab chars count.
		/// </summary>
		public static NumericValue CreateTabSize()
		{
			return new NumericValue(4, 1, 100);
		}

		#endregion
	}
}
