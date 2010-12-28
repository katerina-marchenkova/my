using System;
using System.Collections.Generic;
using System.IO;

namespace Shuruev.StyleCop.Test
{
	public class Class1 : IDisposable
	{
		/// <summary>
		/// 1234567890
		/// 12345678901234567890
        /// 1234567890123456789012345678fgss
		/// 1234567890123456789012345678901234567890
		/// 12345678901234567890123456789012345678901234567890
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
