using System.Collections.Generic;

namespace Shuruev.StyleCop.Test
{
	public class Class1
	{
		private int m_field;

		public int TestProperty1
		{
			get { return m_field; }
			set { m_field = value; }
		}

		public int TestProperty2
		{
			get
			{
				return m_field;
			}
			set
			{
				m_field = value;
			}
		}

		public int TestProperty3
		{
			set
			{
				m_field = value;
			}
			get
			{
				return m_field;
			}
		}

		/*public int TestProperty3 { get; set; }
		public int TestProperty4 { get; set; }*/

		public Class1()

		{ }
	}
}
