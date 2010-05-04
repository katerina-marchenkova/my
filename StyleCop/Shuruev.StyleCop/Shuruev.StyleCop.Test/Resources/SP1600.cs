﻿//# [ERROR]
//# Public class doesn't have summary.
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
//# [ERROR]
//# Public method doesn't have summary.
namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Hello there.
	/// </summary>
	public class TestClass
	{
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
//# [OK]
//# Both class and method have summary.
namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Hello there.
	/// </summary>
	public class TestClass
	{
		/// <summary>
		/// Hello there.
		/// </summary>
		public void TestMethod()
		{
			int a = 10;
		}
	}
}
//# [ERROR]
//# Windows forms event handler is not private.
namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Hello there.
	/// </summary>
	public class TestClass
	{
		public void btnStart_OnClick(object sender, EventArgs e)
		{
			int a = 10;
		}
	}
}
//# [ERROR]
//# First argument in windows forms event handler is not an object.
namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Hello there.
	/// </summary>
	public class TestClass
	{
		private void btnStart_OnClick(NotObject sender, EventArgs e)
		{
			int a = 10;
		}
	}
}
//# [ERROR]
//# First argument in windows forms event handler has wrong name.
namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Hello there.
	/// </summary>
	public class TestClass
	{
		private void btnStart_OnClick(object notSender, EventArgs e)
		{
			int a = 10;
		}
	}
}
//# [ERROR]
//# Second argument in windows forms event handler is not EventArgs.
namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Hello there.
	/// </summary>
	public class TestClass
	{
		private void btnStart_OnClick(object sender, EventArgsWrong e)
		{
			int a = 10;
		}
	}
}
//# [ERROR]
//# Second argument in windows forms event handler has wrong name.
namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Hello there.
	/// </summary>
	public class TestClass
	{
		private void btnStart_OnClick(object sender, EventArgs notE)
		{
			int a = 10;
		}
	}
}
//# [OK]
//# Windows forms event handler is well-formed.
namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Hello there.
	/// </summary>
	public class TestClass
	{
		private void btnStart_OnClick(object sender, EventArgs e)
		{
			int a = 10;
		}
	}
}
//# [OK]
//# Windows forms event handler with non-usual second parameter.
namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Hello there.
	/// </summary>
	public class TestClass
	{
		private void btnStart_OnClick(object sender, MouseEventArgs e)
		{
			int a = 10;
		}
	}
}
//# [END OF FILE]