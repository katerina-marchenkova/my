using System;
using System.Collections.Generic;

namespace Shuruev.Releaser.Interfaces
{
	/// <summary>
	/// Defines property collection.
	/// </summary>
	public class PropertyValues
	{
		private readonly Dictionary<string, List<string>> m_data;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public PropertyValues()
		{
			m_data = new Dictionary<string, List<string>>();
		}

		#region Getting values

		/// <summary>
		/// Gets a list of all property codes.
		/// </summary>
		public List<string> GetCodes()
		{
			return new List<string>(m_data.Keys);
		}

		/// <summary>
		/// Gets a list of all values for specified property code.
		/// </summary>
		public List<string> GetValues(string propertyCode)
		{
			if (!m_data.ContainsKey(propertyCode))
				return new List<string>();

			return m_data[propertyCode];
		}

		#endregion

		#region Setting values

		/// <summary>
		/// Adds value for specified property.
		/// </summary>
		public void Add(string propertyCode, string propertyValue)
		{
			if (StringHelper.IsEmpty(propertyCode))
				throw new ArgumentException("Parameter propertyCode should not be empty.");

			if (StringHelper.IsEmpty(propertyValue))
				throw new ArgumentException("Parameter propertyValue should not be empty.");

			if (!m_data.ContainsKey(propertyCode))
				m_data[propertyCode] = new List<string>();

			m_data[propertyCode].Add(propertyValue);
		}

		/// <summary>
		/// Clears all values for specified property.
		/// </summary>
		public void Clear(string propertyCode)
		{
			if (StringHelper.IsEmpty(propertyCode))
				throw new ArgumentException("Parameter propertyCode should not be empty.");

			m_data.Remove(propertyCode);
		}

		#endregion
	}
}
