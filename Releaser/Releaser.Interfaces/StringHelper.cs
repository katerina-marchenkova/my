using System;
using System.Collections.Generic;

namespace Shuruev.Releaser.Interfaces
{
	/// <summary>
	/// Reading configuration parameters.
	/// </summary>
	public static class StringHelper
	{
		#region Text processing

		/// <summary>
		/// Trims all line breaks, spaces and tabs.
		/// </summary>
		public static string TrimAll(string text)
		{
			return text.Trim('\r', '\n', '\v', '\t', ' ');
		}

		/// <summary>
		/// Removes all double spaces.
		/// </summary>
		public static string RemoveDoubleSpaces(string text)
		{
			while (text.Contains("  "))
			{
				text = text.Replace("  ", " ");
			}

			return text;
		}

		/// <summary>
		/// Trims all lines in text.
		/// </summary>
		public static string TrimLines(string text)
		{
			List<string> parts = new List<string>();
			string[] lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None);
			foreach (string line in lines)
			{
				string part = TrimAll(line);
				parts.Add(part);
			}

			text = String.Join("\r\n", parts.ToArray());
			text = TrimAll(text);
			return text;
		}

		/// <summary>
		/// Prepares text string to single line output.
		/// </summary>
		public static string ToSingleLine(string text)
		{
			text = text.Replace("\r\n", "\n");
			text = text.Replace("\n", " ");
			text = text.Replace("\v", " ");
			text = text.Replace("\t", " ");
			text = RemoveDoubleSpaces(text);
			text = TrimAll(text);
			return text;
		}

		/// <summary>
		/// Returnss true if specified string is null, empty,
		/// or contains whitespace characters only.
		/// </summary>
		public static bool IsEmpty(string text)
		{
			if (String.IsNullOrEmpty(text))
			{
				return true;
			}

			return TrimAll(text).Length == 0;
		}

		#endregion
	}
}
