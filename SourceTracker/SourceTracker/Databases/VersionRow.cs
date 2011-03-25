using System;
using System.Data;
using VX.Sys.DbHelper;

namespace SourceTracker.Databases
{
	public struct VersionRow : IDataRow
	{
		public int FileId;
		public int VersionId;
		public int UserId;
		public DateTime VersionDate;

		#region Implementation of IDataRow

		/// <summary>
		/// Creates table for storing the rows.
		/// </summary>
		public DataTable CreateTable()
		{
			DataTable table = new DataTable();
			table.Columns.Add("FileId", typeof(int));
			table.Columns.Add("VersionId", typeof(int));
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
				FileId,
				VersionId,
				UserId,
				VersionDate);
		}

		#endregion
	}
}
