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

#region CheckAllowedIndentationCharacters // Tabs only

//# (SP2001_Mode = Tabs:False)

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

#region CheckAllowedIndentationCharacters // Spaces only

//# (SP2001_Mode = Spaces:False)

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

#region CheckAllowedIndentationCharacters // Tabs and spaces

//# (SP2001_Mode = Both:False)

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

#region CheckAllowedIndentationCharacters // xxxx

//# (SP2001_Mode = Tabs:True)

//# [OK]
//# Space padding is allowed for multiline expressions.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			bool a = false;
			if (true
			    || true
				   || true)
				a = true;
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

//# [OK]
//# Empty source file should be OK.
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
//# Empty source file should be OK.
//# [END]

#endregion

#region CodeLineMustNotBeLongerThan // Limit = 40:4

//# (SP2100_Limit = 40:4)

//# [OK]
//# All lines are less than 40 chars.
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

//# [OK]
//# Summary line is 40 chars.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		/// <summary>
		/// Test summary (almost large).
		/// </summary>
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
//# [END]

//# [ERROR]
//# Summary line is 41 chars.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		/// <summary>
		/// Test summary (too large now).
		/// </summary>
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
//# [END]

//# [ERROR]
//# First line is 40 chars, second is 41.
namespace Shuruev.StyleCop.Test
{
										
										 
}
//# [END]

#endregion

#region CodeLineMustNotBeLongerThan // Limit = 40:8

//# (SP2100_Limit = 40:8)

//# [ERROR:2]
//# First line is 80 chars, second is 81.
namespace Shuruev.StyleCop.Test
{
										
										 
}
//# [END]

//# [ERROR]
//# First line is 40 chars, second is 41.
namespace Shuruev.StyleCop.Test
{
					
					 
}
//# [END]

#endregion

#region CodeLineMustNotBeLongerThan // Limit = 5:0

//# (SP2100_Limit = 5:0)

//# [ERROR:2]
//# Zero tab size should be treated as 1.
namespace Shuruev.StyleCop.Test
{
					
					 
}
//# [END]

#endregion

#region MethodMustNotContainMoreLinesThan

//# (SP2101_Limit = 3)

//# [OK]
//# All elements size are valid.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public TestClass()
		{
			int a = 10;
		}

		~TestClass()
		{
			int a = 10;
		}

		public void TestMethod()
		{
			int a = 10;
		}

		public static bool operator +(TestClass x, TestClass y)
		{
			return false;
		}
	}
}
//# [END]

//# [ERROR]
//# Constructor violates size limit.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public TestClass()
		{
			int a = 10;
			int b = 20;
		}
	}
}
//# [END]

//# [ERROR]
//# Destructor violates size limit.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		~TestClass()
		{
			int a = 10;
			int b = 20;
		}
	}
}
//# [END]

//# [ERROR]
//# Method violates size limit.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
			int b = 20;
		}
	}
}
//# [END]

//# [ERROR]
//# Operator violates size limit.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public static bool operator +(TestClass x, TestClass y)
		{
			int a = 10;
			return false;
		}
	}
}
//# [END]

#endregion

#region PropertyMustNotContainMoreLinesThan

//# (SP2102_Limit = 3)

//# [OK]
//# All elements size are valid.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int Count
		{
			get
			{
				return 0;
			}
			set
			{
				int a = 10;
			}
		}

		public int this[int index]
		{
			get
			{
				return 0;
			}
			set
			{
				int a = 10;
			}
		}
	}
}
//# [END]

//# [ERROR]
//# Property getter violates size limit.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int Count
		{
			get
			{
				int a = 10;
				return 0;
			}
		}
	}
}
//# [END]

//# [ERROR]
//# Property setter violates size limit.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int Count
		{
			set
			{
				int a = 10;
				int b = 20;
			}
		}
	}
}
//# [END]

//# [ERROR]
//# Indexer getter violates size limit.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int this[int index]
		{
			get
			{
				int a = 10;
				return 0;
			}
		}
	}
}
//# [END]

//# [ERROR]
//# Indexer setter violates size limit.
namespace Shuruev.StyleCop.Test
{
	public class TestClass
	{
		public int this[int index]
		{
			set
			{
				int a = 10;
				int b = 20;
			}
		}
	}
}
//# [END]

#endregion
