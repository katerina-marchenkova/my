using System;
using System.Data;

namespace ContentAnalyzer
{
	public struct DigitalContentRow
	{
		public int SkuId;
		public Guid ContentGuid;
		public string Name;
		public string Extension;

		public DigitalContentRow(IDataRecord reader)
		{
			SkuId = (int)reader["sku_id"];
			ContentGuid = (Guid)reader["content_guid"];
			Name = (string)reader["original_file_name"];
			Extension = (string)reader["original_file_extension"];
		}
	}
}
