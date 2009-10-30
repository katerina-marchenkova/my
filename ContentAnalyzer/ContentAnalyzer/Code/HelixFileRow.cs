using System;
using System.Data;

namespace ContentAnalyzer
{
	public struct HelixFileRow
	{
		public Guid TemplateUid;
		public Guid FileUid;
		public Guid ItemUid;
		public string Name;
		public string Extension;

		public HelixFileRow(IDataRecord reader)
		{
			TemplateUid = (Guid)reader["TemplateUid"];
			FileUid = (Guid)reader["FileUid"];
			ItemUid = (Guid)reader["ItemUid"];
			Name = (string)reader["Name"];
			Extension = (string)reader["Extension"];
		}
	}
}
