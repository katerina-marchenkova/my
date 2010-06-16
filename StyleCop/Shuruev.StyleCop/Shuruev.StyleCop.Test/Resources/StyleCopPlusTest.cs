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

#region AdvancedNamingRules // Methods

//# [OK]
//# Method name is correct.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
		}
	}
}
//# [END]

//# [ERROR]
//# Method name is incorrect.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void Test_Method()
		{
		}
	}
}
//# [END]

//# [OK]
//# Test method names are correct.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		[TestMethod]
		public void TestMethod()
		{
		}

		[TestMethod]
		public void Test_Method()
		{
		}
	}
}
//# [END]

#endregion

#region AdvancedNamingRules // Enumerations

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

#region AdvancedNamingRules // Parameters

//# [OK]
//# There are no parameters at all.
namespace Shuruev.StyleCop.Test
{
	public delegate bool TestDelegate();

	public class TestClass
	{
		public TestClass()
		{
		}

		public void TestMethod()
		{
		}
	}
}
//# [END]

//# [OK]
//# All parameter names are correct.
namespace Shuruev.StyleCop.Test
{
	public delegate bool TestDelegate(IEnumerable<int> list);

	public class TestClass
	{
		public TestClass(int a)
		{
		}

		public int this[int index]
		{
			get { return 0; }
		}

		public void TestMethod(int count)
		{
		}

		public void TestMethod(ref byte size)
		{
		}

		public void TestMethod(out string text)
		{
			text = null;
		}

		public void TestMethod(params string[] args)
		{
		}

		public static explicit operator Foo(int x)
		{
			return null;
		}

		public static implicit operator int(Foo x)
		{
			return 0;
		}

		public static bool operator +(Foo a, Foo b)
		{
			return false;
		}
	}
}
//# [END]

//# [ERROR]
//# Invalid parameter name in constructor.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public TestClass(int A)
		{
		}
	}
}
//# [END]

//# [ERROR]
//# Invalid parameter name in delegate.
namespace Shuruev.StyleCop.Test
{
	public delegate bool TestDelegate(IEnumerable<int> List);
}
//# [END]

//# [ERROR]
//# Invalid parameter name in indexer.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int this[int Index]
		{
			get { return 0; }
		}
	}
}
//# [END]

//# [ERROR:4]
//# Invalid parameter names in methods.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod(int Count)
		{
		}

		public void TestMethod(ref byte Size)
		{
		}

		public void TestMethod(out string Text)
		{
			Text = null;
		}

		public void TestMethod(params string[] Args)
		{
		}
	}
}
//# [END]

//# [ERROR:3]
//# Invalid parameter names in operators.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public static explicit operator Foo(int X)
		{
			return null;
		}

		public static implicit operator int(Foo Y)
		{
			return 0;
		}

		public static bool operator +(Foo A, Foo B)
		{
			return false;
		}
	}
}
//# [END]

//# [ERROR]
//# Only first parameter is invalid.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public static bool operator +(Foo A, Foo b)
		{
			return false;
		}
	}
}
//# [END]

//# [ERROR]
//# Only second parameter is invalid.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public static bool operator +(Foo a, Foo B)
		{
			return false;
		}
	}
}
//# [END]

#endregion

#region AdvancedNamingRules // Type parameters

//# [OK]
//# All type parameter names are correct.
namespace Shuruev.StyleCop.Test
{
	public delegate TOutput TestDelegate<TInput, TOutput>(TInput args)
		where TInput : IEnumerable<byte>
		where TOutput : IEnumerable<byte>;

	public delegate TOutput AnotherDelegate<in TInput, out TOutput>(TInput args)
		where TInput : IEnumerable<byte>
		where TOutput : IEnumerable<byte>;

	public delegate bool InputDelegate<in TInput>(TInput args)
		where TInput : IEnumerable<byte>;

	public delegate TOutput OutputDelegate<out TOutput>(int count)
		where TOutput : IEnumerable<byte>;

	public class TestClass<TKeys>
		where TKeys : IEnumerable<int>
	{
		public TResult TestMethod<TResult>(int arg)
			where TResult : new()
		{
			return new TResult();
		}
	}
}
//# [END]

//# [ERROR]
//# Invalid type parameter name in class.
namespace Shuruev.StyleCop.Test
{
	public class TestClass<Keys>
		where Keys : IEnumerable<int>
	{
	}
}
//# [END]

//# [ERROR]
//# Invalid type parameter name in method.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public Result TestMethod<Result>(int arg)
			where Result : new()
		{
			return new Result();
		}
	}
}
//# [END]

//# [ERROR:3]
//# Invalid type parameter names in delegates.
namespace Shuruev.StyleCop.Test
{
	public delegate Output TestDelegate<Input, Output>(Input args)
		where Input : IEnumerable<byte>
		where Output : IEnumerable<byte>;

	public delegate bool InputDelegate<in Input>(Input args)
		where Input : IEnumerable<byte>;

	public delegate Output OutputDelegate<out Output>(int count)
		where Output : IEnumerable<byte>;
}
//# [END]

//# [ERROR:2]
//# Only first parameter is invalid.
namespace Shuruev.StyleCop.Test
{
	public delegate TOutput TestDelegate<Input, TOutput>(Input args);
	public delegate TOutput AnotherDelegate<in Input, out TOutput>(Input args);
}
//# [END]

//# [ERROR:2]
//# Only second parameter is invalid.
namespace Shuruev.StyleCop.Test
{
	public delegate Output TestDelegate<TInput, Output>(TInput args);
	public delegate Output AnotherDelegate<in TInput, out Output>(TInput args);
}
//# [END]

#endregion