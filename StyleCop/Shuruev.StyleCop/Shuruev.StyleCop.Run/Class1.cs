using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Shuruev.StyleCop.Test
{
	public class Class1
	{
		public class MyObject
		{
			public string Name { get; set; }

			public MyObject(params int[] args)
			{
			}
		}

		public void A()
		{
			var obj = new MyObject(new[] { 5 })
			{
				Name = "Ex1"
			};

			Console.WriteLine("Hello");
			{
				// Limited scope
			}
		}
	}

	public class Class1<TFirst, TSecond>
	{
	}

	/// <summary>
	/// Uses the <see cref="Class1{A,B}"/> class.
	/// </summary>
	public class Class2
	{
	}
}
