﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace CCNet.Common
{
	/// <summary>
	/// Works with command line properties.
	/// </summary>
	public class ArgumentProperties
	{
		private readonly Dictionary<string, string> m_values = new Dictionary<string, string>();

		#region Properties

		/// <summary>
		/// Gets all property keys.
		/// </summary>
		public List<string> Keys
		{
			get
			{
				return new List<string>(m_values.Keys);
			}
		}

		#endregion

		#region Parsing arguments list

		/// <summary>
		/// Adds a new property.
		/// </summary>
		private void Add(string key, string value)
		{
			Contract.Requires(!String.IsNullOrEmpty(key));
			Contract.Requires(!String.IsNullOrEmpty(value));

			m_values[key] = value;
		}

		/// <summary>
		/// Parses a list of argument properties.
		/// </summary>
		public static ArgumentProperties Parse(params string[] args)
		{
			Contract.Requires(args != null);

			ArgumentProperties result = new ArgumentProperties();
			foreach (string arg in args)
			{
				string[] parts = arg.Split('=');
				if (parts.Length != 2)
					throw new InvalidOperationException(
						"Argument {0} doesn't define a property."
						.Display(arg));

				string key = parts[0].Trim();
				string value = parts[1].Trim();
				result.Add(key, value);
			}

			return result;
		}

		#endregion

		#region Getting property values

		/// <summary>
		/// Gets property value for specified key.
		/// </summary>
		public string GetValue(string key)
		{
			Contract.Requires(!String.IsNullOrEmpty(key));

			if (!m_values.ContainsKey(key))
				throw new ArgumentException(
					"Property {0} is not found."
					.Display(key));

			return m_values[key];
		}

		#endregion
	}
}
