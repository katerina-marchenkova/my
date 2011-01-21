using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Shuruev.StyleCop.Test
{
	public class Class1
	{
		public void A()
		{
			bool a = DateTime.Now.Day > 5;
			bool b = DateTime.Now.Day > 5;
			bool c = DateTime.Now.Day > 5;
			bool d = DateTime.Now.Day > 5;

			if (a || b ||
			    c || d)
			{
			}

			if (a || b ||
			        c || d)
			{
			}

			if
			(
			a || b ||
			c || d
			)
			{
			}

			bool z =
					a || ((b && c)
						   || d);
		}
	}
}
