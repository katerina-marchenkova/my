using System;
using System.Collections.Generic;
using System.Web.UI;

namespace ContentAnalyzer
{
	public partial class Helix : Page
	{
		protected List<Guid> m_items = null;
		protected Dictionary<Guid, List<HelixFileRow>> m_files = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			m_items = new List<Guid>();
			m_files = new Dictionary<Guid, List<HelixFileRow>>();

			List<HelixFileRow> rows = AggregatorDb.GetMultipleImagesByFileCount(14);
			foreach (HelixFileRow row in rows)
			{
				if (!m_files.ContainsKey(row.ItemUid))
				{
					m_items.Add(row.ItemUid);
					m_files[row.ItemUid] = new List<HelixFileRow>();
				}

				m_files[row.ItemUid].Add(row);
			}
		}
	}
}
