using System;
using System.Data;

namespace ContentAnalyzer
{
	public struct MultipleImageRow
	{
		public string Url;
		public string OriginalId;

		public MultipleImageRow(IDataRecord reader)
		{
			Url = (string)reader["Url"];
			OriginalId = (string)reader["OriginalId"];
		}

		public string InfoItemId
		{
			get
			{
				return OriginalId.Substring(0, 36);
			}
		}

		public string VXStorageLink
		{
			get
			{
				return String.Format("vx-storage://browse/item/template_id=992D38A1-A8EF-4D2C-82D2-93A4C887872A/item_id={0}", InfoItemId);
			}
		}
	}
}
