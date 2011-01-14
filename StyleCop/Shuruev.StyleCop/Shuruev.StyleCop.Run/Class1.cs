using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Shuruev.StyleCop.Test
{
	public class Class1
	{
		public Class1()
		{
			int aaa = 10;
		}

		public Class1(int z)
			: this()
		{
		zzz:
			int aaa = 10;
		}

		~Class1()
		{
		zzz:
			int aaa = 10;
		}

		public string P
		{
			get
			{
				return null;
			}
			set
			{
				int a = 10;
				int b = 10;
			}
		}

		public void A()
		{
			int a = 10;
		}
	}
}
