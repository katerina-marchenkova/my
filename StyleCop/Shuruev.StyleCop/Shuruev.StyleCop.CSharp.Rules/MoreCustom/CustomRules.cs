using System.Collections.Generic;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// All custom rules.
	/// </summary>
	internal static class CustomRules
	{
		private static readonly List<string> s_groups = new List<string>();
		private static readonly Dictionary<string, List<CustomRule>> s_all = new Dictionary<string, List<CustomRule>>();

		static CustomRules()
		{
			Add(new CustomRuleSP2000(), Resources.GroupSpecial);
			Add(new CustomRuleSP2001(), Resources.GroupMethods);
		}

		/// <summary>
		/// Adds specified rule to inner collection.
		/// </summary>
		private static void Add(CustomRule customRule, string groupName)
		{
			if (!s_all.ContainsKey(groupName))
			{
				s_groups.Add(groupName);
				s_all[groupName] = new List<CustomRule>();
			}

			s_all[groupName].Add(customRule);
		}

		/// <summary>
		/// Returns a list of groups.
		/// </summary>
		public static List<string> GetGroups()
		{
			return new List<string>(s_all.Keys);
		}

		/// <summary>
		/// Returns a list of settings for specified group.
		/// </summary>
		public static List<CustomRule> GetByGroup(string groupName)
		{
			return new List<CustomRule>(s_all[groupName]);
		}
	}
}
