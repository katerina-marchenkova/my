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

// (AdvancedNaming_Enum = Test$(AaBb))
// (AdvancedNaming_EnumItem = Test$(AaBb))

//# [OK]
//# All enumeration and enumeration item names are correct.
namespace Shuruev.StyleCop.Test
{
	public enum TestEnum
	{
		TestItem1,
		TestItem2
	}
}
//# [END]

//# [ERROR:3]
//# All enumeration and enumeration item names are incorrect.
namespace Shuruev.StyleCop.Test
{
	public enum TESTENUM
	{
		ITEM1,
		ITEM2
	}
}
//# [END]

#endregion

#region AdvancedNamingRules // Parameters

//# (AdvancedNaming_Parameter = A:B:X:Y)

//# [OK]
//# All parameter names are correct.
namespace Shuruev.StyleCop.Test
{
	public delegate bool TestDelegate(int X, int Y);

	public class TestClass
	{
		public TestClass(int X)
		{
			TestDelegate lambda = (A, B) => A == B;
		}

		public int Count
		{
			get
			{
				TestDelegate lambda = (A, B) => A == B;
				return 0;
			}
			set
			{
				TestDelegate lambda = (A, B) => A == B;
			}
		}

		public int this[int Y]
		{
			get
			{
				TestDelegate lambda = (A, B) => A == B;
				return 0;
			}
			set
			{
				TestDelegate lambda = (A, B) => A == B;
			}
		}

		public void TestMethod(int X, params string[] Y)
		{
			TestDelegate lambda = (A, B) => A == B;
		}

		public static bool operator +(TestClass X, TestClass Y)
		{
			TestDelegate lambda = (A, B) => A == B;
			return false;
		}
	}
}
//# [END]

//# [ERROR]
//# Invalid parameter names in delegate.
namespace Shuruev.StyleCop.Test
{
	public delegate bool TestDelegate(int X1, int Y1);
}
//# [END]

//# [ERROR:2]
//# Invalid parameter names in constructor.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public TestClass(int X1)
		{
			TestDelegate lambda = (A1, B1) => A1 == B1;
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid parameter names in property.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int Count
		{
			get
			{
				TestDelegate lambda = (A1, B1) => A1 == B1;
				return 0;
			}
			set
			{
				TestDelegate lambda = (A1, B1) => A1 == B1;
			}
		}
	}
}
//# [END]

//# [ERROR:3]
//# Invalid parameter names in indexer.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int this[int Y1]
		{
			get
			{
				TestDelegate lambda = (A1, B1) => A1 == B1;
				return 0;
			}
			set
			{
				TestDelegate lambda = (A1, B1) => A1 == B1;
			}
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid parameter names in method.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod(int X1, params string[] Y1)
		{
			TestDelegate lambda = (A1, B1) => A1 == B1;
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid parameter names in operator.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public static bool operator +(TestClass X1, TestClass Y1)
		{
			TestDelegate lambda = (A1, B1) => A1 == B1;
			return false;
		}
	}
}
//# [END]

#endregion

#region AdvancedNamingRules // Type parameters

// (AdvancedNaming_TypeParameter = T:T$(AaBb))

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

		~TestClass()
		{
			int A = 10;
		}

		public int Count
		{
			get
			{
				int A = 10;
				return 0;
			}
			set
			{
				int A = 10;
			}
		}

		public int this[int index]
		{
			get
			{
				int A = 10;
				return 0;
			}
			set
			{
				int A = 10;
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

		public static bool operator +(TestClass a, TestClass b)
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
//# Invalid local variable name in destructor.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		~TestClass()
		{
			int X = 10;
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid local variable names in property.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int Count
		{
			get
			{
				int X = 10;
				return 0;
			}
			set
			{
				int X = 10;
			}
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid local variable names in indexer.
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
			set
			{
				int X = 10;
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

//# [ERROR]
//# Invalid local variable names in operator.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public static bool operator +(TestClass a, TestClass b)
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

		~TestClass()
		{
			const int A = 10;
		}

		public int Count
		{
			get
			{
				const int A = 10;
				return 0;
			}
			set
			{
				const int A = 10;
			}
		}

		public int this[int index]
		{
			get
			{
				const int A = 10;
				return 0;
			}
			set
			{
				const int A = 10;
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

		public static bool operator +(TestClass a, TestClass b)
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
//# Invalid local constant name in destructor.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		~TestClass()
		{
			const int X = 10;
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid local constant names in property.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int Count
		{
			get
			{
				const int X = 10;
				return 0;
			}
			set
			{
				const int X = 10;
			}
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid local constant names in indexer.
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
			set
			{
				const int X = 10;
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

//# [ERROR]
//# Invalid local constant names in operator.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public static bool operator +(TestClass a, TestClass b)
		{
			const int X = 10;
			return false;
		}
	}
}
//# [END]

#endregion

#region AdvancedNamingRules // Labels

//# (AdvancedNaming_Label = LAB$(*))

//# [OK]
//# All label names are correct.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public TestClass()
		{
			LAB1:
			int a = 10;
		}

		~TestClass()
		{
			LAB1:
			int a = 10;
		}

		public int Count
		{
			get
			{
				LAB2:
				return 0;
			}
			set
			{
				LAB2:
				int a = 10;
			}
		}

		public int this[int index]
		{
			get
			{
				LAB3:
				return 0;
			}
			set
			{
				LAB3:
				int a = 10;
			}
		}

		public void TestMethod()
		{
			LAB4:
			Thread t = new Thread(() =>
			{
				LAB5:
				int a = 10;
			});
		}

		public static bool operator +(TestClass a, TestClass b)
		{
			LAB6:
			return false;
		}
	}
}
//# [END]

//# [ERROR]
//# Invalid label name in constructor.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public TestClass()
		{
			lab1:
			int a = 10;
		}
	}
}
//# [END]

//# [ERROR]
//# Invalid label name in destructor.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		~TestClass()
		{
			lab1:
			int a = 10;
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid label names in property.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int Count
		{
			get
			{
				lab2:
				return 0;
			}
			set
			{
				lab2:
				int a = 10;
			}
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid label names in indexer.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int this[int index]
		{
			get
			{
				lab3:
				return 0;
			}
			set
			{
				lab3:
				int a = 10;
			}
		}
	}
}
//# [END]

//# [ERROR:2]
//# Invalid label names in method.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			lab4:
			Thread t = new Thread(() =>
			{
				lab5:
				int a = 10;
			});
		}
	}
}
//# [END]

//# [ERROR]
//# Invalid label names in operator.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public static bool operator +(TestClass a, TestClass b)
		{
			lab6:
			return false;
		}
	}
}
//# [END]

#endregion
