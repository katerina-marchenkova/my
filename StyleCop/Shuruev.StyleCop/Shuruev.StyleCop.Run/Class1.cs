using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Shuruev.StyleCop.Test
{
	[SuppressMessage("Shuruev.StyleCop.CSharp.StyleCopPlus", "SP2100:CodeLineMustNotBeLongerThan", Justification = "Long lines are allowed for better maintability.")]
	[SuppressMessage("Shuruev.StyleCop.CSharp.StyleCopPlus", "SP0100:AdvancedNamingRules", Justification = "Long lines are allowed for better maintability.")]
	public class Class1
	{
		public Class1()
		{
			int Aaa = 10;
		}

		public Class1(int z)
			: this()
		{
		zzz:
			int aaa = 10;
		}

		~Class1()
		{
		zzz:
			int aaa = 10;
		}

		public string P
		{
			get
			{
				return null;
			}
			set
			{
				int a = 10;
				int b = 10;
			}
		}

		public void A()
		{
			string a = "sdk jfskd jfhskjd hfksjd hfksjdh fksjdhfk sjhd fkjshdk jfsh kdj hfksjdh fkjshd kfjshkdj fhskjd hfskjdh fksjd hfksjdh kfjshdk jfshkdj fhksjdh fksjdh fkjshdk jfshkjd hfksjd hfksjd kjfskdj hfksjd hfksjhd shk jdh kfjs";
		}
	}
}
