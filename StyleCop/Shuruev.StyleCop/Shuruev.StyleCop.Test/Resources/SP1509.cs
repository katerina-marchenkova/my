//# [OK]
//# Well-formed example.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			for (int i = 0; i < 10; i++)
			{
			}
		}
	}
}
//# [ERROR]
//# Excess blank line.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			for (int i = 0; i < 10; i++)

			{
			}
		}
	}
}
//# [ERROR]
//# Excess blank line.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int[] a = new int[]

			{
				10,
				20
			};
		}
	}
}
//# [OK]
//# Well-formed nested blocks.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			{
				int a = 10;
			}

			{
				int a = 10;
			}
		}
	}
}
//# [OK]
//# Well-formed nested blocks with declarations.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;

			{
				int a = 10;
			}

			int b = 10;

			{
				int a = 10;
			}
		}
	}
}
//# [OK]
//# Well-formed nested blocks with comments.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;

			// comment
			{
				int a = 10;
			}

			int b = 10;

			// comment
			{
				int a = 10;
			}
		}
	}
}
//# [OK]
//# Well-formed nested blocks with several declarations.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
			int b = 10;

			int c = 10;
			int d = 10;

			for (int i = 0; i < 10; i++)
			{
				int a = 10;
				int b = 10;
			}

			int e = 10;
			int f = 10;

			{
				int a = 10;
				int b = 10;
			}

			int g = 10;
			int h = 10;
		}
	}
}
//# [END OF FILE]