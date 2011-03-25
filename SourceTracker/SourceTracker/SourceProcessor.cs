using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SourceTracker
{
	/// <summary>
	/// Processes source codes.
	/// </summary>
	public class SourceProcessor
	{
		private static readonly MD5 s_md5 = new MD5CryptoServiceProvider();
		private static readonly HashSet<char> s_goodChars = new HashSet<char>();

		static SourceProcessor()
		{
			string good = "\t `~!@#$%^&*()-_=+[{]};:'\"\\|,<.>/?1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
			foreach (char c in good)
				s_goodChars.Add(c);
		}

		/// <summary>
		/// Calculates hash for specified line.
		/// </summary>
		public static Guid CalculateHash(string line)
		{
			StringBuilder clean = new StringBuilder();
			foreach (char c in line)
			{
				if (!s_goodChars.Contains(c))
					continue;

				clean.Append(c);
			}

			byte[] data = Encoding.UTF8.GetBytes(clean.ToString());
			byte[] crc = s_md5.ComputeHash(data);
			return new Guid(crc);
		}
	}
}
