using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace ContentAnalyzer
{
	public partial class Duplicates : Page
	{
		protected List<int> m_skuIds;
		protected Dictionary<Guid, bool> m_content;
		protected Dictionary<int, List<DigitalContentRow>> m_rows;

		protected void Page_Load(object sender, EventArgs e)
		{
			m_skuIds = new List<int>();
			m_content = new Dictionary<Guid, bool>();
			m_rows = new Dictionary<int, List<DigitalContentRow>>();

			List<DigitalContentRow> rows = GetRows();
			foreach (DigitalContentRow row in rows)
			{
				if (m_content.ContainsKey(row.ContentGuid))
					continue;

				m_content[row.ContentGuid] = true;

				if (!m_rows.ContainsKey(row.SkuId))
				{
					m_skuIds.Add(row.SkuId);
					m_rows[row.SkuId] = new List<DigitalContentRow>();
				}

				m_rows[row.SkuId].Add(row);
			}
		}

		/*xxxprivate List<DigitalContentRow> GetRows()
		{
			List<DigitalContentRow> rows = new List<DigitalContentRow>();
			foreach (string line in File.ReadAllLines(@"D:\SimilarImages.txt"))
			{
				string[] parts = line.Split('\t');

				int skuId = Convert.ToInt32(parts[0]);
				Guid id1 = new Guid(parts[1]);
				Guid id2 = new Guid(parts[2]);

				DigitalContentRow row1 = new DigitalContentRow();
				row1.SkuId = skuId;
				row1.ContentGuid = id1;
				row1.Extension = ".jpg";
				row1.Name = "xxx";

				DigitalContentRow row2 = new DigitalContentRow();
				row2.SkuId = skuId;
				row2.ContentGuid = id2;
				row2.Extension = ".jpg";
				row2.Name = "xxx";

				rows.Add(row1);
				rows.Add(row2);
			}

			return rows;
		}*/

		private List<DigitalContentRow> GetRows()
		{
			List<DigitalContentRow> rows = new List<DigitalContentRow>();
			foreach (string line in File.ReadAllLines(@"D:\WrongCanvas.txt"))
			{
				DigitalContentRow row = new DigitalContentRow();
				row.SkuId = rows.Count;
				row.ContentGuid = new Guid(line);
				row.Extension = ".jpg";
				row.Name = "xxx";

				rows.Add(row);
			}

			return rows;
		}
	}
}
