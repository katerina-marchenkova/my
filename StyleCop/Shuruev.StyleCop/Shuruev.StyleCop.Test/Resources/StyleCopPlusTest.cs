#region CodeLineMustNotEndWithWhitespace

//# [OK]
//# Source file is OK.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
//# [END]

//# [OK]
//# Source file without a line break at the end.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{

			int a = 10;
		}
	}
}//# [END]

//# [ERROR]
//# There is excess whitespace at the end of the line.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10; 
		}
	}
}
//# [END]

//# [ERROR]
//# There is excess whitespace at the end of the line.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;	
		}
	}
}
//# [END]

//# [ERROR]
//# There are excess whitespaces at the end of the line.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{	 
			int a = 10;
		}
	}
}
//# [END]

//# [OK]
//# There are no excess whitespaces at the end of the line.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{

			int a = 10;
		}
	}
}
//# [END]

//# [ERROR]
//# There are excess whitespaces at the end of the line.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			
			int a = 10;
		}
	}
}
//# [END]

//# [OK]
//# There are no excess whitespaces at the end of the source file.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
		}
	}
}

//# [END]

//# [ERROR]
//# There is excess whitespace at the end of the source file.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
 
//# [END]

//# [OK]
//# There are no excess whitespaces at the beginning of the source file.

namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
//# [END]

//# [ERROR]
//# There is excess whitespace at the beginning of the source file.
 
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
//# [END]

#endregion

#region CodeLineMustBeginWithIdenticalWhitespaces

//# [OK]
//# Each line begins with identical characters.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
// [END]

//# [ERROR]
//# One line starts with tab and whitespace.
namespace Shuruev.StyleCop.Test
{
	 public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
// [END]

//# [OK]
//# Each line begins with identical characters.
namespace Shuruev.StyleCop.Test
{
    public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
		}
    }
}
// [END]

//# [OK]
//# Each line begins with identical characters.

namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
    
// [END]

//# [OK]
//# Each line begins with identical characters.
    
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
		}
	}
}

// [END]

#endregion

#region OpeningCurlyBracketsMustNotBePrecededByBlankLine

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
//# [END]

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
//# [END]

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
//# [END]

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
//# [END]

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
//# [END]

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
//# [END]

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
//# [END]

#endregion

#region AdvancedNamingRules // Simple Tests

//# [ERROR:3]
//# Some enumeration and enumeration item names are incorrect.
namespace Shuruev.StyleCop.Test
{
	public enum TestENUM
	{
		ITEM1,
		ITEM2
	}

	public enum TestEnum
	{
		Item1,
		Item2
	}
}
//# [END]

#endregion