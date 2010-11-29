using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
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
		private static PropertyDescriptor GetDescriptor(SourceAnalyzer analyzer, string settingName)
		{
			PropertyDescriptor descriptor = analyzer.PropertyDescriptors[settingName];
			if (descriptor == null)
				throw new InvalidDataException("Setting " + settingName + " is not registered as a known one.");

			return descriptor;
		}

		/// <summary>
		/// Gets property value for specified setting.
		/// </summary>
		public static T GetValue<T>(SourceAnalyzer analyzer, Settings settings, string settingName)
		{
			T customValue;
			if (GetCustomValue(analyzer, settingName, out customValue))
			{
				return customValue;
			}

			PropertyValue<T> setting = (PropertyValue<T>)analyzer.GetSetting(settings, settingName);
			if (setting == null)
			{
				PropertyDescriptor<T> descriptor = (PropertyDescriptor<T>)GetDescriptor(analyzer, settingName);
				return descriptor.DefaultValue;
			}

			return setting.Value;
		}

		/// <summary>
		/// Gets customized value if it is possible.
		/// </summary>
		private static bool GetCustomValue<T>(SourceAnalyzer analyzer, string settingName, out T value)
		{
			if (analyzer is StyleCopPlus)
			{
				StyleCopPlus styleCopPlus = (StyleCopPlus)analyzer;
				if (styleCopPlus.SpecialRunningParameters != null)
				{
					if (styleCopPlus.SpecialRunningParameters.CustomSettings != null)
					{
						if (styleCopPlus.SpecialRunningParameters.CustomSettings.ContainsKey(settingName))
						{
							value = (T)styleCopPlus.SpecialRunningParameters.CustomSettings[settingName];
							return true;
						}
					}
				}
			}

			value = default(T);
			return false;
		}

		/// <summary>
		/// Gets friendly name for specified setting.
		/// </summary>
		public static string GetFriendlyName(SourceAnalyzer analyzer, string settingName)
		{
			PropertyDescriptor descriptor = GetDescriptor(analyzer, settingName);
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
			return GetValue<string>(page.Analyzer, page.TabControl.MergedSettings, settingName);
		}

		/// <summary>
		/// Gets an inherited value for specified setting.
		/// </summary>
		public static string GetInheritedValue(PropertyPage page, string settingName)
		{
			return GetValue<string>(page.Analyzer, page.TabControl.ParentSettings, settingName);
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

		#region Accessing foreign settings

		/// <summary>
		/// Gets rules tree from the user interface.
		/// </summary>
		private static TreeView GetRulesTree(PropertyControl tabControl)
		{
			IPropertyControlPage analyzersOptions = tabControl.Pages[0];
			return (TreeView)analyzersOptions.GetType().InvokeMember(
				"analyzeTree",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField,
				null,
				analyzersOptions,
				null);
		}

		/// <summary>
		/// Gets tree node for specified analyzer.
		/// </summary>
		private static TreeNode GetAnalyzerNode(TreeView tree, string analyzerId)
		{
			foreach (TreeNode parserNode in tree.Nodes)
			{
				foreach (TreeNode analyzerNode in parserNode.Nodes)
				{
					SourceAnalyzer analyzer = (SourceAnalyzer)analyzerNode.Tag;
					if (analyzer.Id == analyzerId)
						return analyzerNode;
				}
			}

			return null;
		}

		/// <summary>
		/// Analyzes rule nodes.
		/// </summary>
		private static void AnalyzeRuleNodes(Dictionary<string, bool> checkedMap, TreeNode parentNode)
		{
			foreach (TreeNode node in parentNode.Nodes)
			{
				if (node.Tag is Rule)
				{
					Rule rule = (Rule)node.Tag;
					checkedMap[rule.Name] = node.Checked;
				}
				else
				{
					AnalyzeRuleNodes(checkedMap, node);
				}
			}
		}

		/// <summary>
		/// Gets internal ID for specified analyzer.
		/// </summary>
		public static string GetAnalyzerId(string analyzerName)
		{
			return String.Format("Microsoft.StyleCop.CSharp.{0}", analyzerName);
		}

		/// <summary>
		/// Grabs the map of checked rules from UI for specified analyzer.
		/// </summary>
		public static Dictionary<string, bool> GrabCheckedRulesMap(PropertyControl tabControl, string analyzerId)
		{
			TreeView rulesTree = GetRulesTree(tabControl);
			TreeNode analyzerNode = GetAnalyzerNode(rulesTree, analyzerId);

			Dictionary<string, bool> checkedMap = new Dictionary<string, bool>();
			AnalyzeRuleNodes(checkedMap, analyzerNode);

			return checkedMap;
		}

		#endregion
	}
}
