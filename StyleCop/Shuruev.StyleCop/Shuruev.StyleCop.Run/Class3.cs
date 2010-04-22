using System;

namespace Shuruev.StyleCop.Test
{
	internal class TestClass
	{
		private class Test2
		{
		}

		public void TestMethod()
		{
			for (int i = 0; i < 10; i++)
			{
			}

			int a = 10;

			{
				int b = 10;
			}

			{
				int c = 10;
			}

			int z = sizeof(bool);
			Type t = typeof(bool);
		}
	}
}
