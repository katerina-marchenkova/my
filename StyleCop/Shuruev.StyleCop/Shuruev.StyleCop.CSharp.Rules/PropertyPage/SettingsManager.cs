using Microsoft.StyleCop;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Settings manager.
	/// </summary>
	public static class SettingsManager
	{
		/// <summary>
		/// Gets a value for naming rule.
		/// </summary>
		public static string GetNamingRule(PropertyPage page, string settingName, out bool modified)
		{
			PropertyDescriptor<string> descriptor = (PropertyDescriptor<string>)page.Analyzer.PropertyDescriptors[settingName];

			StringProperty setting = (StringProperty)page.Analyzer.GetSetting(page.TabControl.MergedSettings, settingName);
			if (setting == null)
			{
				modified = false;
				return descriptor.DefaultValue;
			}

			modified = page.TabControl.SettingsComparer.IsAddInSettingOverwritten(page.Analyzer, settingName, setting);
			return setting.Value;
		}
	}
}
