using System.Data;
using Shuruev.Releaser.Interfaces;

namespace Shuruev.Releaser.Data
{
	/// <summary>
	/// Project in SQLite database.
	/// </summary>
	public class ProjectRowSQLite : ProjectRow
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ProjectRowSQLite(IDataRecord reader)
		{
			Id = (int)Read.Int64(reader, "ProjectId");
			Data = new ProjectData(
				Read.String(reader, "ProjectName"),
				Read.String(reader, "ImageCode"),
				Read.String(reader, "StorageCode"),
				Read.String(reader, "StoragePath"));
		}
	}
}
