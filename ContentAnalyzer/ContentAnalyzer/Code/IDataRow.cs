using System.Data;

namespace ContentAnalyzer
{
	/// <summary>
	/// Interface for data row.
	/// </summary>
	public interface IDataRow
	{
		/// <summary>
		/// Creates table for storing the rows.
		/// </summary>
		DataTable CreateTable();

		/// <summary>
		/// Adds current row to the specified table.
		/// </summary>
		void AddToTable(DataTable table);
	}
}
