using System.Data;
using System.Data.SqlClient;
using VX.Sys;
using VX.Sys.DbHelper;

namespace SourceTracker
{
	/// <summary>
	/// Accessing Source Tracker DB.
	/// </summary>
	public static class SourceTrackerDb
	{
		private static readonly DbHelper s_db;

		static SourceTrackerDb()
		{
			s_db = new DbHelper(
				ConfigurationHelper.ReadConnectionString("SourceTracker"),
				ConfigurationHelper.ReadTimeSpan("SourceTracker.Timeout"));
		}

		/// <summary>
		/// Returns existing file or creates a new one.
		/// </summary>
		public static int ResolveFile(string fullPath, string extension)
		{
			using (SqlConnection conn = s_db.OpenConnection())
			{
				SqlParameter fileId = new SqlParameter("@fileId", SqlDbType.Int) { Direction = ParameterDirection.Output };

				s_db.ExecuteNonQuery(
					conn,
					"ResolveFile",
					new SqlParameter("@fullPath", fullPath),
					new SqlParameter("@extension", extension),
					fileId);

				return (int)fileId.Value;
			}
		}

		/// <summary>
		/// Returns existing user or creates a new one.
		/// </summary>
		public static int ResolveUser(string userName)
		{
			using (SqlConnection conn = s_db.OpenConnection())
			{
				SqlParameter userId = new SqlParameter("@userId", SqlDbType.Int) { Direction = ParameterDirection.Output };

				s_db.ExecuteNonQuery(
					conn,
					"ResolveUser",
					new SqlParameter("@userName", userName),
					userId);

				return (int)userId.Value;
			}
		}
	}
}
