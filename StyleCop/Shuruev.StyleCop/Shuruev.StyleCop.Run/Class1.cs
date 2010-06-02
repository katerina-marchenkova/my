using System.Collections.Generic;

namespace Shuruev.StyleCop.Run
{
	public delegate bool InputDelegate<in TInput, out TOutput>(TInput args)
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
