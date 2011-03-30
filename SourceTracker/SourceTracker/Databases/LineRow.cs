using System;
using System.Data;
using VX.Sys.DbHelper;

namespace SourceTracker
{
	public struct LineRow : IDataRow
	{
		public int VersionId;
		public int LineNumber;
		public Guid LineCrc;
		public string LineText;

		#region Implementation of IDataRow

		/// <summary>
		/// Creates table for storing the rows.
		/// </summary>
		public DataTable CreateTable()
		{
			DataTable table = new DataTable();
			table.Columns.Add("LineCrc", typeof(Guid));
			table.Columns.Add("VersionId", typeof(int));
			table.Columns.Add("LineNumber", typeof(int));
			table.Columns.Add("LineText", typeof(string));
			return table;
		}

		/// <summary>
		/// Adds current row to the specified table.
		/// </summary>
		public void AddToTable(DataTable table)
		{
			table.Rows.Add(
				LineCrc,
				VersionId,
				LineNumber,
				LineText);
		}

		#endregion
	}
}
