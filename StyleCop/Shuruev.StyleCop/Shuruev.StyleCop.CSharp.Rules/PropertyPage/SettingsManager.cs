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
		private static PropertyDescriptor<string> GetPropertyDescriptor(SourceAnalyzer analyzer, string settingName)
		{
			PropertyDescriptor<string> descriptor = (PropertyDescriptor<string>)analyzer.PropertyDescriptors[settingName];
			if (descriptor == null)
				throw new InvalidDataException("Setting " + settingName + " is not registered as a known one.");

			return descriptor;
		}

		/// <summary>
		/// Gets a value for specified setting.
		/// </summary>
		private static string GetValue(SourceAnalyzer analyzer, Settings settings, string settingName)
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
		public static string GetFriendlyName(PropertyPage page, string settingName)
		{
			PropertyDescriptor<string> descriptor = GetPropertyDescriptor(page.Analyzer, settingName);
			return descriptor.FriendlyName;
		}

		/// <summary>
		/// Gets a merged value for specified setting.
		/// </summary>
		public static string GetMergedValue(PropertyPage page, string settingName)
		{
			return GetValue(page.Analyzer, page.TabControl.MergedSettings, settingName);
		}

		/// <summary>
		/// Gets an inherited value for specified setting.
		/// </summary>
		public static string GetInheritedValue(PropertyPage page, string settingName)
		{
			return GetValue(page.Analyzer, page.TabControl.ParentSettings, settingName);
		}

		#endregion
	}
}
