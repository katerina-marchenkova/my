using System;
using System.Collections.Generic;
using System.Web.UI;

namespace ContentAnalyzer
{
	public partial class Mirror : Page
	{
		protected List<int> m_skuIds;
		protected Dictionary<int, List<DigitalContentRow>> m_rows;

		protected void Page_Load(object sender, EventArgs e)
		{
			m_skuIds = new List<int>();
			m_rows = new Dictionary<int, List<DigitalContentRow>>();

			//List<DigitalContentRow> rows = TPD.GetMultipleImagesByFileCount();
			List<DigitalContentRow> rows = TPD.GetLicenseMultipleImages();
			foreach (DigitalContentRow row in rows)
			{
				if (!m_rows.ContainsKey(row.SkuId))
				{
					m_skuIds.Add(row.SkuId);
					m_rows[row.SkuId] = new List<DigitalContentRow>();
				}

				m_rows[row.SkuId].Add(row);
			}
		}
	}
}
