using System;
using System.Xml;

namespace Shuruev.StyleCop.Test
{
	public class A
	{
		public class B1Attribute : System.Attribute
		{
		}

		public class B2Attr : Attribute
		{
		}

		public class D1Attr : B1Attribute
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
