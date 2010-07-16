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

			foreach (var item in required)
			{
				if (!properties.ContainsKey(item.Key))
				{
					message.AppendLine(
						Resources.MissingRequiredProperty
						.Display(item.Key, item.Value));
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

			foreach (var item in allowed)
			{
				if (!properties.ContainsKey(item.Key))
					continue;

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
	}
}
