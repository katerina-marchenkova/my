using System.Data;
using VX.Sys.DbHelper;

namespace SourceTracker.Databases
{
	public struct LineRow : IDataRow
	{
		public int FileId;
		public int VersionId;
		public int LineNumber;
		public byte[] LineCrc;
		public string LineOriginal;

		#region Implementation of IDataRow

		/// <summary>
		/// Creates table for storing the rows.
		/// </summary>
		public DataTable CreateTable()
		{
			DataTable table = new DataTable();
			table.Columns.Add("FileId", typeof(int));
			table.Columns.Add("VersionId", typeof(int));
			table.Columns.Add("LineNumber", typeof(int));
			table.Columns.Add("LineCrc", typeof(byte[]));
			table.Columns.Add("LineOriginal", typeof(string));
			return table;
		}

		/// <summary>
		/// Adds current row to the specified table.
		/// </summary>
		public void AddToTable(DataTable table)
		{
			table.Rows.Add(
				FileId,
				VersionId,
				LineNumber,
				LineCrc,
				LineOriginal);
		}

		#endregion
	}
}
