using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Manages naming macros.
	/// </summary>
	public static class NamingMacro
	{
		private static readonly List<string> s_keys = new List<string>();
		private static readonly Dictionary<string, string> s_descriptions = new Dictionary<string, string>();
		private static readonly Dictionary<string, string> s_markups = new Dictionary<string, string>();
		private static readonly Dictionary<string, string> s_regulars = new Dictionary<string, string>();
		private static readonly Dictionary<string, string> s_samples = new Dictionary<string, string>();

		static NamingMacro()
		{
			AddMacro(
				Resources.MacroPascalCode,
				Resources.MacroPascalDescription,
				Resources.MacroPascalRegular,
				Resources.MacroPascalSample);

			AddMacro(
				Resources.MacroCamelCode,
				Resources.MacroCamelDescription,
				Resources.MacroCamelRegular,
				Resources.MacroCamelSample);

			AddMacro(
				Resources.MacroUpperCode,
				Resources.MacroUpperDescription,
				Resources.MacroUpperRegular,
				Resources.MacroUpperSample);

			AddMacro(
				Resources.MacroLowerCode,
				Resources.MacroLowerDescription,
				Resources.MacroLowerRegular,
				Resources.MacroLowerSample);

			AddMacro(
				Resources.MacroCapitalizedCode,
				Resources.MacroCapitalizedDescription,
				Resources.MacroCapitalizedRegular,
				Resources.MacroCapitalizedSample);
		}

		#region Accessing macros

		/// <summary>
		/// Adds a macro.
		/// </summary>
		private static void AddMacro(
			string key,
			string description,
			string regular,
			string sample)
		{
			string markup = String.Format("$({0})", key);

			s_keys.Add(key);
			s_descriptions.Add(key, description);
			s_markups.Add(key, markup);
			s_regulars.Add(key, regular);
			s_samples.Add(key, sample);
		}

		/// <summary>
		/// Gets keys for all macros.
		/// </summary>
		public static List<string> GetKeys()
		{
			return new List<string>(s_keys);
		}

		/// <summary>
		/// Gets description for specified macro.
		/// </summary>
		public static string GetDescription(string key)
		{
			return s_descriptions[key];
		}

		/// <summary>
		/// Gets markup for specified macro.
		/// </summary>
		public static string GetMarkup(string key)
		{
			return s_markups[key];
		}

		/// <summary>
		/// Gets regular expression for specified macro.
		/// </summary>
		public static string GetRegular(string key)
		{
			return s_regulars[key];
		}

		/// <summary>
		/// Gets sample name for specified macro.
		/// </summary>
		public static string GetSample(string key)
		{
			return s_samples[key];
		}

		#endregion

		#region Parsing rule definitions

		/// <summary>
		/// Cleans meaningless data from specified text.
		/// </summary>
		public static string Clean(string text)
		{
			string[] lines = text.Split(
				new[] { '\r', '\n' },
				StringSplitOptions.RemoveEmptyEntries);

			return String.Join("\r\n", lines);
		}

		/// <summary>
		/// Checks if specified text can describe naming rule.
		/// </summary>
		public static bool Check(string text)
		{
			text = HideMarkups(text);
			text = Clean(text);

			string[] lines = text.Split(
				new[] { '\r', '\n' },
				StringSplitOptions.RemoveEmptyEntries);

			if (lines.Length == 0)
				return false;

			foreach (string line in lines)
			{
				foreach (char c in line)
				{
					if (!IsValidIdentifierChar(c))
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Checks if specified character is valid for using in identifier.
		/// </summary>
		private static bool IsValidIdentifierChar(char c)
		{
			if (Char.IsLetterOrDigit(c))
				return true;

			if (c == '_')
				return true;

			return false;
		}

		/// <summary>
		/// Hides all markups in specified text.
		/// </summary>
		private static string HideMarkups(string text)
		{
			string result = text;
			foreach (string markup in s_markups.Values)
			{
				string mask = markup;
				mask = mask.Replace('$', '_');
				mask = mask.Replace('(', '_');
				mask = mask.Replace(')', '_');
				result = result.Replace(markup, mask);
			}

			return result;
		}

		/// <summary>
		/// Highlights specified rich text box.
		/// </summary>
		public static void Highlight(RichTextBox rich)
		{
			if (rich.ReadOnly)
			{
				rich.SelectAll();
				rich.SelectionColor = SystemColors.GrayText;
				rich.BackColor = SystemColors.ControlLight;
				return;
			}

			if (Check(rich.Text))
			{
				rich.BackColor = Colors.LightYellow;
			}
			else
			{
				rich.BackColor = Colors.LightRed;
			}

			int current = 0;
			foreach (string line in rich.Lines)
			{
				string shadow = HideMarkups(line);

				foreach (string markup in s_markups.Values)
				{
					int index = line.IndexOf(markup);
					while (index >= 0)
					{
						rich.SelectionStart = current + index;
						rich.SelectionLength = markup.Length;
						rich.SelectionColor = Color.Green;
						index = line.IndexOf(markup, index + 1);
					}
				}

				for (int i = 0; i < shadow.Length; i++)
				{
					if (!IsValidIdentifierChar(shadow[i]))
					{
						rich.SelectionStart = current + i;
						rich.SelectionLength = 1;
						rich.SelectionColor = Color.Red;
					}
				}

				current += line.Length + 1;
			}
		}

		#endregion

		#region Applying rule definitions

		/// <summary>
		/// Builds example text for specified rule.
		/// </summary>
		public static string BuildExample(string text)
		{
			text = Clean(text);
			foreach (string key in GetKeys())
			{
				string markup = GetMarkup(key);
				string sample = GetSample(key);
				text = text.Replace(markup, sample);
			}

			text = text.Replace("\r\n", ", ");
			return text;
		}

		#endregion
	}
}
