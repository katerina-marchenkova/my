using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;

namespace Shuruev.Releaser.Interfaces
{
	/// <summary>
	/// Reading configuration parameters.
	/// </summary>
	public static class ConfigurationHelper
	{
		#region Application settings

		/// <summary>
		/// Checks whether configuration has value for specified key.
		/// </summary>
		public static bool HasValue(NameValueCollection source, string name)
		{
			string value = source[name];
			return value != null;
		}

		/// <summary>
		/// Checks whether configuration has value for specified key.
		/// </summary>
		public static bool HasValue(string name)
		{
			return HasValue(ConfigurationManager.AppSettings, name);
		}

		/// <summary>
		/// Reads configuration value.
		/// </summary>
		private static string ReadValue(NameValueCollection source, string name)
		{
			string value = source[name];
			if (value == null)
			{
				string message = String.Format("Configuration parameter '{0}' is not found.", name);
				throw new ConfigurationErrorsException(message);
			}
			return value;
		}

		/// <summary>
		/// Reads string configuration value.
		/// </summary>
		public static string ReadString(NameValueCollection source, string name)
		{
			return ReadValue(source, name);
		}

		/// <summary>
		/// Reads string configuration value.
		/// </summary>
		public static string ReadString(string name)
		{
			return ReadString(ConfigurationManager.AppSettings, name);
		}

		/// <summary>
		/// Reads boolean configuration value.
		/// </summary>
		public static bool ReadBoolean(NameValueCollection source, string name)
		{
			string value = ReadValue(source, name);
			try
			{
				return Boolean.Parse(value);
			}
			catch
			{
				string message = String.Format("Configuration parameter '{0}' must contain a boolean value.", name);
				throw new ConfigurationErrorsException(message);
			}
		}

		/// <summary>
		/// Reads boolean configuration value.
		/// </summary>
		public static bool ReadBoolean(string name)
		{
			return ReadBoolean(ConfigurationManager.AppSettings, name);
		}

		/// <summary>
		/// Reads integer configuration value.
		/// </summary>
		public static int ReadInteger(NameValueCollection source, string name)
		{
			string value = ReadValue(source, name);
			try
			{
				return Int32.Parse(value);
			}
			catch
			{
				string message = String.Format("Configuration parameter '{0}' must contain an integer value.", name);
				throw new ConfigurationErrorsException(message);
			}
		}

		/// <summary>
		/// Reads integer configuration value.
		/// </summary>
		public static int ReadInteger(string name)
		{
			return ReadInteger(ConfigurationManager.AppSettings, name);
		}

		/// <summary>
		/// Reads time span configuration value.
		/// </summary>
		public static TimeSpan ReadTimeSpan(NameValueCollection source, string name)
		{
			string value = ReadValue(source, name);
			try
			{
				return TimeSpan.Parse(value);
			}
			catch
			{
				string message = String.Format("Configuration parameter '{0}' must contain a time span value.", name);
				throw new ConfigurationErrorsException(message);
			}
		}

		/// <summary>
		/// Reads time span configuration value.
		/// </summary>
		public static TimeSpan ReadTimeSpan(string name)
		{
			return ReadTimeSpan(ConfigurationManager.AppSettings, name);
		}

		/// <summary>
		/// Reads date time configuration value.
		/// </summary>
		public static DateTime ReadDateTime(NameValueCollection source, string name)
		{
			string value = ReadValue(source, name);
			try
			{
				return DateTime.Parse(value, DateTimeFormatInfo.InvariantInfo);
			}
			catch
			{
				string message = String.Format("Configuration parameter '{0}' must contain a date/time value.", name);
				throw new ConfigurationErrorsException(message);
			}
		}

		/// <summary>
		/// Reads date time configuration value.
		/// </summary>
		public static DateTime ReadDateTime(string name)
		{
			return ReadDateTime(ConfigurationManager.AppSettings, name);
		}

		/// <summary>
		/// Reads GUID configuration value.
		/// </summary>
		public static Guid ReadGuid(NameValueCollection source, string name)
		{
			string value = ReadValue(source, name);
			try
			{
				return new Guid(value);
			}
			catch
			{
				string message = String.Format("Configuration parameter '{0}' must contain a GUID value.", name);
				throw new ConfigurationErrorsException(message);
			}
		}

		/// <summary>
		/// Reads GUID configuration value.
		/// </summary>
		public static Guid ReadGuid(string name)
		{
			return ReadGuid(ConfigurationManager.AppSettings, name);
		}

		/// <summary>
		/// Reads enumeration value.
		/// </summary>
		public static object ReadEnumeration(NameValueCollection source, string name, Type type)
		{
			string value = ReadValue(source, name);
			try
			{
				return Enum.Parse(type, value);
			}
			catch
			{
				string message = String.Format("Configuration parameter '{0}' must contain a enumeration value of type {1}.", name, type.Name);
				throw new ConfigurationErrorsException(message);
			}
		}

		/// <summary>
		/// Reads enumeration value.
		/// </summary>
		public static object ReadEnumeration(string name, Type type)
		{
			return ReadEnumeration(ConfigurationManager.AppSettings, name, type);
		}

		#endregion

		#region Connection strings

		/// <summary>
		/// Reads connection string.
		/// </summary>
		private static string ReadConnectionString(ConnectionStringSettingsCollection source, string name)
		{
			ConnectionStringSettings settings = source[name];
			if (settings != null)
			{
				string str = settings.ConnectionString;
				if (!String.IsNullOrEmpty(str))
					return str;
			}

			string message = String.Format("Connection string '{0}' is not found.", name);
			throw new ConfigurationErrorsException(message);
		}

		/// <summary>
		/// Reads connection string.
		/// </summary>
		public static string ReadConnectionString(string name)
		{
			return ReadConnectionString(ConfigurationManager.ConnectionStrings, name);
		}

		#endregion
	}
}
