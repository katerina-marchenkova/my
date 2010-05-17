using System;
using System.Collections.Generic;
using System.Text;

namespace Shuruev.StyleCop.Run
{
	public class Foo
	{
		public delegate void OlegOLOLO();
		public event EventHandler OlegOLOLa;

		protected internal readonly int Foo1;
		protected internal readonly int Foo2;

		public string ZzXXX { get; set; }

		public int OlegShuruev
		{
			get { return 10; }
		}

		/// <summary>
		/// Initializes a new instance. of the <see cref="Foo"/> class.
		/// </summary>
		public Foo()
		{
			int x = 10;
			{
				int a = 1;
				this.Foo1 = a + 15;
			}

			{
				int b = 2;
				this.Foo2 = b + 15;
			}
		}
	}
}
