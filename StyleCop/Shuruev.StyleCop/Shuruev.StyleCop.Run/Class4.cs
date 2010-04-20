using System;
using System.Xml;

namespace Shuruev.StyleCop.Test
{
	public interface IComparisonGenerator<T>
	{
		void AddComparedItem(T item);

		void WriteToXmlStream(XmlTextWriter xtw);

		bool Compare();

		bool IsCompareble { get; }
	}

	public class A
	{
		public class B1
		{
		}

		protected class B2
		{
		}

		internal class B3
		{
		}

		protected internal class B4
		{
		}

		private class B5
		{
		}
	}

	internal class B
	{
		public class B1
		{
		}

		protected class B2
		{
		}

		internal class B3
		{
		}

		protected internal class B4
		{
		}

		private class B5
		{
		}
	}
}
