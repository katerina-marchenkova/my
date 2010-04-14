using System.IO;
using Microsoft.StyleCop;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Settings manager.
	/// </summary>
	public static class SettingsManager
	{
		#region Common methods

		/// <summary>
		/// Gets property descriptor with existing check.
		/// </summary>
		public static PropertyDescriptor<string> GetPropertyDescriptor(SourceAnalyzer analyzer, string settingName)
		{
			PropertyDescriptor<string> descriptor = (PropertyDescriptor<string>)analyzer.PropertyDescriptors[settingName];
			if (descriptor == null)
				throw new InvalidDataException("Setting " + settingName + " is not registered as a known one.");

			return descriptor;
		}

		/// <summary>
		/// Gets a value for specified setting.
		/// </summary>
		public static string GetSettingValue(SourceAnalyzer analyzer, Settings settings, string settingName)
		{
			StringProperty setting = (StringProperty)analyzer.GetSetting(settings, settingName);
			if (setting == null)
			{
				PropertyDescriptor<string> descriptor = GetPropertyDescriptor(analyzer, settingName);
				return descriptor.DefaultValue;
			}

			return setting.Value;
		}

		/// <summary>
		/// Gets friendly name for specified setting.
		/// </summary>
		public static string GetFriendlyName(SourceAnalyzer analyzer, string settingName)
		{
			PropertyDescriptor<string> descriptor = GetPropertyDescriptor(analyzer, settingName);
			return descriptor.FriendlyName;
		}

		#endregion

		#region Accessing via property page

		/// <summary>
		/// Gets friendly name for specified setting.
		/// </summary>
		public static string GetFriendlyName(PropertyPage page, string settingName)
		{
			return GetFriendlyName(page.Analyzer, settingName);
		}

		/// <summary>
		/// Gets a merged value for specified setting.
		/// </summary>
		public static string GetMergedValue(PropertyPage page, string settingName)
		{
			return GetSettingValue(page.Analyzer, page.TabControl.MergedSettings, settingName);
		}

		/// <summary>
		/// Gets an inherited value for specified setting.
		/// </summary>
		public static string GetInheritedValue(PropertyPage page, string settingName)
		{
			return GetSettingValue(page.Analyzer, page.TabControl.ParentSettings, settingName);
		}

		/// <summary>
		/// Sets a local value for specified setting.
		/// </summary>
		public static void SetLocalValue(PropertyPage page, string settingName, string value)
		{
			StringProperty property = new StringProperty(page.Analyzer, settingName, value);
			page.Analyzer.SetSetting(page.TabControl.LocalSettings, property);
		}

		/// <summary>
		/// Clears a local value for specified setting.
		/// </summary>
		public static void ClearLocalValue(PropertyPage page, string settingName)
		{
			page.Analyzer.ClearSetting(page.TabControl.LocalSettings, settingName);
		}

		#endregion
	}
}
