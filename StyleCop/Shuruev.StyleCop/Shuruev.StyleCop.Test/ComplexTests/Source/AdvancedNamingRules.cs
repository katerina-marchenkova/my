﻿#region AdvancedNamingRules // Methods

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

		[TestMethod]
		public void StyleCop_Test_Method()
		{
		}

		[TestMethod]
		public void FxCop_Test_Method()
		{
		}
	}
}
//# [END]

//# [OK]
//# Explicit interface implementation.
namespace Shuruev.StyleCop.Test
{
	public class TestClass : IDisposable
	{
		void IDisposable.Dispose()
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

//# [OK]
//# Type parameters should not be confused with generics.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public delegate List<bool> TestDelegate<in TInput>(List<char> args);

		public class TestClass1<TFirst>
		{
			public List<string> TestMethod<TSecond>(IEnumerable<int> list)
			{
				List<byte> a = new List<byte>();
				return new List<string>();
			}
		}

		public class TestClass2 : List<int>
		{
		}

		public class TestClass3<TKeys>
			where TKeys : IEnumerable<int>
		{
		}
	}
}
//# [END]

//# [OK]
//# One-letter type parameters.
namespace Shuruev.StyleCop.Test
{
	public class TestClass<T>
	{
		public void TestMethod()
		{
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
//# Invalid type parameter name in nested class.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public class InnerClass<Keys>
			where Keys : IEnumerable<int>
		{
		}
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

#region AdvancedNamingRules // Local variables

//# (AdvancedNaming_LocalVariable = A:B:t)

//# [OK]
//# All local variable names are correct.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public TestClass()
		{
			int A = 10;
		}

		public int this[int index]
		{
			get
			{
				int A = 10;
				return 0;
			}
		}

		public void TestMethod()
		{
			int A = 10;
			Thread t = new Thread(() =>
			{
				int B = 20;
			});
		}

		public static explicit operator Foo(int x)
		{
			int A = 10;
			return null;
		}

		public static implicit operator int(Foo x)
		{
			int A = 10;
			return 0;
		}

		public static bool operator +(Foo a, Foo b)
		{
			int A = 10;
			return false;
		}
	}
}
//# [END]

//# [ERROR]
//# Invalid local variable name in constructor.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public TestClass()
		{
			int X = 10;
		}
	}
}
//# [END]

//# [ERROR]
//# Invalid local variable name in indexer.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int this[int index]
		{
			get
			{
				int X = 10;
				return 0;
			}
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid local variable names in method.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int X = 10;
			Thread t = new Thread(() =>
			{
				int Z = 20;
			});
		}
	}
}
//# [END]

//# [ERROR:3]
//# Invalid local variable names in operators.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public static explicit operator Foo(int x)
		{
			int X = 10;
			return null;
		}

		public static implicit operator int(Foo x)
		{
			int X = 10;
			return 0;
		}

		public static bool operator +(Foo a, Foo b)
		{
			int X = 10;
			return false;
		}
	}
}
//# [END]

#endregion

#region AdvancedNamingRules // Local constants

//# (AdvancedNaming_LocalVariable = t)
//# (AdvancedNaming_LocalConstant = A:B)

//# [OK]
//# All local constant names are correct.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public TestClass()
		{
			const int A = 10;
		}

		public int this[int index]
		{
			get
			{
				const int A = 10;
				return 0;
			}
		}

		public void TestMethod()
		{
			const int A = 10;
			Thread t = new Thread(() =>
			{
				const int B = 20;
			});
		}

		public static explicit operator Foo(int x)
		{
			const int A = 10;
			return null;
		}

		public static implicit operator int(Foo x)
		{
			const int A = 10;
			return 0;
		}

		public static bool operator +(Foo a, Foo b)
		{
			const int A = 10;
			return false;
		}
	}
}
//# [END]

//# [ERROR]
//# Invalid local constant name in constructor.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public TestClass()
		{
			const int X = 10;
		}
	}
}
//# [END]

//# [ERROR]
//# Invalid local constant name in indexer.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int this[int index]
		{
			get
			{
				const int X = 10;
				return 0;
			}
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid local constant names in method.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			const int X = 10;
			Thread t = new Thread(() =>
			{
				const int Z = 20;
			});
		}
	}
}
//# [END]

//# [ERROR:3]
//# Invalid local constant names in operators.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public static explicit operator Foo(int x)
		{
			const int X = 10;
			return null;
		}

		public static implicit operator int(Foo x)
		{
			const int X = 10;
			return 0;
		}

		public static bool operator +(Foo a, Foo b)
		{
			const int X = 10;
			return false;
		}
	}
}
//# [END]

#endregion