using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace ContentAnalyzer
{
	public partial class Research : Page
	{
		protected List<int> m_nums;
		protected List<string> m_keys;
		protected List<PairContentRow> m_rows;

		protected void Page_Load(object sender, EventArgs e)
		{
			m_keys = new List<string>();
			m_nums = new List<int>();
			m_rows = new List<PairContentRow>();

			foreach (string line in File.ReadAllLines(@"D:\Knowledge.txt"))
			{
				string[] parts = line.Split('\t');

				string key = parts[0];
				int num = Convert.ToInt32(parts[1]);

				m_keys.Add(key);
				m_nums.Add(num);
			}

			string[] keys = m_keys.ToArray();
			int[] nums = m_nums.ToArray();
			Array.Sort(nums, keys);

			for (int i = 0; i < 500; i++)
			{
				PairContentRow row = new PairContentRow();
				row.MagicNumber = nums[i];

				string[] parts = keys[i].Split('_');
				row.ContentGuidA = new Guid(parts[0]);
				row.ContentGuidB = new Guid(parts[1]);

				m_rows.Add(row);
			}
		}
	}
}
