using System;
using System.Collections.Generic;
using System.IO;

namespace Shuruev.StyleCop.Test
{
	public class Class1 : IDisposable
	{
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			label2:
			for (int i = 0; i < 10; i++)
			{
				goto label2;
			}
		}
	}
}
