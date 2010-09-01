using System;
using System.Collections.Generic;
using System.IO;

namespace Shuruev.StyleCop.Test
{
	public class Class1 : IDisposable
	{
		private int m_field = 5;

		public Class1()
		{
			try
			{
				int a1, a2 = 10, a3 = 20;
				const int b1 = 30, b2 = 40;

				m_field = 6;
			}
			catch (Exception ex1)
			{
				throw;
			}

			for (int i1, i2 = 0, i3 = 1; i2 < 10; i2++)
			{
				int a1, a2 = 10, a3 = 20;
				const int b1 = 30, b2 = 40;

				m_field = 6;
			}

			/*using (MemoryStream ms1 = new MemoryStream(),
				ms2 = new MemoryStream(),
				ms3 = new MemoryStream())
			{
				int a1, a2 = 10, a3 = 20;
				m_x = 6;
			}

			m_x = 7;

			int b1, b2 = 10, b3 = 20;
			const int c1 = 40, c2 = 25;

			const string s1 = "";

			for (int z = 0; z < 10; z++)
			{
				m_x = 8;
				int y = 10;
				throw new Exception();
			}*/
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
