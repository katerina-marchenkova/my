using System;
using System.Data;
using VX.Knowledge.DataSource;
using VX.Storage;

namespace ContentAnalyzer
{
	public struct MultipleImageRow
	{
		public Guid ContentUid;
		public string Url;
		public string OriginalId;
		public Guid ItemUid;

		public MultipleImageRow(IDataRecord reader)
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

		public string VXStorageLink
		{
			get
			{
				return VXStorageUri.GetItemUri(
					KB.Template.Multipleimages.Id,
					ItemUid,
					VXStorageUri.UriAction.Browse).AbsoluteUri;
			}
		}
	}
}
