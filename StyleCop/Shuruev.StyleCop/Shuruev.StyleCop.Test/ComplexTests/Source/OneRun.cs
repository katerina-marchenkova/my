#region CheckAllowedIndentationCharacters // Allow padding for expressions

//# (SP2001_Mode = Tabs:True)

//# [OK]
//# Padding is allowed in all cases below.
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

			bool x =
					a || ((b && c)
					       || d);

			bool y =
			        a || ((b && c)
			               || d);
		}
	}
}
//# [END]

//# [ERROR:3]
//# Padding is not allowed when it is not padding actually.
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

			bool x =
    				a || ((b && c)
					       || d);

			bool y =
			    	a || ((b && c)
			               || d);
		}
	}
}
//# [END]

//# [ERROR:3]
//# Padding is not allowed if previous line has different amount of tabs.
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

			bool x =
					a || ((b && c)
						   || d);

			bool y =
				    a || ((b && c)
				           || d);
		}
	}
}
//# [END]

//# [ERROR:3]
//# Padding is not allowed for common entities.
	   namespace Shuruev.StyleCop.Test
	   {
	   }
//# [END]

#endregion
