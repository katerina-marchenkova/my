using System;
using System.Collections.Generic;
using System.Text;

namespace Shuruev.StyleCop.Run
{
	public class Foo
	{
		protected internal readonly int f1;
		protected internal readonly int f2;

		/// <summary>
		/// Initializes a new instance. of the <see cref="Foo"/> class.
		/// </summary>
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
