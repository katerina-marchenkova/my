using System;
using System.Collections.Generic;
using System.Xml;

namespace Shuruev.StyleCop.Test
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
	public class A
	{
		public const int C1 = 0;
		public static int Cz = 0;
		protected const int c_as3452 = 0;
		private const int c_3 = 0;
		internal const int C4 = 0;

		public static A operator --(A a)
		{
			foreach (var VARIABLE in new List<int>())
			{
				int z = 10;
			}
			return new A();
		}

		public class B1Attribute : System.Attribute
		{
		}

		public class B2Attribute : Attribute
		{
		}

		public class D1Attribute : B1Attribute
		{
		}

		protected class B2Exception : System.Exception
		{
		}

		internal class B3<T>
		{
		}

		protected internal class B4
		{
		}

		private class B5 : B3<int>
		{
		}
	}

	/*internal class B
	{
		public class B1
		{
		}

		protected class B2
		{
		}

		internal class B3
		{
		}

		protected internal class B4
		{
		}

		private class B5
		{
		}
	}*/
}
