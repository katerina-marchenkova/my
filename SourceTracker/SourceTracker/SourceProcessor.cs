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
			string good = "`~!@#$%^&*()-_=+[{]};:'\"\\|,<.>/?1234567890abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
			foreach (char c in good)
				s_goodChars.Add(c);
		}

		/// <summary>
		/// Calculates CRC for specified line.
		/// </summary>
		public static Guid CalculateCrc(string line)
		{
			StringBuilder clean = new StringBuilder();
			foreach (char c in line.ToLowerInvariant())
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
