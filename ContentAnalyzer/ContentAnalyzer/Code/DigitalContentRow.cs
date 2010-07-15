using System;
using System.Data;
using VX.Storage;

namespace ContentAnalyzer
{
	public struct DigitalContentRow
	{
		public Guid ContentUid;
		public string Url;
		public string OriginalId;
		public Guid ItemUid;

		public DigitalContentRow(IDataRecord reader)
		{
			ContentUid = Guid.Empty;
			Url = String.Empty;
			OriginalId = String.Empty;
			ItemUid = Guid.Empty;

			for (int i = 0; i < reader.FieldCount; i++)
			{
				switch (reader.GetName(i))
				{
					case "ContentUid":
						ContentUid = (Guid)reader["ContentUid"];
						Url = Helpers.GetDigitalContentUrl(ContentUid);
						break;
					case "Url":
						Url = (string)reader["Url"];
						break;
					case "OriginalId":
						OriginalId = (string)reader["OriginalId"];
						ItemUid = new Guid(OriginalId.Substring(0, 36));
						break;
					case "ItemUid":
						ItemUid = (Guid)reader["ItemUid"];
						break;
				}
			}
		}

		public string GetVXStorageLink(VXGuid templateId)
		{
			return VXStorageUri.GetItemUri(
				templateId,
				ItemUid,
				VXStorageUri.UriAction.Browse).AbsoluteUri;
		}
	}
}
