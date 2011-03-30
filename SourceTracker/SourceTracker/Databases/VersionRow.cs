using System;
using System.Data;
using VX.Sys.DbHelper;

namespace SourceTracker
{
	public struct VersionRow : IDataRow
	{
		public int VersionId;
		public int FileId;
		public int UserId;
		public DateTime VersionDate;

		#region Implementation of IDataRow

		/// <summary>
		/// Creates table for storing the rows.
		/// </summary>
		public DataTable CreateTable()
		{
			DataTable table = new DataTable();
			table.Columns.Add("VersionId", typeof(int));
			table.Columns.Add("FileId", typeof(int));
			table.Columns.Add("UserId", typeof(int));
			table.Columns.Add("VersionDate", typeof(DateTime));
			return table;
		}

		/// <summary>
		/// Adds current row to the specified table.
		/// </summary>
		public void AddToTable(DataTable table)
		{
			table.Rows.Add(
				VersionId,
				FileId,
				UserId,
				VersionDate);
		}

		#endregion
	}
}
