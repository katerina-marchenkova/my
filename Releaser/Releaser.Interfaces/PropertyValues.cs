using System;
using System.Collections.Generic;

namespace Shuruev.Releaser.Interfaces
{
	/// <summary>
	/// Defines property collection.
	/// </summary>
	public class PropertyValues
	{
		private readonly Dictionary<string, List<string>> data;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyValues"/> class.
		/// </summary>
		public PropertyValues()
		{
			this.data = new Dictionary<string, List<string>>();
		}

		#region Getting values

		/// <summary>
		/// Gets a list of all property codes.
		/// </summary>
		public List<string> GetCodes()
		{
			return new List<string>(this.data.Keys);
		}

		/// <summary>
		/// Gets a list of all values for specified property code.
		/// </summary>
		public List<string> GetValues(string propertyCode)
		{
			if (!this.data.ContainsKey(propertyCode))
			{
				return new List<string>();
			}

			return this.data[propertyCode];
		}

		#endregion

		#region Setting values

		/// <summary>
		/// Adds value for specified property.
		/// </summary>
		public void Add(string propertyCode, string propertyValue)
		{
			if (StringHelper.IsEmpty(propertyCode))
			{
				throw new ArgumentException("Parameter propertyCode should not be empty.");
			}

			if (StringHelper.IsEmpty(propertyValue))
			{
				throw new ArgumentException("Parameter propertyValue should not be empty.");
			}

			if (!this.data.ContainsKey(propertyCode))
			{
				this.data[propertyCode] = new List<string>();
			}

			this.data[propertyCode].Add(propertyValue);
		}

		/// <summary>
		/// Clears all values for specified property.
		/// </summary>
		public void Clear(string propertyCode)
		{
			if (StringHelper.IsEmpty(propertyCode))
			{
				throw new ArgumentException("Parameter propertyCode should not be empty.");
			}

			this.data.Remove(propertyCode);
		}

		#endregion
	}
}
