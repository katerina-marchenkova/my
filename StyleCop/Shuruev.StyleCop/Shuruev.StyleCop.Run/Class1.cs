using System;
using System.Collections.Generic;
using System.Text;

namespace Shuruev.StyleCop.Run
{
	public class Foo
	{
		readonly int f1;
		readonly int f2;

		public Foo()
		{
			int x = 10;
			{
				int a = 1;
				this.f1 = a + 15;
			}

			{
				int b = 2;
				this.f2 = b + 15;
			}
		}
	}
}
