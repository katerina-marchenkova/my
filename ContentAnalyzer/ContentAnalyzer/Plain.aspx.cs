using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace ContentAnalyzer
{
	public partial class Plain : Page
	{
		protected List<int> m_skuIds;

		protected void Page_Load(object sender, EventArgs e)
		{
			m_skuIds = new List<int>();

			string path = Server.MapPath("Plain.txt");
			foreach (string line in File.ReadAllLines(path))
			{
				int skuId = Int32.Parse(line);
				m_skuIds.Add(skuId);
			}
		}
	}
}
