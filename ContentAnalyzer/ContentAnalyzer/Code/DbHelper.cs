using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace ContentAnalyzer
{
	/// <summary>
	/// Provides access to the database.
	/// </summary>
	public class DbHelper
	{
		private readonly string m_connectionString;
		private readonly int m_commandTimeoutInSeconds;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public DbHelper(string connectionString, TimeSpan commandTimeout)
		{
			if (String.IsNullOrEmpty(connectionString))
				throw new ArgumentNullException("connectionString");
			if (commandTimeout <= TimeSpan.Zero)
				throw new ArgumentOutOfRangeException("commandTimeout");

			m_connectionString = connectionString;
			m_commandTimeoutInSeconds = Convert.ToInt32(commandTimeout.TotalSeconds);
		}

		#region Properties

		/// <summary>
		/// Gets connection string.
		/// </summary>
		public string ConnectionString
		{
			get { return m_connectionString; }
		}

		/// <summary>
		/// Gets command timeout in seconds.
		/// </summary>
		public int CommandTimeoutInSeconds
		{
			get { return m_commandTimeoutInSeconds; }
		}

		#endregion

		#region Connection

		/// <summary>
		/// Creates and opens new connection.
		/// </summary>
		public SqlConnection OpenConnection()
		{
			SqlConnection conn = new SqlConnection(m_connectionString);
			conn.Open();
			return conn;
		}

		#endregion

		#region Delegates

		/// <summary>
		/// Performs actions inside the reader.
		/// </summary>
		public delegate void DoWithReader(IDataRecord reader);

		/// <summary>
		/// Returns result from the reader.
		/// </summary>
		public delegate T ReturnFromReader<T>(IDataRecord reader);

		#endregion

		#region Commands

		/// <summary>
		/// Creates command.
		/// </summary>
		private SqlCommand CreateCommand(
			SqlConnection connection,
			string commandText,
			params SqlParameter[] parameters)
		{
			SqlCommand cmd = connection.CreateCommand();
			cmd.CommandType = commandText.Contains(" ") ?
				CommandType.Text :
				CommandType.StoredProcedure;

			cmd.CommandTimeout = m_commandTimeoutInSeconds;
			cmd.CommandText = commandText;

			foreach (SqlParameter parameter in parameters)
			{
				cmd.Parameters.Add(parameter);
			}

			return cmd;
		}

		/// <summary>
		/// Executes non-query command.
		/// </summary>
		public void ExecuteNonQuery(
			SqlConnection connection,
			string commandText,
			params SqlParameter[] parameters)
		{
			using (SqlCommand cmd = CreateCommand(
				connection,
				commandText,
				parameters))
			{
				cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Executes scalar command.
		/// </summary>
		public object ExecuteScalar(
			SqlConnection connection,
			string commandText,
			params SqlParameter[] parameters)
		{
			using (SqlCommand cmd = CreateCommand(
				connection,
				commandText,
				parameters))
			{
				return cmd.ExecuteScalar();
			}
		}

		/// <summary>
		/// Executes reader command.
		/// </summary>
		public void ExecuteReader(
			SqlConnection connection,
			string commandText,
			DoWithReader action,
			params SqlParameter[] parameters)
		{
			using (SqlCommand cmd = CreateCommand(
				connection,
				commandText,
				parameters))
			{
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						action(reader);
					}
				}
			}
		}

		/// <summary>
		/// Executes reader command.
		/// </summary>
		public T ExecuteReader<T>(
			SqlConnection connection,
			string commandText,
			ReturnFromReader<T> action,
			params SqlParameter[] parameters)
		{
			using (SqlCommand cmd = CreateCommand(
				connection,
				commandText,
				parameters))
			{
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						return action(reader);
					}
				}
			}

			return default(T);
		}

		#endregion

		#region Bulk copy

		/// <summary>
		/// Creates bulk table for data rows.
		/// </summary>
		public DataTable CreateBulkTable<T>(IEnumerable<T> rows) where T : IDataRow, new()
		{
			T sample = new T();
			DataTable table = sample.CreateTable();
			table.Locale = CultureInfo.InvariantCulture;

			foreach (T row in rows)
				row.AddToTable(table);

			return table;
		}

		/// <summary>
		/// Creates bulk table.
		/// </summary>
		public DataTable CreateBulkTable(IEnumerable<int> ids, string columnName)
		{
			DataTable table = new DataTable();
			table.Locale = CultureInfo.InvariantCulture;
			table.Columns.Add(columnName, typeof(int));

			foreach (int id in ids)
				table.Rows.Add(id);

			return table;
		}

		/// <summary>
		/// Creates bulk table.
		/// </summary>
		public DataTable CreateBulkTable(IEnumerable<string> values, string columnName)
		{
			DataTable table = new DataTable();
			table.Locale = CultureInfo.InvariantCulture;
			table.Columns.Add(columnName, typeof(string));

			foreach (string value in values)
				table.Rows.Add(value);

			return table;
		}

		/// <summary>
		/// Executes bulk copy command.
		/// </summary>
		public void ExecuteBulkCopy(
			SqlConnection connection,
			string destinationTableName,
			DataTable table,
			params SqlBulkCopyColumnMapping[] mappings)
		{
			using (SqlBulkCopy bulk = new SqlBulkCopy(connection))
			{
				bulk.BulkCopyTimeout = m_commandTimeoutInSeconds;
				bulk.DestinationTableName = destinationTableName;

				foreach (SqlBulkCopyColumnMapping mapping in mappings)
				{
					bulk.ColumnMappings.Add(mapping);
				}

				bulk.WriteToServer(table);
			}
		}

		#endregion
	}
}
