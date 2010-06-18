using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace ContentAnalyzer
{
	public partial class Plain : Page
	{
		protected List<int> m_skuIds;
		protected Dictionary<int, List<MultipleImageRow>> m_images;
		protected Dictionary<int, SkuInfoRow> m_infos;

		protected void Page_Load(object sender, EventArgs e)
		{
			m_skuIds = new List<int>();

			string path = Server.MapPath("Plain.txt");
			foreach (string line in File.ReadAllLines(path))
			{
				int skuId = Int32.Parse(line);
				m_skuIds.Add(skuId);
			}

			m_images = TPD.GetMultipleImages(m_skuIds);
			m_infos = AggregatorDb.GetSkuInfo(m_skuIds);
		}
	}
}
