﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Manages naming macros.
	/// </summary>
	public static class NamingMacro
	{
		private const string c_ruleSeparator = ":";

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

			AddMacro(
				Resources.MacroAnyCode,
				Resources.MacroAnyDescription,
				Resources.MacroAnyRegular,
				Resources.MacroAnySample);
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
		/// Creates rule definition string from specified text.
		/// </summary>
		public static string ParseRuleFromText(string text)
		{
			string[] lines = text.Split(
				new[] { '\r', '\n' },
				StringSplitOptions.RemoveEmptyEntries);

			return String.Join(c_ruleSeparator, lines);
		}

		/// <summary>
		/// Creates text describing rule definition string.
		/// </summary>
		public static string ConvertRuleToText(string ruleDefinition)
		{
			return ruleDefinition.Replace(c_ruleSeparator, Environment.NewLine);
		}

		/// <summary>
		/// Checks if specified text can describe naming rule.
		/// </summary>
		public static bool CheckRule(string text)
		{
			text = HideMarkups(text);

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
				mask = mask.Replace('*', '_');
				result = result.Replace(markup, mask);
			}

			return result;
		}

		/// <summary>
		/// Highlights rich text box with rule definition text.
		/// </summary>
		public static void HighlightRule(RichTextBox rich)
		{
			if (rich.ReadOnly)
			{
				rich.SelectAll();
				rich.SelectionColor = SystemColors.GrayText;
				rich.BackColor = SystemColors.ControlLight;
				return;
			}

			if (CheckRule(rich.Text))
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

		#region Parsing special settings

		/// <summary>
		/// Creates definition string from whitespace-delimited text.
		/// </summary>
		public static string ParseWhitespacedSettingFromText(string text)
		{
			bool closed = true;
			StringBuilder sb = new StringBuilder();
			foreach (char c in text)
			{
				if (Char.IsWhiteSpace(c))
				{
					if (!closed)
					{
						sb.Append(' ');
						closed = true;
					}
				}
				else
				{
					sb.Append(c);
					closed = false;
				}
			}

			return sb.ToString().TrimEnd(' ');
		}

		/// <summary>
		/// Creates text describing whitespace-delimited setting.
		/// </summary>
		public static string ConvertWhitespacedSettingToText(string ruleDefinition)
		{
			return ruleDefinition;
		}

		/// <summary>
		/// Checks if specified text can describe a simple setting.
		/// </summary>
		public static bool CheckSimpleSetting(string text, ICharValidator validator)
		{
			foreach (char c in text)
			{
				if (!validator.IsValidChar(c))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Highlights rich text box with simple setting definition string.
		/// </summary>
		public static void HighlightSimpleSetting(
			RichTextBox rich,
			ITextValidator textValidator,
			ICharValidator charValidator)
		{
			if (textValidator.IsValidText(rich.Text))
			{
				rich.ResetBackColor();
			}
			else
			{
				rich.BackColor = Colors.LightRed;
			}

			int current = 0;
			foreach (string line in rich.Lines)
			{
				for (int i = 0; i < line.Length; i++)
				{
					if (!charValidator.IsValidChar(line[i]))
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
		public static string BuildExample(string ruleDefinition)
		{
			string text = ruleDefinition;
			foreach (string key in GetKeys())
			{
				string markup = GetMarkup(key);
				string sample = GetSample(key);
				text = text.Replace(markup, sample);
			}

			text = text.Replace(c_ruleSeparator, ", ");
			return text;
		}

		/// <summary>
		/// Builds regular expression for specified rule.
		/// </summary>
		public static Regex BuildRegex(
			string ruleDefinition,
			string abbreviations,
			string words)
		{
			string[] abbreviationsList = abbreviations.Split(
				new[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries);

			string abbreviationsExtension = String.Empty;
			if (abbreviationsList.Length > 0)
			{
				abbreviationsExtension = String.Format("|{0}", String.Join("|", abbreviationsList));
			}

			string[] wordsList = words.Split(
				new[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries);

			string wordsExtension = String.Empty;
			if (wordsList.Length > 0)
			{
				wordsExtension = String.Format("|{0}", String.Join("|", wordsList));
			}

			string extension = String.Format("{0}{1}", abbreviationsExtension, wordsExtension);

			string pattern = ruleDefinition;
			foreach (string key in GetKeys())
			{
				string markup = GetMarkup(key);
				string regular = "(" + GetRegular(key) + ")";
				regular = regular.Replace("UPPER", "[\\p{Lu}\\p{Nd}]");
				regular = regular.Replace("LOWER", "[\\p{Ll}\\p{Nd}]");
				regular = regular.Replace("DIGIT", "[\\p{Nd}]");
				regular = regular.Replace("EXTENSION", extension);
				pattern = pattern.Replace(markup, regular);
			}

			string[] lines = pattern.Split(
				new[] { c_ruleSeparator },
				StringSplitOptions.RemoveEmptyEntries);

			List<string> parts = new List<string>();
			foreach (string line in lines)
			{
				string part = String.Format("^{0}$", line);
				parts.Add(part);
			}

			pattern = String.Join("|", parts.ToArray());
			return new Regex(pattern);
		}

		#endregion
	}
}
