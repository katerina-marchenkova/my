#region CodeLineMustBeginWithIdenticalWhitespaces // Mode = Tabs

//# (SP2001_Mode = Tabs)

//# [OK]
//# Each line uses tabs for indentation.
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
//# [END]

//# [ERROR:2]
//# Spaces should not be used for indentation.
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
//# Each line uses tabs for indentation.

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
//# Each line uses tabs for indentation.
	
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
//# Spaces should not be used for indentation.
    
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

#region CodeLineMustBeginWithIdenticalWhitespaces // Mode = Spaces

//# (SP2001_Mode = Spaces)

//# [OK]
//# Each line uses spaces for indentation.
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
//# [END]

//# [ERROR:2]
//# Tabs should not be used for indentation.
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
//# Each line uses spaces for indentation.

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
//# Tabs should not be used for indentation.
	
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
//# Each line uses spaces for indentation.
    
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

#region CodeLineMustBeginWithIdenticalWhitespaces // Mode = Both

//# (SP2001_Mode = Both)

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
//# [END]

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
//# [END]

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
//# [END]

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

//# [END]

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

//# [END]

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
	
//# [END]

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

//# [END]

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
    
//# [END]

#endregion
