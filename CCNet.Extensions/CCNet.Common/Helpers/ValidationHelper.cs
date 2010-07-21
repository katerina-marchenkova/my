using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using CCNet.Common.Properties;

namespace CCNet.Common
{
	/// <summary>
	/// Commond methods for different validations.
	/// </summary>
	public static class ValidationHelper
	{
		#region Checking properties

		/// <summary>
		/// Checks properties filling.
		/// Returns true if everything is correct.
		/// </summary>
		public static bool CheckProperties(
			IDictionary<string, string> properties,
			IDictionary<string, string> required,
			IDictionary<string, string> allowed,
			out string description)
		{
			Contract.Ensures(
				(Contract.Result<bool>() == true
					&& Contract.ValueAtReturn(out description) == null)
				|| (Contract.Result<bool>() == false
					&& Contract.ValueAtReturn(out description) != null
					&& Contract.ValueAtReturn(out description).Length > 0));

			description = null;
			StringBuilder message = new StringBuilder();

			CheckPropertyItems(properties, required, true, message);
			CheckPropertyItems(properties, allowed, false, message);

			foreach (string key in required.Keys)
			{
				properties.Remove(key);
			}

			foreach (string key in allowed.Keys)
			{
				properties.Remove(key);
			}

			foreach (var item in properties)
			{
				message.AppendLine(
					Resources.UnexpectedProperty
					.Display(item.Key, item.Value));
			}

			if (message.Length == 0)
				return true;

			description = message.ToString();
			return false;
		}

		/// <summary>
		/// Checks property values.
		/// </summary>
		private static void CheckPropertyItems(
			IDictionary<string, string> properties,
			IEnumerable<KeyValuePair<string, string>> items,
			bool isRequired,
			StringBuilder message)
		{
			foreach (var item in items)
			{
				if (!properties.ContainsKey(item.Key))
				{
					if (isRequired)
					{
						message.AppendLine(
							Resources.MissingRequiredProperty
							.Display(item.Key, item.Value));
					}

					continue;
				}

				if (item.Value == null)
					continue;

				string value = properties[item.Key];
				if (value != item.Value)
				{
					message.AppendLine(
						Resources.InvalidPropertyValue
						.Display(item.Key, item.Value, value));

					continue;
				}
			}
		}

		#endregion

		#region Checking entries

		/// <summary>
		/// Checks text entries.
		/// Returns true if everything is correct.
		/// </summary>
		public static bool CheckEntries(
			string text,
			IEnumerable<string> required,
			IEnumerable<string> forbidden,
			out string description)
		{
			Contract.Ensures(
				(Contract.Result<bool>() == true
					&& Contract.ValueAtReturn(out description) == null)
				|| (Contract.Result<bool>() == false
					&& Contract.ValueAtReturn(out description) != null
					&& Contract.ValueAtReturn(out description).Length > 0));

			description = null;
			StringBuilder message = new StringBuilder();

			foreach (string line in required)
			{
				if (!text.Contains(line))
				{
					message.AppendLine(
						Resources.MissingRequiredProperty
						.Display(line));
				}
			}

			foreach (string line in forbidden)
			{
				if (text.Contains(line))
				{
					message.AppendLine(
						Resources.ForbiddenEntry
						.Display(line));
				}
			}

			if (message.Length == 0)
				return true;

			description = message.ToString();
			return false;
		}

		#endregion
	}
}
