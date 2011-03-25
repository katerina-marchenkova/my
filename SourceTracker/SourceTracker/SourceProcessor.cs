using System;
using System.Collections.Generic;
using System.Linq;
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
	}
}
