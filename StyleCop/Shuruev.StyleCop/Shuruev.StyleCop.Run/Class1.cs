namespace Shuruev.StyleCop.Run
{
	public class Foo
	{
		protected internal readonly int Foo1;
		protected internal readonly int Foo2;

		/// <summary>
		/// Initializes a new instance of the <see cref="Foo"/> class.
		/// </summary>
		public Foo() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Foo"/> class.
		/// </summary>
		public Foo(int y)
		{
			string[] lines = new[] { "xxx" };

			int x = 10;
			{
				int a = 1;
				this.Foo1 = a + 15;
			}

			{
				int b = 2;
				this.Foo2 = b + 15;
			}
		}
	}
}
