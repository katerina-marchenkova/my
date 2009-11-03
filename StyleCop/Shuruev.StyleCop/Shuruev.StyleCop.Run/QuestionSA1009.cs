using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.StyleCop;

namespace Shuruev.StyleCop.Run
{
	/// <summary>
	/// Question about SA1009.
	/// </summary>
	public static class QuestionSA1009
	{
		/// <summary>
		/// Test method.
		/// </summary>
		public static void Test1()
		{
			object obj = new object();
			bool flag = (bool)typeof(Uri).InvokeMember(
				"flag",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField,
				null,
				obj,
				null);

			string text = "text";
			string copy = (string) text.Clone();
		}

		/// <summary>
		/// Test method.
		/// </summary>
		public static void Test2()
		{
			string text = "text";
			text = (string)text.Clone();
		}
	}
}
