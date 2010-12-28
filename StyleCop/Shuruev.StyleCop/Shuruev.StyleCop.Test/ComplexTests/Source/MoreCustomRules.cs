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

//# [ERROR:3]
//# Summary contains whitespaces at the end of the lines.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		/// <summary> 
		/// Test summary. 
		/// </summary> 
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
//# [END]

#endregion

#region CheckAllowedIndentationCharacters // Mode = Tabs

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

#region CheckAllowedIndentationCharacters // Mode = Spaces

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

#region CheckAllowedIndentationCharacters // Mode = Both

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

//# [ERROR:3]
//# Summary contains whitespaces after indentation tabs.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		 /// <summary>
		 /// Test summary.
		 /// </summary>
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
//# [END]

#endregion

#region CheckWhetherLastCodeLineIsEmpty // Mode = Empty

//# (SP2002_Mode = Empty)

//# [OK]
//# Source file is OK.
namespace Shuruev.StyleCop.Test
{
}
//# [END]

//# [ERROR]
//# Source file without a line break at the end.
namespace Shuruev.StyleCop.Test
{
}//# [END]

//# [OK]
//# Source file contains line break at the end.

//# [END]

//# [ERROR]
//# Source file doesn't contain a line break at the end.
//# [END]

#endregion

#region CheckWhetherLastCodeLineIsEmpty // Mode = NotEmpty

//# (SP2002_Mode = NotEmpty)

//# [ERROR]
//# Source file with a line break at the end.
namespace Shuruev.StyleCop.Test
{
}
//# [END]

//# [OK]
//# Source file is OK.
namespace Shuruev.StyleCop.Test
{
}//# [END]

//# [ERROR]
//# Source file contains line break at the end.

//# [END]

//# [OK]
//# Source file doesn't contain a line break at the end.
//# [END]

#endregion
