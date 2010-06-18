using System.Collections.Generic;

namespace Shuruev.StyleCop.Test
{
	public class AAA
	{
		public class Class1 : List<int>
		{
		}

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
}
