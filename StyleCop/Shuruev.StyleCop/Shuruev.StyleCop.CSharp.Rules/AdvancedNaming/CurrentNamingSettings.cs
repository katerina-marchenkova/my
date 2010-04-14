using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.StyleCop;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Naming settings in current analyzer session.
	/// </summary>
	public class CurrentNamingSettings
	{
		private readonly Dictionary<string, string> m_names;
		private readonly Dictionary<string, string> m_examples;
		private readonly Dictionary<string, Regex> m_regulars;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CurrentNamingSettings()
		{
			m_names = new Dictionary<string, string>();
			m_examples = new Dictionary<string, string>();
			m_regulars = new Dictionary<string, Regex>();
		}

		/// <summary>
		/// Initializes settings from specified document.
		/// </summary>
		public void Initialize(SourceAnalyzer analyzer, CodeDocument document)
		{
			m_names.Clear();
			m_examples.Clear();
			m_regulars.Clear();

			foreach (string setting in NamingSettings.All)
			{
				string name = SettingsManager.GetFriendlyName(analyzer, setting);
				m_names.Add(setting, name);

				string value = SettingsManager.GetSettingValue(analyzer, document.Settings, setting);
				if (String.IsNullOrEmpty(value))
				{
					m_examples.Add(setting, null);
					m_regulars.Add(setting, null);
				}
				else
				{
					string example = NamingMacro.BuildExample(value);
					m_examples.Add(setting, example);

					Regex regex = NamingMacro.BuildRegex(value);
					m_regulars.Add(setting, regex);
				}
			}
		}

		/// <summary>
		/// Gets friendly name for specified setting.
		/// </summary>
		public string GetFriendlyName(string settingName)
		{
			return m_names[settingName];
		}

		/// <summary>
		/// Gets example text for specified setting.
		/// </summary>
		public string GetExample(string settingName)
		{
			return m_examples[settingName];
		}

		/// <summary>
		/// Gets regular expression for specified setting.
		/// </summary>
		public Regex GetRegex(string settingName)
		{
			return m_regulars[settingName];
		}
	}
}
